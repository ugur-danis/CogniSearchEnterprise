namespace BuildingBlocks.Events;

public record DocumentProcessingEvent
{
    public Guid Id { get; init; }
    public DateTime StartedAt { get; init; }
}