using CryptoBank.WebAPI.Features.Auth.Models;
using CryptoBank.WebAPI.Pipeline;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptoBank.WebAPI.Features.Auth.Requests.Controllers;

[ApiController]
[Route("/auth")]
public class AuthController : Controller
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [AllowAnonymous]
    [HttpPost("authenticate")]
    public async Task<AccessTokenModel> Authenticate(Authenticate.Request request,
        CancellationToken cancellationToken)
    {
        return (await _mediator.Send(request, cancellationToken)).Token;
    }
       
}