namespace Testing.CryptoBank.WebAPI.Integrations.Features.Auth.Contracts;

public class AccessTokenContract
{
    public Token Token { get; set; }
}

public class Token
{
    public string AccessToken { get; set; }
}