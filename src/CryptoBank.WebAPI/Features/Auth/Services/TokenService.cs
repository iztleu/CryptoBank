using System.Security.Claims;
using System.Text;
using CryptoBank.WebAPI.Domain;
using CryptoBank.WebAPI.Features.Auth.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace CryptoBank.WebAPI.Features.Auth.Services;

public class TokenService
{
    private readonly AuthOptions.JwtOptions _jwtOptions;
    
    public TokenService(IOptions<AuthOptions> options)
    {
        _jwtOptions = options.Value.Jwt;
    }

    public string CreateAccessToken(long userId, string email, Role[] roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var utcNow = DateTime.UtcNow;

        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Expires = utcNow.Add(_jwtOptions.Duration),
            NotBefore = utcNow,
            SigningCredentials = signingCredentials,
            Claims = new Dictionary<string, object>
            {
                { ClaimTypes.NameIdentifier, userId },
                { ClaimTypes.Email, email },
                { ClaimTypes.Role, roles.Select(role => role.ToString()).ToArray() },
            },
        };

        var accessToken = new JsonWebTokenHandler().CreateToken(securityTokenDescriptor);

        return accessToken;
    }
}