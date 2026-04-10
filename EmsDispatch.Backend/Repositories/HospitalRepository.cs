using MongoDB.Driver;
using EmsDispatch.Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmsDispatch.Backend.Repositories
{
    public interface IHospitalRepository
    {
        Task<Hospital?> GetByIdAsync(string id);
        Task<List<Hospital>> GetAllAsync();
        Task<List<Hospital>> GetBySpecialtyAsync(string specialty);
        Task<Hospital> CreateAsync(Hospital hospital);
        Task<Hospital> UpdateAsync(Hospital hospital);
        Task DeleteAsync(string id);
        Task<List<Hospital>> GetNearestHospitalsAsync(double latitude, double longitude, double radiusKm = 5);
    }

    public class HospitalRepository : IHospitalRepository
    {
        private readonly IMongoCollection<Hospital> _hospitalCollection;

        public HospitalRepository(IMongoCollection<Hospital> hospitalCollection)
        {
            _hospitalCollection = hospitalCollection;
        }

        public async Task<Hospital?> GetByIdAsync(string id)
        {
            return await _hospitalCollection.Find(h => h.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Hospital>> GetAllAsync()
        {
            return await _hospitalCollection.Find(_ => true).ToListAsync();
        }

        public async Task<List<Hospital>> GetBySpecialtyAsync(string specialty)
        {
            return await _hospitalCollection.Find(h => h.Specialties.Contains(specialty)).ToListAsync();
        }

        public async Task<Hospital> CreateAsync(Hospital hospital)
        {
            await _hospitalCollection.InsertOneAsync(hospital);
            return hospital;
        }

        public async Task<Hospital> UpdateAsync(Hospital hospital)
        {
            await _hospitalCollection.ReplaceOneAsync(h => h.Id == hospital.Id, hospital);
            return hospital;
        }

        public async Task DeleteAsync(string id)
        {
            await _hospitalCollection.DeleteOneAsync(h => h.Id == id);
        }

        public async Task<List<Hospital>> GetNearestHospitalsAsync(double latitude, double longitude, double radiusKm = 5)
        {
            // Simple distance calculation (Haversine formula)
            // MongoDB geospatial queries could be used for better performance in production
            var allHospitals = await GetAllAsync();
            var nearby = new List<Hospital>();

            foreach (var hospital in allHospitals)
            {
                var distance = CalculateDistance(latitude, longitude, hospital.Location.Latitude, hospital.Location.Longitude);
                if (distance <= radiusKm)
                {
                    nearby.Add(hospital);
                }
            }

            return nearby.OrderBy(h => CalculateDistance(latitude, longitude, h.Location.Latitude, h.Location.Longitude)).ToList();
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
