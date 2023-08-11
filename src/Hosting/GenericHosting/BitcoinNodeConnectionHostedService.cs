using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GenericHosting;

public class BitcoinNodeConnectionHostedService : IHostedService
{
    private readonly ILogger<BitcoinNodeConnectionHostedService> _logger;

    public BitcoinNodeConnectionHostedService(ILogger<BitcoinNodeConnectionHostedService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Connection to Bitcoin node opened");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Connection to Bitcoin node closed");

        return Task.CompletedTask;
    }
}
