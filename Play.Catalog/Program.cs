
using MassTransit;
using Play.Catalog.Entities;
using Play.Common.ExtensionsMethods;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMongo()
    .AddMongoRepository<Item>("Items")
    .AddMassTransitWithRabbitMq();


// builder.Services.AddControllers(opt => opt.SuppressAsyncSuffixInActionNames = false);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Catalog Microservice", Version = "v1" });
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
