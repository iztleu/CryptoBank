using Hosting.Services.DI.Bitcoin;
using Hosting.Services.DI.Repository;
using Microsoft.Extensions.Logging;

namespace Hosting.Services.DI.Processor
{
    public interface IDepositConfirmationsProcessor
    {
        Task Process(CancellationToken cancellationToken);
    }

    public class DepositConfirmationsProcessor : IDepositConfirmationsProcessor
    {
        private readonly IDepositRepository _depositRepository;
        private readonly IBitcoinBlockchainScanner _bitcoinBlockchainScanner;
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<DepositConfirmationsProcessor> _logger;

        public DepositConfirmationsProcessor(
            IAccountRepository accountRepository, 
            IBitcoinBlockchainScanner bitcoinBlockchainScanner, 
            IDepositRepository depositRepository, 
            ILogger<DepositConfirmationsProcessor> logger)
        {
            _accountRepository = accountRepository;
            _bitcoinBlockchainScanner = bitcoinBlockchainScanner;
            _depositRepository = depositRepository;
            _logger = logger;
        }

        public async Task Process(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deposit confirmations processing started");

            var unconfirmedDeposits = await _depositRepository.LoadUnconfirmedDeposits(cancellationToken);

            await _bitcoinBlockchainScanner.UpdateDepositConfirmations(unconfirmedDeposits, cancellationToken);

            await _depositRepository.UpdateDepositConfirmations(unconfirmedDeposits, cancellationToken);

            var confirmedDeposits = unconfirmedDeposits.Where(d => d.IsConfirmed);

            await _accountRepository.DepositToAccounts(confirmedDeposits, cancellationToken);

            _logger.LogInformation("Deposits confirmed");
        }
    }
}