using CryptoBank.WebAPI.Domain;
using CryptoBank.WebAPI.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptoBank.WebAPI.Features.Users.Requests.Controllers;

[ApiController]
[Route("/users")]
public class UserController : Controller
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<Register.Response> Register([FromBody] Register.Request request,
        CancellationToken cancellationToken)
    {
        return await _mediator.Send(request, cancellationToken);
    }
    
    [Authorize]
    [HttpGet("profile")]
    public async Task<UserModel> GetProfile(CancellationToken cancellationToken)
    {
        return (await _mediator.Send(new GetProfile.Request(), cancellationToken)).Profile;
    }
    
    
    [HttpPut("roles")]
    [Authorize(Roles = nameof(Role.Administrator))]
    public async Task<IActionResult> UpdateRoles(
        [FromBody] UpdateRoles.Request request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(request, cancellationToken);

        return new NoContentResult();
    }
    
}