namespace WebApplication1.RabbitMQ{

public interface IBus
{
    public Task ExecuteAsync(CancellationToken stoppingToken);
}
}