namespace CryptoBank.WebAPI.Features.User.Domain;

public class User
{
    public ulong Id { get; set; }
    public DateTime RegisteredAt { get; set; }
    public DateOnly BirthDate { get; set; }

    public string Email { get; set; }
    public string PasswordHash { get; set; }

    public Role[] Roles { get; init; } 
}