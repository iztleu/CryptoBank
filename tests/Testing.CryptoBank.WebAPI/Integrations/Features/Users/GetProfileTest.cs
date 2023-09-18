using System.Net.Http.Headers;
using CryptoBank.WebAPI.Common.Services.PasswordHasher;
using CryptoBank.WebAPI.Domain;
using CryptoBank.WebAPI.Features.Auth.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Testing.CryptoBank.WebAPI.Integrations.Features.Users.Contracts;
using Testing.CryptoBank.WebAPI.Integrations.Fixtures;
using Testing.CryptoBank.WebAPI.Integrations.Helpers;

namespace Testing.CryptoBank.WebAPI.Integrations.Features.Users;

[Collection(UsersTestsCollection.Name)]
public class GetProfileTest: IAsyncLifetime
{
    private readonly TestFixture _fixture;
    private AsyncServiceScope _scope;

    public GetProfileTest(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Should_get_profile()
    {
        // Arrange
        var client = _fixture.HttpClient.CreateClient();
        var passwordHasher = _scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var tokenServer = _scope.ServiceProvider.GetRequiredService<TokenService>();

        var user = new User(DateTimeOffset.UtcNow, new DateOnly(2000, 01, 31), "test@test.com",
            passwordHasher.Hash("qwerty123456A!"));

        await _fixture.Database.Execute(async x =>
        {
            await x.Users.AddAsync(user);
            await x.SaveChangesAsync();
        });

        var token = tokenServer.CreateAccessToken(user);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var (response, httpResponse) =
            await client.GetFromJsonAsync<ProfileContract>("/users/profile", Create.CancellationToken());

        // Assert
        httpResponse.EnsureSuccessStatusCode();

        response.Should().NotBeNull();
        response!.Email.Should().Be(user.Email);
        response.BirthDate.Should().Be(user.BirthDate);
        response.RegisteredAt.Should().BeCloseTo(user.RegisteredAt, TimeSpan.FromSeconds(10));
    }
    
    public async Task InitializeAsync()
    {
        await _fixture.Database.Clear(Create.CancellationToken());

        _scope = _fixture.Factory.Services.CreateAsyncScope();
    }

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }
}