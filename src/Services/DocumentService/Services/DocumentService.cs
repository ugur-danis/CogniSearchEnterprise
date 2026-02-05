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
    IConfiguration configuration)
    : IDocumentService
{
    public async Task<Guid> CreateDocumentAsync(CreateDocumentDto dto, Guid userId)
    {
        var uploadPath = configuration["FileStorage:UploadPath"];
        var allowedExtensions = configuration["FileStorage:AllowedExtensions"]
            ?.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim().ToLowerInvariant())
            .ToArray();

        if (string.IsNullOrEmpty(uploadPath))
        {
            logger.LogInformation("FileStorage:UploadPath not set");
            throw new Exception("FileStorage:UploadPath not set");
        }

        if (allowedExtensions is null || allowedExtensions.Length == 0)
        {
            logger.LogInformation("FileStorage:AllowedExtensions not set");
            throw new Exception("FileStorage:AllowedExtensions not set");
        }

        var fileExtension = Path.GetExtension(dto.File.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
        {
            logger.LogWarning("Invalid file format: {Ext}. User: {User}", fileExtension, userId);
            throw new ArgumentException(
                $"Invalid file format! Only the following formats are accepted: {string.Join(", ", allowedExtensions)}");
        }

        // If the incoming path does not start with "C:/" or "/" (i.e., it is relative),
        // combine it with the application's root directory to make it an absolute path.
        if (!Path.IsPathRooted(uploadPath))
        {
            uploadPath = Path.Combine(Directory.GetCurrentDirectory(), uploadPath);
        }

        if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.File.FileName)}";
        var filePath = Path.Combine(uploadPath, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

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

        try
        {
            await publishEndpoint.Publish(new DocumentUploadedEvent
            {
                Id = document.Id,
                UserId = userId,
                Title = document.Title,
                FilePath = document.FilePath
            });

            logger.LogInformation("Event published to RabbitMQ: DocumentUploadedEvent");
        }
        catch (Exception e)
        {
            await MarkAsFailedAsync(document.Id, $"Failed to send to the queue: {e.Message}");
            throw new Exception("The file was uploaded, but it could not be added to the processing queue.");
        }

        return document.Id;
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