using MongoDB.Driver;
using EmsDispatch.Backend.Models;
using EmsDispatch.Backend.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmsDispatch.Backend.Repositories
{
    public interface IDispatchRepository
    {
        Task<Dispatch?> GetByIdAsync(string id);
        Task<List<Dispatch>> GetAllAsync();
        Task<List<Dispatch>> GetByStatusAsync(DispatchStatus status);
        Task<List<Dispatch>> GetByAssignedDriverAsync(string driverId);
        Task<Dispatch> CreateAsync(Dispatch dispatch);
        Task<Dispatch> UpdateAsync(Dispatch dispatch);
        Task DeleteAsync(string id);
        Task<List<Dispatch>> GetRecentDispatchesAsync(int days = 7);
    }

    public class DispatchRepository : IDispatchRepository
    {
        private readonly IMongoCollection<Dispatch> _dispatchCollection;

        public DispatchRepository(IMongoCollection<Dispatch> dispatchCollection)
        {
            _dispatchCollection = dispatchCollection;
        }

        public async Task<Dispatch?> GetByIdAsync(string id)
        {
            return await _dispatchCollection.Find(d => d.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Dispatch>> GetAllAsync()
        {
            return await _dispatchCollection.Find(_ => true).ToListAsync();
        }

        public async Task<List<Dispatch>> GetByStatusAsync(DispatchStatus status)
        {
            return await _dispatchCollection.Find(d => d.Status == status).ToListAsync();
        }

        public async Task<List<Dispatch>> GetByAssignedDriverAsync(string driverId)
        {
            return await _dispatchCollection.Find(d => d.AssignedDriverId == driverId).ToListAsync();
        }

        public async Task<Dispatch> CreateAsync(Dispatch dispatch)
        {
            await _dispatchCollection.InsertOneAsync(dispatch);
            return dispatch;
        }

        public async Task<Dispatch> UpdateAsync(Dispatch dispatch)
        {
            await _dispatchCollection.ReplaceOneAsync(d => d.Id == dispatch.Id, dispatch);
            return dispatch;
        }

        public async Task DeleteAsync(string id)
        {
            await _dispatchCollection.DeleteOneAsync(d => d.Id == id);
        }

        public async Task<List<Dispatch>> GetRecentDispatchesAsync(int days = 7)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            return await _dispatchCollection.Find(d => d.CreatedAt >= startDate).ToListAsync();
        }
    }
}
