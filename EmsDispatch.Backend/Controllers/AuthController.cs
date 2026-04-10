using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmsDispatch.Backend.DTOs;
using EmsDispatch.Backend.Services;
using EmsDispatch.Backend.Models.Enums;
using System.Security.Claims;

namespace EmsDispatch.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new ApiResponse { Success = false, Message = "Email and password are required" });

            var result = await _authService.LoginAsync(request);
            return Ok(new ApiResponse<LoginResponseDto>
            {
                Success = true,
                Message = "Login successful",
                Data = result
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Login error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "An error occurred during login" });
        }
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<UserDto>>> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new ApiResponse { Success = false, Message = "Email and password are required" });

            if (request.Password.Length < 6)
                return BadRequest(new ApiResponse { Success = false, Message = "Password must be at least 6 characters" });

            var result = await _authService.RegisterAsync(request, UserRole.Driver);
            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Message = "Registration successful",
                Data = result
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Registration error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "An error occurred during registration" });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Logout()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await _authService.LogoutAsync(userId);
            return Ok(new ApiResponse { Success = true, Message = "Logout successful" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Logout error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "An error occurred during logout" });
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetCurrentUser()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound(new ApiResponse { Success = false, Message = "User not found" });

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Message = "User retrieved",
                Data = user
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get current user error: {ex.Message}");
            return StatusCode(500, new ApiResponse { Success = false, Message = "An error occurred" });
        }
    }
}
