# EMS Dispatcher Mobile App - Complete System

A real-time Emergency Medical Services (EMS) dispatch management system with intelligent hospital prediction using machine learning, built with ASP.NET Core backend, Flutter mobile app, and FastAPI ML service.

## System Architecture

```
┌─────────────────┐     ┌──────────────────┐     ┌─────────────────┐
│  Flutter Mobile │────▶│  ASP.NET Core    │────▶│  MongoDB        │
│  App (Android   │     │  Backend API     │     │  Database       │
│  & iOS)         │     │  (SignalR)       │     │                 │
└─────────────────┘     └──────────────────┘     └─────────────────┘
                               │
                               ▼
                        ┌──────────────────┐
                        │  FastAPI ML      │
                        │  Service         │
                        │  (Hospital       │
                        │  Prediction)     │
                        └──────────────────┘
```

## Features

### Real-Time Dispatch Management
- Create and manage emergency dispatches with patient details
- Real-time status tracking (Pending → Assigned → InProgress → Complete)
- Multi-level priority system (Low, Medium, High, Critical)

### Intelligent Hospital Routing
- ML-based hospital prediction using patient condition and location
- Automated route optimization considering traffic
- EMS base-to-patient-to-hospital routing

### Real-Time Tracking
- Live ambulance location streaming via SignalR
- WebSocket-based real-time updates for all connected clients
- Driver and ambulance status management

### Role-Based Access Control
- **Admin**: Full system access, user management
- **Dispatcher**: Create and manage dispatches, assign drivers
- **EMS Operator**: Monitor dispatch status from EMS bases
- **Driver**: View assigned dispatches, update status, navigate

### Multi-Platform Mobile Support
- Flutter app targeting both Android and iOS
- Offline-capable with sync capabilities
- Native geolocation and mapping integration

## Tech Stack

### Backend
- **Framework**: ASP.NET Core 8.0
- **Database**: MongoDB
- **Real-Time**: SignalR WebSockets
- **Authentication**: JWT with refresh tokens
- **Logging**: Serilog

### ML Service
- **Framework**: FastAPI (Python)
- **ML Model**: scikit-learn Random Forest
- **Distance Calculation**: OpenRouteService API with Haversine fallback
- **Data Processing**: Pandas, NumPy

### Mobile
- **Framework**: Flutter (Dart)
- **State Management**: Provider / GetX
- **Maps**: Google Maps Flutter
- **Real-Time**: SignalR Client
- **Location**: Geolocator

## Project Structure

```
EmsDispatch.Backend/
├── Models/
│   ├── User.cs
│   ├── Dispatch.cs
│   ├── Driver.cs
│   ├── Ambulance.cs
│   ├── Hospital.cs
│   └── Enums/
├── Services/
│   ├── AuthService.cs
│   ├── DispatchService.cs
│   ├── DriverService.cs
│   ├── AmbulanceService.cs
│   └── SessionService.cs
├── Controllers/
│   ├── AuthController.cs
│   ├── DispatchController.cs
│   ├── DriverController.cs
│   └── AmbulanceController.cs
├── Hubs/
│   ├── DispatchHub.cs
│   └── LocationHub.cs
├── Configuration/
│   └── MongoDbContext.cs
└── Program.cs

ml_api.py              # FastAPI application
ml_service.py          # ML prediction logic
requirements.txt       # Python dependencies

models/
├── hospital_prediction_model.pkl
├── le_severity.pkl
└── le_condition.pkl

datasets/
├── hospital/
├── patient/
└── ems/
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- Python 3.11+
- MongoDB 5.0+
- Docker & Docker Compose (optional)
- Flutter SDK (for mobile development)

### Backend Setup (ASP.NET Core)

1. **Navigate to backend directory**:
   ```bash
   cd EmsDispatch.Backend
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Configure MongoDB connection** in `appsettings.Development.json`:
   ```json
   "MongoDb": {
     "ConnectionString": "mongodb://localhost:27017",
     "DatabaseName": "ems_dispatch_dev"
   }
   ```

4. **Run migrations** (if needed):
   ```bash
   dotnet ef database update
   ```

5. **Start the backend**:
   ```bash
   dotnet run
   ```

