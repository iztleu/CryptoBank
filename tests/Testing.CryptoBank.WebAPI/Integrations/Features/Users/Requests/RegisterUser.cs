using CryptoBank.WebAPI.Features.Users.Requests;
using Testing.CryptoBank.WebAPI.Integrations.Common.Helpers;
using Testing.CryptoBank.WebAPI.Integrations.Fixtures;

namespace Testing.CryptoBank.WebAPI.Integrations.Features.Users;

public class RegisterUserTest : IAsyncLifetime
{
    private readonly TestFixture _fixture;

    public RegisterUserTest(TestFixture fixture) => _fixture = fixture;
   
    [Fact]
    public async Task Should_register_user()
    {
        
    }

    public async Task InitializeAsync()
    {
        await _fixture.Database.Clear(Create.CancellationToken());
    }

    public Task DisposeAsync() => Task.CompletedTask;
}