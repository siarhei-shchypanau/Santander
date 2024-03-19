using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;

namespace WebApplication1.RabbitMQ
{
    public static class RabbitServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbit(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqConfiguration>(configuration.GetSection("RabbitMQ"));

            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddSingleton<IPooledObjectPolicy<IModel>, RabbitModelPooledObjectPolicy>();

            services.AddTransient(typeof(IRpcCLient<,>), typeof(RpcClient<,>));

            return services;
        }

        public static void AddRabbitMqEndpoints(this IServiceCollection services,
            Action<EndpointsConfiguration> configuration)
        {
            var cfg = new EndpointsConfiguration();
            configuration(cfg);
            services.AddSingleton<IEndpointsConfiguration>(cfg);
            services.AddHostedService<RabbitMQHostedService>();
        }
    }
}