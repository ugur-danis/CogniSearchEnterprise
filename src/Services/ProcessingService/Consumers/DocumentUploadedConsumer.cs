using BuildingBlocks.Events;
using MassTransit;

namespace ProcessingService.Consumers;

public class DocumentUploadedConsumer(ILogger<DocumentUploadedConsumer> logger) : IConsumer<DocumentUploadedEvent>
{
    public async Task Consume(ConsumeContext<DocumentUploadedEvent> context)
    {
        var message = context.Message;

        logger.LogInformation("[ProcessingService] New file detected! ID: {Id}, Title: {Title}", message.Id,
            message.Title);

        // TODO: Gerçekte burada Semantic Kernel ile PDF okuyup özet çıkaracağız.
        // Let’s wait 5 seconds here as if the AI were working (Simulation).
        logger.LogInformation("Starting AI analysis...");
        await Task.Delay(5000);

        logger.LogInformation("AI analysis completed. Vectors have been generated.");
    }
}