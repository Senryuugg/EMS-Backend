# EMS Dispatcher System - Testing Guide

## Overview

This document provides comprehensive testing guidelines for the EMS Dispatcher system across all components: ASP.NET Core backend, Python ML service, and Flutter mobile app.

## Backend Testing (ASP.NET Core)

### Unit Tests

Create a test project: `EmsDispatch.Backend.Tests`

```bash
dotnet new xunit -n EmsDispatch.Backend.Tests
cd EmsDispatch.Backend.Tests
dotnet add reference ../EmsDispatch.Backend/EmsDispatch.Backend.csproj
```

### Key Test Areas

#### Authentication Tests
```csharp
[Fact]
public async Task Login_WithValidCredentials_ReturnsToken()
{
    // Arrange
    var authService = new AuthService(...);
    var loginRequest = new LoginRequest 
    { 
        Email = "test@ems.local", 
        Password = "TestPassword123!" 
    };

    // Act
    var result = await authService.LoginAsync(loginRequest);

    // Assert
    Assert.NotNull(result.Token);
    Assert.NotEmpty(result.Token);
}

[Fact]
public async Task Login_WithInvalidPassword_ThrowsException()
{
    // Arrange & Act & Assert
    await Assert.ThrowsAsync<UnauthorizedAccessException>(
        () => authService.LoginAsync(invalidRequest)
    );
}
```

#### Dispatch Service Tests
```csharp
[Fact]
public async Task CreateDispatch_WithValidData_ReturnsDispatch()
{
    // Arrange
    var dispatchService = new DispatchService(...);
    var createRequest = new CreateDispatchRequest { ... };

    // Act
    var result = await dispatchService.CreateDispatchAsync(createRequest);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("Pending", result.Status);
}
```

### Integration Tests

Test API endpoints with in-memory database:

```csharp
[Collection("API Collection")]
public class DispatchControllerIntegrationTests
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public DispatchControllerIntegrationTests()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetAllDispatches_ReturnsOkResult()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/dispatch");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

### Running Backend Tests

```bash
cd EmsDispatch.Backend.Tests
dotnet test
dotnet test --filter "Category=Integration"
```

## API Testing (Postman/Insomnia)

### Test Collection Structure

```
EMS Dispatcher API
├── Authentication
│   ├── Register User
│   ├── Login
│   ├── Refresh Token
│   └── Logout
├── Dispatch Management
│   ├── Get All Dispatches
│   ├── Create Dispatch
│   ├── Update Dispatch Status
│   └── Assign Driver
├── Driver Management
│   ├── Get Available Drivers
│   ├── Update Driver Location
│   └── Update Driver Status
├── Hospital Management
│   ├── Get All Hospitals
│   ├── Get Nearby Hospitals
│   └── Get Hospital Details
└── Health Checks
    └── API Health
```

### Sample API Test Cases

#### 1. Login Test
```
POST /api/auth/login
Content-Type: application/json

{
  "email": "dispatcher@ems.local",
  "password": "DispatchPass123!"
}

Expected: 200 OK with token
```

#### 2. Create Dispatch Test
```
POST /api/dispatch
Authorization: Bearer {token}
Content-Type: application/json

{
  "patientInfo": {
    "name": "John Doe",
    "age": 45,
    "condition": "Chest Pain",
    "allergies": ["Penicillin"]
  },
  "location": {
    "latitude": 14.5995,
    "longitude": 120.9842
  },
  "priority": "Critical",
  "description": "Patient experiencing chest pain"
}

Expected: 201 Created
```

#### 3. Update Driver Location Test
```
PUT /api/driver/{driverId}/location
Authorization: Bearer {token}
Content-Type: application/json

{
  "latitude": 14.6000,
  "longitude": 120.9845
}

Expected: 200 OK
```

## Python ML Service Testing

### Unit Tests

```python
# tests/test_hospital_prediction.py
import pytest
from ml_service import HospitalPredictor

@pytest.fixture
def predictor():
    return HospitalPredictor()

def test_predict_hospital_with_valid_data(predictor):
    # Arrange
    patient_data = {
        'latitude': 14.5995,
        'longitude': 120.9842,
        'condition': 'Chest Pain',
        'severity': 'High'
    }

    # Act
    result = predictor.predict(patient_data)

    # Assert
    assert result is not None
    assert 'hospital_id' in result
    assert 'confidence' in result
    assert result['confidence'] > 0

def test_predict_hospital_with_invalid_coordinates(predictor):
    # Arrange
    patient_data = {
        'latitude': 200,  # Invalid
        'longitude': 400,  # Invalid
        'condition': 'Chest Pain',
        'severity': 'High'
    }

    # Act & Assert
    with pytest.raises(ValueError):
        predictor.predict(patient_data)
```

### Running ML Tests

```bash
cd /path/to/ml_service
pip install pytest
pytest tests/
pytest tests/ -v  # Verbose output
```

## Flutter App Testing

### Widget Testing

```dart
// test/screens/login_screen_test.dart
import 'package:flutter_test/flutter_test.dart';
import 'package:provider/provider.dart';
import 'package:ems_dispatch_mobile/screens/login_screen.dart';
import 'package:ems_dispatch_mobile/providers/auth_provider.dart';

