using CryptoBank.WebAPI.Domain;

namespace CryptoBank.WebAPI.Features.Users.Models;

public record UserModel(long Id, string Email, DateOnly BirthDate, DateTimeOffset RegisteredAt, string[] Roles);