using BuildingBlocks.Core;
using DocumentService.Options;
using Microsoft.Extensions.Options;

namespace DocumentService.Services;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file, CancellationToken cancellationToken = default);
}

public class FileStorageService(IOptions<FileStorageOptions> options, ILogger<FileStorageService> logger)
    : IFileStorageService
{
    private readonly FileStorageOptions _options = options.Value;

    public async Task<string> SaveFileAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowedExtensions = _options.AllowedExtensions
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim().ToLowerInvariant())
            .ToArray();

        if (!allowedExtensions.Contains(extension))
        {
            logger.LogWarning("Invalid file format attempted: {Extension}", extension);
            throw new ValidationException($"Invalid file format! Allowed: {string.Join(", ", allowedExtensions)}");
        }

        var uploadPath = _options.UploadPath;
        if (!Path.IsPathRooted(uploadPath))
        {
            uploadPath = Path.Combine(Directory.GetCurrentDirectory(), uploadPath);
        }

        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadPath, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        logger.LogInformation("File saved to {FilePath}", filePath);
        
        return filePath;
    }
}
