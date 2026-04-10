# EMS Dispatcher - Development Guide

## Local Development Setup

### Prerequisites

- .NET 8.0 SDK
- Python 3.11+
- Flutter 3.0+
- Docker & Docker Compose
- Git
- Visual Studio Code or Visual Studio

### Initial Setup

1. Clone the repository
```bash
git clone https://github.com/Senryuugg/EMS-Backend.git
cd EMS-Backend
```

2. Start services with Docker Compose
```bash
docker-compose up -d
```

3. Verify services are running
```bash
docker-compose ps
```

Services available at:
- Backend API: http://localhost:5000
- API Documentation: http://localhost:5000/swagger
- MongoDB: mongodb://localhost:27017
- MongoDB Express: http://localhost:8081
- ML Service: http://localhost:8000
- ML Docs: http://localhost:8000/docs

## Backend Development

### Project Structure

```
EmsDispatch.Backend/
├── Models/              # Domain entities
│   ├── Enums/          # Status and role enums
│   ├── User.cs         # User entity
│   ├── Dispatch.cs     # Dispatch entity
│   └── ...
├── Services/           # Business logic layer
│   ├── AuthService.cs
│   ├── DispatchService.cs
│   └── ...
├── Controllers/        # API endpoints
│   ├── AuthController.cs
│   ├── DispatchController.cs
│   └── ...
├── Hubs/              # SignalR real-time
│   ├── DispatchHub.cs
│   └── LocationHub.cs
├── DTOs/              # Data transfer objects
└── appsettings.json   # Configuration
```

### Running Backend Locally

```bash
cd EmsDispatch.Backend
dotnet restore
dotnet run
```

The API will start on https://localhost:7xxx

### Database Seeding

Create a seed script to initialize demo data:

```csharp
// Seeds/DatabaseSeeder.cs
public class DatabaseSeeder
{
    public static async Task SeedAsync(IMongoDatabase database)
    {
        // Create collections
        var usersCollection = database.GetCollection<User>("users");
        var hospitalsCollection = database.GetCollection<Hospital>("hospitals");
        
        // Add initial data
        if (await usersCollection.CountDocumentsAsync(new BsonDocument()) == 0)
        {
            var users = new List<User>
            {
                new User
                {
                    Email = "admin@ems.local",
                    Name = "Admin User",
                    Role = UserRole.Admin,
                    PasswordHash = HashPassword("Admin123!"),
                }
            };
            await usersCollection.InsertManyAsync(users);
        }
    }
}
```

### API Testing

#### Using Swagger

1. Navigate to http://localhost:5000/swagger
2. Authorize with test token
3. Execute endpoints

#### Using cURL

```bash
# Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@ems.local",
    "password": "Admin123!"
  }'

# Create Dispatch (with JWT token)
curl -X POST http://localhost:5000/api/dispatch \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "patientInfo": {
      "name": "John Doe",
      "age": 45,
      "condition": "TraumaInjury"
    },
    "priority": "High",
    "location": {
      "latitude": 14.5995,
      "longitude": 121.0437
    }
  }'
```

## Python ML Service Development

### Project Structure

```
├── ml_api.py           # FastAPI application
├── ml_service.py       # ML service logic
├── requirements.txt    # Python dependencies
└── models/            # Trained models
    ├── hospital_prediction_model.pkl
    ├── le_condition.pkl
    └── le_severity.pkl
```

### Running ML Service Locally

```bash
pip install -r requirements.txt
python ml_api.py
```

The service will start on http://localhost:8000

### Testing ML Endpoints

```bash
# Predict best hospital
curl -X POST http://localhost:8000/predict/hospital \
  -H "Content-Type: application/json" \
  -d '{
    "latitude": 14.5995,
    "longitude": 121.0437,
    "condition": "TraumaInjury",
    "severity": "High"
  }'

# Optimize route
curl -X POST http://localhost:8000/optimize/route \
  -H "Content-Type: application/json" \
  -d '{
    "ambulance_lat": 14.6000,
    "ambulance_lng": 121.0440,
    "patient_lat": 14.5995,
    "patient_lng": 121.0437,
    "hospital_lat": 14.5900,
    "hospital_lng": 121.0350
  }'
```

## Flutter Development

### Project Structure

```
ems_dispatch_mobile/
├── lib/
│   ├── main.dart              # Entry point
│   ├── config/               # Configuration
│   │   └── api_config.dart
│   ├── models/               # Data models
│   ├── providers/            # State management
│   │   ├── auth_provider.dart
│   │   ├── dispatch_provider.dart
│   │   └── location_provider.dart
│   ├── screens/              # UI screens
│   ├── services/             # API/SignalR services
│   └── widgets/              # Reusable widgets
├── pubspec.yaml              # Dependencies
└── .env                       # Environment variables
```

### Running Flutter App

```bash
cd ems_dispatch_mobile

# Get dependencies
flutter pub get

# Run on Android
flutter run -d android

# Run on iOS
flutter run -d ios

# Run on web
flutter run -d web

# Run with verbose logging
flutter run -v
```

### Flutter Testing

```bash
# Run all tests
flutter test

# Run specific test file
flutter test test/providers/auth_provider_test.dart

# Run tests with coverage
flutter test --coverage
```

### Debugging Flutter App

