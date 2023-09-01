using System.Security.Cryptography;
using System.Text;
using CryptoBank.WebAPI.Common.Services.PasswordHasher.Argon2PasswordHasher.Models;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace CryptoBank.WebAPI.Common.Services.PasswordHasher.Argon2PasswordHasher;

public class Argon2PasswordHasher: IPasswordHasher
{
    private readonly Argon2ConfigOptions _argon2ConfigOptions;
    
    private byte[] GetSecureSalt()
    {
        return RandomNumberGenerator.GetBytes(32);
    }
    
    public Argon2PasswordHasher(IOptions<Argon2ConfigOptions> options)
    {
        _argon2ConfigOptions = options.Value;
    }
    
    public string Hash(string password)
    {
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
        
        argon2.Salt = GetSecureSalt();
        argon2.DegreeOfParallelism = _argon2ConfigOptions.DegreeOfParallelism; // four cores
        argon2.Iterations = _argon2ConfigOptions.Iterations;
        argon2.MemorySize = _argon2ConfigOptions.MemorySize; // 1 GB

        var bytes = argon2.GetBytes(16);
        var hash = Convert.ToBase64String(bytes);

        return
            $"$argon2id$m={argon2.MemorySize},t={argon2.Iterations},p={argon2.DegreeOfParallelism}${Convert.ToBase64String(argon2.Salt)}${hash}";
    }

    public bool Verify(string hashedPassword, string providedPassword)
    {

        var settings = GetSettingsFromHexArgon2(hashedPassword);
        
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(providedPassword));

        argon2.Salt = Convert.FromBase64String(settings.Salt);
        argon2.DegreeOfParallelism = settings.DegreeOfParallelism;
        argon2.Iterations = settings.Iterations;
        argon2.MemorySize = settings.MemorySize;

        var bytes = argon2.GetBytes(16);

        return Convert.ToBase64String(bytes) == settings.Hash;
    }
    
    private SettingsFromHexArgon GetSettingsFromHexArgon2(string hex)
    {
        var splitHex = hex.Split("$");

        if (splitHex.Length != 5)
        {
            throw new ArgumentException("Invalid hash");
        }
        
        var salt = splitHex[3];
        var hash = splitHex[4];
        
        var payload = splitHex[2];
        var settings = payload.Split(",");
        var memorySize = int.Parse(settings[0].Substring(2));
        var iterations = int.Parse(settings[1].Substring(2));
        var degreeOfParallelism = int.Parse(settings[2].Substring(2));

        return new SettingsFromHexArgon
        {
            Salt = salt,
            Hash = hash,
            Iterations = iterations,
            MemorySize = memorySize,
            DegreeOfParallelism = degreeOfParallelism
        };
    }
}
