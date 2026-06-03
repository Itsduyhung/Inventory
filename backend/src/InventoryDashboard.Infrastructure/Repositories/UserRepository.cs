using InventoryDashboard.Domain.Entities;
using InventoryDashboard.Domain.Interfaces;
using InventoryDashboard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryDashboard.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return context.Users
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return context.Users.FindAsync([id], cancellationToken).AsTask();
    }

    public Task<User?> GetByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken cancellationToken = default)
    {
        return context.Users
            .FirstOrDefaultAsync(
                u => u.RefreshTokenHash == refreshTokenHash && u.IsActive,
                cancellationToken);
    }

    public Task<User?> GetByPasswordResetTokenAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return context.Users
            .FirstOrDefaultAsync(u => u.PasswordResetToken == tokenHash, cancellationToken);
    }

    public Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        return context.Users.AnyAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetStaffUsersAsync(CancellationToken cancellationToken = default)
    {
        return await context.Users
            .Where(u => u.Role == Domain.Enums.UserRole.Staff && u.IsActive)
            .OrderBy(u => u.FullName)
            .ToListAsync(cancellationToken);
    }
}
