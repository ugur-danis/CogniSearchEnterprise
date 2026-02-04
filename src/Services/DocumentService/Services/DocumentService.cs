using DocumentService.Domain.Entities;
using DocumentService.DTOs;
using DocumentService.Infrastructure.Data;

namespace DocumentService.Services;

public class DocumentService(DocumentDbContext context, ILogger<DocumentService> logger) : IDocumentService
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

        logger.LogInformation("Document created successfully with ID: {DocumentId}", document.Id);

        return document.Id;
    }
}