using Hosting.Domain;
using Hosting.Services.DI.Bitcoin;
using Hosting.Services.DI.Options;
using Hosting.Services.DI.Processor;
using Hosting.Services.DI.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

services.AddTransient<IDepositAddressRepository, DepositAddressRepository>();
services.AddTransient<IDepositRepository, DepositRepository>();
services.AddTransient<IAccountRepository, AccountRepository>();

services.AddTransient<IBitcoinBlockchainScanner, BitcoinBlockchainScanner>();
services.AddTransient<IBitcoinNodeClient, BitcoinNodeClient>();

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
});

services.AddSingleton(loggerFactory);
services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug);
});

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

services.AddSingleton(configuration);

services.Configure<BitcoinNodeClientOptions>(configuration.GetSection("BitcoinNodeClient"));
services.Configure<NewDepositProcessingOptions>(configuration.GetSection("NewDepositProcessing"));
services.Configure<DepositConfirmationsProcessingOptions>(configuration.GetSection("DepositConfirmationsProcessing"));

var serviceProvider = services.BuildServiceProvider();

var programLogger = loggerFactory.CreateLogger("Program");

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
            programLogger.LogError(ex, "New deposits processing timed out");
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
            programLogger.LogError(ex, "Deposit confirmations processing timed out");
        }
    }
});


programLogger.LogInformation("Program started. Press Ctrl+C to exit");

await Task.WhenAll(newDepositsTask, depositConfirmationsTask);

programLogger.LogInformation("Program stopped");