The API will be available at `https://localhost:5001` (or `http://localhost:5000` in Development)
API Documentation: `https://localhost:5001/swagger`

### ML Service Setup (FastAPI)

1. **Install Python dependencies**:
   ```bash
   pip install -r requirements.txt
   ```

2. **Start the ML service**:
   ```bash
   python ml_api.py
   ```
   
   Or with uvicorn:
   ```bash
   uvicorn ml_api:app --reload --port 8000
   ```

The ML API will be available at `http://localhost:8000`
API Documentation: `http://localhost:8000/docs`

### Docker Compose Setup (Recommended for Local Development)

1. **Start all services**:
   ```bash
   docker-compose up -d
   ```

This starts:
- MongoDB on `localhost:27017`
- MongoDB Express on `localhost:8081` (UI for MongoDB)
- ASP.NET Backend on `localhost:5000`
- FastAPI ML Service on `localhost:8000`

2. **View logs**:
   ```bash
   docker-compose logs -f backend
   docker-compose logs -f ml-service
   ```

3. **Stop services**:
   ```bash
   docker-compose down
   ```

### Flutter Mobile App Setup

1. **Create Flutter project** (if not already created):
   ```bash
   flutter create ems_dispatch_mobile --template=app
   cd ems_dispatch_mobile
   ```

2. **Add dependencies** to `pubspec.yaml`:
   ```yaml
   dependencies:
     flutter:
       sdk: flutter
     http: ^1.1.0
     google_maps_flutter: ^2.5.0
     signalr_client: ^0.7.1
     location: ^6.0.0
     geolocator: ^9.0.2
     provider: ^6.0.0
     secure_storage: ^8.0.0
   ```

3. **Configure API endpoints** in your app:
   ```dart
   const String API_BASE_URL = "http://10.0.2.2:5000"; // Android emulator
   const String SIGNALR_HUB_URL = "http://10.0.2.2:5000/hubs";
   ```

4. **Run the app**:
   ```bash
   flutter run
   ```

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `POST /api/auth/logout` - User logout
- `GET /api/auth/me` - Get current user info

### Dispatches
- `POST /api/dispatch` - Create new dispatch
- `GET /api/dispatch` - Get all dispatches
- `GET /api/dispatch/{id}` - Get dispatch by ID
- `GET /api/dispatch/status/{status}` - Get dispatches by status
- `PUT /api/dispatch/{id}/status/{status}` - Update dispatch status
- `PUT /api/dispatch/{id}/assign` - Assign dispatch to driver

### Drivers
- `POST /api/driver` - Create driver
- `GET /api/driver/{id}` - Get driver by ID
- `GET /api/driver/available` - Get available drivers
- `PUT /api/driver/{id}/location` - Update driver location
- `PUT /api/driver/{id}/status/{status}` - Update driver status

### Ambulances
- `POST /api/ambulance` - Create ambulance
- `GET /api/ambulance/{id}` - Get ambulance by ID
- `GET /api/ambulance/available` - Get available ambulances
- `PUT /api/ambulance/{id}/location` - Update ambulance location
- `PUT /api/ambulance/{id}/status/{status}` - Update ambulance status

### ML Service
- `GET /health` - Service health check
- `POST /predict/hospital` - Predict best hospital for patient
- `POST /optimize/route` - Optimize ambulance route

### SignalR Hubs
- `/hubs/dispatch` - Real-time dispatch updates
- `/hubs/location` - Real-time location tracking

## Configuration

### Environment Variables

Create `.env` file in the root directory:

```env
# MongoDB
MONGO_CONNECTION_STRING=mongodb://localhost:27017
MONGO_DB_NAME=ems_dispatch

# JWT
JWT_SECRET_KEY=your-super-secret-key-min-32-chars-long!
JWT_ISSUER=ems-dispatch-api
JWT_AUDIENCE=ems-dispatch-mobile-app
JWT_EXPIRATION_MINUTES=60

# ML Service
ML_SERVICE_BASE_URL=http://localhost:8000
ORS_API_KEY=your-openrouteservice-api-key
```

### Database Initialization

MongoDB collections are automatically created on first run with appropriate indexes.

To seed initial data, create a seed script:

