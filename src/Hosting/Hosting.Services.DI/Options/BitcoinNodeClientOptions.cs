namespace Hosting.Services.DI.Options;

public class BitcoinNodeClientOptions
{
    public string RpcUrl { get; set; }
    public TimeSpan RpcTimeout { get; set; }
}