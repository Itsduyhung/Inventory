using InventoryDashboard.Application.DTOs;
using InventoryDashboard.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryDashboard.API.Controllers;

[ApiController]
[Route("api/admin/sales")]
[Authorize(Roles = "Admin")]
public class AdminSalesController(ISaleService saleService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<SaleRecordDto>>> GetSales(
        [FromQuery] AdminSaleHistoryQueryDto query,
        CancellationToken cancellationToken)
    {
        return Ok(await saleService.GetAllSalesHistoryAsync(query, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SaleRecordDetailDto>> GetSaleDetail(
        Guid id,
        CancellationToken cancellationToken)
    {
        var detail = await saleService.GetAdminSaleDetailAsync(id, cancellationToken);
        return detail is null ? NotFound() : Ok(detail);
    }
}
