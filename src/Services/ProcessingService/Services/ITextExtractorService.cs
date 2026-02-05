namespace ProcessingService.Services;

public interface ITextExtractorService
{
    Task<string> ExtractTextAsync(string filePath);
}