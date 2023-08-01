using Hosting.Domain;
using Hosting.Services.Processor;
using Hosting.Services.Repository;
using Hosting.Services.Bitcoin;
using Microsoft.Extensions.DependencyInjection;

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (sender, args) =>
{
    cts.Cancel();
    args.Cancel = true;
};

var services = new ServiceCollection();

services.AddScoped<DbContext>();

services.AddTransient<INewDepositProcessor, NewDepositProcessor>();
services.AddTransient<IDepositConfirmationsProcessor, DepositConfirmationsProcessor>();

services.AddTransient<IDepositAddressRespository, DepositAddressRespository>();
services.AddTransient<IDepositRepository, DepositRepository>();
services.AddTransient<IAccountRepository, AccountRepository>();

services.AddTransient<IBitcoinBlockchainScanner, BitcoinBlockchainScanner>();
services.AddTransient<IBitcoinNodeClient, BitcoinNodeClient>();

var serviceProvider = services.BuildServiceProvider();


var bitcoinNodeClient = new BitcoinNodeClient();

var newDepositsTask = Task.Run(async () =>
{
    while (!cts.IsCancellationRequested)
    {
        var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

        try
        {
            await using var scope = serviceProvider.CreateAsyncScope();

            var newDepositProcessor = scope.ServiceProvider.GetRequiredService<INewDepositProcessor>();

            await newDepositProcessor.Process(timeoutCts.Token);

            await Task.Delay(TimeSpan.FromSeconds(5));
        }
        catch (OperationCanceledException ex) when (ex.CancellationToken == timeoutCts.Token)
        {
            Console.WriteLine("New deposits processing timed out");
        }     
    }
});

var depositConfirmationsTask = Task.Run(async () =>
{
    while (!cts.IsCancellationRequested)
    {
        var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

        try
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var depositConfirmationsProcessor = scope.ServiceProvider.GetRequiredService<INewDepositProcessor>();

            await depositConfirmationsProcessor.Process(timeoutCts.Token);

            await Task.Delay(TimeSpan.FromSeconds(11));
        }
        catch (OperationCanceledException ex) when (ex.CancellationToken == timeoutCts.Token)
        {
            Console.WriteLine("Deposit confirmations processing timed out");
        }
    }
});

Console.WriteLine("Program started. Press Ctrl+C to exit");

await Task.WhenAll(newDepositsTask, depositConfirmationsTask);

Console.WriteLine("Program stopped");