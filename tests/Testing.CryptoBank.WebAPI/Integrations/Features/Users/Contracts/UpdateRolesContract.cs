namespace Testing.CryptoBank.WebAPI.Integrations.Features.Users.Contracts;

public record UpdateRolesContract(long UserId, RoleContract[] NewRoles);