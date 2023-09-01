using CryptoBank.WebAPI.Observability;
using MediatR;

namespace CryptoBank.WebAPI.Pipeline.Behaviors;

public class MetricsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        return await HandlerMetrics.Meter<TRequest, TResponse>(async () => await next());
    }
}