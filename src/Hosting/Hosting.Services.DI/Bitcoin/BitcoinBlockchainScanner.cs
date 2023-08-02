using Hosting.Domain;
using Microsoft.Extensions.Logging;

namespace Hosting.Services.DI.Bitcoin
{
    public interface IBitcoinBlockchainScanner
    {
        Task<IEnumerable<Deposit>> FindNewDeposits(IEnumerable<DepositAddress> addresses, CancellationToken cancellationToken);
        Task UpdateDepositConfirmations(IEnumerable<Deposit> deposits, CancellationToken cancellationToken);
    }

    public class BitcoinBlockchainScanner : IBitcoinBlockchainScanner
    {
        private readonly IBitcoinNodeClient _bitcoinNodeClient;
        private readonly int _minConfirmations = 3;
        private readonly ILogger<BitcoinBlockchainScanner> _logger;
    
        public BitcoinBlockchainScanner(IBitcoinNodeClient bitcoinNodeClient, ILogger<BitcoinBlockchainScanner> logger)
        {
            _bitcoinNodeClient = bitcoinNodeClient;
            _logger = logger;
        }
    
        public Task<IEnumerable<Deposit>> FindNewDeposits(IEnumerable<DepositAddress> addresses, CancellationToken cancellationToken)
        {
            IEnumerable<Deposit> deposits = new[] { new Deposit(), new Deposit() };

            _logger.LogInformation("New deposits found");

            return Task.FromResult(deposits);
        }
    
        public Task UpdateDepositConfirmations(IEnumerable<Deposit> deposits, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deposit confirmations updated");

            return Task.CompletedTask;
        }
    }
}