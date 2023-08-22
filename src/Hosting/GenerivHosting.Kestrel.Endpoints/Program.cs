using GenerivHosting.Kestrel.Endpoints.Controllers.Extensions;
using GenerivHosting.Kestrel.Endpoints.HostedService;
using GenerivHosting.Kestrel.Endpoints.Middlewares;
using GenerivHosting.Kestrel.Endpoints.Pipeline;
using Hosting.Services.DI.Repository;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<KestrelHostedServicePipeline>();
            services.AddTransient<IDepositRepository, DepositRepository>();
        }
    ).AddPipeline(builder => builder
        .Use<LoggingMiddleware>()
        .Use<ExceptionPageMiddleware>()
        .Use<RoutingMiddleware>()
        .Use<RateLimitingMiddleware>()
        .Use<EndpointExecutionMiddleware>()
        .UseEndpoint("/exception", (context, scope) =>
        {
            throw new Exception("You hit the exception route");
        })
        .UseControllerEndpoints()
    ).Build();
    
await  host.RunAsync();