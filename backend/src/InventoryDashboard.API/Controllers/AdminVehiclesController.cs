using System.Security.Claims;
using InventoryDashboard.Application.DTOs;
using InventoryDashboard.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryDashboard.API.Controllers;

[ApiController]
[Route("api/admin/vehicles")]
[Authorize(Roles = "Admin")]
public class AdminVehiclesController(IVehicleService vehicleService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<VehicleDto>>> GetVehicles(
        [FromQuery] VehicleQueryDto query,
        [FromQuery] bool includeDisabled = true,
        CancellationToken cancellationToken = default)
    {
        var result = await vehicleService.GetVehiclesAsync(
            query, includeDisabled, staffAvailableOnly: false, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<VehicleDetailDto>> GetVehicle(
        Guid id,
        CancellationToken cancellationToken)
    {
        var vehicle = await vehicleService.GetVehicleDetailAsync(id, cancellationToken);
        return vehicle is null ? NotFound() : Ok(vehicle);
    }

    [HttpPost]
    public async Task<ActionResult<VehicleDetailDto>> CreateVehicle(
        [FromBody] CreateVehicleDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await vehicleService.CreateVehicleAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetVehicle), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<VehicleDetailDto>> UpdateVehicle(
        Guid id,
        [FromBody] UpdateVehicleDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await vehicleService.UpdateVehicleAsync(id, dto, cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("summary")]
    public async Task<ActionResult<InventorySummaryDto>> GetSummary(CancellationToken cancellationToken)
    {
        return Ok(await vehicleService.GetSummaryAsync(cancellationToken));
    }

    [HttpPatch("{id:guid}/disable")]
    public async Task<ActionResult<VehicleDetailDto>> DisableVehicle(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await vehicleService.DisableVehicleAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id:guid}/add-quantity")]
    public async Task<ActionResult<VehicleDetailDto>> AddQuantity(
        Guid id,
        [FromBody] AddQuantityDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await vehicleService.AddQuantityAsync(id, dto.Amount, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
