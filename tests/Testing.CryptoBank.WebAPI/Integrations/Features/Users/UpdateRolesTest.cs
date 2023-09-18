using System.Net.Http.Headers;
using System.Net.Http.Json;
using CryptoBank.WebAPI.Common.Services.PasswordHasher;
using CryptoBank.WebAPI.Database;
using CryptoBank.WebAPI.Domain;
using CryptoBank.WebAPI.Features.Auth.Services;
using CryptoBank.WebAPI.Features.Users.Requests;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Testing.CryptoBank.WebAPI.Integrations.Fixtures;
using Testing.CryptoBank.WebAPI.Integrations.Helpers;

namespace Testing.CryptoBank.WebAPI.Integrations.Features.Users;

[Collection(UsersTestsCollection.Name)]
public class UpdateRolesTest : IAsyncLifetime
{
    private readonly TestFixture _fixture;
    private AsyncServiceScope _scope;
    
    public UpdateRolesTest(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Should_update_roles()
    {
        // Arrange
        var client = _fixture.HttpClient.CreateClient();
        var passwordHasher = _scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var tokenServer = _scope.ServiceProvider.GetRequiredService<TokenService>();

        var adminUser = new User(DateTimeOffset.UtcNow, new DateOnly(2000, 01, 31), "test@test.com",
            passwordHasher.Hash("qwerty123456A!"), new []{ Role.Administrator });

        var user = new User(DateTimeOffset.UtcNow, new DateOnly(2000, 01, 31), "user@test.com",
            passwordHasher.Hash("qwerty123456A!"));
        
        await _fixture.Database.Execute(async x =>
        {
            await x.Users.AddRangeAsync(adminUser, user);
            await x.SaveChangesAsync();
        });

        var token = tokenServer.CreateAccessToken(adminUser);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    
        // Act
        (await client.PutAsJsonAsync("/users/roles", new
        {
            UserId = user.Id,
            NewRoles = new []{ Role.Administrator }
        })).EnsureSuccessStatusCode();
        
        // Assert
        user = await _fixture.Database.Execute(async x =>
            await x.Users.SingleOrDefaultAsync(u => u.Id == user.Id));

        user.Should().NotBeNull();
        user!.Roles.Should().Contain(new List<Role>{Role.Administrator});
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

[Collection(UsersTestsCollection.Name)]
public class UpdateRolesValidatorTest : IAsyncLifetime
{
    private readonly TestFixture _fixture;
    private AsyncServiceScope _scope;
    private UpdateRoles.RequestValidator? _validator;
    
    public UpdateRolesValidatorTest(TestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task Should_validate_correct_request()
    {
        var user = new User(DateTimeOffset.UtcNow, new DateOnly(2000, 01, 31), "user@test.com","qwerty123456A!");
        
        await _fixture.Database.Execute(async x =>
        {
            await x.Users.AddRangeAsync(user);
            await x.SaveChangesAsync();
        });
        
        var result = await _validator.TestValidateAsync(
            new UpdateRoles.Request(user.Id, user.Roles));
        result.ShouldNotHaveAnyValidationErrors();
    }
    
    
    [Fact]
    public async Task? Should_have_error_when_user_roles_empty()
    {
        var user = new User(DateTimeOffset.UtcNow, new DateOnly(2000, 01, 31), "user@test.com","qwerty123456A!");
        
        await _fixture.Database.Execute(async x =>
        {
            await x.Users.AddRangeAsync(user);
            await x.SaveChangesAsync();
        });
        
        var result = await _validator.TestValidateAsync(
            new UpdateRoles.Request(user.Id, null));
        
        result.ShouldHaveValidationErrorFor(x => x.NewRoles)
            .WithErrorCode("users_validation_roles_required");
    }
    
    
    [Fact]
    public async Task Should_have_error_when_user_not_found()
    {
        var result = await _validator.TestValidateAsync(
            new UpdateRoles.Request(1, new []{ Role.Administrator }));
        
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorCode("users_validation_user_not_found");
    }
    
    
    public async Task InitializeAsync()
    {
        await _fixture.Database.Clear(Create.CancellationToken());

        _scope = _fixture.Factory.Services.CreateAsyncScope();

        _validator = new UpdateRoles.RequestValidator(_scope.ServiceProvider.GetRequiredService<AppDbContext>());
    }

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }
}