using System.Reflection;
using GenerivHosting.Kestrel.Endpoints.Controllers.Attributes;
using GenerivHosting.Kestrel.Endpoints.Endpoints;
using GenerivHosting.Kestrel.Endpoints.Middlewares.Abstract;
using GenerivHosting.Kestrel.Endpoints.Pipeline;

namespace GenerivHosting.Kestrel.Endpoints.Controllers.Extensions;

public static class ControllerEndpointsExtensions
{
    public static IPipelineBuilder UseControllerEndpoints(this IPipelineBuilder pipelineBuilder)
    {
        Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => x.IsSubclassOf(typeof(Controller)))
            .ToList()
            .ForEach(x => pipelineBuilder.UseSingleControllerEndpoints(x));

        return pipelineBuilder;
    }

    public static IPipelineBuilder UseSingleControllerEndpoints(this IPipelineBuilder pipelineBuilder, Type controllerType)
    {
        var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (var method in methods)
        {
            var pathAttribute = method.GetCustomAttribute<PathAttribute>()!;
            var rateLimitingAttribute = method.GetCustomAttribute<RateLimitingAttribute>();
            Dictionary<string, object> metadata = new();
            if (rateLimitingAttribute is not null)
            {
                metadata[EndpointMetadataKeys.RateLimitingInverval] = rateLimitingAttribute.IntervalMs;
            }
            var endpointDelegate = CreateEndpointDelegate(controllerType, method);
            pipelineBuilder.UseEndpoint(pathAttribute.Path, endpointDelegate, metadata);
        }

        return pipelineBuilder;
    }

    private static Func<HttpApplicationContext, IServiceScope, Task> CreateEndpointDelegate(Type controllerType, MethodInfo method)
    {
        var controllerInstance = Activator.CreateInstance(controllerType);
        return (Func<HttpApplicationContext, IServiceScope, Task>)method.CreateDelegate(typeof(Func<HttpApplicationContext, IServiceScope, Task>), controllerInstance);
    }
}