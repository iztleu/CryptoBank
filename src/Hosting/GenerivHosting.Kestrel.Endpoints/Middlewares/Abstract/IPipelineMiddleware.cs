using Microsoft.AspNetCore.Http.Features;

namespace GenerivHosting.Kestrel.Endpoints.Middlewares.Abstract;

public interface IPipelineMiddleware
{
    Task Invoke(HttpApplicationContext context, IServiceScope scope, Func<Task> next);
}

public record HttpApplicationContext(IFeatureCollection Features);