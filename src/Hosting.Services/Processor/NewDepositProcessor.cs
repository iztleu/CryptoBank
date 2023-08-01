using Hosting.Domain;
using Hosting.Services.Bitcoin;
using Hosting.Services.Repository;

namespace Hosting.Services.Processor;

public class NewDepositProcessor
{
    private readonly BitcoinBlockchainScanner _bitcoinBlockchainScanner;
    private readonly DepositRepository _depositRepository;
    private readonly DepositAddressRespository _depositAddressRepository;

    public NewDepositProcessor(BitcoinNodeClient bitcoinNodeClient)
    {
        var sharedDbContext = new DbContext();
        _bitcoinBlockchainScanner = new BitcoinBlockchainScanner(bitcoinNodeClient);
        _depositRepository = new DepositRepository(sharedDbContext);
        _depositAddressRepository = new DepositAddressRespository(sharedDbContext);
    }
    
    public async Task Process(CancellationToken cancellationToken)
    {
        Console.WriteLine("New deposit processing started");

        var depositAddresses = await _depositAddressRepository.LoadDepositAddresses(cancellationToken);

        var deposits = await _bitcoinBlockchainScanner.FindNewDeposits(depositAddresses, cancellationToken);

        await _depositRepository.SaveDeposits(deposits, cancellationToken);

        Console.WriteLine("New deposit processing finished");
    }
}