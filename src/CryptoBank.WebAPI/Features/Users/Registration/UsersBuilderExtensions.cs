using CryptoBank.WebAPI.Features.Users.Options;

namespace CryptoBank.WebAPI.Features.Users.Registration;

public static class UsersBuilderExtensions
{
    public static WebApplicationBuilder AddDeposits(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<UsersOptions>(builder.Configuration.GetSection("Features:Users"));
        return builder;
    }
}