void main() {
  group('LoginScreen Widget Tests', () {
    testWidgets('Login button is visible', (WidgetTester tester) async {
      // Arrange
      await tester.pumpWidget(
        MultiProvider(
          providers: [
            ChangeNotifierProvider(create: (_) => MockAuthProvider()),
          ],
          child: const MaterialApp(home: LoginScreen()),
        ),
      );

      // Act
      expect(find.byType(ElevatedButton), findsWidgets);
    });

    testWidgets('Email validation works', (WidgetTester tester) async {
      // Arrange & Act
      await tester.pumpWidget(
        MultiProvider(
          providers: [
            ChangeNotifierProvider(create: (_) => MockAuthProvider()),
          ],
          child: const MaterialApp(home: LoginScreen()),
        ),
      );

      final emailField = find.byType(TextField).first;
      await tester.enterText(emailField, 'invalid-email');
      await tester.tap(find.byType(ElevatedButton));
      await tester.pumpAndSettle();

      // Assert
      expect(find.text('Invalid email'), findsWidgets);
    });
  });
}
```

### Integration Testing

```dart
// test_driver/app_test.dart
import 'package:flutter_test/flutter_test.dart';
import 'package:integration_test/integration_test.dart';
import 'package:ems_dispatch_mobile/main.dart' as app;

void main() {
  IntegrationTestWidgetsFlutterBinding.ensureInitialized();

  group('EMS App Integration Tests', () {
    testWidgets('Complete login flow', (WidgetTester tester) async {
      app.main();
      await tester.pumpAndSettle();

      // Verify splash screen
      expect(find.byType(CircularProgressIndicator), findsWidgets);
      await Future.delayed(Duration(seconds: 3));
      await tester.pumpAndSettle();

      // Enter email
      await tester.enterText(find.byType(TextField).first, 'driver1@ems.local');
      
      // Enter password
      await tester.enterText(find.byType(TextField).last, 'DriverPass123!');

      // Tap login
      await tester.tap(find.byType(ElevatedButton).first);
      await tester.pumpAndSettle();

      // Verify dashboard appears
      expect(find.byType(DriverDashboard), findsWidgets);
    });
  });
}
```

### Running Flutter Tests

```bash
cd ems_dispatch_mobile

# Unit tests
flutter test

# Widget tests
flutter test test/screens/

# Integration tests
flutter drive --target=test_driver/app_test.dart

# With coverage
flutter test --coverage
lcov --list coverage/lcov.info
```

## Load & Performance Testing

### Backend Load Testing (Artillery)

```yaml
# artillery.yml
config:
  target: "http://localhost:5000"
  phases:
    - duration: 60
      arrivalRate: 10
      name: "Warm up"
    - duration: 120
      arrivalRate: 50
      name: "Ramp up load"
    - duration: 60
      arrivalRate: 50
      name: "Sustained load"

scenarios:
  - name: "Dispatch API"
    flow:
      - post:
          url: "/api/auth/login"
          json:
            email: "dispatcher@ems.local"
            password: "DispatchPass123!"
          capture:
            json: "$.data.token"
            as: "authToken"
      - get:
          url: "/api/dispatch"
          headers:
            Authorization: "Bearer {{ authToken }}"
```

```bash
npm install -g artillery
artillery run artillery.yml
```

## Testing Checklist

### Backend
- [ ] Authentication (login, register, token refresh)
- [ ] Authorization (role-based access control)
- [ ] Dispatch CRUD operations
- [ ] Driver management
- [ ] Ambulance location updates
- [ ] Hospital queries
- [ ] Error handling
- [ ] Input validation
- [ ] Database transactions

### ML Service
- [ ] Hospital prediction accuracy
- [ ] Route optimization
- [ ] Error handling with invalid input
- [ ] Performance (prediction response time)
- [ ] Model loading

### Mobile App
- [ ] Login flow
- [ ] Logout flow
- [ ] Dispatch creation (dispatcher)
- [ ] Dispatch listing
- [ ] Location updates (driver)
- [ ] Real-time updates via SignalR
- [ ] Map rendering
- [ ] Offline handling

### Integration
- [ ] Backend ↔ Mobile app API calls
- [ ] Backend ↔ ML service integration
- [ ] SignalR real-time updates
- [ ] End-to-end dispatch workflow

## CI/CD Testing

### GitHub Actions Example

```yaml
# .github/workflows/backend-tests.yml
name: Backend Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    services:
      mongodb:
        image: mongo:5.0
        options: >-
          --health-cmd "mongo --eval 'db.adminCommand(\"ping\")'
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5
        ports:
          - 27017:27017

    steps:
      - uses: actions/checkout@v2
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '8.0.x'
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Build
        run: dotnet build --no-restore
      
      - name: Run tests
        run: dotnet test --no-build --verbosity normal
```

## Best Practices

1. **Write tests early** - TDD approach
2. **Mock external dependencies** - Database, API calls
3. **Test edge cases** - Invalid input, boundary conditions
4. **Keep tests isolated** - No dependencies between tests
5. **Use meaningful names** - Clear what is being tested
6. **Maintain test data** - Clean, reproducible fixtures
7. **Automate testing** - CI/CD pipelines
8. **Monitor coverage** - Aim for >80% code coverage
9. **Performance testing** - Ensure API meets SLAs
10. **Security testing** - Test authentication, authorization, injection

## Resources

- [xUnit.net Documentation](https://xunit.net/)
- [MSTest for .NET](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-mstest)
- [Postman Testing](https://learning.postman.com/docs/testing-apis/testing-apis-overview/)
- [Flutter Testing Guide](https://flutter.dev/docs/testing)
- [pytest Documentation](https://docs.pytest.org/)

