
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace HackerNewsRstAPI.Data;

public class RedisNewsProvider(INewsProvider provider, IConnectionMultiplexer connection, IDatabase cache, IOptions<RedisOptions> options) : INewsProvider
{
    private readonly RedisOptions _options = options.Value;
    public Task<IReadOnlyCollection<int>> GetIdsAsync()
    {
        return GetIdsWithCacheAsync();
    }

    public Task<IReadOnlyCollection<BestStory>> GetStoriesAsync(IReadOnlyCollection<int> ids, int count = 10)
    {
        return GetStoriesWithCacheAsync(ids, count);
    }

    private async Task<IReadOnlyCollection<int>> GetIdsWithCacheAsync()
    {
        var key = "ids";
        string? idsString = await cache.StringGetAsync(key).ConfigureAwait(false);
        if (idsString is null)
        {
            var originalIds = await provider.GetIdsAsync().ConfigureAwait(false);
            idsString = JsonSerializer.Serialize(originalIds);
            await cache.StringSetAsync(key, idsString, expiry: TimeSpan.FromHours(_options.ExpiryHours)).ConfigureAwait(false);
        }

        return JsonSerializer.Deserialize<int[]>(idsString) ?? Array.Empty<int>();
    }

    private async Task<IReadOnlyCollection<BestStory>> GetStoriesWithCacheAsync(IReadOnlyCollection<int> ids, int count = 10)
    {
        var keyPrefix = "story:";
        var endpoint = connection.GetEndPoints().First();
        List<RedisKey> keys = connection.GetServer(endpoint).Keys(pattern: $"{keyPrefix}*").ToList();
        if (keys.Count < count)
        {
            var foundIds = keys.Select(x => int.Parse(x.ToString().Substring(keyPrefix.Length)));
            var missingIds = ids.Except(foundIds).ToArray();
            var missingCount = count - keys.Count;
            var originalStories = await provider.GetStoriesAsync(missingIds, missingCount).ConfigureAwait(false);
            var addStories = originalStories.Select(async originalStory =>
            {
                var storyString = JsonSerializer.Serialize(originalStory);
                var addedKey = new RedisKey($"{keyPrefix}{originalStory.Id}");
                await cache.StringSetAsync(addedKey, storyString, expiry: TimeSpan.FromHours(_options.ExpiryHours)).ConfigureAwait(false);
                return addedKey;
            });

            var additionalKeys = await Task.WhenAll(addStories).ConfigureAwait(false);
            keys.AddRange(additionalKeys);
        }

        var tasks = keys.Take(count).Select(async x =>
        {
            string? value = await cache.StringGetAsync(x).ConfigureAwait(false);
            if (value is null)
            {
                return null;
            }
            var story = JsonSerializer.Deserialize<BestStory>(value);
            return story;
        });

        var stories = await Task.WhenAll(tasks).ConfigureAwait(false);
        return stories.Cast<BestStory>().ToArray();
    }
}
