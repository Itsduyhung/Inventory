using System.Security.Claims;
using InventoryDashboard.Application.DTOs;
using InventoryDashboard.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryDashboard.API.Controllers;

[ApiController]
[Route("api/staff")]
[Authorize(Roles = "Staff")]
public class StaffController(
    IVehicleService vehicleService,
    ISaleService saleService) : ControllerBase
{
    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("inventory")]
    public async Task<ActionResult<PagedResultDto<VehicleDto>>> GetInventory(
        [FromQuery] VehicleQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await vehicleService.GetVehiclesAsync(
            query, includeDisabled: false, staffAvailableOnly: true, cancellationToken);
        return Ok(result);
    }

    [HttpGet("inventory/{id:guid}")]
    public async Task<ActionResult<VehicleDetailDto>> GetVehicleDetail(
        Guid id,
        CancellationToken cancellationToken)
    {
        var vehicle = await vehicleService.GetVehicleDetailAsync(id, cancellationToken);
        if (vehicle is null || vehicle.IsDisabled || vehicle.Quantity <= 0)
            return NotFound();

        return Ok(vehicle);
    }

    [HttpPost("inventory/{id:guid}/sale")]
    public async Task<ActionResult<VehicleDto>> ProcessSale(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await saleService.ProcessSaleAsync(id, GetUserId(), cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("history")]
    public async Task<ActionResult<PagedResultDto<SaleRecordDto>>> GetHistory(
        [FromQuery] SaleHistoryQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await saleService.GetStaffHistoryAsync(GetUserId(), query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("history/{id:guid}")]
    public async Task<ActionResult<SaleRecordDetailDto>> GetHistoryDetail(
        Guid id,
        CancellationToken cancellationToken)
    {
        var detail = await saleService.GetStaffSaleDetailAsync(id, GetUserId(), cancellationToken);
        return detail is null ? NotFound() : Ok(detail);
    }
}
