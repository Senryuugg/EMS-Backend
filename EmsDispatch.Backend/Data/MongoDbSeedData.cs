using MongoDB.Driver;
using MongoDB.Bson;
using EmsDispatch.Backend.Models;
using EmsDispatch.Backend.Models.Enums;

namespace EmsDispatch.Backend.Data
{
    public static class MongoDbSeedData
    {
        public static async Task SeedDefaultDataAsync(IMongoDatabase database)
        {
            await SeedHospitalsAsync(database);
            await SeedUsersAsync(database);
            await SeedAmbulancesAsync(database);
        }

        private static async Task SeedHospitalsAsync(IMongoDatabase database)
        {
            var hospitalsCollection = database.GetCollection<Hospital>("hospitals");

            // Check if hospitals already exist
            var count = await hospitalsCollection.CountDocumentsAsync(_ => true);
            if (count > 0)
                return;

            var hospitals = new List<Hospital>
            {
                new Hospital
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = "Central Medical Center",
                    Location = new Location { Latitude = 14.5995, Longitude = 120.9842 },
                    Phone = "555-0001",
                    Specialties = new List<string> { "Emergency", "Trauma", "Cardiology" },
                    Capacity = 500,
                    CurrentLoad = 150,
                    Rating = 4.8,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Hospital
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = "St. Johns Hospital",
                    Location = new Location { Latitude = 14.5994, Longitude = 120.9855 },
                    Phone = "555-0002",
                    Specialties = new List<string> { "Emergency", "Pediatrics", "Neurology" },
                    Capacity = 350,
                    CurrentLoad = 120,
                    Rating = 4.6,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Hospital
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = "Metro Medical Complex",
                    Location = new Location { Latitude = 14.6010, Longitude = 120.9830 },
                    Phone = "555-0003",
                    Specialties = new List<string> { "Emergency", "Orthopedics", "General Surgery" },
                    Capacity = 400,
                    CurrentLoad = 180,
                    Rating = 4.5,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Hospital
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = "City Emergency Hospital",
                    Location = new Location { Latitude = 14.5980, Longitude = 120.9820 },
                    Phone = "555-0004",
                    Specialties = new List<string> { "Emergency", "Burn Unit", "Respiratory" },
                    Capacity = 300,
                    CurrentLoad = 95,
                    Rating = 4.4,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await hospitalsCollection.InsertManyAsync(hospitals);
        }

        private static async Task SeedUsersAsync(IMongoDatabase database)
        {
            var usersCollection = database.GetCollection<User>("users");

            // Check if users already exist
            var count = await usersCollection.CountDocumentsAsync(_ => true);
            if (count > 0)
                return;

            var users = new List<User>
            {
                new User
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Email = "admin@ems.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("AdminPassword123!"),
                    FullName = "System Admin",
                    Role = UserRole.Admin,
                    Phone = "555-9001",
                    Status = UserStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Email = "dispatcher@ems.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("DispatchPass123!"),
                    FullName = "John Dispatcher",
                    Role = UserRole.Dispatcher,
                    Phone = "555-9002",
                    Status = UserStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Email = "operator@ems.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("OperatorPass123!"),
                    FullName = "Jane Operator",
                    Role = UserRole.EmsOperator,
                    Phone = "555-9003",
                    Status = UserStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Email = "driver1@ems.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("DriverPass123!"),
                    FullName = "Mike Driver",
                    Role = UserRole.Driver,
                    Phone = "555-9004",
                    Status = UserStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Email = "driver2@ems.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("DriverPass123!"),
                    FullName = "Sarah Driver",
                    Role = UserRole.Driver,
                    Phone = "555-9005",
                    Status = UserStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await usersCollection.InsertManyAsync(users);
        }

        private static async Task SeedAmbulancesAsync(IMongoDatabase database)
        {
            var ambulancesCollection = database.GetCollection<Ambulance>("ambulances");

            // Check if ambulances already exist
            var count = await ambulancesCollection.CountDocumentsAsync(_ => true);
            if (count > 0)
                return;

            var ambulances = new List<Ambulance>
            {
                new Ambulance
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    RegistrationNumber = "AMB-001",
                    CurrentLocation = new Location { Latitude = 14.5995, Longitude = 120.9842 },
                    Status = DriverStatus.Available,
                    Capacity = 2,
                    Equipment = new List<string> { "Defibrillator", "Oxygen", "Stretcher", "Monitor" },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Ambulance
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    RegistrationNumber = "AMB-002",
                    CurrentLocation = new Location { Latitude = 14.5994, Longitude = 120.9855 },
                    Status = DriverStatus.Available,
                    Capacity = 2,
                    Equipment = new List<string> { "Defibrillator", "Oxygen", "Stretcher", "Monitor" },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Ambulance
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    RegistrationNumber = "AMB-003",
                    CurrentLocation = new Location { Latitude = 14.6010, Longitude = 120.9830 },
                    Status = DriverStatus.Busy,
                    Capacity = 2,
                    Equipment = new List<string> { "Defibrillator", "Oxygen", "Stretcher", "Monitor" },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await ambulancesCollection.InsertManyAsync(ambulances);
        }
    }
}
