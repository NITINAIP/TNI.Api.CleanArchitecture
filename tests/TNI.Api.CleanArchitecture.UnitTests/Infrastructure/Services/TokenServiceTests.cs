using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TNI.Api.CleanArchitecture.Domain.Entities;
using TNI.Api.CleanArchitecture.Infrastructure.Persistence;
using TNI.Api.CleanArchitecture.Infrastructure.Services;
using Xunit;

namespace TNI.Api.CleanArchitecture.UnitTests.Infrastructure.Services;

public class TokenServiceTests
{
    private static TokenService CreateService(out ApplicationDbContext dbContext)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        dbContext = new ApplicationDbContext(options);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "JwtSettings:Secret", "test-secret-key-minimum-32-characters-long" },
                { "JwtSettings:Issuer", "test-issuer" },
                { "JwtSettings:Audience", "test-audience" },
                { "JwtSettings:AccessTokenExpirationMinutes", "60" }
            })
            .Build();

        return new TokenService(config, dbContext);
    }

    private static Dictionary<string, JsonElement> DecodeJwtPayload(string token)
    {
        var parts = token.Split('.');
        var payload = parts[1];
        payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
        var bytes = Convert.FromBase64String(payload);
        return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(bytes)!;
    }

    [Fact]
    public void GenerateAccessToken_ShouldReturnValidJwt()
    {
        var service = CreateService(out _);
        var user = User.Create("test@example.com", "hash");

        var token = service.GenerateAccessToken(user);

        token.Should().NotBeNullOrWhiteSpace();
        var payload = DecodeJwtPayload(token);
        payload["sub"].GetString().Should().Be(user.Id.ToString());
        payload["email"].GetString().Should().Be(user.Email);
        payload.Should().ContainKey("jti");
    }

    [Fact]
    public void GenerateAccessToken_ShouldHaveCorrectIssuerAndAudience()
    {
        var service = CreateService(out _);
        var user = User.Create("user@test.com", "hash");

        var token = service.GenerateAccessToken(user);
        var payload = DecodeJwtPayload(token);

        payload["iss"].GetString().Should().Be("test-issuer");
        payload["aud"].GetString().Should().Be("test-audience");
    }
}
