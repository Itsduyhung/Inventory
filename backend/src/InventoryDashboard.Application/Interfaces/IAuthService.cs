using InventoryDashboard.Application.DTOs;

namespace InventoryDashboard.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<StaffUserDto> CreateStaffUserAsync(CreateStaffUserDto request, CancellationToken cancellationToken = default);
    Task<AuthResponseDto?> RefreshAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request, CancellationToken cancellationToken = default);
}
