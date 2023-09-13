namespace CryptoBank.WebAPI.Common.Services.PasswordHasher;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string hashedPassword, string password);
}