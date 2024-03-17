using HackerNewsRstAPI.Data;
using HackerNewsRstAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HackerNewsRstAPI.Controllers
{
    [Route("best-stories")]
    [ApiController]
    public class HackerNewsController(INewsProvider provider) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<BestStoryModel>> GetAsync(int count = 10)
        {
            var ids = await provider.GetIdsAsync().ConfigureAwait(false);
            var stories = await provider.GetStoriesAsync(ids, count).ConfigureAwait(false);
            return stories.ToModels();
        }
    }
}
