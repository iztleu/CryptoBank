using CryptoBank.WebAPI.Features.User.Models;
using MediatR;

namespace CryptoBank.WebAPI.Features.User.Requests;

public class RegisterUser
{
    public record Request(string Email, string Password, DateOnly? BirthDate) : IRequest<Response>;
    
    public record Response(ulong Id);
    
    
}