using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using CryptoBank.WebAPI.Common.Services.PasswordHasher;
using CryptoBank.WebAPI.Domain;
using CryptoBank.WebAPI.Features.Auth.Options;
using CryptoBank.WebAPI.Features.Auth.Requests;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Testing.CryptoBank.WebAPI.Integrations.Errors.Contracts;
using Testing.CryptoBank.WebAPI.Integrations.Features.Auth.Contracts;
using Testing.CryptoBank.WebAPI.Integrations.Fixtures;
using Testing.CryptoBank.WebAPI.Integrations.Helpers;

namespace Testing.CryptoBank.WebAPI.Integrations.Features.Auth;

[Collection(AuthTestsCollection.Name)]
public class AuthenticateTests: IAsyncLifetime
{
    private readonly TestAuthFixture _userFixture;

    private AsyncServiceScope _scope;

    public AuthenticateTests(TestAuthFixture userFixture)
    {
        _userFixture = userFixture;
    }

    private async Task<(AccessTokenContract?, HttpResponseMessage)> Act(AuthenticateRequestContract request)
    {
        var client = _userFixture.HttpClient.CreateClient();
        return await client.PostAsJsonAsync<AccessTokenContract>("/auth/authenticate", request, Create.CancellationToken());
    }
   
    [Fact]
    public async Task Should_authenticate_user()
    {
        // Arrange
        var passwordHasher = _scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        
        var user = new User
        {
            Email = "test@test.com",
            PasswordHash = passwordHasher.Hash("qwerty123456A!"),
            BirthDate = new DateOnly(2000, 01, 31),
            RegisteredAt = DateTime.UtcNow,
        };
        
        await _userFixture.Database.Execute(async x =>
        {
            x.Users.Add(user);
            await x.SaveChangesAsync();
        });

        // Act
         var (responsePost, httpResponse) = await Act(new("test@test.com", "qwerty123456A!"));
        
         httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
         responsePost.Should().NotBeNull();
         
         responsePost!.AccessToken.Should().NotBeNullOrEmpty();

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtOptions = _scope.ServiceProvider.GetRequiredService<IOptions<AuthOptions>>().Value.Jwt;
        var key = jwtOptions.SigningKey;
        
        tokenHandler.ValidateToken(responsePost.AccessToken, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(key)),
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
        }, out var validatedToken);

        validatedToken.Should().NotBeNull();
        var jwtToken = (JwtSecurityToken)validatedToken;
        var userId = long.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
        var email = jwtToken.Claims.First(x => x.Type == ClaimTypes.Email).Value;
        
        
        userId.Should().Be(user.Id);
        email.Should().Be(user.Email);
    }

    [Fact]
    public async Task Should_not_authenticate_user_with_wrong_password()
    {
        // Arrange
        var passwordHasher = _scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        
        var user = new User
        {
            Email = "test@test.com",
            PasswordHash = passwordHasher.Hash("qwerty123456A!"),
            BirthDate = new DateOnly(2000, 01, 31),
            RegisteredAt = DateTime.UtcNow,
        };
        
        await _userFixture.Database.Execute(async x =>
        {
            x.Users.Add(user);
            await x.SaveChangesAsync();
        });
        
        
        var client = _userFixture.HttpClient.CreateClient();
        
        // Act
        var (responsePost, httpResponse) = await client.PostAsJsonAsync<ValidationProblemDetailsContract>("/auth/authenticate", new
        {
            Email = "test@test.com",
            Password = "wrong_password",
        }, Create.CancellationToken());

        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responsePost.Should().NotBeNull();
        // Assert
        responsePost!.ShouldContain(string.Empty, "Wrong email or password", "auth_validation_wrong_credentials");
    }

    
    
    public record AuthenticateRequestContract(string Email, string Password);
    
    public async Task InitializeAsync()
    {
        await _userFixture.Database.Clear(Create.CancellationToken());

        _scope = _userFixture.Factory.Services.CreateAsyncScope();
    }

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }
}

public class AuthenticateValidatorTests
{
    private readonly Authenticate.RequestValidator _validator = new();

    [Fact]
    public void Should_validate_correct_request()
    {
        var result = _validator.TestValidate(new Authenticate.Request("test@test.com", "password"));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_require_email(string email)
    {
        var result = _validator.TestValidate(new Authenticate.Request(email, "password"));
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorCode("auth_validation_email_required");
    }

    [Theory]
    [InlineData("test")]
    [InlineData("@test.com")]
    [InlineData("test@")]
    public void Should_validate_email_format(string email)
    {
        var result = _validator.TestValidate(new Authenticate.Request(email, "password"));
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorCode("auth_validation_invalid_email_format");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_require_password(string password)
    {
        var result = _validator.TestValidate(new Authenticate.Request("test@test.com", password));
        result.ShouldHaveValidationErrorFor(x => x.Password).WithErrorCode("auth_validation_password_required");
    }
}