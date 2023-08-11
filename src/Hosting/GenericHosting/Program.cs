// See https://aka.ms/new-console-template for more information

using GenericHosting;
using Hosting.Domain;
using Hosting.Services.DI.Bitcoin;
using Hosting.Services.DI.Options;
using Hosting.Services.DI.Processor;
using Hosting.Services.DI.Repository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

IHost host1 = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<LifetimeHostedService>();
        services.AddHostedService<BitcoinNodeConnectionHostedService>();
        services.AddHostedService<NewDepositHostedService>();
        services.AddHostedService<DepositConfirmationsHostedService>();
        
        services.AddScoped<DbContext>();

        services.AddTransient<INewDepositProcessor, NewDepositProcessor>();
        services.AddTransient<IDepositConfirmationsProcessor, DepositConfirmationsProcessor>();

        services.AddTransient<IDepositAddressRepository, DepositAddressRepository>();
        services.AddTransient<IDepositRepository, DepositRepository>();
        services.AddTransient<IAccountRepository, AccountRepository>();

        services.AddTransient<IBitcoinBlockchainScanner, BitcoinBlockchainScanner>();

        services.AddSingleton<IBitcoinNodeClient, BitcoinNodeClient>();

        services.Configure<BitcoinNodeClientOptions>(hostContext.Configuration.GetSection("BitcoinNodeClient"));
        services.Configure<NewDepositProcessingOptions>(hostContext.Configuration.GetSection("NewDepositProcessing"));
        services.Configure<DepositConfirmationsProcessingOptions>(hostContext.Configuration.GetSection("DepositConfirmationsProcessing"));
    })
    .ConfigureLogging((context, builder) =>
    {
        // Configure existing logging providers
        // Add custom logging providers
    })
    .ConfigureAppConfiguration(builder =>
    {
        // Configure existing configuration providers
        // Add custom configuration providers
    })
    .Build();
    
IHost host2 = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<LifetimeHostedService>();
        services.AddHostedService<BitcoinNodeConnectionHostedService>();
        services.AddHostedService<NewDepositHostedService>();
        services.AddHostedService<DepositConfirmationsHostedService>();
        
        services.AddScoped<DbContext>();

        services.AddTransient<INewDepositProcessor, NewDepositProcessor>();
        services.AddTransient<IDepositConfirmationsProcessor, DepositConfirmationsProcessor>();

        services.AddTransient<IDepositAddressRepository, DepositAddressRepository>();
        services.AddTransient<IDepositRepository, DepositRepository>();
        services.AddTransient<IAccountRepository, AccountRepository>();

        services.AddTransient<IBitcoinBlockchainScanner, BitcoinBlockchainScanner>();

        services.AddSingleton<IBitcoinNodeClient, BitcoinNodeClient>();

        services.Configure<BitcoinNodeClientOptions>(hostContext.Configuration.GetSection("BitcoinNodeClient"));
        services.Configure<NewDepositProcessingOptions>(hostContext.Configuration.GetSection("NewDepositProcessing"));
        services.Configure<DepositConfirmationsProcessingOptions>(hostContext.Configuration.GetSection("DepositConfirmationsProcessing"));
    })
    .ConfigureLogging((context, builder) =>
    {
        // Configure existing logging providers
        // Add custom logging providers
    })
    .ConfigureAppConfiguration(builder =>
    {
        // Configure existing configuration providers
        // Add custom configuration providers
    })
    .Build();

await Task.WhenAll(
    host1.RunAsync(),
    host2.RunAsync());