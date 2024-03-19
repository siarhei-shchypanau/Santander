using MediatR;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using WebApplication1.Handlers.BackgroundHandlers.GetBestStory;
using WebApplication1.Handlers.BestStoreiesController.GetBestStories;
using WebApplication1.Handlers.BestStoreiesController.GetStoriesFromCache;
using WebApplication1.Interfaces;
using WebApplication1.RabbitMQ;
using IBus = MassTransit.IBus;

namespace WebApplication1.Handlers.BestStoreiesController.Get;

public class GetRequest : IRequest<List<BestStory>>
{
    public int Count { get; set; }
}

public class GetHandler : IRequestHandler<GetRequest, List<BestStory>>
{
    readonly IMediator _mediator;
    private IRpcCLient<GetBestStoryQuery, BestStory> _cLient;
    private readonly IOptions<RedisConfiguration> _options;


    public GetHandler(IMediator mediator, IRpcCLient<GetBestStoryQuery, BestStory> cLient,
        IOptions<RedisConfiguration> options)
    {
        _mediator = mediator;
        _cLient = cLient;
        _options = options;
    }


    public async Task<List<BestStory>> Handle(GetRequest request, CancellationToken cancellationToken)
    {
        var list = await _mediator.Send(new GetBestStoriesQuery());

        Console.WriteLine($"connect to redis {_options.Value.Hostname}");
        var options = ConfigurationOptions.Parse(_options.Value.Hostname); // host1:port1, host2:port2, ...
        options.Password = _options.Value.Password;

        ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync(options);

        var keys = redis.GetServer(_options.Value.Hostname).Keys().Select(x => x.ToString()).Where(x => x.StartsWith("story:"))
            .ToList();

        var cachedkeys = keys
            .Select(x => int.Parse(x.Replace("story:", ""))).ToList();


        List<int> notexist = list.Where(i => cachedkeys.Contains(i) == false).ToList();


        if (notexist.Count > 0)
        {
            Task.WaitAll(notexist.Select(x =>
                _cLient.CallAsync(new GetBestStoryQuery() { Id = x }, exchange: "", queueName: "get-best-story",
                    cancellationToken)).ToArray(), cancellationToken);
        }

        var stories = await _mediator.Send(new GetStoriesFromCacheRequest { Count = request.Count });

        return stories.ToList();
    }
}