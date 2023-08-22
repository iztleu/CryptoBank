using GenerivHosting.Kestrel.Endpoints.Endpoints;
using GenerivHosting.Kestrel.Endpoints.Middlewares.Abstract;

namespace GenerivHosting.Kestrel.Endpoints.Pipeline;

public interface IPipelineBuilder
{
    IPipelineBuilder Use<TMiddleware>() where TMiddleware : class, IPipelineMiddleware;
    
    IPipelineBuilder UseEndpoint(string pathPattern, Func<HttpApplicationContext, IServiceScope, Task> endpointDelegate, Dictionary<string, object>? metadata = null);
}

public class PipelineBuilder : IPipelineBuilder
{
    private readonly List<Type> _middlewareTypes = new();
    private readonly IServiceCollection _services;
    
    private readonly EndpointCollection _endpointCollection = new();

    public PipelineBuilder(IServiceCollection services)
    {
        _services = services;
    }
    
    public IPipelineBuilder Use<TMiddleware>() where TMiddleware : class, IPipelineMiddleware
    {
        _services.AddTransient<TMiddleware>();
        _middlewareTypes.Add(typeof(TMiddleware));
        return this;
    }
    
    public IPipelineBuilder UseEndpoint(string pathPattern, Func<HttpApplicationContext, IServiceScope, Task> endpointDelegate, Dictionary<string, object>? metadata = null)
    {
        var endpoint = new REndpoint(pathPattern, endpointDelegate, metadata);
        _endpointCollection.Add(endpoint);
        return this;
    }
    
    public Pipeline Build()
    {
        return new Pipeline(_middlewareTypes);
    }
}