namespace CryptoBank.WebAPI.Features.Auth.Models;

public class AccessTokenModel
{
    public AccessTokenModel(string accessToken)
    {
        AccessToken = accessToken;
    }

    public string AccessToken { get; set; }
}