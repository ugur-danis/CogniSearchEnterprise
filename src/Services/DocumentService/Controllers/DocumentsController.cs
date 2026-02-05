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
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromForm] CreateDocumentDto request)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return BadRequest("File is required");
        }

        var userId = Guid.NewGuid(); // In a real app, extract from claims
        var result = await documentService.CreateDocumentAsync(request, userId);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(Create), new { id = result.Value }, result.Value);
        }

        return BadRequest(result.ErrorMessage);
    }
}