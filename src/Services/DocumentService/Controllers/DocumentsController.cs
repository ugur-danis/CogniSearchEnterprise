using DocumentService.DTOs;
using DocumentService.Services;
using Microsoft.AspNetCore.Mvc;

namespace DocumentService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController(IDocumentService documentService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateDocumentDto request)
    {
        var userId = Guid.NewGuid();
        var documentId = await documentService.CreateDocumentAsync(request, userId);
        return CreatedAtAction(nameof(Create), new { id = documentId }, new { id = documentId });
    }
}