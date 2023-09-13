using CryptoBank.WebAPI.Pipeline;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptoBank.WebAPI.Features.Auth.Requests.Controllers;

[ApiController]
[Route("/auth")]
public class AuthController : Controller
{
    private readonly Dispatcher _dispatcher;

    public AuthController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public async Task<Authenticate.Response>
        Authenticate(Authenticate.Request request, CancellationToken cancellationToken) =>
        await _dispatcher.Dispatch(request, cancellationToken);
}