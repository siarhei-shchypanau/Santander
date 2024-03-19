using Microsoft.Extensions.Options;

namespace WebApplication1.RabbitMQ{

public interface IEndpointConfiguration
{
    void WithBinding(string exchange, string routingKey);
    IBus BuildWrapper(IServiceProvider services, IOptions<RabbitMqConfiguration> options);
}
}