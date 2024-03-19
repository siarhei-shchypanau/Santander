using Refit;
using WebApplication1.Interfaces;

namespace TestProject;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task GetBestStoreies()
    {
        var gitHubApi = RestService.For<IHackerNewsAPI>("https://hacker-news.firebaseio.com/");
        
        var stories = await gitHubApi.GetBestStoriesAsync();
        
        Assert.IsTrue(stories.Count > 0);

    }
    
    [Test]
    public  async Task GetBestStory()
    {
        var gitHubApi = RestService.For<IHackerNewsAPI>("https://hacker-news.firebaseio.com/");
        
        var story = await gitHubApi.GetBestStoryAsync(21233041);
        
        Assert.IsTrue(story.Id == 21233041);
    }
}