using Hosting.Services.DI.Options;
using Microsoft.Extensions.Options;

namespace Hosting.Services.DI.Bitcoin
{
    public interface IBitcoinNodeClient
    {
    }

    public class BitcoinNodeClient : IBitcoinNodeClient
    {
        private readonly string _rpcUrl;
        private readonly TimeSpan _rpcTimeout;

        public BitcoinNodeClient(IOptions<BitcoinNodeClientOptions> options)
        {
            _rpcUrl = options.Value.RpcUrl;
            _rpcTimeout = options.Value.RpcTimeout;
        }
    }
}