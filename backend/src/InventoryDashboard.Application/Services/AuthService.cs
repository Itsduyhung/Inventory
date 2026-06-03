using System.Text.RegularExpressions;
using InventoryDashboard.Application.DTOs;
using InventoryDashboard.Application.Interfaces;
using InventoryDashboard.Domain.Entities;
using InventoryDashboard.Domain.Enums;
using InventoryDashboard.Domain.Interfaces;
using InventoryDashboard.Application.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InventoryDashboard.Application.Services;

public partial class AuthService(
    IUserRepository userRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IEmailService emailService,
    IConfiguration configuration,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<AuthResponseDto?> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByUsernameAsync(
            request.Username.Trim().ToLowerInvariant(),
            cancellationToken);
        if (user is null || !user.IsActive)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<StaffUserDto> CreateStaffUserAsync(
        CreateStaffUserDto request,
        CancellationToken cancellationToken = default)
    {
        var username = request.Username.Trim().ToLowerInvariant();
        ValidateUsername(username);
        ValidatePassword(request.Password);

        if (await userRepository.UsernameExistsAsync(username, cancellationToken))
            throw new InvalidOperationException("Username is already taken.");

        var email = string.IsNullOrWhiteSpace(request.Email)
            ? null
            : request.Email.Trim().ToLowerInvariant();

        if (email is not null && await userRepository.GetByEmailAsync(email, cancellationToken) is not null)
            throw new InvalidOperationException("Email is already registered.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            FullName = request.FullName.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.Staff,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await userRepository.AddAsync(user, cancellationToken);

        return new StaffUserDto(
            user.Id,
            user.Username,
            user.FullName,
            user.Email,
            user.Role.ToString(),
            user.CreatedAt);
    }

    public async Task<AuthResponseDto?> RefreshAsync(
        RefreshTokenRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return null;

        var hash = TokenHasher.Hash(request.RefreshToken);
        var user = await userRepository.GetByRefreshTokenHashAsync(hash, cancellationToken);
        if (user is null || !user.IsActive)
            return null;

        if (user.RefreshTokenExpiresAt is null || user.RefreshTokenExpiresAt < DateTime.UtcNow)
            return null;

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<MessageResponseDto> ForgotPasswordAsync(
        ForgotPasswordRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var input = request.UsernameOrEmail.Trim();
        var normalized = input.ToLowerInvariant();
        User? user = await userRepository.GetByUsernameAsync(normalized, cancellationToken)
            ?? await userRepository.GetByEmailAsync(normalized, cancellationToken);

        const string genericMessage =
            "If an account exists, password reset instructions have been sent.";

        if (user is null || !user.IsActive)
            return new MessageResponseDto(genericMessage);

        var resetToken = jwtTokenGenerator.GenerateRefreshToken();
        user.PasswordResetToken = TokenHasher.Hash(resetToken);
        user.PasswordResetExpiresAt = DateTime.UtcNow.AddHours(1);
        await userRepository.UpdateAsync(user, cancellationToken);

        var emailSent = false;
        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            try
            {
                await emailService.SendPasswordResetAsync(
                    user.Email,
                    user.FullName,
                    resetToken,
                    cancellationToken);
                emailSent = true;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Could not send password reset email to {Email}", user.Email);
            }
        }

        var includeTokenFallback = bool.Parse(configuration["Auth:ReturnResetTokenInResponse"] ?? "false");
        return new MessageResponseDto(
            emailSent
                ? "If an account exists, check your email for reset instructions."
                : genericMessage,
            !emailSent && includeTokenFallback ? resetToken : null);
    }

    public async Task<MessageResponseDto> ResetPasswordAsync(
        ResetPasswordRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ValidatePassword(request.NewPassword);

        var tokenHash = TokenHasher.Hash(request.Token.Trim());
        var user = await userRepository.GetByPasswordResetTokenAsync(tokenHash, cancellationToken);
        if (user is null || user.PasswordResetExpiresAt is null || user.PasswordResetExpiresAt < DateTime.UtcNow)
            throw new InvalidOperationException("Invalid or expired reset token.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetExpiresAt = null;
        user.RefreshTokenHash = null;
        user.RefreshTokenExpiresAt = null;
        await userRepository.UpdateAsync(user, cancellationToken);

        return new MessageResponseDto("Password has been reset. You can sign in with your new password.");
    }

    private async Task<AuthResponseDto> IssueTokensAsync(User user, CancellationToken cancellationToken)
    {
        var (accessToken, expiresAt) = jwtTokenGenerator.GenerateAccessToken(user);
        var refreshToken = jwtTokenGenerator.GenerateRefreshToken();
        var refreshDays = int.Parse(configuration["Jwt:RefreshTokenExpireDays"] ?? "7");

        user.RefreshTokenHash = TokenHasher.Hash(refreshToken);
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(refreshDays);
        await userRepository.UpdateAsync(user, cancellationToken);

        return BuildAuthResponse(user, accessToken, refreshToken, expiresAt);
    }

    private static AuthResponseDto BuildAuthResponse(
        User user,
        string accessToken,
        string refreshToken,
        DateTime expiresAt) =>
        new(
            accessToken,
            refreshToken,
            expiresAt,
            user.Id,
            user.Username,
            user.FullName,
            user.Role.ToString());

    private static void ValidateUsername(string username)
    {
        if (username.Length < 3 || username.Length > 32)
            throw new InvalidOperationException("Username must be 3–32 characters.");

        if (!UsernameRegex().IsMatch(username))
            throw new InvalidOperationException("Username may only contain letters, numbers, and underscores.");
    }

    private static void ValidatePassword(string password)
    {
        if (password.Length < 6)
            throw new InvalidOperationException("Password must be at least 6 characters.");
    }

    [GeneratedRegex("^[a-z0-9_]+$", RegexOptions.IgnoreCase)]
    private static partial Regex UsernameRegex();
}
