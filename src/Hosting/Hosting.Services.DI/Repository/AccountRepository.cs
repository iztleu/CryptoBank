using Hosting.Domain;
using Microsoft.Extensions.Logging;

namespace Hosting.Services.DI.Repository
{
    public interface IAccountRepository
    {
        Task DepositToAccounts(IEnumerable<Deposit> deposits, CancellationToken cancellationToken);
    }

    public class AccountRepository : IAccountRepository
    {
        private readonly DbContext _dbContext;
        private readonly ILogger<AccountRepository> _logger;

        public AccountRepository(DbContext dbContext, ILogger<AccountRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
    
        public Task DepositToAccounts(IEnumerable<Deposit> deposits, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Accounts deposited");

            return Task.CompletedTask;
        }
    }
}