using ManyBoxApi.Models;
using ManyBoxApi.Models.Pickup; // <-- Add using for new models
using ManyBoxApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ManyBoxApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DhlController : ControllerBase
    {
        private readonly DhlService _dhlService;
        private readonly ILogger<DhlController> _logger;

        public DhlController(DhlService dhlService, ILogger<DhlController> logger)
        {
            _dhlService = dhlService;
            _logger = logger;
        }

        [HttpGet("rates")]
        public async Task<IActionResult> GetRates([FromQuery] DhlRateRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState inválido en GetRates: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("[DHL][GetRates] Parámetros recibidos: {@Request}", request);
                // Loguea cada parámetro individualmente para mayor detalle
                _logger.LogInformation("[DHL][GetRates] accountNumber={AccountNumber}, originCountryCode={OriginCountryCode}, originCityName={OriginCityName}, destinationCountryCode={DestinationCountryCode}, destinationCityName={DestinationCityName}, weight={Weight}, length={Length}, width={Width}, height={Height}, plannedShippingDate={PlannedShippingDate}, isCustomsDeclarable={IsCustomsDeclarable}, unitOfMeasurement={UnitOfMeasurement}, originPostalCode={OriginPostalCode}, destinationPostalCode={DestinationPostalCode}, nextBusinessDay={NextBusinessDay}",
                    request.AccountNumber, request.OriginCountryCode, request.OriginCityName, request.DestinationCountryCode, request.DestinationCityName,
                    request.Weight, request.Length, request.Width, request.Height, request.PlannedShippingDate, request.IsCustomsDeclarable, request.UnitOfMeasurement, request.OriginPostalCode, request.DestinationPostalCode, request.NextBusinessDay);

                var rates = await _dhlService.GetRatesAsync(request);
                _logger.LogInformation("[DHL][GetRates] Respuesta recibida de DHL: {Rates}", rates);
                return Ok(rates);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "[DHL][GetRates] Error de comunicación con DHL API. Mensaje: {Message}", ex.Message);
                return StatusCode(500, new { message = "Error communicating with DHL API", error = ex.Message, stack = ex.StackTrace });
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "[DHL][GetRates] Error deserializando respuesta de DHL. Mensaje: {Message}", ex.Message);
                return StatusCode(500, new { message = "Error deserializing DHL response", error = ex.Message, stack = ex.StackTrace });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DHL][GetRates] Excepción inesperada. Mensaje: {Message}", ex.Message);
                return StatusCode(500, new { message = "Unexpected error in DHL rates", error = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpPost("shipments")]
        public async Task<IActionResult> CreateShipment([FromBody] DhlCreateShipmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var shipmentResponse = await _dhlService.CreateShipmentAsync(request);
                return Ok(shipmentResponse);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { message = "Error communicating with DHL API", error = ex.Message });
            }
            catch (JsonException ex)
            {
                return StatusCode(500, new { message = "Error deserializing DHL response", error = ex.Message });
            }
        }

        [HttpGet("tracking")]
        public async Task<IActionResult> TrackShipment([FromQuery] string trackingNumbers)
        {
            if (string.IsNullOrEmpty(trackingNumbers))
            {
                return BadRequest(new { message = "Tracking numbers are required." });
            }

            try
            {
                var trackingResponse = await _dhlService.TrackShipmentAsync(trackingNumbers);
                return Ok(trackingResponse);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { message = "Error communicating with DHL API", error = ex.Message });
            }
            catch (JsonException ex)
            {
                return StatusCode(500, new { message = "Error deserializing DHL response", error = ex.Message });
            }
        }

        [HttpPost("pickup")] // Changed route to /api/Dhl/pickup for clarity
        public async Task<IActionResult> CreatePickup([FromBody] PickupRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var pickupResponse = await _dhlService.CreatePickupAsync(request);
                return Ok(pickupResponse);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { message = "Error communicating with DHL API for pickup", error = ex.Message });
            }
            catch (JsonException ex)
            {
                return StatusCode(500, new { message = "Error deserializing DHL pickup response", error = ex.Message });
            }
        }

        [HttpGet("address-validate")]
        public async Task<IActionResult> ValidateAddress([FromQuery] DhlAddressValidateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var validationResponse = await _dhlService.ValidateAddressAsync(request);
                return Ok(validationResponse);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { message = "Error communicating with DHL API for address validation", error = ex.Message });
            }
            catch (JsonException ex)
            {
                return StatusCode(500, new { message = "Error deserializing DHL address validation response", error = ex.Message });
            }
        }

        [HttpPost("landed-cost")]
        public async Task<IActionResult> GetLandedCost([FromBody] DhlLandedCostRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var landedCostResponse = await _dhlService.GetLandedCostAsync(request);
                return Ok(landedCostResponse);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { message = "Error communicating with DHL API for landed cost", error = ex.Message });
            }
            catch (JsonException ex)
            {
                return StatusCode(500, new { message = "Error deserializing DHL landed cost response", error = ex.Message });
            }
        }

        [HttpDelete("pickups/{dispatchConfirmationNumber}")]
        public async Task<IActionResult> CancelPickup(string dispatchConfirmationNumber, [FromQuery] string requestorName, [FromQuery] string reason)
        {
            if (string.IsNullOrEmpty(requestorName) || string.IsNullOrEmpty(reason))
            {
                return BadRequest("Requestor name and reason are required.");
            }

            try
            {
                await _dhlService.CancelPickupAsync(dispatchConfirmationNumber, requestorName, reason);
                return Ok(new { message = "Pickup cancellation request sent successfully." });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { message = "Error communicating with DHL API for pickup cancellation", error = ex.Message });
            }
        }

        [HttpGet("shipments/{shipmentTrackingNumber}/proof-of-delivery")]
        public async Task<IActionResult> GetProofOfDelivery(string shipmentTrackingNumber, [FromQuery] string? shipperAccountNumber, [FromQuery] string? content)
        {
            try
            {
                var podResponse = await _dhlService.GetProofOfDeliveryAsync(shipmentTrackingNumber, shipperAccountNumber, content);
                return Ok(podResponse);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { message = "Error communicating with DHL API for proof of delivery", error = ex.Message });
            }
            catch (JsonException ex)
            {
                return StatusCode(500, new { message = "Error deserializing DHL proof of delivery response", error = ex.Message });
            }
        }

        [HttpGet("shipments/{shipmentTrackingNumber}/get-image")]
        public async Task<IActionResult> GetImage(string shipmentTrackingNumber, [FromQuery] DhlGetImageRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var imageResponse = await _dhlService.GetImageAsync(shipmentTrackingNumber, request);
                return Ok(imageResponse);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { message = "Error communicating with DHL API for get image", error = ex.Message });
            }
            catch (JsonException ex)
            {
                return StatusCode(500, new { message = "Error deserializing DHL get image response", error = ex.Message });
            }
        }

        [HttpPatch("pickups/{dispatchConfirmationNumber}")]
        public async Task<IActionResult> UpdatePickup(string dispatchConfirmationNumber, [FromBody] DhlUpdatePickupRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updateResponse = await _dhlService.UpdatePickupAsync(dispatchConfirmationNumber, request);
                return Ok(updateResponse);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { message = "Error communicating with DHL API for update pickup", error = ex.Message });
            }
            catch (JsonException ex)
            {
                return StatusCode(500, new { message = "Error deserializing DHL update pickup response", error = ex.Message });
            }
        }
    }
}