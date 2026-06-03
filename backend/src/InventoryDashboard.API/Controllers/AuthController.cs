using InventoryDashboard.Application.DTOs;
using InventoryDashboard.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace InventoryDashboard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);
        if (result is null)
            return Unauthorized(new { message = "Invalid username or password." });

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> Refresh(
        [FromBody] RefreshTokenRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await authService.RefreshAsync(request, cancellationToken);
        if (result is null)
            return Unauthorized(new { message = "Invalid or expired refresh token." });

        return Ok(result);
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<MessageResponseDto>> ForgotPassword(
        [FromBody] ForgotPasswordRequestDto request,
        CancellationToken cancellationToken)
    {
        return Ok(await authService.ForgotPasswordAsync(request, cancellationToken));
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<MessageResponseDto>> ResetPassword(
        [FromBody] ResetPasswordRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await authService.ResetPasswordAsync(request, cancellationToken));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
