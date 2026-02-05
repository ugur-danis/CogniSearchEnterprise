using BuildingBlocks.Events;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using ProcessingService.Models;
using ProcessingService.Services;

namespace ProcessingService.Consumers;

public class DocumentUploadedConsumer(
    ILogger<DocumentUploadedConsumer> logger,
    ElasticsearchClient elasticClient,
    IAiService aiService)
    : IConsumer<DocumentUploadedEvent>
{
    public async Task Consume(ConsumeContext<DocumentUploadedEvent> context)
    {
        var message = context.Message;
        logger.LogInformation("Document is being processed: {Title}", message.Title);

        const string fileContent = $$"""
                                     This document is about the {message.Title}. 
                                     The project is built around modern cloud architectures, microservices, and AI integration.
                                     "RabbitMQ is used as the message queue, and Elasticsearch as the search engine.
                                     "The goal is to intelligently manage corporate documents.
                                     """;

        logger.LogInformation("AI summary is being generated...");

        string aiSummary;
        try
        {
            aiSummary = await aiService.SummarizeAsync(fileContent);
            logger.LogInformation("AI Summary: {Summary}", aiSummary);
        }
        catch (Exception ex)
        {
            logger.LogError("AI Error: {Message}. Using default summary.", ex.Message);
            aiSummary = "Failed to generate automatic summary.";
        }

        var indexModel = new DocumentIndexModel
        {
            Id = message.Id,
            UserId = message.UserId,
            Title = message.Title,
            Description = aiSummary,
            Content = fileContent,
            CreatedAt = DateTime.UtcNow
        };

        var response = await elasticClient.IndexAsync(indexModel);

        if (response.IsValidResponse)
            logger.LogInformation("✅ Elastic Index Successfully!");
    }
}