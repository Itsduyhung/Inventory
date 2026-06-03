using InventoryDashboard.Domain.Entities;

namespace InventoryDashboard.Application.Interfaces;

public interface IJwtTokenGenerator
{
    (string AccessToken, DateTime ExpiresAt) GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
