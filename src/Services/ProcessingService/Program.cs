using Elastic.Clients.Elasticsearch;
using MassTransit;
using ProcessingService.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Elasticsearch
builder.Services.AddSingleton<ElasticsearchClient>(_ =>
{
    var settings = new ElasticsearchClientSettings(new Uri("http://localhost:9200"))
        .DefaultIndex("documents");
    return new ElasticsearchClient(settings);
});

// RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<DocumentUploadedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("admin");
            h.Password("admin");
        });
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();