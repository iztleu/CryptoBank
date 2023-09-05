using CryptoBank.WebAPI.Common.Services.PasswordHasher.Argon2PasswordHasher;
using FluentAssertions;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace Testing.CryptoBank.WebAPI.Common;

public class Argon2IdPasswordHasherTests
{
    [Fact]
    public void HashPassword_should_embed_parameters_salt_and_hash_into_result()
    {
        // Arrange
        var options = new Argon2ConfigOptions
        {
            DegreeOfParallelism = 4,
            MemorySize = 4096,
            Iterations = 1,
            PasswordHashSizeInBytes = 32,
            SaltSizeInBytes = 16
        };
        
        var hasher = new Argon2PasswordHasher(Options.Create(options));

        // Act
        var passwordHash = hasher.Hash("test_password");

        // Assert
        passwordHash.Should().NotBeNullOrWhiteSpace();
        var parts = passwordHash.Split('$', StringSplitOptions.RemoveEmptyEntries);
        parts[0].Should().Be("argon2id");
        parts[1].Should().Be($"m={options.MemorySize}");
        parts[2].Should().Be($"i={options.Iterations}");
        parts[3].Should().Be($"p={options.DegreeOfParallelism}");

        var saltBytes = Convert.FromBase64String(parts[4]);
        saltBytes.Should().HaveCount(options.SaltSizeInBytes);

        var hashBytes = Convert.FromBase64String(parts[5]);
        hashBytes.Should().HaveCount(options.PasswordHashSizeInBytes);

        using var argon2Id = new Argon2id("test_password"u8.ToArray())
        {
            Salt = saltBytes,
            DegreeOfParallelism = options.DegreeOfParallelism,
            MemorySize = options.MemorySize,
            Iterations = options.Iterations
        };
        var expectedHashBytes = argon2Id.GetBytes(options.PasswordHashSizeInBytes);
        hashBytes.Should().BeEquivalentTo(expectedHashBytes);
    }
}