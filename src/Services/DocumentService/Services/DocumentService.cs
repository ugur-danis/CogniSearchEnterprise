using BuildingBlocks.Core;
using BuildingBlocks.Events;
using DocumentService.Domain.Entities;
using DocumentService.DTOs;
using DocumentService.Infrastructure.Data;
using MassTransit;

namespace DocumentService.Services;

public class DocumentService(
    DocumentDbContext context,
    ILogger<DocumentService> logger,
    IPublishEndpoint publishEndpoint,
    IFileStorageService fileStorageService)
    : IDocumentService
{
    public async Task<Result<Guid>> CreateDocumentAsync(CreateDocumentDto dto, Guid userId)
    {
        try
        {
            var filePath = await fileStorageService.SaveFileAsync(dto.File);

            var document = Document.Create(
                userId,
                dto.Title,
                filePath,
                dto.File.Length,
                dto.File.ContentType
            );

            context.Documents.Add(document);
            await context.SaveChangesAsync();

            logger.LogInformation("Record inserted: {DocumentId}", document.Id);

            await PublishDocumentUploadedEventAsync(document);

            return Result<Guid>.Success(document.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create document for user {UserId}", userId);
            throw;
        }
    }

    private async Task PublishDocumentUploadedEventAsync(Document document)
    {
        try
        {
            await publishEndpoint.Publish(new DocumentUploadedEvent
            {
                Id = document.Id,
                UserId = document.UserId,
                Title = document.Title,
                FilePath = document.FilePath
            });

            logger.LogInformation("Event published to RabbitMQ: DocumentUploadedEvent");
        }
        catch (Exception e)
        {
            await MarkAsFailedAsync(document.Id, $"Failed to send to the queue: {e.Message}");

            throw new Exception("The file was uploaded, but it could not be added to the processing queue.", e);
        }
    }

    private async Task MarkAsFailedAsync(Guid id, string reason)
    {
        var document = await context.Documents.FindAsync(id);
        if (document != null)
        {
            document.Status = DocumentStatus.Failed;
            document.FailureReason = reason;
            document.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
        }
    }
}