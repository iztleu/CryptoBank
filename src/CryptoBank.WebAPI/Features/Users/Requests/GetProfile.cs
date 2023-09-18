using CryptoBank.WebAPI.Common.Services;
using CryptoBank.WebAPI.Database;
using CryptoBank.WebAPI.Features.Users.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Common.Errors.Exceptions;

using static CryptoBank.WebAPI.Features.Users.Errors.UserValidationErrors;

namespace CryptoBank.WebAPI.Features.Users.Requests;

public class GetProfile
{
    public record Request : IRequest<Response>;
    
    public record Response(UserModel Profile);

    public class RequestHandler : IRequestHandler<Request, Response>
    {
        private readonly AppDbContext _dbContext;
        
        private readonly CurrentAuthInfoSource _currentAuthInfoSource;

        public RequestHandler(AppDbContext dbContext, CurrentAuthInfoSource currentAuthInfoSource)
        {
            _dbContext = dbContext;
            _currentAuthInfoSource = currentAuthInfoSource;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _currentAuthInfoSource.GetUserId();
            var userModel = await _dbContext.Users
                .Where(user => user.Id == userId)
                .Select(user => new UserModel(user.Id, user.Email, user.BirthDate, user.RegisteredAt, user.Roles))
                .SingleOrDefaultAsync(cancellationToken);
            
            if (userModel is null)
                throw new InternalErrorException(UserNotFound);

            return new Response(userModel);
        }
    }
}