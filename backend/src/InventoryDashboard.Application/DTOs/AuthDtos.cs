namespace InventoryDashboard.Application.DTOs;

public record LoginRequestDto(string Username, string Password);

public record ForgotPasswordRequestDto(string UsernameOrEmail);

public record ResetPasswordRequestDto(string Token, string NewPassword);

public record RefreshTokenRequestDto(string RefreshToken);

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    Guid UserId,
    string Username,
    string FullName,
    string Role);

public record MessageResponseDto(string Message, string? ResetToken = null);
