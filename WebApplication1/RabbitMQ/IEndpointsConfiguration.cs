namespace WebApplication1.RabbitMQ
{

    public interface IEndpointsConfiguration
    {
        List<IEndpointConfiguration> Endpoints { get; set; }
    }
}