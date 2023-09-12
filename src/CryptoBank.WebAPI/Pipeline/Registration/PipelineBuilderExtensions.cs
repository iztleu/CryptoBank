using System.Reflection;
using CryptoBank.WebAPI.Features.Users.Options;
using CryptoBank.WebAPI.Pipeline.Behaviors;

namespace CryptoBank.WebAPI.Pipeline.Registration;

public static class PipelineBuilderExtensions
{
    public static WebApplicationBuilder AddBehaviors(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg => cfg
            .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
            // Can be merged if necessary
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(MetricsBehavior<,>))
            .AddOpenBehavior(typeof(TracingBehavior<,>))
            .AddOpenBehavior(typeof(ValidationBehavior<,>)));
        
        builder.Services.AddSingleton<Dispatcher>();
        
        return builder;
    }
}