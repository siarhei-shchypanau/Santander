using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;
using WebApplication1.Interfaces;
using WebApplication1.RabbitMQ;

namespace WebApplication1.Handlers.BackgroundHandlers.PutStoryToCache;

public class PutStoryToCacheRequest : IRequest
{
    public BestStory Story { get; set; }
    
}

public class PutStoryToCacheHandler : IRequestHandler<PutStoryToCacheRequest>
{
    private IOptions<RedisConfiguration> _options;

    public PutStoryToCacheHandler(IOptions<RedisConfiguration> options)
    {
        _options = options;
    }

    public Task Handle(PutStoryToCacheRequest request, CancellationToken cancellationToken)
    {
        var options = ConfigurationOptions.Parse(_options.Value.Hostname); // host1:port1, host2:port2, ...
        options.Password = _options.Value.Password;      
        
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(options);
        IDatabase db = redis.GetDatabase();
        
        db.StringSet($"story:{request.Story.Id}", JsonConvert.SerializeObject(request.Story));
        db.SortedSetAdd("story_scores", $"{request.Story.Id}", request.Story.Score);
        
        return Task.CompletedTask;
    }
}