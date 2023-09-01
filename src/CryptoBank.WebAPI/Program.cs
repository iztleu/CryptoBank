using FluentValidation;
using System.Reflection;
using CryptoBank.WebAPI.Features.Users.Registration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.AddCommon();

var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.Run();