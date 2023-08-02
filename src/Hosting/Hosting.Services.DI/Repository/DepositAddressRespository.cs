using Hosting.Domain;
using Microsoft.Extensions.Logging;

namespace Hosting.Services.DI.Repository
{
    public interface IDepositAddressRespository
    {
        Task<IEnumerable<DepositAddress>> LoadDepositAddresses(CancellationToken cancellationToken);
    }

    public class DepositAddressRespository : IDepositAddressRespository
    {
        private readonly  DbContext _dbContext;
        private readonly ILogger<DepositAddressRespository> _logger;
    
        public DepositAddressRespository(DbContext dbContext, ILogger<DepositAddressRespository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
    
        public Task<IEnumerable<DepositAddress>> LoadDepositAddresses(CancellationToken cancellationToken)
        {
            IEnumerable<DepositAddress> addresses = new[] { new DepositAddress(), new DepositAddress() };

            _logger.LogInformation("Deposit addresses loaded");

            return Task.FromResult(addresses);
        }
    }
}