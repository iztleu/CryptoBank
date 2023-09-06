using CryptoBank.WebAPI.Domain;
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
    
    public virtual DbSet<User> Users => Set<User>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        MapUsers(modelBuilder);
    }
    
    private void MapUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(user =>
        {
            user.HasKey(x => x.Id);

            user.Property(x => x.Id)
                .UseIdentityAlwaysColumn();
          
            user.Property(x => x.Email)
                .IsRequired();

            user.HasIndex(x => x.Email)
                .IsUnique();
            
            user.Property(x => x.PasswordHash)
                .IsRequired();

            user.Property(x => x.BirthDate)
                .IsRequired();
            
            user.Property(x => x.RegisteredAt)
                .IsRequired();
            
            user.Property(x => x.Roles)
                .IsRequired();
        });
    }
}