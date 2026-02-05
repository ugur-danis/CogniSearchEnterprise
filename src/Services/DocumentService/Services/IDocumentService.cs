using BuildingBlocks.Core;
using DocumentService.DTOs;

namespace DocumentService.Services;

public interface IDocumentService
{
    Task<Result<Guid>> CreateDocumentAsync(CreateDocumentDto dto, Guid userId);
}