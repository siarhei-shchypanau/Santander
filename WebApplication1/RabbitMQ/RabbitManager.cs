using System.Collections.Concurrent;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WebApplication1.RabbitMQ;

public class RpcClient<TRequest, TResponse> : IRpcCLient<TRequest, TResponse>, IDisposable
{
    private const string QUEUE_NAME = "rpc_queue";

    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string replyQueueName;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<TResponse>> callbackMapper = new();

    public RpcClient(IOptions<RabbitMqConfiguration> options)
    {
        var factory = new ConnectionFactory
        {
            HostName = options.Value.Hostname,
            UserName = options.Value.UserName,
            Password = options.Value.Password,
            Port = AmqpTcpEndpoint.UseDefaultPort,
            VirtualHost = options.Value.VHost
        };

        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        // declare a server-named queue
        replyQueueName = channel.QueueDeclare().QueueName;
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            if (!callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out var tcs))
                return;
            var body = ea.Body.ToArray();
            var response = JsonConvert.DeserializeObject<TResponse>(Encoding.UTF8.GetString(body));
            tcs.TrySetResult(response);
        };

        channel.BasicConsume(consumer: consumer,
            queue: replyQueueName,
            autoAck: true);
    }

    public Task<TResponse> CallAsync(TRequest message,string exchange = default,string queueName=default,
        CancellationToken cancellationToken = default)
    {
        IBasicProperties props = channel.CreateBasicProperties();
        var correlationId = Guid.NewGuid().ToString();
        props.CorrelationId = correlationId;
        props.ReplyTo = replyQueueName;
        var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
        var tcs = new TaskCompletionSource<TResponse>();

        callbackMapper.TryAdd(correlationId, tcs);

        channel.BasicPublish(exchange: exchange,
            routingKey: queueName,
            basicProperties: props,
            body: messageBytes);

        cancellationToken.Register(() => callbackMapper.TryRemove(correlationId, out _));
        return tcs.Task;
    }

    public void Dispose()
    {
        connection.Close();
    }
}