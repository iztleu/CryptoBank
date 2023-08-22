using GenerivHosting.Kestrel.Endpoints.Middlewares.Abstract;

namespace GenerivHosting.Kestrel.Endpoints.Endpoints;

public record REndpoint(string PathPattern, Func<HttpApplicationContext, IServiceScope, Task> EndpointDelegate, Dictionary<string, object>? Metadata = null);

public class EndpointCollection : List<REndpoint>
{
}

public static class EndpointMetadataKeys
{
    public const string RateLimitingInverval = nameof(RateLimitingInverval);
}