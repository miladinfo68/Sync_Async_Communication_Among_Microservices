using GreenPipes;
using MassTransit;
using MassTransit.Definition;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Play.Common.Abstractions.Repositories;
using Play.Common.Concretes;
using Play.Common.Entities;
using Play.Common.Settings;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Play.Common.ExtensionsMethods;

public static class CommonExtensions
{
    public static IServiceCollection AddMongo(this IServiceCollection services)
    {
        //needed config to normalize _id as a Guid and DateTimeOffset as long date in database
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));


        services.AddSingleton(serviceProvider =>
        {
            var configuration = serviceProvider.GetService<IConfiguration>();
            var serviceSettings = configuration?.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            var mongodbSettigns = configuration?.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            var mongoClient = new MongoClient(mongodbSettigns?.ConnectionString);
            return mongoClient.GetDatabase(serviceSettings?.ServiceName);
        });

        return services;
    }

    public static IServiceCollection AddMongoRepository<T>(
        this IServiceCollection services, string collectionName) where T : IEntity
    {
        services.AddSingleton<IBaseRepository<T>, MongoRepository<T>>(serviceProvider =>
        {
            var database = serviceProvider.GetService<IMongoDatabase>();
            return new MongoRepository<T>(database, collectionName);
        });

        return services;
    }

    public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
    {
        services.AddMassTransit(mt =>
        {
            mt.AddConsumers(Assembly.GetEntryAssembly());

            mt.UsingRabbitMq((cntx, cnfg) =>
            {
                var configuration = cntx.GetRequiredService<IConfiguration>();
                var rabbitMqSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                if (rabbitMqSettings != null) cnfg.Host(rabbitMqSettings.Host);
                cnfg.ConfigureEndpoints(cntx, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
                //when consumer occured to error 5 time's try to doing opertion
                cnfg.UseMessageRetry(reTryConfig =>
                {
                    //try at most 5 time to perform operation in consumer
                    reTryConfig.Interval(5, TimeSpan.FromSeconds(new Random(5).Next(1, 10)));
                });
            });
        });

        //start rabbitmq buss
        services.AddMassTransitHostedService();
        return services;
    }

}

