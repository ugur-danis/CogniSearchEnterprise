using Microsoft.SemanticKernel;

namespace ProcessingService.Services;

public class SemanticKernelService(Kernel kernel) : IAiService
{
    public async Task<string> SummarizeAsync(string text)
    {
        var prompt = $"""
                      Aşağıdaki metni analiz et ve Türkçe olarak en fazla 2 cümlelik, 
                      profesyonel bir özet çıkar.

                      Metin:
                      {text}
                      """;

        var result = await kernel.InvokePromptAsync(prompt);
        return result.ToString();
    }
}