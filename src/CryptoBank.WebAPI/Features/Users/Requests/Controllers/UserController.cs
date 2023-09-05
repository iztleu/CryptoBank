using CryptoBank.WebAPI.Pipeline;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CryptoBank.WebAPI.Features.Users.Requests.Controllers;

[ApiController]
[Route("/users")]
public class UserController : Controller
{
    private readonly Dispatcher _dispatcher;

    public UserController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    public async Task<RegisterUser.Response> RegisterUser([FromBody] RegisterUser.Request request,
        CancellationToken cancellationToken)
    {
        return await _dispatcher.Dispatch(request, cancellationToken);
    }
}