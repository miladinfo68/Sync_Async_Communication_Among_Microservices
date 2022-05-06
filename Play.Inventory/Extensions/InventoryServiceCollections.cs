using Polly;
using Polly.Timeout;
using Play.Inventory.Client;

namespace Play.Inventory.Extensions;
public static class InventoryServiceCollections
{
    private static Uri catalogServiceUri = new Uri("https://localhost:5001");
    public static IServiceCollection AddConfiguredHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<CatalogClient>(client =>
        {
            //catalog micro service uri
            client.BaseAddress = catalogServiceUri;
            //client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        return services;
    }

    public static IServiceCollection AddConfiguredHttpClientWithPolly(this IServiceCollection services)
    {

        var sp = services.BuildServiceProvider();

        services.AddHttpClient<CatalogClient>(client =>
        {
            client.BaseAddress = catalogServiceUri;
        })
        //second policy
        .AddTransientHttpErrorPolicy(builder =>
            //combine 2 palicies --> by Or<TimeoutRejectedException>() function
            builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
                retryCount: 5,
                sleepDurationProvider: retryAttempt =>
                     TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(new Random().Next(0, 1000)),

                //on production mode comment onRetry callback function
                onRetry: (outcome, timespan, retryAttempt) =>
                {
                    var msg = $"Delaying for {timespan.TotalSeconds}, then making retry {retryAttempt} ";
                    sp.GetService<ILogger<CatalogClient>>()?.LogWarning(msg);
                }
        ))
        //first policy
        .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));


        //mechanism of both policy is that way --> at the beginig first policy check weather request has been timed out or not
        //if yes, retry policy triggers

        return services;
    }



    public static IServiceCollection AddConfiguredHttpClientWithPollyAndCircuitBreaker(this IServiceCollection services)
    {

        var sp = services.BuildServiceProvider();

        services.AddHttpClient<CatalogClient>(client =>
        {
            client.BaseAddress = catalogServiceUri;
        })
        //second policy
        .AddTransientHttpErrorPolicy(builder =>
            builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
                retryCount: 5,
                sleepDurationProvider: retryAttempt =>
                     TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(new Random().Next(0, 1000)),

                onRetry: (outcome, timespan, retryAttempt) =>
                {
                    var msg = $"Delaying for {timespan.TotalSeconds}, then making retry {retryAttempt} ";
                    sp.GetService<ILogger<CatalogClient>>()?.LogWarning(msg);
                }
        ))
        //third policy
        .AddTransientHttpErrorPolicy(builder =>
            builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 3,//send 3 requests, if fail open circuit and stop sending more requests
                durationOfBreak: TimeSpan.FromSeconds(15), //wait 15 second then  try to send request
                onBreak: (outcome, timespan) =>
                {
                    var msg = $"Opening the circuit for {timespan.TotalSeconds} seconds...";
                    sp.GetService<ILogger<CatalogClient>>()?.LogWarning(msg);
                },
                onReset: () =>
                {
                    var msg = $"Closing the circuit...";
                    sp.GetService<ILogger<CatalogClient>>()?.LogWarning(msg);
                }
        ))

        //first policy
        .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));

        return services;
    }

}