```bash
# Hot reload
Press 'r' during development

# Hot restart
Press 'R'

# Toggle debug painting
Press 'p'

# Toggle performance overlay
Press 't'

# Open DevTools
flutter pub global run devtools
```

## Testing Strategy

### Unit Testing

```csharp
// Tests/Services/AuthServiceTests.cs
[TestClass]
public class AuthServiceTests
{
    private AuthService _authService;
    private IMongoDatabase _database;
    
    [TestInitialize]
    public void Setup()
    {
        // Initialize test database
        _database = new MongoClient("mongodb://localhost:27017").GetDatabase("test_db");
        _authService = new AuthService(_database);
    }
    
    [TestMethod]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var email = "test@ems.local";
        var password = "Test123!";
        
        // Act
        var result = await _authService.LoginAsync(email, password);
        
        // Assert
        Assert.IsNotNull(result.Token);
    }
}
```

### Integration Testing

```csharp
// Tests/Controllers/DispatchControllerTests.cs
[TestClass]
public class DispatchControllerIntegrationTests
{
    private HttpClient _httpClient;
    private string _token;
    
    [TestInitialize]
    public async Task Setup()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };
        
        // Login and get token
        var loginResponse = await _httpClient.PostAsync(
            "/api/auth/login",
            new StringContent(JsonConvert.SerializeObject(
                new { email = "dispatcher@ems.local", password = "Test123!" }
            ), Encoding.UTF8, "application/json")
        );
        
        var loginResult = JsonConvert.DeserializeObject<LoginResponse>(
            await loginResponse.Content.ReadAsStringAsync()
        );
        _token = loginResult.Token;
    }
    
    [TestMethod]
    public async Task CreateDispatch_WithValidData_ReturnsCreated()
    {
        // Arrange
        var dispatchData = new { /* dispatch data */ };
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/dispatch")
        {
            Content = new StringContent(
                JsonConvert.SerializeObject(dispatchData),
                Encoding.UTF8,
                "application/json"
            ),
            Headers = { Authorization = new("Bearer", _token) }
        };
        
        // Act
        var response = await _httpClient.SendAsync(request);
        
        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
    }
}
```

## Debugging

### Backend Debugging

Using Visual Studio:
1. Set breakpoint
2. Press F5 to start debugging
3. API will pause at breakpoint

Using VSCode:
1. Install C# extension
2. Configure launch.json
3. Press F5 to debug

### Database Debugging

Access MongoDB Express at http://localhost:8081
- Username: admin
- Password: password

### API Logging

Set log level in appsettings.Development.json:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Debug"
    }
  }
}
```

### ML Service Debugging

```python
# Add debug logging in ml_api.py
import logging

logging.basicConfig(level=logging.DEBUG)
logger = logging.getLogger(__name__)

@app.post("/predict/hospital")
async def predict_hospital(request: HospitalPredictionRequest):
    logger.debug(f"Received prediction request: {request}")
    # ... rest of code
```

## Performance Profiling

### Backend Profiling

```bash
# Profile with dotnet
dotnet build -c Release
dotnet run -c Release

# Monitor with Performance Monitor (Windows)
# or similar tools for Linux/macOS
```

### Database Query Analysis

```javascript
// MongoDB explain for query optimization
db.dispatches.find({ status: "Pending" }).explain("executionStats")
```

### Flutter Performance

```bash
# Enable performance overlay
flutter run --profile

# Generate detailed timeline
flutter screenshot --type=rasterizer
```

## Common Issues & Solutions

### Issue: MongoDB Connection Fails
**Solution**: 
- Verify MongoDB is running: `docker-compose ps`
- Check connection string in appsettings.json
- Ensure network isolation isn't blocking connections

### Issue: SignalR Connection Timeout
**Solution**:
- Check WebSocket support in proxy/firewall
- Verify SignalR hub routes in Program.cs
- Enable cross-origin requests in CORS

### Issue: Flutter: Module not found
**Solution**:
- Run: `flutter clean && flutter pub get`
- Delete pubspec.lock and regenerate
- Verify pub.dev access

### Issue: JWT Token Expired
**Solution**:
- Implement token refresh mechanism
- Store refresh token in secure storage
- Automatically refresh before expiry

## Git Workflow

```bash
# Create feature branch
git checkout -b feature/new-feature

# Commit changes
git add .
git commit -m "Add new feature"

# Push to remote
git push origin feature/new-feature

# Create pull request on GitHub
# Review and merge
```

## Useful Commands

```bash
# Backend
dotnet clean
dotnet restore
dotnet build
dotnet test
dotnet run

# Python
pip list
pip install -r requirements.txt
python -m pytest

# Flutter
flutter doctor
flutter devices
flutter clean
flutter pub get
flutter analyze
flutter test
flutter build apk
flutter build ipa

# Docker
docker-compose up -d
docker-compose down
docker-compose logs -f
docker ps
docker exec -it container_name bash
```

## Performance Checklist

- [ ] Database indexes created
- [ ] API response times < 200ms
- [ ] ML predictions < 1 second
- [ ] Mobile app startup < 3 seconds
- [ ] Real-time updates < 500ms latency
- [ ] Memory usage monitored
- [ ] CPU usage optimized
- [ ] Database connections pooled

---

For deployment instructions, see [DEPLOYMENT_GUIDE.md](./DEPLOYMENT_GUIDE.md).
