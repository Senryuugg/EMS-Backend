using MongoDB.Driver;
using EmsDispatch.Backend.Models;
using EmsDispatch.Backend.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmsDispatch.Backend.Repositories
{
    public interface IAmbulanceRepository
    {
        Task<Ambulance?> GetByIdAsync(string id);
        Task<List<Ambulance>> GetAllAsync();
        Task<List<Ambulance>> GetAvailableAsync();
        Task<Ambulance?> GetByDriverIdAsync(string driverId);
        Task<Ambulance> CreateAsync(Ambulance ambulance);
        Task<Ambulance> UpdateAsync(Ambulance ambulance);
        Task DeleteAsync(string id);
        Task<List<Ambulance>> GetByStatusAsync(DriverStatus status);
    }

    public class AmbulanceRepository : IAmbulanceRepository
    {
        private readonly IMongoCollection<Ambulance> _ambulanceCollection;

        public AmbulanceRepository(IMongoCollection<Ambulance> ambulanceCollection)
        {
            _ambulanceCollection = ambulanceCollection;
        }

        public async Task<Ambulance?> GetByIdAsync(string id)
        {
            return await _ambulanceCollection.Find(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Ambulance>> GetAllAsync()
        {
            return await _ambulanceCollection.Find(_ => true).ToListAsync();
        }

        public async Task<List<Ambulance>> GetAvailableAsync()
        {
            return await _ambulanceCollection.Find(a => a.Status == DriverStatus.Available).ToListAsync();
        }

        public async Task<Ambulance?> GetByDriverIdAsync(string driverId)
        {
            return await _ambulanceCollection.Find(a => a.DriverId == driverId).FirstOrDefaultAsync();
        }

        public async Task<Ambulance> CreateAsync(Ambulance ambulance)
        {
            await _ambulanceCollection.InsertOneAsync(ambulance);
            return ambulance;
        }

        public async Task<Ambulance> UpdateAsync(Ambulance ambulance)
        {
            await _ambulanceCollection.ReplaceOneAsync(a => a.Id == ambulance.Id, ambulance);
            return ambulance;
        }

        public async Task DeleteAsync(string id)
        {
            await _ambulanceCollection.DeleteOneAsync(a => a.Id == id);
        }

        public async Task<List<Ambulance>> GetByStatusAsync(DriverStatus status)
        {
            return await _ambulanceCollection.Find(a => a.Status == status).ToListAsync();
        }
    }
}
