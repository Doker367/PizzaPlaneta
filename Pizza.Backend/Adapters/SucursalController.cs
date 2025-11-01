using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza.Backend.Application.DTOs;
using Pizza.Backend.Ports;

namespace Pizza.Backend.Adapters;

[Authorize]
[ApiController]
[Route("api/sucursales")]
public class SucursalController : ControllerBase
{
    private readonly ISucursalService _sucursalService;
    private readonly IMenuService _menuService; // Added IMenuService

    public SucursalController(ISucursalService sucursalService, IMenuService menuService)
    {
        _sucursalService = sucursalService;
        _menuService = menuService; // Initialize IMenuService
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sucursales = await _sucursalService.GetAllAsync();
        return Ok(sucursales);
    }

    [HttpGet("{id}/menu")]
    public async Task<IActionResult> GetMenu(int id)
    {
        var menu = await _sucursalService.GetMenuBySucursalId(id);
        return Ok(menu);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateSucursalDto sucursalDto)
    {
        var createdSucursal = await _sucursalService.CreateAsync(sucursalDto);
        return Ok(createdSucursal);
    }

    [HttpPost("{sucursalId}/menu")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddMenuItem(int sucursalId, AddMenuItemDto menuItemDto)
    {
        var newMenuItem = await _sucursalService.AddMenuItemAsync(sucursalId, menuItemDto);
        return Ok(newMenuItem);
    }

    [HttpPut("{sucursalId}/menu/{menuItemId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateMenuItem(int sucursalId, int menuItemId, UpdateMenuItemDto menuItemDto)
    {
        try
        {
            // Note: sucursalId is used for routing, but the update is on the specific menuItemId
            var updatedMenuItem = await _menuService.UpdateAsync(menuItemId, menuItemDto);
            return Ok(updatedMenuItem);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{sucursalId}/menu/{menuItemId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteMenuItem(int sucursalId, int menuItemId)
    {
        try
        {
            // Note: sucursalId is used for routing, but the deletion is on the specific menuItemId
            await _menuService.DeleteAsync(menuItemId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
