# EMS Dispatcher API Reference

## Base URL
```
http://localhost:5000/api
```

## Authentication

All endpoints (except `/auth/login` and `/auth/register`) require JWT token in the Authorization header:

```
Authorization: Bearer YOUR_JWT_TOKEN
```

---

## Authentication Endpoints

### Login
- **URL**: `/auth/login`
- **Method**: POST
- **Auth Required**: No

**Request Body**:
```json
{
  "email": "user@ems.local",
  "password": "SecurePassword123"
}
```

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "userId": "507f1f77bcf86cd799439011",
    "email": "user@ems.local",
    "name": "John Dispatcher",
    "role": "Dispatcher"
  }
}
```

### Register
- **URL**: `/auth/register`
- **Method**: POST
- **Auth Required**: No

**Request Body**:
```json
{
  "email": "newuser@ems.local",
  "password": "SecurePassword123",
  "name": "New User",
  "role": "Driver",
  "phone": "+1234567890"
}
```

**Response** (201 Created):
```json
{
  "success": true,
  "message": "User registered successfully",
  "data": {
    "userId": "507f1f77bcf86cd799439011",
    "email": "newuser@ems.local"
  }
}
```

### Get Current User
- **URL**: `/auth/me`
- **Method**: GET
- **Auth Required**: Yes

**Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "userId": "507f1f77bcf86cd799439011",
    "email": "user@ems.local",
    "name": "John Dispatcher",
    "role": "Dispatcher",
    "phone": "+1234567890",
    "status": "Active"
  }
}
```

### Logout
- **URL**: `/auth/logout`
- **Method**: POST
- **Auth Required**: Yes

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Logout successful"
}
```

---

## Dispatch Management Endpoints

### Get All Dispatches
- **URL**: `/dispatch`
- **Method**: GET
- **Auth Required**: Yes
- **Query Parameters**:
  - `status`: Filter by status (Pending, Assigned, InProgress, Arrived, Complete)
  - `priority`: Filter by priority (Low, Medium, High, Critical)
  - `skip`: Pagination skip (default: 0)
  - `limit`: Results per page (default: 20)

**Example**:
```
GET /dispatch?status=Pending&priority=High&skip=0&limit=10
```

**Response** (200 OK):
```json
{
  "success": true,
  "data": [
    {
      "id": "507f1f77bcf86cd799439011",
      "callId": "CALL-001",
      "patientInfo": {
        "name": "Jane Smith",
        "age": 45,
        "phone": "+1987654321",
        "condition": "TraumaInjury",
        "severity": "High"
      },
      "priority": "High",
      "status": "InProgress",
      "location": {
        "latitude": 14.5995,
        "longitude": 121.0437
      },
      "assignedDriverId": "507f1f77bcf86cd799439012",
      "assignedAmbulanceId": "507f1f77bcf86cd799439013",
      "predictedHospitalId": "507f1f77bcf86cd799439014",
      "createdAt": "2024-01-15T10:30:00Z",
      "updatedAt": "2024-01-15T10:35:00Z",
      "completedAt": null
    }
  ],
  "pagination": {
    "total": 45,
    "skip": 0,
    "limit": 10
  }
}
```

### Get Dispatch by ID
- **URL**: `/dispatch/{id}`
- **Method**: GET
- **Auth Required**: Yes

**Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "id": "507f1f77bcf86cd799439011",
    "callId": "CALL-001",
    "patientInfo": { /* ... */ },
    "priority": "High",
    "status": "InProgress",
    "location": { /* ... */ },
    "assignedDriverId": "507f1f77bcf86cd799439012",
    "assignedAmbulanceId": "507f1f77bcf86cd799439013",
    "createdAt": "2024-01-15T10:30:00Z"
  }
}
```

### Create Dispatch
- **URL**: `/dispatch`
- **Method**: POST
- **Auth Required**: Yes (Dispatcher role)

**Request Body**:
```json
{
  "patientInfo": {
    "name": "Jane Smith",
    "age": 45,
    "phone": "+1987654321",
    "condition": "TraumaInjury",
    "severity": "High"
  },
  "priority": "High",
  "location": {
    "latitude": 14.5995,
    "longitude": 121.0437
  }
}
```

