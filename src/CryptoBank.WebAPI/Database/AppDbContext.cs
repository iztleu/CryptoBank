using Microsoft.EntityFrameworkCore;

namespace CryptoBank.WebAPI.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public AppDbContext()
    {

    }
}