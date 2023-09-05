using CryptoBank.WebAPI.Features.Users.Options;

namespace CryptoBank.WebAPI.Features.Users.Registration;

public static class UsersBuilderExtensions
{
    public static WebApplicationBuilder AddUsers(this WebApplicationBuilder builder)
    {
        var section = builder.Configuration.GetSection("Features:Users");
        builder.Services.Configure<UsersOptions>(section);
        return builder;
    }
}