namespace HackerNewsRstAPI.Data;

public record BestStory
{
    public int Id { get; init; }
    public string? Title { get; init; }
    public bool Deleted { get; init; }
    public bool Dead { get; init; }
    public string? Type { get; init; }
    public string? By { get; init; }
    public Uri? Url { get; init; }
    public long Time { get; init; }
    public int Score { get; init; }
    public int[]? Kids { get; init; }
}
