using Hosting.Domain;

namespace Hosting.Services;

public class AccountRepository
{
    private readonly DbContext _dbContext;

    public AccountRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Task DepositToAccounts(IEnumerable<Deposit> deposits, CancellationToken cancellationToken)
    {
        Console.WriteLine("Accounts deposited");

        return Task.CompletedTask;
    }
}