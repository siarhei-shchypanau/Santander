using MediatR;
using Refit;
using WebApplication1.Interfaces;

namespace WebApplication1.Handlers.BestStoreiesController.GetBestStories;

public class GetBestStoriesQuery : IRequest<IEnumerable<int>>
{
}


public class GetBestStoriesHandler : IRequestHandler<GetBestStoriesQuery, IEnumerable<int>>
{
    public async Task<IEnumerable<int>> Handle(GetBestStoriesQuery request, CancellationToken cancellationToken)
    {
        var gitHubApi = RestService.For<IHackerNewsAPI>("https://hacker-news.firebaseio.com/");
        
        var stories = await gitHubApi.GetBestStoriesAsync();
       
        return stories;
    }
}