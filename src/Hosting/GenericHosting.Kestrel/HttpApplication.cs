using System.Text;
using System.Text.Json;
using Hosting.Services.DI.Repository;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace GenericHosting.Kestrel;

internal class HttpApplication : IHttpApplication<HttpApplicationContext>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public HttpApplication(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public HttpApplicationContext CreateContext(IFeatureCollection contextFeatures)
    {
        return new HttpApplicationContext(contextFeatures);
    }

    public void DisposeContext(HttpApplicationContext context, Exception? exception)
    {
    }

    public async Task ProcessRequestAsync(HttpApplicationContext context)
    {
        var requestFeature = context.Features.Get<IHttpRequestFeature>()!;
        var responseFeature = context.Features.Get<IHttpResponseFeature>()!;
        var responseBodyFeature = context.Features.Get<IHttpResponseBodyFeature>()!;

        if (requestFeature.Path == "/deposits")
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();

            var depositRepository = scope.ServiceProvider.GetRequiredService<IDepositRepository>();

            var depositModels = (await depositRepository.LoadAllDeposits(CancellationToken.None))
                .Select(x => new DepositDto
                {
                    UserId = x.UserId,
                    Currency = x.Currency,
                    Amount = x.Amount,
                    IsConfirmed = x.IsConfirmed,
                });

            responseFeature.Headers.Add("Content-Type", new StringValues("application/json; charset=UTF-8"));
            await responseBodyFeature.Stream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(depositModels)));
        }
        else
        {
            responseFeature.StatusCode = StatusCodes.Status404NotFound;
        }
    }
}