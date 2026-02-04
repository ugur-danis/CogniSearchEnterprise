using DocumentService.DTOs;

namespace DocumentService.Services;

public interface IDocumentService
{
    Task<Guid> CreateDocumentAsync(CreateDocumentDto dto, Guid userId);
}