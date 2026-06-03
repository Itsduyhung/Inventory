using InventoryDashboard.Domain.Entities;

namespace InventoryDashboard.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken cancellationToken = default);
    Task<User?> GetByPasswordResetTokenAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);
    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> GetStaffUsersAsync(CancellationToken cancellationToken = default);
}
