using InventoryDashboard.Application.DTOs;
using InventoryDashboard.Application.Interfaces;
using InventoryDashboard.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryDashboard.API.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUsersController(
    IAuthService authService,
    IUserRepository userRepository,
    ILogger<AdminUsersController> logger) : ControllerBase
{
    [HttpGet("staff")]
    public async Task<ActionResult<IReadOnlyList<StaffUserDto>>> ListStaff(CancellationToken cancellationToken)
    {
        var staff = await userRepository.GetStaffUsersAsync(cancellationToken);
        var dtos = staff.Select(u => new StaffUserDto(
            u.Id,
            u.Username,
            u.FullName,
            u.Email,
            u.Role.ToString(),
            u.CreatedAt)).ToList();
        return Ok(dtos);
    }

    [HttpPost("staff")]
    public async Task<ActionResult<StaffUserDto>> CreateStaff(
        [FromBody] CreateStaffUserDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await authService.CreateStaffUserAsync(request, cancellationToken);
            return CreatedAtAction(nameof(CreateStaff), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "Create staff database error");
            return BadRequest(new { message = "Username or email is already registered." });
        }
    }
}
