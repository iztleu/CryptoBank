namespace CryptoBank.WebAPI.Common.Services.PasswordHasher.Argon2PasswordHasher;

public record Argon2ConfigOptions
{
    public const string ArgonSecuritySectionName = "Argon2IdParameters";
    public int DegreeOfParallelism { get; set; }
    public int Iterations { get; set; }
    public int MemorySize { get; set; }
}