using BuildingBlocks.Events;
using DocumentService.Domain.Entities;
using DocumentService.DTOs;
using DocumentService.Infrastructure.Data;
using MassTransit;

namespace DocumentService.Services;

public class DocumentService(
    DocumentDbContext context,
    ILogger<DocumentService> logger,
    IPublishEndpoint publishEndpoint)
    : IDocumentService
{
    public async Task<Guid> CreateDocumentAsync(CreateDocumentDto dto, Guid userId)
    {
        var document = Document.Create(
            userId,
            dto.Title,
            filePath: $"temp/{Guid.NewGuid()}_{dto.Title}",
            dto.FileSize,
            dto.ContentType
        );

        context.Documents.Add(document);
        await context.SaveChangesAsync();

        logger.LogInformation("Record inserted: {DocumentId}", document.Id);

        await publishEndpoint.Publish(new DocumentUploadedEvent
        {
            Id = document.Id,
            UserId = userId,
            Title = document.Title,
            FilePath = document.FilePath
        });

        logger.LogInformation("Event published to RabbitMQ: DocumentUploadedEvent");

        return document.Id;
    }
}