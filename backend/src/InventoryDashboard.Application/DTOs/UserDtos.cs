namespace InventoryDashboard.Application.DTOs;

public record CreateStaffUserDto(
    string Username,
    string Password,
    string FullName,
    string? Email = null);

public record StaffUserDto(
    Guid Id,
    string Username,
    string FullName,
    string? Email,
    string Role,
    DateTime CreatedAt);
