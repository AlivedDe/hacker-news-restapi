using HackerNewsRstAPI.Data;

namespace HackerNewsRstAPI.Extensions;

public static class BestStoriesModelExtensions
{

    public static IReadOnlyCollection<BestStoryModel> ToModels(this IEnumerable<BestStory>? stories)
    {
        if (stories is null)
        {
            return Array.Empty<BestStoryModel>();
        }
        return stories.AsParallel().Select(story => {
            return new BestStoryModel
            {
                CommentCount = story.Kids?.Length ?? 0,
                PostedBy = story.By ?? string.Empty,
                Score = story.Score,
                Time = DateTime.UnixEpoch.AddMilliseconds(story.Time),
                Title = story.Title ?? string.Empty,
                Uri = story.Url?.ToString() ?? string.Empty,
            };
        }).ToList();
    }
}
