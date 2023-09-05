using System.Security.Cryptography;
using System.Text;
using CryptoBank.WebAPI.Common.Services.PasswordHasher.Argon2PasswordHasher.Models;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace CryptoBank.WebAPI.Common.Services.PasswordHasher.Argon2PasswordHasher;

public class Argon2PasswordHasher: IPasswordHasher
{
    private readonly Argon2ConfigOptions _argon2ConfigOptions;
    
    private static byte[] GetSecureSalt(int size)
    {
        return RandomNumberGenerator.GetBytes(size);
    }
    
    public Argon2PasswordHasher(IOptions<Argon2ConfigOptions> options)
    {
        _argon2ConfigOptions = options.Value;
    }
    
    public string Hash(string password)
    {
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
        
        argon2.Salt = GetSecureSalt(_argon2ConfigOptions.SaltSizeInBytes);
        argon2.DegreeOfParallelism = _argon2ConfigOptions.DegreeOfParallelism; // four cores
        argon2.Iterations = _argon2ConfigOptions.Iterations;
        argon2.MemorySize = _argon2ConfigOptions.MemorySize; // 1 GB

        var bytes = argon2.GetBytes(_argon2ConfigOptions.PasswordHashSizeInBytes);
        var hash = Convert.ToBase64String(bytes);
        var saltInBase64 = Convert.ToBase64String(argon2.Salt);
        return $"$argon2id$m={argon2.MemorySize}$i={argon2.Iterations}$p={argon2.DegreeOfParallelism}${saltInBase64}${hash}";
    }

    public bool Verify(string hashedPassword, string providedPassword)
    {

        if (string.IsNullOrWhiteSpace(hashedPassword) || string.IsNullOrWhiteSpace(providedPassword))
        {
            return false;
        }

        SettingsFromHexArgon settings;
        try
        {
            settings = GetSettingsFromHexArgon2(hashedPassword);
        }
        catch (ArgumentException)
        {
            return false;
        }
       
        
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(providedPassword));

        argon2.Salt = Convert.FromBase64String(settings.Salt);
        argon2.DegreeOfParallelism = settings.DegreeOfParallelism;
        argon2.Iterations = settings.Iterations;
        argon2.MemorySize = settings.MemorySize;

        var passwordHashBytes = Convert.FromBase64String(settings.Hash);
        
        var bytes = argon2.GetBytes(passwordHashBytes.Length);

        return bytes.SequenceEqual(passwordHashBytes);
    }
    
    private SettingsFromHexArgon GetSettingsFromHexArgon2(string hex)
    {
        var splitHex = hex.Split("$", StringSplitOptions.RemoveEmptyEntries);

        if (splitHex.Length != 6)
        {
            throw new ArgumentException("Invalid hash");
        }
        
        var salt = splitHex[4];
        var hash = splitHex[5];
        
        var memorySize = int.Parse(splitHex[1].Substring(2));
        var iterations = int.Parse(splitHex[2].Substring(2));
        var degreeOfParallelism = int.Parse(splitHex[3].Substring(2));
        
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
