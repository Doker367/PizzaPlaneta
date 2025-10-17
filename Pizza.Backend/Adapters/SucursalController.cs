using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza.Backend.Ports;

namespace Pizza.Backend.Adapters;

[Authorize]
[ApiController]
[Route("api/sucursales")]
public class SucursalController : ControllerBase
{
    private readonly ISucursalService _sucursalService;

    public SucursalController(ISucursalService sucursalService)
    {
        _sucursalService = sucursalService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sucursales = await _sucursalService.GetAllAsync();
        return Ok(sucursales);
    }
}
