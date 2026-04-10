using Microsoft.AspNetCore.Mvc;
using EmsDispatch.Backend.Models;
using EmsDispatch.Backend.Services;
using EmsDispatch.Backend.Utilities;

namespace EmsDispatch.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HospitalController : ControllerBase
    {
        private readonly MongoDbContext _dbContext;
        private readonly ILogger<HospitalController> _logger;

        public HospitalController(MongoDbContext dbContext, ILogger<HospitalController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHospitals()
        {
            try
            {
                var hospitals = await _dbContext.Hospitals.Find(_ => true).ToListAsync();
                return Ok(ApiResponse<List<Hospital>>.SuccessResponse(hospitals, "Hospitals retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving hospitals");
                return StatusCode(500, ApiResponse.ErrorResponse("Error retrieving hospitals"));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHospitalById(string id)
        {
            try
            {
                var hospital = await _dbContext.Hospitals.Find(h => h.Id == id).FirstOrDefaultAsync();
                if (hospital == null)
                {
                    return NotFound(ApiResponse.ErrorResponse("Hospital not found"));
                }

                return Ok(ApiResponse<Hospital>.SuccessResponse(hospital, "Hospital retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving hospital");
                return StatusCode(500, ApiResponse.ErrorResponse("Error retrieving hospital"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateHospital([FromBody] Hospital hospital)
        {
            try
            {
                if (hospital == null)
                {
                    return BadRequest(ApiResponse.ErrorResponse("Hospital data is required"));
                }

                hospital.Id = ObjectId.GenerateNewId().ToString();
                hospital.CreatedAt = DateTime.UtcNow;
                hospital.UpdatedAt = DateTime.UtcNow;

                await _dbContext.Hospitals.InsertOneAsync(hospital);
                return CreatedAtAction(nameof(GetHospitalById), new { id = hospital.Id }, 
                    ApiResponse<Hospital>.SuccessResponse(hospital, "Hospital created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating hospital");
                return StatusCode(500, ApiResponse.ErrorResponse("Error creating hospital"));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHospital(string id, [FromBody] Hospital hospitalUpdate)
        {
            try
            {
                var hospital = await _dbContext.Hospitals.Find(h => h.Id == id).FirstOrDefaultAsync();
                if (hospital == null)
                {
                    return NotFound(ApiResponse.ErrorResponse("Hospital not found"));
                }

                hospitalUpdate.Id = id;
                hospitalUpdate.UpdatedAt = DateTime.UtcNow;

                await _dbContext.Hospitals.ReplaceOneAsync(h => h.Id == id, hospitalUpdate);
                return Ok(ApiResponse<Hospital>.SuccessResponse(hospitalUpdate, "Hospital updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating hospital");
                return StatusCode(500, ApiResponse.ErrorResponse("Error updating hospital"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHospital(string id)
        {
            try
            {
                var result = await _dbContext.Hospitals.DeleteOneAsync(h => h.Id == id);
                if (result.DeletedCount == 0)
                {
                    return NotFound(ApiResponse.ErrorResponse("Hospital not found"));
                }

                return Ok(ApiResponse.SuccessResponse("Hospital deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting hospital");
                return StatusCode(500, ApiResponse.ErrorResponse("Error deleting hospital"));
            }
        }

        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearbyHospitals([FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] double radiusKm = 5)
        {
            try
            {
                if (!ValidationUtilities.IsValidCoordinates(latitude, longitude))
                {
                    return BadRequest(ApiResponse.ErrorResponse("Invalid coordinates"));
                }

                var allHospitals = await _dbContext.Hospitals.Find(_ => true).ToListAsync();
                var nearby = allHospitals
                    .Select(h => new
                    {
                        Hospital = h,
                        Distance = CalculateDistance(latitude, longitude, h.Location.Latitude, h.Location.Longitude)
                    })
                    .Where(x => x.Distance <= radiusKm)
                    .OrderBy(x => x.Distance)
                    .Select(x => x.Hospital)
                    .ToList();

                return Ok(ApiResponse<List<Hospital>>.SuccessResponse(nearby, "Nearby hospitals retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving nearby hospitals");
                return StatusCode(500, ApiResponse.ErrorResponse("Error retrieving hospitals"));
            }
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Earth's radius in kilometers
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
    }
}
