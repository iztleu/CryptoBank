namespace CryptoBank.WebAPI.Features.Users.Domain;

public class User
{
    public User(long id)
    {
        Id = id;
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

    public Role[] Roles { get; init; } 
}