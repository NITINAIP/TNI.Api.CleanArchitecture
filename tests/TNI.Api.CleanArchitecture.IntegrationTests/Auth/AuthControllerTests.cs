using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TNI.Api.CleanArchitecture.Application.Auth.DTOs;
using Xunit;

namespace TNI.Api.CleanArchitecture.IntegrationTests.Auth;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidRequest_ShouldReturn201()
    {
        var payload = new { Email = "newuser@test.com", Password = "Password1!", ConfirmPassword = "Password1!" };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<RegisteredUserDto>();
        body!.Email.Should().Be("newuser@test.com");
        body.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Register_DuplicateEmail_ShouldReturn409()
    {
        var payload = new { Email = "duplicate@test.com", Password = "Password1!", ConfirmPassword = "Password1!" };
        await _client.PostAsJsonAsync("/api/v1/auth/register", payload);

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Register_WeakPassword_ShouldReturn422()
    {
        var payload = new { Email = "user@test.com", Password = "short", ConfirmPassword = "short" };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", payload);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturn200WithTokenPair()
    {
        // First register
        var email = "logintest@test.com";
        var password = "Password1!";
        await _client.PostAsJsonAsync("/api/v1/auth/register",
            new { Email = email, Password = password, ConfirmPassword = password });

        // Then login
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new { Email = email, Password = password });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tokens = await response.Content.ReadFromJsonAsync<TokenPairDto>();
        tokens!.AccessToken.Should().NotBeNullOrWhiteSpace();
        tokens.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturn401()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new { Email = "nonexistent@test.com", Password = "wrongpassword" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_WithValidToken_ShouldReturn200WithNewTokenPair()
    {
        // Register + login first
        var email = "refreshtest@test.com";
        var password = "Password1!";
        await _client.PostAsJsonAsync("/api/v1/auth/register",
            new { Email = email, Password = password, ConfirmPassword = password });
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new { Email = email, Password = password });
        var tokens = await loginResponse.Content.ReadFromJsonAsync<TokenPairDto>();

        // Refresh
        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh",
            new { RefreshToken = tokens!.RefreshToken });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var newTokens = await response.Content.ReadFromJsonAsync<TokenPairDto>();
        newTokens!.AccessToken.Should().NotBeNullOrWhiteSpace();
        newTokens.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Refresh_WithInvalidToken_ShouldReturn401()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh",
            new { RefreshToken = "invalid-token-xyz" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
