using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GenericHosting.Kestrel;

public class KestrelHostedService : IHostedService
{
    
    private const int Port = 8080;

    private readonly ILoggerFactory _loggerFactory;
    private KestrelServer? _server;
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var transportOptions = new OptionsWrapper<SocketTransportOptions>(new SocketTransportOptions());
        var transportFactory = new SocketTransportFactory(transportOptions, _loggerFactory);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_server is not null)
        {
            await _server.StopAsync(cancellationToken);
            _server.Dispose();
        }
    }
}