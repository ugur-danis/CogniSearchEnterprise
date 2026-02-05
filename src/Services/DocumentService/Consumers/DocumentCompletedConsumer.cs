using BuildingBlocks.Events;
using DocumentService.Domain.Entities;
using DocumentService.Infrastructure.Data;
using MassTransit;

namespace DocumentService.Consumers;

public class DocumentCompletedConsumer(DocumentDbContext context, ILogger<DocumentCompletedConsumer> logger)
    : IConsumer<DocumentCompletedEvent>
{
    public async Task Consume(ConsumeContext<DocumentCompletedEvent> consumeContext)
    {
        var message = consumeContext.Message;

        var document = await context.Documents.FindAsync(message.Id);

        if (document == null)
        {
            logger.LogError("Completed document not found in the database! ID: {Id}", message.Id);
            return;
        }

        document.Status = DocumentStatus.Completed;
        document.UpdatedAt = message.CompletedAt;

        await context.SaveChangesAsync();

        logger.LogInformation("Document status updated to 'Completed'. ID: {Id}", message.Id);
    }
}