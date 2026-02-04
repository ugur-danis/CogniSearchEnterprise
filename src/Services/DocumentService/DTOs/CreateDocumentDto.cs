namespace DocumentService.DTOs;

public record CreateDocumentDto(
    string Title,
    string Description,
    long FileSize,
    string ContentType
);

public record DocumentResponseDto(
    Guid Id,
    string Title,
    string Status,
    DateTime CreatedAt
);