**Response** (201 Created):
```json
{
  "success": true,
  "message": "Dispatch created successfully",
  "data": {
    "id": "507f1f77bcf86cd799439011",
    "callId": "CALL-001",
    "status": "Pending"
  }
}
```

### Update Dispatch Status
- **URL**: `/dispatch/{id}/status/{status}`
- **Method**: PUT
- **Auth Required**: Yes
- **Path Parameters**:
  - `id`: Dispatch ID
  - `status`: New status (Pending, Assigned, InProgress, Arrived, Complete, Cancelled)

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Dispatch status updated",
  "data": {
    "id": "507f1f77bcf86cd799439011",
    "status": "InProgress",
    "updatedAt": "2024-01-15T10:35:00Z"
  }
}
```

### Assign Driver to Dispatch
- **URL**: `/dispatch/{id}/assign`
- **Method**: PUT
- **Auth Required**: Yes (Dispatcher role)

**Request Body**:
```json
{
  "driverId": "507f1f77bcf86cd799439012",
  "ambulanceId": "507f1f77bcf86cd799439013"
}
```

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Driver assigned successfully",
  "data": {
    "id": "507f1f77bcf86cd799439011",
    "assignedDriverId": "507f1f77bcf86cd799439012",
    "assignedAmbulanceId": "507f1f77bcf86cd799439013",
    "status": "Assigned"
  }
}
```

---

## Driver Management Endpoints

### Get Driver by ID
- **URL**: `/driver/{id}`
- **Method**: GET
- **Auth Required**: Yes

**Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "id": "507f1f77bcf86cd799439012",
    "userId": "507f1f77bcf86cd799439011",
    "name": "John Driver",
    "licenseNumber": "DL123456",
    "ambulanceId": "507f1f77bcf86cd799439013",
    "currentLocation": {
      "latitude": 14.6000,
      "longitude": 121.0440,
      "timestamp": "2024-01-15T10:35:00Z"
    },
    "status": "Available",
    "createdAt": "2024-01-10T09:00:00Z"
  }
}
```

### Get Available Drivers
- **URL**: `/driver/available`
- **Method**: GET
- **Auth Required**: Yes

**Response** (200 OK):
```json
{
  "success": true,
  "data": [
    {
      "id": "507f1f77bcf86cd799439012",
      "name": "John Driver",
      "status": "Available",
      "ambulanceId": "507f1f77bcf86cd799439013",
      "currentLocation": { /* ... */ }
    }
  ]
}
```

### Create Driver
- **URL**: `/driver`
- **Method**: POST
- **Auth Required**: Yes (Admin role)

**Request Body**:
```json
{
  "userId": "507f1f77bcf86cd799439011",
  "licenseNumber": "DL123456",
  "ambulanceId": "507f1f77bcf86cd799439013"
}
```

**Response** (201 Created):
```json
{
  "success": true,
  "message": "Driver created successfully",
  "data": {
    "id": "507f1f77bcf86cd799439012"
  }
}
```

### Update Driver Location
- **URL**: `/driver/{id}/location`
- **Method**: PUT
- **Auth Required**: Yes

**Request Body**:
```json
{
  "latitude": 14.6010,
  "longitude": 121.0450
}
```

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Location updated",
  "data": {
    "id": "507f1f77bcf86cd799439012",
    "currentLocation": {
      "latitude": 14.6010,
      "longitude": 121.0450,
      "timestamp": "2024-01-15T10:40:00Z"
    }
  }
}
```

### Update Driver Status
- **URL**: `/driver/{id}/status/{status}`
- **Method**: PUT
- **Auth Required**: Yes
- **Path Parameters**:
  - `status`: New status (Available, Busy, OnBreak, Offline)

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Driver status updated",
  "data": {
    "id": "507f1f77bcf86cd799439012",
    "status": "Busy"
  }
}
```

---

## Ambulance Management Endpoints

### Get Ambulance by ID
- **URL**: `/ambulance/{id}`
- **Method**: GET
- **Auth Required**: Yes

**Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "id": "507f1f77bcf86cd799439013",
    "registrationNumber": "AMB-001",
    "status": "Available",
    "driverId": "507f1f77bcf86cd799439012",
    "currentLocation": {
      "latitude": 14.6000,
      "longitude": 121.0440
    },
    "capacity": 4,
    "equipment": ["Monitor", "Defibrillator", "Oxygen"],
    "createdAt": "2024-01-10T09:00:00Z"
  }
}
```

