using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using Refit;
using StackExchange.Redis;
using WebApplication1.Handlers.BackgroundHandlers.PutStoryToCache;
using WebApplication1.Interfaces;
using WebApplication1.RabbitMQ;

namespace WebApplication1.Handlers.BackgroundHandlers.GetBestStory;

public class GetBestStoryQuery : IRequest<BestStory>
{
    public int Id { get; set; }
}

public class GetBestStoryHandler : IRequestHandler<GetBestStoryQuery, BestStory>
{
    private IMediator _mediator;
    private IOptions<RedisConfiguration> _options;

    public GetBestStoryHandler(IMediator mediator, IOptions<RedisConfiguration> options)
    {
        _mediator = mediator;
        _options = options;
    }

    public async Task<BestStory> Handle(GetBestStoryQuery request, CancellationToken cancellationToken)
    {
        var options = ConfigurationOptions.Parse(_options.Value.Hostname); // host1:port1, host2:port2, ...
        options.Password = _options.Value.Password;

        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(options);
        IDatabase db = redis.GetDatabase();

        if (!db.KeyExists("story:" + request.Id))
        {
            var gitHubApi = RestService.For<IHackerNewsAPI>("https://hacker-news.firebaseio.com/");

            var story = await gitHubApi.GetBestStoryAsync(request.Id);

            await _mediator.Send(new PutStoryToCacheRequest { Story = story });
            return story;
        }
        else
        {
            return null;
        }
    }
}