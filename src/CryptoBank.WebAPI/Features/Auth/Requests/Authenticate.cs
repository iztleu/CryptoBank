using CryptoBank.WebAPI.Common.Services.PasswordHasher;
using CryptoBank.WebAPI.Database;
using CryptoBank.WebAPI.Errors.Exceptions;
using CryptoBank.WebAPI.Features.Auth.Models;
using CryptoBank.WebAPI.Features.Auth.Services;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

using static CryptoBank.WebAPI.Features.Auth.Errors.AuthValidationErrors;

namespace CryptoBank.WebAPI.Features.Auth.Requests;

public class Authenticate
{
    public record Request(string Email , string Password) : IRequest<Response>;

    public record Response(AccessTokenModel Token);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleFor(x => x.Email)
                .NotEmpty().WithErrorCode(EmailRequired)
                .EmailAddress().WithErrorCode(InvalidEmailFormat);
            RuleFor(x => x.Password)
                .NotEmpty().WithErrorCode(PasswordRequired);
        }
    }
    
public class RequestHandler : IRequestHandler<Request, Response>
    {
        private readonly AppDbContext _dbContext;
        private readonly TokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;

        public RequestHandler(AppDbContext dbContext, TokenService tokenService, IPasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var user =  await _dbContext.Users.SingleOrDefaultAsync(user => user.Email == request.Email, cancellationToken);

            if (user is null || !_passwordHasher.Verify(user.PasswordHash, request.Password))
            {
                throw new ValidationErrorsException(string.Empty, "Wrong email or password", WrongCredentials);
            } 
            
            var token = _tokenService.CreateAccessToken(user.Id, user.Email, user.Roles);
            return new Response(new AccessTokenModel(token));
        }
    }
    
}

