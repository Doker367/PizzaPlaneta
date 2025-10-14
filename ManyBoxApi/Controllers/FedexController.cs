using Microsoft.AspNetCore.Mvc;
using ManyBoxApi.Models;
using ManyBoxApi.Services;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class FedexController : ControllerBase
{
    private readonly FedexAddressValidationService _fedexService;

    public FedexController(FedexAddressValidationService fedexService)
    {
        _fedexService = fedexService;
    }

    [HttpPost("validate-address")]
    public async Task<IActionResult> ValidateAddress([FromBody] FedexPostalRequest request)
    {
        var result = await _fedexService.ValidateAddressAsync(request);
        return Ok(result);
    }
}