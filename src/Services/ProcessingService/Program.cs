using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.SemanticKernel;
using ProcessingService.Consumers;
using ProcessingService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AI / SEMANTIC KERNEL (GEMINI)
builder.Services.AddKernel();

// Google Gemini
var geminiApiKey = builder.Configuration["AI:GeminiKey"];
var modelId = builder.Configuration["AI:GeminiModelId"];
if (string.IsNullOrEmpty(geminiApiKey) || string.IsNullOrEmpty(modelId))
{
    throw new InvalidOperationException(
        "API Key or Model Id not found! Please run the command 'dotnet user-secrets set AI:GeminiKey | AI:ModelId ...'.");
}

builder.Services.AddGoogleAIGeminiChatCompletion(
    modelId: modelId,
    apiKey: geminiApiKey
);
builder.Services.AddScoped<IAiService, SemanticKernelService>();

// Text Read
builder.Services.AddScoped<ITextExtractorService, TextExtractorService>();

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