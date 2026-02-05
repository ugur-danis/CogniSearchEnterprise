using BuildingBlocks.Events;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using ProcessingService.Models;

namespace ProcessingService.Consumers;

public class DocumentUploadedConsumer(ILogger<DocumentUploadedConsumer> logger, ElasticsearchClient elasticClient)
    : IConsumer<DocumentUploadedEvent>
{
    public async Task Consume(ConsumeContext<DocumentUploadedEvent> context)
    {
        var message = context.Message;
        logger.LogInformation("Processing document: {Title}", message.Title);

        // TODO: Use AI to process/read the text here.
        var simulatedContent =
            $"This document is about {message.Title}. Its content includes artificial intelligence and backend topics.";
        const string simulatedSummary = "Automatically generated summary by artificial intelligence.";

        var indexModel = new DocumentIndexModel
        {
            Id = message.Id,
            UserId = message.UserId,
            Title = message.Title,
            Description = simulatedSummary,
            Content = simulatedContent,
            CreatedAt = DateTime.UtcNow
        };

        var response = await elasticClient.IndexAsync(indexModel);
        if (response.IsValidResponse)
        {
            logger.LogInformation("The document was successfully indexed into Elasticsearch! Index: {Index}",
                response.Index);
        }
        else
        {
            logger.LogError("Elasticsearch error: {DebugInformation}", response.DebugInformation);
        }
    }
}