using MongoDB.Bson;
using MongoDB.Driver;
using EmsDispatch.Backend.Models;
using EmsDispatch.Backend.Models.Enums;
using EmsDispatch.Backend.DTOs;

namespace EmsDispatch.Backend.Services;

public interface IDispatchService
{
    Task<string> CreateDispatchAsync(DispatchDto dto, string createdById);
    Task<DispatchDto?> GetDispatchByIdAsync(string id);
    Task<List<DispatchDto>> GetDispatchesByStatusAsync(DispatchStatus status);
    Task<List<DispatchDto>> GetAllDispatchesAsync();
    Task UpdateDispatchStatusAsync(string dispatchId, DispatchStatus newStatus);
    Task AssignDispatchAsync(string dispatchId, string driverId, string ambulanceId);
    Task UpdateDispatchAsync(string id, DispatchDto dto);
}

public class DispatchService : IDispatchService
{
    private readonly IMongoDbContext _context;
    private readonly ILogger<DispatchService> _logger;

    public DispatchService(IMongoDbContext context, ILogger<DispatchService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string> CreateDispatchAsync(DispatchDto dto, string createdById)
    {
        try
        {
            var dispatch = new Dispatch
            {
                CallId = GenerateCallId(),
                PatientInfo = new PatientInfo
                {
                    Name = dto.PatientInfo.Name,
                    Age = dto.PatientInfo.Age,
                    Gender = dto.PatientInfo.Gender,
                    MedicalCondition = dto.PatientInfo.MedicalCondition,
                    BloodType = dto.PatientInfo.BloodType,
                    Allergies = dto.PatientInfo.Allergies
                },
                Priority = Enum.Parse<Priority>(dto.Priority),
                Status = DispatchStatus.Pending,
                PickupLocation = new Location
                {
                    Latitude = dto.PickupLocation.Latitude,
                    Longitude = dto.PickupLocation.Longitude,
                    Address = dto.PickupLocation.Address
                },
                CreatedById = createdById,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Dispatches.InsertOneAsync(dispatch);
            _logger.LogInformation($"Dispatch {dispatch.CallId} created successfully");

            return dispatch.Id!;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating dispatch: {ex.Message}");
            throw;
        }
    }

    public async Task<DispatchDto?> GetDispatchByIdAsync(string id)
    {
        try
        {
            if (!ObjectId.TryParse(id, out _))
                return null;

            var dispatch = await _context.Dispatches.Find(d => d.Id == id).FirstOrDefaultAsync();
            return dispatch != null ? MapToDto(dispatch) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting dispatch: {ex.Message}");
            throw;
        }
    }

    public async Task<List<DispatchDto>> GetDispatchesByStatusAsync(DispatchStatus status)
    {
        try
        {
            var dispatches = await _context.Dispatches
                .Find(d => d.Status == status)
                .SortByDescending(d => d.CreatedAt)
                .ToListAsync();

            return dispatches.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting dispatches by status: {ex.Message}");
            throw;
        }
    }

    public async Task<List<DispatchDto>> GetAllDispatchesAsync()
    {
        try
        {
            var dispatches = await _context.Dispatches
                .Find(_ => true)
                .SortByDescending(d => d.CreatedAt)
                .ToListAsync();

            return dispatches.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting all dispatches: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateDispatchStatusAsync(string dispatchId, DispatchStatus newStatus)
    {
        try
        {
            var dispatch = await _context.Dispatches.Find(d => d.Id == dispatchId).FirstOrDefaultAsync();
            if (dispatch == null)
                throw new InvalidOperationException("Dispatch not found");

            dispatch.Status = newStatus;
            dispatch.UpdatedAt = DateTime.UtcNow;

            if (newStatus == DispatchStatus.Complete)
                dispatch.CompletedAt = DateTime.UtcNow;

            await _context.Dispatches.ReplaceOneAsync(d => d.Id == dispatchId, dispatch);
            _logger.LogInformation($"Dispatch {dispatchId} status updated to {newStatus}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating dispatch status: {ex.Message}");
            throw;
        }
    }

    public async Task AssignDispatchAsync(string dispatchId, string driverId, string ambulanceId)
    {
        try
        {
            var dispatch = await _context.Dispatches.Find(d => d.Id == dispatchId).FirstOrDefaultAsync();
            if (dispatch == null)
                throw new InvalidOperationException("Dispatch not found");

            dispatch.AssignedDriverId = driverId;
            dispatch.AssignedAmbulanceId = ambulanceId;
            dispatch.Status = DispatchStatus.Assigned;
            dispatch.UpdatedAt = DateTime.UtcNow;

            await _context.Dispatches.ReplaceOneAsync(d => d.Id == dispatchId, dispatch);

            // Update driver
            var driver = await _context.Drivers.Find(d => d.Id == driverId).FirstOrDefaultAsync();
            if (driver != null)
            {
                driver.ActiveDispatchId = dispatchId;
                driver.Status = DriverStatus.Busy;
                await _context.Drivers.ReplaceOneAsync(d => d.Id == driverId, driver);
            }

            _logger.LogInformation($"Dispatch {dispatchId} assigned to driver {driverId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error assigning dispatch: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateDispatchAsync(string id, DispatchDto dto)
    {
        try
        {
            var dispatch = await _context.Dispatches.Find(d => d.Id == id).FirstOrDefaultAsync();
            if (dispatch == null)
                throw new InvalidOperationException("Dispatch not found");

            dispatch.PatientInfo.Name = dto.PatientInfo.Name;
            dispatch.PatientInfo.Age = dto.PatientInfo.Age;
            dispatch.Priority = Enum.Parse<Priority>(dto.Priority);
            dispatch.UpdatedAt = DateTime.UtcNow;

            await _context.Dispatches.ReplaceOneAsync(d => d.Id == id, dispatch);
            _logger.LogInformation($"Dispatch {id} updated");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating dispatch: {ex.Message}");
            throw;
        }
    }

    private DispatchDto MapToDto(Dispatch dispatch)
    {
        return new DispatchDto
        {
            Id = dispatch.Id!,
            CallId = dispatch.CallId,
            PatientInfo = new PatientInfoDto
            {
                Name = dispatch.PatientInfo.Name,
                Age = dispatch.PatientInfo.Age,
                Gender = dispatch.PatientInfo.Gender,
                MedicalCondition = dispatch.PatientInfo.MedicalCondition,
                BloodType = dispatch.PatientInfo.BloodType,
                Allergies = dispatch.PatientInfo.Allergies
            },
            Priority = dispatch.Priority.ToString(),
            Status = dispatch.Status.ToString(),
            PickupLocation = new LocationDto
            {
                Latitude = dispatch.PickupLocation.Latitude,
                Longitude = dispatch.PickupLocation.Longitude,
                Address = dispatch.PickupLocation.Address
            },
            AssignedDriverId = dispatch.AssignedDriverId,
            AssignedAmbulanceId = dispatch.AssignedAmbulanceId,
            CreatedAt = dispatch.CreatedAt,
            UpdatedAt = dispatch.UpdatedAt
        };
    }

    private string GenerateCallId()
    {
        return $"CALL-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
    }
}
