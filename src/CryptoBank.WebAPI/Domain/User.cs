namespace CryptoBank.WebAPI.Domain;

public class User
{
    public User()
    {
    }
    
    public User(DateTimeOffset registeredAt, DateOnly birthDate, string email, string passwordHash, Role[] roles)
    {
        RegisteredAt = registeredAt;
        BirthDate = birthDate;
        Email = email;
        PasswordHash = passwordHash;
        Roles = roles;
    }
    
    public long Id { get; set; }
    public DateTimeOffset RegisteredAt { get; set; }
    public DateOnly BirthDate { get; set; }

    public string Email { get; set; }
    public string PasswordHash { get; set; }

    public Role[] Roles { get; init; } = Array.Empty<Role>();
}