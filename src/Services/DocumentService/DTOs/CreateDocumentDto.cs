namespace DocumentService.DTOs;

public record CreateDocumentDto(
    string Title,
    IFormFile File
);

public record DocumentResponseDto(
    Guid Id,
    string Title,
    string Status,
    DateTime CreatedAt
);