using CryptoBank.WebAPI.Common.Services.PasswordHasher;
using CryptoBank.WebAPI.Database;
using CryptoBank.WebAPI.Domain;
using CryptoBank.WebAPI.Features.Users.Options;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using static CryptoBank.WebAPI.Features.Users.Errors.UserValidationErrors;

namespace CryptoBank.WebAPI.Features.Users.Requests;

public class Register
{
    public record Request(string Email, string Password, DateOnly BirthDate) : IRequest<Response>;
    
    public record Response(long Id);
    
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator(AppDbContext dbContext)
        {
            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithErrorCode(PasswordRequired)
                .MinimumLength(7)
                .WithErrorCode(PasswordToShort);
            
            RuleFor(x => x.BirthDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithErrorCode(BirthDateRequired)
                .LessThan(DateOnly.FromDateTime(DateTime.Now))
                .WithErrorCode(DateCannotBeInTheFutureOrToday);
            
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithErrorCode(EmailRequired)
                .EmailAddress()
                .WithErrorCode(EmailFormatIsWrong)
                .MustAsync(async (x, token) =>
                {
                    var userExists = await dbContext.Users.AnyAsync(user => user.Email == x, token);
                    return !userExists;
                }).WithErrorCode(EmailAlreadyExists);
        }
    }

    public class RequestHandler : IRequestHandler<Request, Response>
    {
        private readonly AppDbContext _dbContext;
        private readonly UsersOptions _usersOptions;
        private readonly IPasswordHasher _passwordHasher;

        public RequestHandler(AppDbContext dbContext, IOptions<UsersOptions> usersOptions, IPasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _usersOptions = usersOptions.Value;
            _passwordHasher = passwordHasher;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var roles = new List<Role>{ Role.User };
            if (request.Email == _usersOptions.AdministratorEmail)
            {
                roles.Add(Role.Administrator);
            }
            
            var passwordHash = _passwordHasher.Hash(request.Password);

            var user = new User(DateTime.UtcNow, request.BirthDate, request.Email, passwordHash, roles.ToArray());
            
            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new Response(user.Id);
        }
    }
}