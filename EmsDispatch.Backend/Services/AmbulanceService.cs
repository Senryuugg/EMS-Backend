using MongoDB.Driver;
using EmsDispatch.Backend.Models;
using EmsDispatch.Backend.Models.Enums;
using EmsDispatch.Backend.DTOs;

namespace EmsDispatch.Backend.Services;

public interface IAmbulanceService
{
    Task<string> CreateAmbulanceAsync(string registrationNumber, int capacity);
    Task<AmbulanceDto?> GetAmbulanceByIdAsync(string id);
    Task<List<AmbulanceDto>> GetAvailableAmbulancesAsync();
    Task UpdateAmbulanceLocationAsync(string ambulanceId, LocationDto location);
    Task UpdateAmbulanceStatusAsync(string ambulanceId, DriverStatus status);
    Task AssignAmbulanceToDriverAsync(string ambulanceId, string driverId);
    Task<List<AmbulanceDto>> GetAllAmbulancesAsync();
}

public class AmbulanceService : IAmbulanceService
{
    private readonly IMongoDbContext _context;
    private readonly ILogger<AmbulanceService> _logger;

    public AmbulanceService(IMongoDbContext context, ILogger<AmbulanceService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string> CreateAmbulanceAsync(string registrationNumber, int capacity)
    {
        try
        {
            var ambulance = new Ambulance
            {
                RegistrationNumber = registrationNumber,
                Status = DriverStatus.Available,
                Capacity = capacity,
                Equipment = new List<string> { "Stretcher", "Oxygen", "First Aid Kit" },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _context.Ambulances.InsertOneAsync(ambulance);
            _logger.LogInformation($"Ambulance {registrationNumber} created");

            return ambulance.Id!;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating ambulance: {ex.Message}");
            throw;
        }
    }

    public async Task<AmbulanceDto?> GetAmbulanceByIdAsync(string id)
    {
        try
        {
            var ambulance = await _context.Ambulances.Find(a => a.Id == id).FirstOrDefaultAsync();
            return ambulance != null ? MapToDto(ambulance) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting ambulance: {ex.Message}");
            throw;
        }
    }

    public async Task<List<AmbulanceDto>> GetAvailableAmbulancesAsync()
    {
        try
        {
            var ambulances = await _context.Ambulances
                .Find(a => a.Status == DriverStatus.Available && a.IsActive)
                .ToListAsync();

            return ambulances.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting available ambulances: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateAmbulanceLocationAsync(string ambulanceId, LocationDto location)
    {
        try
        {
            var ambulance = await _context.Ambulances.Find(a => a.Id == ambulanceId).FirstOrDefaultAsync();
            if (ambulance == null)
                throw new InvalidOperationException("Ambulance not found");

            ambulance.CurrentLocation = new Location
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Address = location.Address,
                Timestamp = DateTime.UtcNow
            };
            ambulance.UpdatedAt = DateTime.UtcNow;

            await _context.Ambulances.ReplaceOneAsync(a => a.Id == ambulanceId, ambulance);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating ambulance location: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateAmbulanceStatusAsync(string ambulanceId, DriverStatus status)
    {
        try
        {
            var ambulance = await _context.Ambulances.Find(a => a.Id == ambulanceId).FirstOrDefaultAsync();
            if (ambulance == null)
                throw new InvalidOperationException("Ambulance not found");

            ambulance.Status = status;
            ambulance.UpdatedAt = DateTime.UtcNow;

            await _context.Ambulances.ReplaceOneAsync(a => a.Id == ambulanceId, ambulance);
            _logger.LogInformation($"Ambulance {ambulanceId} status updated to {status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating ambulance status: {ex.Message}");
            throw;
        }
    }

    public async Task AssignAmbulanceToDriverAsync(string ambulanceId, string driverId)
    {
        try
        {
            var ambulance = await _context.Ambulances.Find(a => a.Id == ambulanceId).FirstOrDefaultAsync();
            if (ambulance == null)
                throw new InvalidOperationException("Ambulance not found");

            ambulance.DriverId = driverId;
            ambulance.UpdatedAt = DateTime.UtcNow;

            await _context.Ambulances.ReplaceOneAsync(a => a.Id == ambulanceId, ambulance);

            var driver = await _context.Drivers.Find(d => d.Id == driverId).FirstOrDefaultAsync();
            if (driver != null)
            {
                driver.AmbulanceId = ambulanceId;
                await _context.Drivers.ReplaceOneAsync(d => d.Id == driverId, driver);
            }

            _logger.LogInformation($"Ambulance {ambulanceId} assigned to driver {driverId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error assigning ambulance: {ex.Message}");
            throw;
        }
    }

    public async Task<List<AmbulanceDto>> GetAllAmbulancesAsync()
    {
        try
        {
            var ambulances = await _context.Ambulances.Find(_ => true).ToListAsync();
            return ambulances.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting all ambulances: {ex.Message}");
            throw;
        }
    }

    private AmbulanceDto MapToDto(Ambulance ambulance)
    {
        return new AmbulanceDto
        {
            Id = ambulance.Id!,
            RegistrationNumber = ambulance.RegistrationNumber,
            DriverId = ambulance.DriverId,
            CurrentLocation = ambulance.CurrentLocation != null ? new LocationDto
            {
                Latitude = ambulance.CurrentLocation.Latitude,
                Longitude = ambulance.CurrentLocation.Longitude,
                Address = ambulance.CurrentLocation.Address
            } : null,
            Status = ambulance.Status.ToString(),
            Capacity = ambulance.Capacity,
            Equipment = ambulance.Equipment
        };
    }
}
