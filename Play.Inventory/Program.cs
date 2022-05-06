using Play.Inventory.Client;
using Play.Inventory.Entities;
using Play.Inventory.Extensions;
using Play.Common.ExtensionsMethods;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMongo()
    .AddMongoRepository<InventoryItem>("InventoryItems")
    .AddMongoRepository<CatalogItem>("CatalogItems")
    .AddMassTransitWithRabbitMq();

// builder.Services.AddConfiguredHttpClient();
//builder.Services.AddConfiguredHttpClientWithPolly();
builder.Services.AddConfiguredHttpClientWithPollyAndCircuitBreaker();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Inventory Microservice", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
