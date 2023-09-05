using FluentValidation;
using System.Reflection;
using CryptoBank.WebAPI.Database;
using CryptoBank.WebAPI.Features.Users.Registration;
using CryptoBank.WebAPI.Observability;
using CryptoBank.WebAPI.Pipeline.Behaviors;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg => cfg
    .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
    // Can be merged if necessary
    .AddOpenBehavior(typeof(LoggingBehavior<,>))
    .AddOpenBehavior(typeof(MetricsBehavior<,>))
    .AddOpenBehavior(typeof(TracingBehavior<,>))
    .AddOpenBehavior(typeof(ValidationBehavior<,>)));

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.AddUsers();
builder.AddCommon();

var app = builder.Build();
Telemetry.Init("WebApi");

if (app.Environment.IsDevelopment())
{
    RunMigration(app);
}

app.MapMetrics();
app.UseHttpsRedirection();

app.MapControllers();
app.Run();



void RunMigration(WebApplication webApplication)
{
    using (var scope = webApplication.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        if (dbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            dbContext.Database.Migrate();
    }
}