using System.Net.Http.Json;
using System.Text.Json;

namespace Testing.CryptoBank.WebAPI.Integrations.Helpers;

public static class HttpClientExtensions
{
    public static async Task<(TResponse?, HttpResponseMessage httpRespnse)> PostAsJsonAsync<TResponse>(this HttpClient client, string url, object? body,
        CancellationToken cancellationToken)
    {
        var httpResponse = await client.PostAsJsonAsync(url, body, cancellationToken);

        var responseString = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        var response = JsonSerializer.Deserialize<TResponse>(responseString, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        return (response, httpResponse);
    }
}
