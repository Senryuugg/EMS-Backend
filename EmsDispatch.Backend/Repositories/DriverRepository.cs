using MongoDB.Driver;
using EmsDispatch.Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmsDispatch.Backend.Repositories
{
    public interface IDriverRepository
    {
        Task<Driver?> GetByIdAsync(string id);
        Task<Driver?> GetByUserIdAsync(string userId);
        Task<List<Driver>> GetAvailableDriversAsync();
        Task<List<Driver>> GetAllAsync();
        Task<Driver> CreateAsync(Driver driver);
        Task<Driver> UpdateAsync(Driver driver);
        Task DeleteAsync(string id);
        Task<List<Driver>> GetByStatusAsync(string status);
    }

    public class DriverRepository : IDriverRepository
    {
        private readonly IMongoCollection<Driver> _driverCollection;

        public DriverRepository(IMongoCollection<Driver> driverCollection)
        {
            _driverCollection = driverCollection;
        }

        public async Task<Driver?> GetByIdAsync(string id)
        {
            return await _driverCollection.Find(d => d.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Driver?> GetByUserIdAsync(string userId)
        {
            return await _driverCollection.Find(d => d.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<List<Driver>> GetAvailableDriversAsync()
        {
            return await _driverCollection.Find(d => d.Status == "Available").ToListAsync();
        }

        public async Task<List<Driver>> GetAllAsync()
        {
            return await _driverCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Driver> CreateAsync(Driver driver)
        {
            await _driverCollection.InsertOneAsync(driver);
            return driver;
        }

        public async Task<Driver> UpdateAsync(Driver driver)
        {
            await _driverCollection.ReplaceOneAsync(d => d.Id == driver.Id, driver);
            return driver;
        }

        public async Task DeleteAsync(string id)
        {
            await _driverCollection.DeleteOneAsync(d => d.Id == id);
        }

        public async Task<List<Driver>> GetByStatusAsync(string status)
        {
            return await _driverCollection.Find(d => d.Status == status).ToListAsync();
        }
    }
}
