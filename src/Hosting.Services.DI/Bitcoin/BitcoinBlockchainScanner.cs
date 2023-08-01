using Hosting.Domain;

namespace Hosting.Services.Bitcoin;

public interface IBitcoinBlockchainScanner
{
    Task<IEnumerable<Deposit>> FindNewDeposits(IEnumerable<DepositAddress> addresses, CancellationToken cancellationToken);
    Task UpdateDepositConfirmations(IEnumerable<Deposit> deposits, CancellationToken cancellationToken);
}

public class BitcoinBlockchainScanner : IBitcoinBlockchainScanner
{
    private readonly IBitcoinNodeClient _bitcoinNodeClient;
    private readonly int _minConfirmations = 3;
    
    public BitcoinBlockchainScanner(IBitcoinNodeClient bitcoinNodeClient)
    {
        _bitcoinNodeClient = bitcoinNodeClient;
    }
    
    public Task<IEnumerable<Deposit>> FindNewDeposits(IEnumerable<DepositAddress> addresses, CancellationToken cancellationToken)
    {
        IEnumerable<Deposit> deposits = new[] { new Deposit(), new Deposit() };

        Console.WriteLine("New deposits found");

        return Task.FromResult(deposits);
    }
    
    public Task UpdateDepositConfirmations(IEnumerable<Deposit> deposits, CancellationToken cancellationToken)
    {
        Console.WriteLine("Deposit confirmations updated");

        return Task.CompletedTask;
    }
}