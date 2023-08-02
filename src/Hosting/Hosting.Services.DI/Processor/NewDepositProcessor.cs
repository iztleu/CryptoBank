using Hosting.Services.DI.Bitcoin;
using Hosting.Services.DI.Repository;
using Microsoft.Extensions.Logging;

namespace Hosting.Services.DI.Processor
{
    public interface INewDepositProcessor
    {
        Task Process(CancellationToken cancellationToken);
    }

    public class NewDepositProcessor : INewDepositProcessor
    {
        private readonly IBitcoinBlockchainScanner _bitcoinBlockchainScanner;
        private readonly IDepositRepository _depositRepository;
        private readonly IDepositAddressRespository _depositAddressRepository;
        private readonly ILogger<NewDepositProcessor> _logger;

        public NewDepositProcessor(
            IBitcoinBlockchainScanner bitcoinBlockchainScanner, 
            IDepositRepository depositRepository, 
            IDepositAddressRespository depositAddressRepository, 
            ILogger<NewDepositProcessor> logger)
        {
            _bitcoinBlockchainScanner = bitcoinBlockchainScanner;
            _depositRepository = depositRepository;
            _depositAddressRepository = depositAddressRepository;
            _logger = logger;
        }
    
        public async Task Process(CancellationToken cancellationToken)
        {
            _logger.LogInformation("New deposit processing started");

            var depositAddresses = await _depositAddressRepository.LoadDepositAddresses(cancellationToken);

            var deposits = await _bitcoinBlockchainScanner.FindNewDeposits(depositAddresses, cancellationToken);

            await _depositRepository.SaveDeposits(deposits, cancellationToken);

            _logger.LogInformation("New deposit processing finished");
        }
    }
}