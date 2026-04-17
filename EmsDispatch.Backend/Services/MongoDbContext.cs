using MongoDB.Driver;
using EmsDispatch.Backend.Configuration;
using EmsDispatch.Backend.Models;

namespace EmsDispatch.Backend.Services;

public interface IMongoDbContext
{
    IMongoDatabase Database { get; }
    IMongoCollection<User> Users { get; }
    IMongoCollection<Dispatch> Dispatches { get; }
    IMongoCollection<Driver> Drivers { get; }
    IMongoCollection<Ambulance> Ambulances { get; }
    IMongoCollection<Hospital> Hospitals { get; }
    IMongoCollection<UserSession> UserSessions { get; }
}

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    public IMongoDatabase Database => _database;
    public IMongoCollection<User> Users { get; }
    public IMongoCollection<Dispatch> Dispatches { get; }
    public IMongoCollection<Driver> Drivers { get; }
    public IMongoCollection<Ambulance> Ambulances { get; }
    public IMongoCollection<Hospital> Hospitals { get; }
    public IMongoCollection<UserSession> UserSessions { get; }

    public MongoDbContext(MongoSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        _database = client.GetDatabase(settings.DatabaseName);

        Users = _database.GetCollection<User>("users");
        Dispatches = _database.GetCollection<Dispatch>("dispatches");
        Drivers = _database.GetCollection<Driver>("drivers");
        Ambulances = _database.GetCollection<Ambulance>("ambulances");
        Hospitals = _database.GetCollection<Hospital>("hospitals");
        UserSessions = _database.GetCollection<UserSession>("user_sessions");

        CreateIndexes();
    }

    private void CreateIndexes()
    {
        // Users indexes
        var userEmailIndex = new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(u => u.Email),
            new CreateIndexOptions { Unique = true }
        );
        Users.Indexes.CreateOneAsync(userEmailIndex).Wait();

        // Dispatches indexes
        var dispatchStatusIndex = new CreateIndexModel<Dispatch>(
            Builders<Dispatch>.IndexKeys.Ascending(d => d.Status)
        );
        Dispatches.Indexes.CreateOneAsync(dispatchStatusIndex).Wait();

        var dispatchCreatedIndex = new CreateIndexModel<Dispatch>(
            Builders<Dispatch>.IndexKeys.Descending(d => d.CreatedAt)
        );
        Dispatches.Indexes.CreateOneAsync(dispatchCreatedIndex).Wait();

        // Driver indexes
        var driverUserIdIndex = new CreateIndexModel<Driver>(
            Builders<Driver>.IndexKeys.Ascending(d => d.UserId),
            new CreateIndexOptions { Unique = true }
        );
        Drivers.Indexes.CreateOneAsync(driverUserIdIndex).Wait();

        // UserSession indexes
        var sessionUserIndex = new CreateIndexModel<UserSession>(
            Builders<UserSession>.IndexKeys.Descending(s => s.LoginAt)
        );
        UserSessions.Indexes.CreateOneAsync(sessionUserIndex).Wait();
    }
}
