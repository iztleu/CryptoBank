using GenericHosting.Kestrel;
using Hosting.Domain;
using Hosting.Services.DI.Repository;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<KestrelHostedService>();
            services.AddScoped<DbContext>();
            services.AddTransient<IDepositRepository, DepositRepository>();
        }
    )
    .Build();
    
await host.RunAsync();