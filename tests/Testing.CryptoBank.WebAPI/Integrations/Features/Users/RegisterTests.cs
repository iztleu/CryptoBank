using System.Net.Http.Json;
using CryptoBank.WebAPI.Common.Services.PasswordHasher;
using CryptoBank.WebAPI.Database;
using CryptoBank.WebAPI.Domain;
using CryptoBank.WebAPI.Features.Users.Options;
using CryptoBank.WebAPI.Features.Users.Requests;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Testing.CryptoBank.WebAPI.Integrations.Fixtures;
using Testing.CryptoBank.WebAPI.Integrations.Helpers;

namespace Testing.CryptoBank.WebAPI.Integrations.Features.Users;

[Collection(UsersTestsCollection.Name)]
public class RegisterTests : IAsyncLifetime
{
    private readonly TestFixture _fixture;

    private AsyncServiceScope _scope;

    public RegisterTests(TestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task Should_register_user()
    {
        // Arrange
        var client = _fixture.HttpClient.CreateClient();
    
        // Act
        (await client.PostAsJsonAsync("/users", new
            {
                Email = "test@test.com",
                Password = "qwerty123456A!",
                BirthDate = "2000-01-31",
            }))
            .EnsureSuccessStatusCode();

        // Assert
        var user = await _fixture.Database.Execute(async x =>
            await x.Users.SingleOrDefaultAsync(u => u.Email == "test@test.com"));

        user.Should().NotBeNull();
        user!.RegisteredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        user.BirthDate.Should().Be(new DateOnly(2000, 01, 31));

        var passwordHasher = _scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        passwordHasher.Verify(user.PasswordHash, "qwerty123456A!").Should().BeTrue();
    }

    [Fact]
    public async Task Should_register_user_and_set_user_role()
    {
        // Arrange
        var client = _fixture.HttpClient.CreateClient();

        // Act
        (await client.PostAsJsonAsync("/users", new
            {
                Email = "test@test.com",
                Password = "qwerty123456A!",
                BirthDate = "2000-01-31",
            }))
            .EnsureSuccessStatusCode();

        // Assert
        var user = await _fixture.Database.Execute(async x =>
            await x.Users.SingleOrDefaultAsync(u => u.Email == "test@test.com"));

        user.Should().NotBeNull();
        user!.Roles.Should().Contain(new List<Role>{Role.User});
        user.Roles.Should().NotContain(new List<Role>{Role.Analyst, Role.Administrator});
    }
    
    [Fact]
    public async Task Should_register_user_and_set_admin_roles()
    {
        // Arrange
        var client = _fixture.HttpClient.CreateClient();
        var option = _fixture.Factory.Services.GetRequiredService<IOptions<UsersOptions>>();

        // Act
        (await client.PostAsJsonAsync("/users", new
            {
                Email = option.Value.AdministratorEmail,
                Password = "qwerty123456A!",
                BirthDate = "2000-01-31",
            }))
            .EnsureSuccessStatusCode();

        // Assert
        var user = await _fixture.Database.Execute(async x =>
            await x.Users.SingleOrDefaultAsync(u => u.Email == option.Value.AdministratorEmail));

        user.Should().NotBeNull();
        user!.Roles.Should().Contain(new List<Role>{Role.Administrator, Role.User});
        user.Roles.Should().NotContain(new List<Role>{Role.Analyst});
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
public class RegisterValidatorTests : IAsyncLifetime
{
    private readonly TestFixture _fixture;
    
    private AsyncServiceScope _scope;

    private Register.RequestValidator? _validator;

    public RegisterValidatorTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Should_validate_correct_request()
    {
        var result = await _validator.TestValidateAsync(
            new Register.Request("test@test.com", "password", new DateOnly(2000, 01, 31)));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Should_require_email(string email)
    {
        var result = await _validator.TestValidateAsync(
            new Register.Request(email, "password", new DateOnly(2000, 01, 31)));
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorCode("users_validation_email_required");
    }

    [Theory]
    [InlineData("test")]
    [InlineData("@test.com")]
    [InlineData("test@")]
    public async Task Should_validate_email_format(string email)
    {
        var result = await _validator.TestValidateAsync(new
            Register.Request(email, "password", new DateOnly(2000, 01, 31)));
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorCode("users_validation_email_format_is_wrong");
    }

    [Fact]
    public async Task Should_validate_email_taken()
    {
        const string email = "test@test.com";

        var existingUser = new User
        {
            Email = email,
            PasswordHash = "123",
            RegisteredAt = DateTime.UtcNow,
            BirthDate = new DateOnly(2000, 01, 31)
        };

        await _fixture.Database.Execute(async x =>
        {
            x.Users.Add(existingUser);
            await x.SaveChangesAsync();
        });

        var result = await _validator.TestValidateAsync(new
            Register.Request(email, "password", new DateOnly(2000, 01, 31)));
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorCode("users_validation_email_already_exists");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Should_require_password(string password)
    {
        var result = await _validator.TestValidateAsync(
            new Register.Request("test@test.com", password, new DateOnly(2000, 01, 31)));
        result.ShouldHaveValidationErrorFor(x => x.Password).WithErrorCode("users_validation_password_required");
    }

    [Theory]
    [InlineData("1")]
    [InlineData("123")]
    [InlineData("123456")]
    public async Task Should_validate_password_length(string password)
    {
        var result = await _validator.TestValidateAsync(
            new Register.Request("test@test.com", password, new DateOnly(2000, 01, 31)));
        result.ShouldHaveValidationErrorFor(x => x.Password).WithErrorCode("users_validation_password_to_short");
    }

    
    public async Task InitializeAsync()
    {
        await _fixture.Database.Clear(Create.CancellationToken());

        _scope = _fixture.Factory.Services.CreateAsyncScope();

        _validator = new Register.RequestValidator(_scope.ServiceProvider.GetRequiredService<AppDbContext>());
    }

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }
}
