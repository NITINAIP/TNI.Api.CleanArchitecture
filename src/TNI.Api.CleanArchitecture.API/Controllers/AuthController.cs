using Microsoft.AspNetCore.Mvc;
using TNI.Api.CleanArchitecture.Application.Auth.Commands.Login;
using TNI.Api.CleanArchitecture.Application.Auth.Commands.RefreshToken;
using TNI.Api.CleanArchitecture.Application.Auth.Commands.RegisterUser;
using TNI.Api.CleanArchitecture.Application.Auth.DTOs;

namespace TNI.Api.CleanArchitecture.API.Controllers;

/// <summary>Authentication endpoints for registration, login, and token refresh.</summary>
public class AuthController : BaseApiController
{
    /// <summary>Register a new user account.</summary>
    /// <response code="201">User created successfully.</response>
    /// <response code="409">Email already in use.</response>
    /// <response code="422">Validation error.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisteredUserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(Register), new { id = result.Id }, result);
    }

    /// <summary>Login with email and password to receive a token pair.</summary>
    /// <response code="200">Login successful, returns access and refresh tokens.</response>
    /// <response code="401">Invalid credentials.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenPairDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>Refresh an access token using a valid refresh token.</summary>
    /// <response code="200">Returns new token pair.</response>
    /// <response code="401">Invalid or expired refresh token.</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(TokenPairDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
