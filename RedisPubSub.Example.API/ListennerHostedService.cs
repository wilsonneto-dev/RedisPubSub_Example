namespace RedisPubSub.Example.API;

public class ListennerHostedService : IHostedService, IDisposable
{
    private readonly ILogger<ListennerHostedService> _logger;
    private readonly IMessageBusService _bus;

    public ListennerHostedService(ILogger<ListennerHostedService> logger, IMessageBusService bus)
    {
        _logger = logger;
        _bus = bus;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Hosted Service running.");
        _bus.Subscribe("*", (channel, message) => {
            _logger.LogInformation($"[{channel}]: {message}");
        });
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Hosted Service stop.");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
    }
}
