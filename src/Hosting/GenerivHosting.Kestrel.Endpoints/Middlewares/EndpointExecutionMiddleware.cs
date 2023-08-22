using GenerivHosting.Kestrel.Endpoints.Endpoints;
using GenerivHosting.Kestrel.Endpoints.Middlewares.Abstract;

namespace GenerivHosting.Kestrel.Endpoints.Middlewares;

public class EndpointExecutionMiddleware : IPipelineMiddleware
{
    public async Task Invoke(HttpApplicationContext context, IServiceScope scope, Func<Task> next)
    {
        await context.Features.Get<EndpointFeature>()!.Endpoint!.EndpointDelegate(context, scope);
    }
}
