using System.Diagnostics;

namespace CryptoBank.WebAPI.Observability;

public static class Telemetry
{
    public static ActivitySource ActivitySource { get; private set; }

    public static void Init(string serviceName)
    {
        ActivitySource = new ActivitySource(serviceName);
    }
}