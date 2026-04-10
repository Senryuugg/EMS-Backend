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
public class DispatchController : ControllerBase
{
    private readonly IDispatchService _dispatchService;
    private readonly ILogger<DispatchController> _logger;

    public DispatchController(IDispatchService dispatchService, ILogger<DispatchController> logger)
    {
        _dispatchService = dispatchService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Dispatcher,Admin")]
    public async Task<ActionResult<ApiResponse<string>>> CreateDispatch([FromBody] DispatchDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var dispatchId = await _dispatchService.CreateDispatchAsync(dto, userId);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Dispatch created successfully",
                Data = dispatchId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Create dispatch error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "Error creating dispatch" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<DispatchDto>>> GetDispatch(string id)
    {
        try
        {
            var dispatch = await _dispatchService.GetDispatchByIdAsync(id);
            if (dispatch == null)
                return NotFound(new ApiResponse { Success = false, Message = "Dispatch not found" });

            return Ok(new ApiResponse<DispatchDto>
            {
                Success = true,
                Message = "Dispatch retrieved",
                Data = dispatch
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get dispatch error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "Error retrieving dispatch" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<DispatchDto>>>> GetAllDispatches()
    {
        try
        {
            var dispatches = await _dispatchService.GetAllDispatchesAsync();
            return Ok(new ApiResponse<List<DispatchDto>>
            {
                Success = true,
                Message = "Dispatches retrieved",
                Data = dispatches
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get all dispatches error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "Error retrieving dispatches" });
        }
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult<ApiResponse<List<DispatchDto>>>> GetDispatchesByStatus(string status)
    {
        try
        {
            if (!Enum.TryParse<DispatchStatus>(status, true, out var dispatchStatus))
                return BadRequest(new ApiResponse { Success = false, Message = "Invalid status" });

            var dispatches = await _dispatchService.GetDispatchesByStatusAsync(dispatchStatus);
            return Ok(new ApiResponse<List<DispatchDto>>
            {
                Success = true,
                Message = "Dispatches retrieved",
                Data = dispatches
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get dispatches by status error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "Error retrieving dispatches" });
        }
    }

    [HttpPut("{id}/status/{status}")]
    [Authorize(Roles = "Dispatcher,Driver,EmsOperator,Admin")]
    public async Task<ActionResult<ApiResponse>> UpdateDispatchStatus(string id, string status)
    {
        try
        {
            if (!Enum.TryParse<DispatchStatus>(status, true, out var dispatchStatus))
                return BadRequest(new ApiResponse { Success = false, Message = "Invalid status" });

            await _dispatchService.UpdateDispatchStatusAsync(id, dispatchStatus);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Dispatch status updated successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ApiResponse { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Update dispatch status error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "Error updating dispatch" });
        }
    }

    [HttpPut("{id}/assign")]
    [Authorize(Roles = "Dispatcher,Admin")]
    public async Task<ActionResult<ApiResponse>> AssignDispatch(string id, [FromBody] AssignDispatchDto dto)
    {
        try
        {
            await _dispatchService.AssignDispatchAsync(id, dto.DriverId, dto.AmbulanceId);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Dispatch assigned successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ApiResponse { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Assign dispatch error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "Error assigning dispatch" });
        }
    }
}

public class AssignDispatchDto
{
    public string DriverId { get; set; } = string.Empty;
    public string AmbulanceId { get; set; } = string.Empty;
}
