using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CryptoBank.WebAPI.Features.User.Requests.Controllers;

[ApiController]
public class UserController : Controller
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator) => _mediator = mediator;
}