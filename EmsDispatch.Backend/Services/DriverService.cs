using MongoDB.Driver;
using EmsDispatch.Backend.Models;
using EmsDispatch.Backend.Models.Enums;
using EmsDispatch.Backend.DTOs;

namespace EmsDispatch.Backend.Services;

public interface IDriverService
{
    Task<string> CreateDriverAsync(string userId, string licenseNumber);
    Task<DriverDto?> GetDriverByIdAsync(string id);
    Task<DriverDto?> GetDriverByUserIdAsync(string userId);
    Task<List<DriverDto>> GetAvailableDriversAsync();
    Task UpdateDriverLocationAsync(string driverId, LocationDto location);
    Task UpdateDriverStatusAsync(string driverId, DriverStatus status);
    Task<List<DriverDto>> GetAllDriversAsync();
}

public class DriverService : IDriverService
{
    private readonly IMongoDbContext _context;
    private readonly ILogger<DriverService> _logger;

    public DriverService(IMongoDbContext context, ILogger<DriverService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string> CreateDriverAsync(string userId, string licenseNumber)
    {
        try
        {
            var driver = new Driver
            {
                UserId = userId,
                LicenseNumber = licenseNumber,
                Status = DriverStatus.Available,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _context.Drivers.InsertOneAsync(driver);
            _logger.LogInformation($"Driver created for user {userId}");

            return driver.Id!;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating driver: {ex.Message}");
            throw;
        }
    }

    public async Task<DriverDto?> GetDriverByIdAsync(string id)
    {
        try
        {
            var driver = await _context.Drivers.Find(d => d.Id == id).FirstOrDefaultAsync();
            return driver != null ? MapToDto(driver) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting driver: {ex.Message}");
            throw;
        }
    }

    public async Task<DriverDto?> GetDriverByUserIdAsync(string userId)
    {
        try
        {
            var driver = await _context.Drivers.Find(d => d.UserId == userId).FirstOrDefaultAsync();
            return driver != null ? MapToDto(driver) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting driver by user ID: {ex.Message}");
            throw;
        }
    }

    public async Task<List<DriverDto>> GetAvailableDriversAsync()
    {
        try
        {
            var drivers = await _context.Drivers
                .Find(d => d.Status == DriverStatus.Available && d.IsActive)
                .ToListAsync();

            return drivers.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting available drivers: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateDriverLocationAsync(string driverId, LocationDto location)
    {
        try
        {
            var driver = await _context.Drivers.Find(d => d.Id == driverId).FirstOrDefaultAsync();
            if (driver == null)
                throw new InvalidOperationException("Driver not found");

            driver.CurrentLocation = new Location
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Address = location.Address,
                Timestamp = DateTime.UtcNow
            };
            driver.UpdatedAt = DateTime.UtcNow;

            await _context.Drivers.ReplaceOneAsync(d => d.Id == driverId, driver);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating driver location: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateDriverStatusAsync(string driverId, DriverStatus status)
    {
        try
        {
            var driver = await _context.Drivers.Find(d => d.Id == driverId).FirstOrDefaultAsync();
            if (driver == null)
                throw new InvalidOperationException("Driver not found");

            driver.Status = status;
            driver.UpdatedAt = DateTime.UtcNow;

            await _context.Drivers.ReplaceOneAsync(d => d.Id == driverId, driver);
            _logger.LogInformation($"Driver {driverId} status updated to {status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating driver status: {ex.Message}");
            throw;
        }
    }

    public async Task<List<DriverDto>> GetAllDriversAsync()
    {
        try
        {
            var drivers = await _context.Drivers.Find(_ => true).ToListAsync();
            return drivers.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting all drivers: {ex.Message}");
            throw;
        }
    }

    private DriverDto MapToDto(Driver driver)
    {
        return new DriverDto
        {
            Id = driver.Id!,
            UserId = driver.UserId,
            LicenseNumber = driver.LicenseNumber,
            AmbulanceId = driver.AmbulanceId,
            CurrentLocation = driver.CurrentLocation != null ? new LocationDto
            {
                Latitude = driver.CurrentLocation.Latitude,
                Longitude = driver.CurrentLocation.Longitude,
                Address = driver.CurrentLocation.Address
            } : null,
            Status = driver.Status.ToString(),
            ExperienceYears = driver.ExperienceYears,
            ActiveDispatchId = driver.ActiveDispatchId
        };
    }
}
