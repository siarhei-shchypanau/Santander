using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using WebApplication1.Interfaces;
using WebApplication1.RabbitMQ;

namespace WebApplication1.Handlers.BestStoreiesController.GetStoriesFromCache;

public class GetStoriesFromCacheRequest : IRequest<IEnumerable<BestStory>>
{
    public int Count { get; set; }
}

public class GetStoriesFromCacheHandler : IRequestHandler<GetStoriesFromCacheRequest, IEnumerable<BestStory>>
{
    private readonly IOptions<RedisConfiguration> _options;
    public GetStoriesFromCacheHandler(IOptions<RedisConfiguration> options)
    {
        _options = options;
    }


    public async Task<IEnumerable<BestStory>> Handle(GetStoriesFromCacheRequest request,
        CancellationToken cancellationToken)
    {
        var options = ConfigurationOptions.Parse(_options.Value.Hostname); // host1:port1, host2:port2, ...
        options.Password = _options.Value.Password;

        ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync(options);

        IDatabase db = redis.GetDatabase();

        var keys = db.SortedSetRangeByRank("story_scores", 0, request.Count - 1, Order.Descending);

        var keys1 = keys.Select(x => (RedisKey)("story:" + x.ToString())).ToArray();

        var values = db.StringGet(keys1);
        
        return await Task.FromResult(values.ToArray()
            .Select(x => JsonConvert.DeserializeObject<BestStory>(x.ToString())!));
    }
}