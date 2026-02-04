using BuildingBlocks.Core;

namespace DocumentService.Domain.Entities;

public enum DocumentStatus
{
    Uploaded = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3
}

public class Document : Entity
{
    public string Title { get; set; } = default!;
    public string FilePath { get; set; } = default!;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = default!;

    public DocumentStatus Status { get; set; }
    public string? VectorId { get; set; } // AI veritabanındaki ID'si
    public string? FailureReason { get; set; }
    public Guid UserId { get; set; } // Dosyayı kim yükledi?

    public Document()
    {
    }

    public static Document Create(Guid userId, string title, string filePath, long fileSize, string contentType)
    {
        if (fileSize <= 0) throw new ArgumentException("Dosya boyutu geçersiz.");

        return new Document
        {
            UserId = userId,
            Title = title,
            FilePath = filePath,
            FileSize = fileSize,
            ContentType = contentType,
            Status = DocumentStatus.Uploaded // Varsayılan durum
        };
    }

    public void MarkAsProcessing()
    {
        Status = DocumentStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsCompleted(string vectorId)
    {
        Status = DocumentStatus.Completed;
        VectorId = vectorId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string reason)
    {
        Status = DocumentStatus.Failed;
        FailureReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }
}