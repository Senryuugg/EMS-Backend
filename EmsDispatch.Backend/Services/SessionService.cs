using MongoDB.Driver;
using EmsDispatch.Backend.Models;

namespace EmsDispatch.Backend.Services;

public interface ISessionService
{
    Task<List<UserSession>> GetActiveSessionsAsync();
    Task<List<string>> GetOnlineUserIdsAsync();
    Task UpdateLastActivityAsync(string userId);
}

public class SessionService : ISessionService
{
    private readonly IMongoDbContext _context;
    private readonly ILogger<SessionService> _logger;

    public SessionService(IMongoDbContext context, ILogger<SessionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<UserSession>> GetActiveSessionsAsync()
    {
        try
        {
            var sessions = await _context.UserSessions
                .Find(s => s.IsOnline)
                .SortByDescending(s => s.LoginAt)
                .ToListAsync();

            return sessions;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting active sessions: {ex.Message}");
            throw;
        }
    }

    public async Task<List<string>> GetOnlineUserIdsAsync()
    {
        try
        {
            var sessions = await GetActiveSessionsAsync();
            return sessions.Select(s => s.UserId).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting online users: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateLastActivityAsync(string userId)
    {
        try
        {
            var session = await _context.UserSessions
                .Find(s => s.UserId == userId && s.IsOnline)
                .FirstOrDefaultAsync();

            if (session != null)
            {
                session.LastActivity = DateTime.UtcNow;
                await _context.UserSessions.ReplaceOneAsync(s => s.Id == session.Id, session);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating last activity: {ex.Message}");
        }
    }
}
