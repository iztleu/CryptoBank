namespace CryptoBank.WebAPI.Domain;

public class User
{
    public User()
    {
    }
    public User(long Id)
    {
        this.Id = Id;
    }
    
    public User(DateTimeOffset registeredAt, DateOnly birthDate, string email, string passwordHash, Role[]? roles = null)
    {
        RegisteredAt = registeredAt;
        BirthDate = birthDate;
        Email = email;
        PasswordHash = passwordHash;
        Roles = roles ?? new []{Role.User};
    }
    
    public long Id { get; set; }
    public DateTimeOffset RegisteredAt { get; set; }
    public DateOnly BirthDate { get; set; }

    public string Email { get; set; }
    public string PasswordHash { get; set; }

    public Role[] Roles { get; init; }
}