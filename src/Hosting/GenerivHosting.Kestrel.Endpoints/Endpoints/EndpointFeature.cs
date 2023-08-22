namespace GenerivHosting.Kestrel.Endpoints.Endpoints;

public class EndpointFeature
{
    public EndpointFeature(REndpoint? endpoint)
    {
        Endpoint = endpoint;
    }

    public REndpoint? Endpoint { get; }
}