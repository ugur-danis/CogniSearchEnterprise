namespace BuildingBlocks.Events;

public record DocumentCompletedEvent
{
    public Guid Id { get; init; }

    public DateTime CompletedAt { get; init; }
}