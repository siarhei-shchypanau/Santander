namespace WebApplication1.RabbitMQ
{
    public interface IRpcCLient<TRequest, TResponse>
    {
        Task<TResponse> CallAsync(TRequest empty, string exchange, string queueName,
            CancellationToken cancellationToken);
    }
}