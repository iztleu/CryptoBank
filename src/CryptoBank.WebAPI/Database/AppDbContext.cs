using CryptoBank.WebAPI.Features.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace CryptoBank.WebAPI.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }

    public AppDbContext()
    {

    }
    
    public virtual DbSet<User> Users => Set<User>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        MapUsers(modelBuilder);
    }
    
    private void MapUsers(ModelBuilder modelBuilder)
    {
        var userEntityBuilder = modelBuilder.Entity<User>();
        userEntityBuilder.ToTable("users");
        userEntityBuilder.HasKey(user => user.Id);
        userEntityBuilder.Property(user => user.Id).HasColumnName("id").IsRequired().UseIdentityAlwaysColumn();
        userEntityBuilder.Property(user => user.Email).HasColumnName("email").IsRequired();
        userEntityBuilder.Property(user => user.PasswordHash).HasColumnName("password_hash").IsRequired();
        userEntityBuilder.Property(user => user.BirthDate).HasColumnName("birth_date");
        userEntityBuilder.Property(user => user.RegisteredAt).HasColumnName("registered_at").IsRequired();
        userEntityBuilder.Property(user => user.Roles).HasColumnName("roles").IsRequired();

        userEntityBuilder.HasIndex(user => user.Email).IsUnique();
    }
}