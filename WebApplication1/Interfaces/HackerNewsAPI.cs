using Refit;

namespace WebApplication1.Interfaces;

public interface IHackerNewsAPI
{
    [Get("/v0/beststories.json")]
    Task<List<int>> GetBestStoriesAsync();
   
    [Get("/v0/item/{id}.json")]
    Task<BestStory> GetBestStoryAsync(int id);
}