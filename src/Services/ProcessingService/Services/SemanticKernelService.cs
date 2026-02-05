using Microsoft.SemanticKernel;

namespace ProcessingService.Services;

public class SemanticKernelService(Kernel kernel) : IAiService
{
    public async Task<string> SummarizeAsync(string text)
    {
        // AI'ya ne yapması gerektiğini söylüyoruz (Prompt Engineering)
        var prompt = $"""
                      Aşağıdaki metni analiz et ve Türkçe olarak en fazla 2 cümlelik, 
                      profesyonel bir özet çıkar.

                      Metin:
                      {text}
                      """;

        // Kernel üzerinden AI'yı çağırıyoruz
        var result = await kernel.InvokePromptAsync(prompt);

        return result.ToString();
    }
}