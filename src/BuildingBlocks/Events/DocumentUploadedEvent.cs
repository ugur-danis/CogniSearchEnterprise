namespace BuildingBlocks.Events;

public record DocumentUploadedEvent
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Title { get; init; } = default!;
    public string FilePath { get; init; } = default!;
}