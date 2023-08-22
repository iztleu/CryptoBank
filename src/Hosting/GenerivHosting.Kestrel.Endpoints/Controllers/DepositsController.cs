using System.Text;
using System.Text.Json;
using GenerivHosting.Kestrel.Endpoints.Controllers.Attributes;
using GenerivHosting.Kestrel.Endpoints.Endpoints;
using GenerivHosting.Kestrel.Endpoints.Middlewares.Abstract;
using Hosting.Domain.Dtos;
using Hosting.Services.DI.Repository;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace GenerivHosting.Kestrel.Endpoints.Controllers;

public class DepositsController: Controller
{
    [RateLimiting(10_000)]
    [Path("/deposits")]
    public async Task Get(HttpApplicationContext context, IServiceScope scope)
    {
        var depositRepository = scope.ServiceProvider.GetRequiredService<IDepositRepository>();

        var depositModels = (await depositRepository.LoadAllDeposits(CancellationToken.None))
            .Select(x => new DepositDto
            {
                UserId = x.UserId,
                Currency = x.Currency,
                Amount = x.Amount,
                IsConfirmed = x.IsConfirmed,
            });

        var responseFeature = context.Features.Get<IHttpResponseFeature>()!;
        var responseBodyFeature = context.Features.Get<IHttpResponseBodyFeature>()!;

        responseFeature.Headers.Add("Content-Type", new StringValues("application/json; charset=UTF-8"));
        await responseBodyFeature.Stream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(depositModels)));
    }
}