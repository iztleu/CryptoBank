using CryptoBank.WebAPI.Common.Services.PasswordHasher;
using CryptoBank.WebAPI.Common.Services.PasswordHasher.Argon2PasswordHasher;

namespace CryptoBank.WebAPI.Features.Users.Registration;

public static class CommonBuilderExtensions
{
    public static WebApplicationBuilder AddCommon(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<Argon2ConfigOptions>(builder.Configuration.GetSection($"Common:{Argon2ConfigOptions.ArgonSecuritySectionName}"));
        builder.Services.AddTransient<IPasswordHasher, Argon2PasswordHasher>();
        return builder;
    }
}