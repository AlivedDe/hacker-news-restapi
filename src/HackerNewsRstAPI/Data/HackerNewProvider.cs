using HackerNewsRstAPI.Controllers;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace HackerNewsRstAPI.Data;

public class HackerNewProvider(IHttpClientFactory factory, IOptions<HackerNewsOptions> options, ILogger<HackerNewProvider> logger) : INewsProvider
{
    private HttpClient _client = factory.CreateClient(HackerNewsOptions.SectionName);
    private HackerNewsOptions _options = options.Value;

    public async Task<IReadOnlyCollection<int>> GetIdsAsync()
    {
        try
        {
            var response = await _client.GetAsync(_options.BestStoriesEndpoint).ConfigureAwait(false);
            if (response.StatusCode is not HttpStatusCode.OK)
            {
                return Array.Empty<int>();
            }
            var value = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (value is null)
            {
                return Array.Empty<int>();
            }
            var ids = JsonSerializer.Deserialize<int[]>(value);
            return ids?.ToArray() ?? Array.Empty<int>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get stories Ids");
            throw;
        }
    }

    public async Task<IReadOnlyCollection<BestStory>> GetStoriesAsync(IReadOnlyCollection<int> ids, int count = 10)
    {
        var idsByCount = ids.Take(count);
        var stories = idsByCount.Select(async storyId =>
        {
            return await GetStoryDetailsAsync(logger, storyId).ConfigureAwait(false);
        });
        BestStory?[] results = await Task.WhenAll(stories).ConfigureAwait(false);
        var resultedCount = results.Count(story => story is not null);
        if (resultedCount == count)
        {
            return results.Cast<BestStory>().ToArray();
        }

        var diff = count - resultedCount;
        var additional = await GetStoriesAsync(ids.Skip(resultedCount).ToArray(), diff);
        return results.Concat(additional).Cast<BestStory>().ToArray();
    }

    private async Task<BestStory?> GetStoryDetailsAsync(ILogger<HackerNewProvider> logger, int storyId)
    {
        try
        {
            var endpoint = string.Format(_options.StoryDetailsEndpoint, storyId);
            var response = await _client.GetAsync(endpoint).ConfigureAwait(false);
            if (response.StatusCode is not HttpStatusCode.OK)
            {
                return null;
            }
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var story = JsonSerializer.Deserialize<BestStory>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (story is null || story.Type is not "story" || story.Deleted || story.Dead)
            {
                return null;
            }
            return story;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get story details for Id {Id}", storyId);
            return null;
        }
    }
}
