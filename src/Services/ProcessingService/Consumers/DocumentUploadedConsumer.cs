using BuildingBlocks.Events;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using ProcessingService.Models;
using ProcessingService.Services;

namespace ProcessingService.Consumers;

public class DocumentUploadedConsumer(
    ILogger<DocumentUploadedConsumer> logger,
    ElasticsearchClient elasticClient,
    IAiService aiService,
    ITextExtractorService textExtractor,
    IPublishEndpoint publishEndpoint)
    : IConsumer<DocumentUploadedEvent>
{
    public async Task Consume(ConsumeContext<DocumentUploadedEvent> context)
    {
        var message = context.Message;
        logger.LogInformation("Document is being processed: {Title}", message.Title);

        await publishEndpoint.Publish(new DocumentProcessingEvent
        {
            Id = message.Id,
            StartedAt = DateTime.UtcNow
        });

        var fileContent = await textExtractor.ExtractTextAsync(message.FilePath);

        if (string.IsNullOrWhiteSpace(fileContent) || fileContent.Length < 10)
        {
            logger.LogWarning("File content is empty or too short, AI summary is skipped.");
            await publishEndpoint.Publish(new DocumentCompletedEvent
            {
                Id = message.Id,
                CompletedAt = DateTime.UtcNow
            });
        }

        var contentForAi = fileContent.Length > 5000 ? fileContent[..5000] : fileContent;

        logger.LogInformation("Generating AI summary (Text Length: {Len})...", contentForAi.Length);

        var aiSummary = await aiService.SummarizeAsync(contentForAi);

        logger.LogInformation("AI summary:{AiSummary}", aiSummary);

        var indexModel = new DocumentIndexModel
        {
            Id = message.Id,
            UserId = message.UserId,
            Title = message.Title,
            Description = aiSummary,
            Content = fileContent,
            CreatedAt = DateTime.UtcNow
        };

        await elasticClient.IndexAsync(indexModel);
        logger.LogInformation("Elastic Index Successfully!");

        await publishEndpoint.Publish(new DocumentCompletedEvent
        {
            Id = message.Id,
            CompletedAt = DateTime.UtcNow
        });

        logger.LogInformation("DocumentCompletedEvent was fired.");
    }
}