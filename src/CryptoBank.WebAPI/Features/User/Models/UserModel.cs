using CryptoBank.WebAPI.Features.User.Domain;

namespace CryptoBank.WebAPI.Features.User.Models;

public record UserModel(int Id, string Email, DateOnly? BirthDate, DateTimeOffset RegisteredAt, Role[] Roles);