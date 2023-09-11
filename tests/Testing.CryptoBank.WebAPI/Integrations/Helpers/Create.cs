using System.Diagnostics;

namespace Testing.CryptoBank.WebAPI.Integrations.Helpers;

public static class Create
{
    public static CancellationToken CancellationToken(int timeoutInSeconds = 10) =>
        new CancellationTokenSource(
            Debugger.IsAttached
                ? TimeSpan.FromMinutes(10)
                : TimeSpan.FromSeconds(timeoutInSeconds))
            .Token;
}
