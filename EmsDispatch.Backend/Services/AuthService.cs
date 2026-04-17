using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using BCrypt.Net;
using EmsDispatch.Backend.Models;
using EmsDispatch.Backend.Models.Enums;
using EmsDispatch.Backend.DTOs;

namespace EmsDispatch.Backend.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<UserDto> RegisterAsync(RegisterRequestDto request, UserRole role = UserRole.Driver);
    Task<LoginResponseDto> RefreshTokenAsync(string refreshToken); // Not yet implemented — requires DB-backed token storage
    Task LogoutAsync(string userId);
    Task<UserDto?> GetUserByIdAsync(string userId);
    Task<UserDto?> GetUserByEmailAsync(string email);
}

public class AuthService : IAuthService
{
    private readonly IMongoDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private readonly string _jwtSecretKey;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly int _jwtExpirationMinutes;

    public AuthService(IMongoDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
        _jwtSecretKey = configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT secret key not configured");
        _jwtIssuer = configuration["Jwt:Issuer"] ?? "ems-dispatch-api";
        _jwtAudience = configuration["Jwt:Audience"] ?? "ems-dispatch-mobile-app";
        _jwtExpirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"] ?? "60");
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        try
        {
            var user = await _context.Users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning($"Login failed for email: {request.Email}");
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("User account is inactive");
            }

            // Update last login
            user.LastLogin = DateTime.UtcNow;
            await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user);

            // Create session
            var session = new UserSession
            {
                UserId = user.Id!,
                LoginAt = DateTime.UtcNow,
                LastActivity = DateTime.UtcNow,
                IsOnline = true
            };
            await _context.UserSessions.InsertOneAsync(session);

            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            _logger.LogInformation($"User {user.Email} logged in successfully");

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = MapToUserDto(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Login error: {ex.Message}");
            throw;
        }
    }

    public async Task<UserDto> RegisterAsync(RegisterRequestDto request, UserRole role = UserRole.Driver)
    {
        try
        {
            var existingUser = await _context.Users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email already registered");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                FullName = request.FullName,
                Phone = request.Phone,
                Role = role,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _context.Users.InsertOneAsync(user);

            _logger.LogInformation($"User {user.Email} registered successfully with role {role}");

            return MapToUserDto(user);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Registration error: {ex.Message}");
            throw;
        }
    }

    public Task<LoginResponseDto> RefreshTokenAsync(string refreshToken)
    {
        // Refresh token storage in DB is required for a full implementation.
        throw new NotImplementedException("Refresh token logic needs database storage");
    }

    public async Task LogoutAsync(string userId)
    {
        try
        {
            var session = await _context.UserSessions
                .Find(s => s.UserId == userId && s.IsOnline)
                .FirstOrDefaultAsync();

            if (session != null)
            {
                session.LogoutAt = DateTime.UtcNow;
                session.IsOnline = false;
                await _context.UserSessions.ReplaceOneAsync(s => s.Id == session.Id, session);
            }

            _logger.LogInformation($"User {userId} logged out");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Logout error: {ex.Message}");
        }
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        var user = await _context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        return user != null ? MapToUserDto(user) : null;
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
        return user != null ? MapToUserDto(user) : null;
    }

    private string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSecretKey);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id!),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim("role", user.Role.ToString()),
            new Claim("status", user.Status.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
            Issuer = _jwtIssuer,
            Audience = _jwtAudience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    private UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id!,
            Email = user.Email,
            FullName = user.FullName,
            Phone = user.Phone,
            Role = user.Role.ToString(),
            Status = user.Status.ToString()
        };
    }
}
