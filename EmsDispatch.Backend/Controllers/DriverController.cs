using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EmsDispatch.Backend.DTOs;
using EmsDispatch.Backend.Services;
using EmsDispatch.Backend.Models.Enums;

namespace EmsDispatch.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DriverController : ControllerBase
{
    private readonly IDriverService _driverService;
    private readonly ILogger<DriverController> _logger;

    public DriverController(IDriverService driverService, ILogger<DriverController> logger)
    {
        _driverService = driverService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<string>>> CreateDriver([FromBody] CreateDriverDto dto)
    {
        try
        {
            var driverId = await _driverService.CreateDriverAsync(dto.UserId, dto.LicenseNumber);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Driver created successfully",
                Data = driverId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Create driver error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "Error creating driver" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<DriverDto>>> GetDriver(string id)
    {
        try
        {
            var driver = await _driverService.GetDriverByIdAsync(id);
            if (driver == null)
                return NotFound(new ApiResponse { Success = false, Message = "Driver not found" });

            return Ok(new ApiResponse<DriverDto>
            {
                Success = true,
                Message = "Driver retrieved",
                Data = driver
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get driver error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "Error retrieving driver" });
        }
    }

    [HttpGet("available")]
    public async Task<ActionResult<ApiResponse<List<DriverDto>>>> GetAvailableDrivers()
    {
        try
        {
            var drivers = await _driverService.GetAvailableDriversAsync();

            return Ok(new ApiResponse<List<DriverDto>>
            {
                Success = true,
                Message = "Available drivers retrieved",
                Data = drivers
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get available drivers error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "Error retrieving drivers" });
        }
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin,Dispatcher")]
    public async Task<ActionResult<ApiResponse<List<DriverDto>>>> GetAllDrivers()
    {
        try
        {
            var drivers = await _driverService.GetAllDriversAsync();

            return Ok(new ApiResponse<List<DriverDto>>
            {
                Success = true,
                Message = "All drivers retrieved",
                Data = drivers
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get all drivers error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "Error retrieving drivers" });
        }
    }

    [HttpPut("{id}/location")]
    [Authorize(Roles = "Driver")]
    public async Task<ActionResult<ApiResponse>> UpdateLocation(string id, [FromBody] LocationDto location)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await _driverService.UpdateDriverLocationAsync(id, location);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Location updated successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ApiResponse { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Update location error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "Error updating location" });
        }
    }

    [HttpPut("{id}/status/{status}")]
    [Authorize(Roles = "Driver,Admin")]
    public async Task<ActionResult<ApiResponse>> UpdateDriverStatus(string id, string status)
    {
        try
        {
            if (!Enum.TryParse<DriverStatus>(status, true, out var driverStatus))
                return BadRequest(new ApiResponse { Success = false, Message = "Invalid status" });

            await _driverService.UpdateDriverStatusAsync(id, driverStatus);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Driver status updated successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ApiResponse { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Update driver status error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "Error updating driver status" });
        }
    }
}

public class CreateDriverDto
{
    public string UserId { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
}
