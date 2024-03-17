namespace HackerNewsRstAPI.Controllers;

public class HackerNewsOptions
{
    public const string SectionName = "HackerNews";
    public required Uri Url { get; init; }
    public required string BestStoriesEndpoint { get; init; }
    public required string StoryDetailsEndpoint { get; init; }
}
