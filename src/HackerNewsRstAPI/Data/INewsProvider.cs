using System.Linq;

namespace HackerNewsRstAPI.Data;

public interface INewsProvider
{
    Task<IReadOnlyCollection<int>> GetIdsAsync();
    Task<IReadOnlyCollection<BestStory>> GetStoriesAsync(IReadOnlyCollection<int> ids, int count = 10);
}
