using Hosting.Domain;

namespace Hosting.Services.Repository;

public interface IDepositAddressRespository
{
    Task<IEnumerable<DepositAddress>> LoadDepositAddresses(CancellationToken cancellationToken);
}

public class DepositAddressRespository : IDepositAddressRespository
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