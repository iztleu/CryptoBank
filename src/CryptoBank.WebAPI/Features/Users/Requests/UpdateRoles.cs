using CryptoBank.WebAPI.Database;
using CryptoBank.WebAPI.Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static CryptoBank.WebAPI.Features.Users.Errors.UserValidationErrors;

namespace CryptoBank.WebAPI.Features.Users.Requests;
public class UpdateRoles
{
    public record Request(long UserId, Role[] NewRoles) : IRequest;

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator(AppDbContext dbContext)
        {
            
            RuleFor(request => request.UserId)
                .MustAsync(async (x, token) =>
                {
                    var userExists = await dbContext.Users.AnyAsync(user => user.Id == x, token);
                    return userExists;
                })
                .WithErrorCode(UserNotFound);
            RuleFor(request => request.NewRoles)
                .NotEmpty()
                .WithErrorCode(RolesRequired);
        }
    }

    public class RequestHandler : IRequestHandler<Request>
    {
        private readonly AppDbContext _dbContext;

        public RequestHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(Request request, CancellationToken cancellationToken)
        {
            var updatedUser = new User(request.UserId) { Roles = request.NewRoles };
            _dbContext.Attach(updatedUser);
            _dbContext.Entry(updatedUser).Property(user => user.Roles).IsModified = true;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}