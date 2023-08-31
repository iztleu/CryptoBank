using CryptoBank.WebAPI.Features.User.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CryptoBank.WebAPI.Features.User.Requests.Controllers;

[ApiController]
public class UserController : Controller
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator) => _mediator = mediator;
    
    [HttpPost]
    public async Task<RegisterUser.Response> RegisterUser([FromBody] RegisterUser.Request request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response;
    }
}