```csharp
// Seeds/DatabaseSeeder.cs
public static class DatabaseSeeder
{
    public static async Task SeedAsync(IMongoDbContext context)
    {
        // Add hospitals
        var hospitals = new List<Hospital> { /* ... */ };
        await context.Hospitals.InsertManyAsync(hospitals);
        
        // Add EMS bases
        // ... etc
    }
}
```

## Development Workflow

1. **Run MongoDB**:
   ```bash
   docker run -d -p 27017:27017 -e MONGO_INITDB_ROOT_USERNAME=admin -e MONGO_INITDB_ROOT_PASSWORD=password mongo
   ```

2. **Start Backend** (Terminal 1):
   ```bash
   cd EmsDispatch.Backend
   dotnet run
   ```

3. **Start ML Service** (Terminal 2):
   ```bash
   python ml_api.py
   ```

4. **Start Flutter App** (Terminal 3):
   ```bash
   cd ems_dispatch_mobile
   flutter run
   ```

## Testing

### Backend Tests
```bash
cd EmsDispatch.Backend.Tests
dotnet test
```

### ML Service Tests
```bash
pytest tests/
```

## Deployment

### Docker Deployment

1. **Build Docker images**:
   ```bash
   docker build -t ems-dispatch-backend -f EmsDispatch.Backend/Dockerfile .
   docker build -t ems-dispatch-ml -f Dockerfile.ml .
   ```

2. **Deploy with docker-compose** (production config):
   ```bash
   docker-compose -f docker-compose.prod.yml up -d
   ```

### Kubernetes Deployment

Create Kubernetes manifests for backend, ML service, and MongoDB:

```yaml
# k8s/deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: ems-dispatch-backend
spec:
  replicas: 3
  selector:
    matchLabels:
      app: ems-dispatch-backend
  template:
    metadata:
      labels:
        app: ems-dispatch-backend
    spec:
      containers:
      - name: backend
        image: ems-dispatch-backend:latest
        ports:
        - containerPort: 80
        env:
        - name: MONGO_CONNECTION_STRING
          valueFrom:
            secretKeyRef:
              name: ems-secrets
              key: mongo-connection-string
```

## Security Considerations

1. **JWT Token Storage**: Use secure storage on mobile (SecureStorage package)
2. **HTTPS**: Use HTTPS in production
3. **CORS**: Configure CORS properly for production domains
4. **Rate Limiting**: Implement rate limiting on API endpoints
5. **Input Validation**: All endpoints validate user input
6. **Role-Based Access**: Enforce role-based authorization

## Troubleshooting

### MongoDB Connection Issues
```bash
# Check if MongoDB is running
docker ps | grep mongo

# Connect to MongoDB
mongo -u admin -p password --authenticationDatabase admin
```

### Backend Won't Start
```bash
# Clear build and try again
dotnet clean
dotnet restore
dotnet run
```

### ML Service Errors
```bash
# Check Python version
python --version  # Should be 3.11+

# Reinstall dependencies
pip install -r requirements.txt --force-reinstall
```

### SignalR Connection Issues
- Check WebSocket support in your network/firewall
- Verify CORS configuration in `Program.cs`
- Check browser console for connection errors

## Performance Optimization

1. **Database Indexing**: Indexes created automatically for common queries
2. **Caching**: Implement Redis caching for hospital data
3. **Pagination**: Use pagination for large result sets
4. **Location Updates**: Throttle location updates to 5-10 seconds
5. **ML Service**: Cache predictions for similar inputs

## Future Enhancements

- [ ] Multi-language support
- [ ] Advanced analytics dashboard
- [ ] Integration with public EMS APIs
- [ ] Audio/video call support
- [ ] Historical data analysis
- [ ] Predictive dispatch suggestions
- [ ] Mobile app offline mode with sync
- [ ] Advanced traffic prediction integration
- [ ] SMS notifications
- [ ] Integration with emergency hotlines

## Contributing

1. Create a feature branch
2. Make your changes
3. Submit a pull request

## License

MIT License - See LICENSE file for details

## Support

For issues, questions, or suggestions:
1. Check existing issues
2. Create a detailed issue with steps to reproduce
3. Contact: support@emsdispatch.local

## Team

- Backend: ASP.NET Core Developer
- ML/Data: Python ML Engineer
- Mobile: Flutter Developer
- DevOps: Docker/Kubernetes Specialist
