using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using InventoryDashboard.Application.Interfaces;
using InventoryDashboard.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace InventoryDashboard.Infrastructure.Auth;

public class JwtTokenGenerator(IConfiguration configuration) : IJwtTokenGenerator
{
    public (string AccessToken, DateTime ExpiresAt) GenerateAccessToken(User user)
    {
        var key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var issuer = configuration["Jwt:Issuer"] ?? "InventoryDashboard";
        var audience = configuration["Jwt:Audience"] ?? "InventoryDashboard";
        var expireMinutes = int.Parse(configuration["Jwt:AccessTokenExpireMinutes"] ?? "60");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("fullName", user.FullName)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(expireMinutes);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}
