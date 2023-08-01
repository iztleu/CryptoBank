using Hosting.Domain;
using Hosting.Services.Bitcoin;
using Hosting.Services.Repository;

namespace Hosting.Services.Processor;

public interface INewDepositProcessor
{
    Task Process(CancellationToken cancellationToken);
}

public class NewDepositProcessor : INewDepositProcessor
{
    private readonly IBitcoinBlockchainScanner _bitcoinBlockchainScanner;
    private readonly IDepositRepository _depositRepository;
    private readonly IDepositAddressRespository _depositAddressRepository;

    public NewDepositProcessor(
        IBitcoinBlockchainScanner bitcoinBlockchainScanner, 
        IDepositRepository depositRepository, 
        IDepositAddressRespository depositAddressRepository)
    {
        _bitcoinBlockchainScanner = bitcoinBlockchainScanner;
        _depositRepository = depositRepository;
        _depositAddressRepository = depositAddressRepository;
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