using HQVerse.Application.DTOs.Auth;
using HQVerse.Application.Interfaces;
using HQVerse.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace HQVerse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    private int GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return claim is not null ? int.Parse(claim) : 0;
    }

    [HttpPost("register")]
    [EnableRateLimiting("AuthPolicy")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<AuthResponseDto>> Register(
        [FromBody] RegisterDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(dto, cancellationToken);
        return Ok(result);
    }

    [HttpPost("login")]
    [EnableRateLimiting("AuthPolicy")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<AuthResponseDto>> Login(
        [FromBody] LoginDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(dto, cancellationToken);
        return Ok(result);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Refresh(
        [FromBody] RefreshTokenDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(dto.RefreshToken, cancellationToken);
        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await _authService.LogoutAsync(userId, cancellationToken);
        return NoContent();
    }

    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDto>> GetProfile(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var user = await _authService.GetProfileAsync(userId, cancellationToken);
        return Ok(user);
    }

    [HttpPut("profile")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> UpdateProfile(
        [FromBody] UpdateUserDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var user = await _authService.UpdateProfileAsync(userId, dto, cancellationToken);
        return Ok(user);
    }

    [HttpPost("change-password")]
    [Authorize]
    [EnableRateLimiting("AuthPolicy")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await _authService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword, cancellationToken);
        return NoContent();
    }
}
