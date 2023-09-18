using System.ComponentModel;

namespace Testing.CryptoBank.WebAPI.Integrations.Features.Users.Contracts;

public class ProfileContract
{
    public long Id { get; set; }
    public string Email { get; set; }
    public DateOnly BirthDate { get; set; }
    public DateTimeOffset RegisteredAt { get; set; }
    public RoleContract[] Roles { get; set; }
}


public enum RoleContract
{
    [Description("User")]
    User = 1,
    [Description("Administrator")]
    Administrator = 2,
    [Description("Analyst")]
    Analyst = 3
}