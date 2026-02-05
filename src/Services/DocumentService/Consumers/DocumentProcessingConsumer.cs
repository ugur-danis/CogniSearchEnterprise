using BuildingBlocks.Events;
using DocumentService.Domain.Entities;
using DocumentService.Infrastructure.Data;
using MassTransit;

namespace DocumentService.Consumers;

public class DocumentProcessingConsumer(DocumentDbContext context, ILogger<DocumentProcessingConsumer> logger)
    : IConsumer<DocumentProcessingEvent>
{
    public async Task Consume(ConsumeContext<DocumentProcessingEvent> consumeContext)
    {
        var message = consumeContext.Message;
        var document = await context.Documents.FindAsync(message.Id);

        if (document != null)
        {
            document.Status = DocumentStatus.Processing;
            document.UpdatedAt = message.StartedAt;

            await context.SaveChangesAsync();
            logger.LogInformation("Document status updated to 'Processing'.. ID: {Id}", message.Id);
        }
    }
}