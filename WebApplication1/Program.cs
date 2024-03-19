using MassTransit;
using WebApplication1.Handlers.BackgroundHandlers.GetBestStory;
using WebApplication1.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.Configure<RedisConfiguration>(builder.Configuration.GetSection("Redis"));

builder.Services.AddRabbit(configuration: builder.Configuration);

builder.Services.AddRabbitMqEndpoints(cfg =>
{
    cfg.MapEndpoint<GetBestStoryQuery>(
        queue: "get-best-story",
        durable: true,
        exclusive: false,
        autoDelete: false,
        arguments: null).WithBinding("story", "");
});

builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(typeof(Program).Assembly); });

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.MapControllers();


app.Run();