using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmsDispatch.Backend.DTOs;
using EmsDispatch.Backend.Services;
using EmsDispatch.Backend.Models.Enums;

namespace EmsDispatch.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AmbulanceController : ControllerBase
{
    private readonly IAmbulanceService _ambulanceService;
    private readonly ILogger<AmbulanceController> _logger;

    public AmbulanceController(IAmbulanceService ambulanceService, ILogger<AmbulanceController> logger)
    {
        _ambulanceService = ambulanceService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<string>>> CreateAmbulance([FromBody] CreateAmbulanceDto dto)
    {
        try
        {
            var ambulanceId = await _ambulanceService.CreateAmbulanceAsync(dto.RegistrationNumber, dto.Capacity);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Ambulance created successfully",
                Data = ambulanceId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Create ambulance error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "Error creating ambulance" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<AmbulanceDto>>> GetAmbulance(string id)
    {
        try
        {
            var ambulance = await _ambulanceService.GetAmbulanceByIdAsync(id);
            if (ambulance == null)
                return NotFound(new ApiResponse { Success = false, Message = "Ambulance not found" });

            return Ok(new ApiResponse<AmbulanceDto>
            {
                Success = true,
                Message = "Ambulance retrieved",
                Data = ambulance
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get ambulance error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "Error retrieving ambulance" });
        }
    }

    [HttpGet("available")]
    public async Task<ActionResult<ApiResponse<List<AmbulanceDto>>>> GetAvailableAmbulances()
    {
        try
        {
            var ambulances = await _ambulanceService.GetAvailableAmbulancesAsync();

            return Ok(new ApiResponse<List<AmbulanceDto>>
            {
                Success = true,
                Message = "Available ambulances retrieved",
                Data = ambulances
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get available ambulances error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "Error retrieving ambulances" });
        }
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin,Dispatcher,EmsOperator")]
    public async Task<ActionResult<ApiResponse<List<AmbulanceDto>>>> GetAllAmbulances()
    {
        try
        {
            var ambulances = await _ambulanceService.GetAllAmbulancesAsync();

            return Ok(new ApiResponse<List<AmbulanceDto>>
            {
                Success = true,
                Message = "All ambulances retrieved",
                Data = ambulances
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get all ambulances error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "Error retrieving ambulances" });
        }
    }

    [HttpPut("{id}/location")]
    public async Task<ActionResult<ApiResponse>> UpdateLocation(string id, [FromBody] LocationDto location)
    {
        try
        {
            await _ambulanceService.UpdateAmbulanceLocationAsync(id, location);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Ambulance location updated successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ApiResponse { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Update ambulance location error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "Error updating location" });
        }
    }

    [HttpPut("{id}/status/{status}")]
    [Authorize(Roles = "Driver,Admin,EmsOperator")]
    public async Task<ActionResult<ApiResponse>> UpdateAmbulanceStatus(string id, string status)
    {
        try
        {
            if (!Enum.TryParse<DriverStatus>(status, true, out var ambulanceStatus))
                return BadRequest(new ApiResponse { Success = false, Message = "Invalid status" });

            await _ambulanceService.UpdateAmbulanceStatusAsync(id, ambulanceStatus);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Ambulance status updated successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ApiResponse { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Update ambulance status error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "Error updating ambulance status" });
        }
    }

    [HttpPut("{id}/assign/{driverId}")]
    [Authorize(Roles = "Admin,Dispatcher")]
    public async Task<ActionResult<ApiResponse>> AssignAmbulanceToDriver(string id, string driverId)
    {
        try
        {
            await _ambulanceService.AssignAmbulanceToDriverAsync(id, driverId);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Ambulance assigned to driver successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ApiResponse { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Assign ambulance error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "Error assigning ambulance" });
        }
    }
}

public class CreateAmbulanceDto
{
    public string RegistrationNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
}