### Get Available Ambulances
- **URL**: `/ambulance/available`
- **Method**: GET
- **Auth Required**: Yes

**Response** (200 OK):
```json
{
  "success": true,
  "data": [
    {
      "id": "507f1f77bcf86cd799439013",
      "registrationNumber": "AMB-001",
      "status": "Available",
      "driverId": "507f1f77bcf86cd799439012",
      "currentLocation": { /* ... */ }
    }
  ]
}
```

### Create Ambulance
- **URL**: `/ambulance`
- **Method**: POST
- **Auth Required**: Yes (Admin role)

**Request Body**:
```json
{
  "registrationNumber": "AMB-002",
  "capacity": 4,
  "equipment": ["Monitor", "Defibrillator", "Oxygen"]
}
```

**Response** (201 Created):
```json
{
  "success": true,
  "message": "Ambulance created successfully",
  "data": {
    "id": "507f1f77bcf86cd799439014"
  }
}
```

### Update Ambulance Status
- **URL**: `/ambulance/{id}/status/{status}`
- **Method**: PUT
- **Auth Required**: Yes

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Ambulance status updated",
  "data": {
    "id": "507f1f77bcf86cd799439013",
    "status": "Maintenance"
  }
}
```

---

## Session Endpoints

### Get Active Sessions
- **URL**: `/session/active`
- **Method**: GET
- **Auth Required**: Yes (Admin role)

**Response** (200 OK):
```json
{
  "success": true,
  "data": [
    {
      "userId": "507f1f77bcf86cd799439011",
      "userName": "John Dispatcher",
      "role": "Dispatcher",
      "status": "Online",
      "lastActivity": "2024-01-15T10:40:00Z"
    }
  ]
}
```

---

## Error Responses

### 400 Bad Request
```json
{
  "success": false,
  "message": "Invalid request parameters",
  "errors": {
    "email": "Email is required"
  }
}
```

### 401 Unauthorized
```json
{
  "success": false,
  "message": "Unauthorized - Please login"
}
```

### 403 Forbidden
```json
{
  "success": false,
  "message": "Forbidden - Insufficient permissions"
}
```

### 404 Not Found
```json
{
  "success": false,
  "message": "Resource not found"
}
```

### 500 Internal Server Error
```json
{
  "success": false,
  "message": "Internal server error",
  "details": "Error details (development only)"
}
```

---

## Rate Limiting

- **Requests per minute**: 60 per IP
- **Burst limit**: 100 requests
- **Rate limit headers**:
  - `X-RateLimit-Limit`: 60
  - `X-RateLimit-Remaining`: 45
  - `X-RateLimit-Reset`: 1705319400

---

## SignalR Hub Methods

### Dispatch Hub (`/hubs/dispatch`)

**Client methods** (server pushes to client):
- `DispatchCreated` - New dispatch created
- `DispatchAssigned` - Dispatch assigned to driver
- `DispatchStatusChanged` - Dispatch status updated
- `DriverStatusChanged` - Driver status updated

**Server methods** (client invokes):
- `UpdateDispatchStatus(dispatchId, status)` - Update dispatch status
- `JoinDispatchGroup(groupId)` - Join dispatch group

### Location Hub (`/hubs/location`)

**Client methods**:
- `LocationUpdated` - Ambulance location updated

**Server methods**:
- `BroadcastLocation(latitude, longitude)` - Send current location
- `StartLocationTracking()` - Start tracking
- `StopLocationTracking()` - Stop tracking

---

For more details, visit the interactive documentation at:
- Swagger UI: http://localhost:5000/swagger
- ReDoc: http://localhost:5000/api/redoc

