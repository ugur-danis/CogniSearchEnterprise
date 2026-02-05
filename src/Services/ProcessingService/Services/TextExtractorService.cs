using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace ProcessingService.Services;

public class TextExtractorService(ILogger<TextExtractorService> logger) : ITextExtractorService
{
    public async Task<string> ExtractTextAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            logger.LogWarning("File not found: {Path}", filePath);
            return string.Empty;
        }

        var extension = Path.GetExtension(filePath).ToLower();

        return extension switch
        {
            ".pdf" => ExtractFromPdf(filePath),
            ".txt" or ".md" => await File.ReadAllTextAsync(filePath),
            _ => "This file format is not supported."
        };
    }

    private string ExtractFromPdf(string filePath)
    {
        try
        {
            using var pdf = PdfDocument.Open(filePath);

            return pdf.GetPages().Aggregate(string.Empty,
                (current, page) => current + (ContentOrderTextExtractor.GetText(page) + " "));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "PDF reading error.");
            return "Failed to read the file.";
        }
    }
}