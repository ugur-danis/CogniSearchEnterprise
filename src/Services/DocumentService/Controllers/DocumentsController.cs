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
    public async Task<IActionResult> Create([FromForm] CreateDocumentDto request)
    {
        if (request.File.Length == 0)
        {
            return BadRequest("File is required");
        }

        var userId = Guid.NewGuid();
        var result = await documentService.CreateDocumentAsync(request, userId);
        return Ok(result);
    }
}