using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmsDispatch.Backend.DTOs;
using EmsDispatch.Backend.Services;

namespace EmsDispatch.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SessionController : ControllerBase
{
    private readonly ISessionService _sessionService;
    private readonly ILogger<SessionController> _logger;

    public SessionController(ISessionService sessionService, ILogger<SessionController> logger)
    {
        _sessionService = sessionService;
        _logger = logger;
    }

    [HttpGet("active")]
    [Authorize(Roles = "Admin,Dispatcher")]
    public async Task<ActionResult<ApiResponse<int>>> GetActiveSessionCount()
    {
        try
        {
            var sessions = await _sessionService.GetActiveSessionsAsync();
            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Active session count retrieved",
                Data = sessions.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get active sessions error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "Error retrieving sessions" });
        }
    }

    [HttpGet("online-users")]
    [Authorize(Roles = "Admin,Dispatcher")]
    public async Task<ActionResult<ApiResponse<List<string>>>> GetOnlineUsers()
    {
        try
        {
            var onlineUsers = await _sessionService.GetOnlineUserIdsAsync();
            return Ok(new ApiResponse<List<string>>
            {
                Success = true,
                Message = "Online users retrieved",
                Data = onlineUsers
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get online users error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "Error retrieving online users" });
        }
    }
}
