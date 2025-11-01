using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza.Backend.Ports;

namespace Pizza.Backend.Adapters;

[Authorize]
[ApiController]
[Route("api/menu")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var menus = await _menuService.GetAllAsync();
        return Ok(menus);
    }
}
