using FluentValidation;
using System.Reflection;
using CryptoBank.WebAPI.Common.Registration;
using CryptoBank.WebAPI.Database;
using CryptoBank.WebAPI.Errors.Extensions;
using CryptoBank.WebAPI.Features.Auth.Registration;
using CryptoBank.WebAPI.Features.Users.Registration;
using CryptoBank.WebAPI.Observability;
using CryptoBank.WebAPI.Pipeline.Behaviors;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg
    .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
    .AddOpenBehavior(typeof(ValidationBehavior<,>)));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.AddCommon()
    .AddUsers()
    .AddAuth();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpContextAccessor();




var app = builder.Build();
Telemetry.Init("WebApi");

if (app.Environment.IsDevelopment())
{
    RunMigration(app);
}

app.UseAuthentication();
app.UseAuthorization();

app.MapMetrics();
app.UseHttpsRedirection();

app.MapControllers();
app.MapProblemDetails();

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

public partial class Program {}