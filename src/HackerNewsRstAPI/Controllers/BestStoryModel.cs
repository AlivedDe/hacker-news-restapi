public record BestStoryModel
{
    public string Title { get; init; }
    public string Uri { get; init; }
    public string PostedBy { get; init; }
    public DateTime Time { get; init; }
    public int Score { get; init; }
    public int CommentCount { get; init; }
}