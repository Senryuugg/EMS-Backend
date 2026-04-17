using MongoDB.Driver;
using EmsDispatch.Backend.Models;
using EmsDispatch.Backend.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmsDispatch.Backend.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByEmailAsync(string email);
        Task<List<User>> GetByRoleAsync(UserRole role);
        Task<List<User>> GetAllAsync();
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(string id);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _userCollection;

        public UserRepository(IMongoCollection<User> userCollection)
        {
            _userCollection = userCollection;
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await _userCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetByRoleAsync(UserRole role)
        {
            return await _userCollection.Find(u => u.Role == role).ToListAsync();
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _userCollection.Find(_ => true).ToListAsync();
        }

        public async Task<User> CreateAsync(User user)
        {
            await _userCollection.InsertOneAsync(user);
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            await _userCollection.ReplaceOneAsync(u => u.Id == user.Id, user);
            return user;
        }

        public async Task DeleteAsync(string id)
        {
            await _userCollection.DeleteOneAsync(u => u.Id == id);
        }
    }
}
