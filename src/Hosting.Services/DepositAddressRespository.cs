using Hosting.Domain;

namespace Hosting.Services;

public class DepositAddressRespository
{
    private readonly  DbContext _dbContext;
    
    public DepositAddressRespository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Task<IEnumerable<DepositAddress>> LoadDepositAddresses(CancellationToken cancellationToken)
    {
        IEnumerable<DepositAddress> addresses = new[] { new DepositAddress(), new DepositAddress() };

        Console.WriteLine("Deposit addresses loaded");

        return Task.FromResult(addresses);
    }
}