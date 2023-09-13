using System.ComponentModel;

namespace CryptoBank.WebAPI.Domain;

public enum Role
{
    [Description("User")]
    User = 1,
    [Description("Administrator")]
    Administrator = 2,
    [Description("Analyst")]
    Analyst = 3
}