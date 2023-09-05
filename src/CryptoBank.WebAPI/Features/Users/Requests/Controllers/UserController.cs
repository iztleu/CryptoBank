using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CryptoBank.WebAPI.Features.Users.Requests.Controllers;

[ApiController]
[Route("/users")]
public class UserController : Controller
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator) => _mediator = mediator;
    
    [HttpPost]
    public async Task<RegisterUser.Response> RegisterUser([FromBody] RegisterUser.Request request,
        CancellationToken cancellationToken)
    {
        return await _mediator.Send(request, cancellationToken);
    }
}