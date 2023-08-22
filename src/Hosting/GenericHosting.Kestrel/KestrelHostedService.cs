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
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private KestrelServer? _server;

    public KestrelHostedService(ILoggerFactory loggerFactory, IServiceScopeFactory serviceScopeFactory)
    {
        _loggerFactory = loggerFactory;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var transportOptions = new OptionsWrapper<SocketTransportOptions>(new SocketTransportOptions());
        var transportFactory = new SocketTransportFactory(transportOptions, _loggerFactory);

        var serverOptions = new OptionsWrapper<KestrelServerOptions>(new());
        serverOptions.Value.ListenAnyIP(Port);

        _server = new KestrelServer(serverOptions, transportFactory, _loggerFactory);

        await _server.StartAsync(new HttpApplication(_serviceScopeFactory), cancellationToken);
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