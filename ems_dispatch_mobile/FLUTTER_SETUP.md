# EMS Dispatcher Mobile App - Flutter Project Structure

## Project Overview

Flutter cross-platform mobile application (Android & iOS) for EMS dispatch management. The app connects to the ASP.NET Core backend via HTTP/WebSocket and supports role-based dashboards.

## Directory Structure

```
ems_dispatch_mobile/
├── lib/
│   ├── main.dart                         # App entry point
│   ├── config/
│   │   └── api_config.dart              # API configuration and endpoints
│   ├── models/
│   │   ├── user_model.dart              # User data model
│   │   └── dispatch_model.dart          # Dispatch and related models
│   ├── providers/
│   │   ├── auth_provider.dart           # Authentication provider (state management)
│   │   ├── dispatch_provider.dart       # Dispatch management provider
│   │   └── location_provider.dart       # Location tracking provider
│   ├── screens/
│   │   ├── splash_screen.dart           # App initialization screen
│   │   ├── login_screen.dart            # User login interface
│   │   ├── dispatcher_dashboard.dart    # Dispatcher interface
│   │   ├── driver_dashboard.dart        # Driver interface
│   │   └── admin_dashboard.dart         # Admin interface
│   └── services/
│       ├── api_service.dart             # HTTP client wrapper (to be created)
│       ├── signalr_service.dart         # SignalR connection manager (to be created)
│       └── location_service.dart        # Location tracking service (to be created)
├── assets/
│   ├── images/                          # App images
│   ├── icons/                           # Custom icons
│   └── fonts/                           # Custom fonts
├── android/                             # Android native code
├── ios/                                 # iOS native code
├── test/                                # Unit and widget tests
├── .env                                 # Environment variables
├── pubspec.yaml                         # Flutter dependencies
└── README.md                            # Flutter app documentation
```

## Getting Started

### Prerequisites

- Flutter SDK (latest version)
- Android Studio or Xcode
- Android device/emulator or iOS simulator

### Installation

1. **Create Flutter project structure**:
   ```bash
   cd ems_dispatch_mobile
   ```

2. **Install dependencies**:
   ```bash
   flutter pub get
   ```

3. **Configure Android**:
   - Update `android/app/build.gradle`:
     ```gradle
     android {
         ...
         defaultConfig {
             minSdkVersion 21
             targetSdkVersion 34
         }
     }
     ```
   - Add Google Maps key to `android/app/src/main/AndroidManifest.xml`

4. **Configure iOS**:
   - Update `ios/Podfile` for minimum deployment target:
     ```ruby
     post_install do |installer|
       installer.pods_project.targets.each do |target|
         target.build_configurations.each do |config|
           config.build_settings['GCC_PREPROCESSOR_DEFINITIONS'] ||= [
             '$(inherited)',
             'PERMISSION_LOCATION=1',
           ]
         end
       end
     end
     ```

5. **Configure Google Maps API**:
   - Get API key from Google Cloud Console
   - Add to Android manifest and iOS Info.plist

6. **Configure location permissions**:
   - Android: `android/app/src/main/AndroidManifest.xml`
     ```xml
     <uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
     <uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
     ```
   - iOS: `ios/Runner/Info.plist`
     ```xml
     <key>NSLocationWhenInUseUsageDescription</key>
     <string>This app needs access to your location for emergency response.</string>
     <key>NSLocationAlwaysAndWhenInUseUsageDescription</key>
     <string>This app needs access to your location for emergency response.</string>
     ```

### Running the App

1. **For Android**:
   ```bash
   flutter run -d android
   ```

2. **For iOS**:
   ```bash
   flutter run -d ios
   ```

3. **With specific device**:
   ```bash
   flutter devices                    # List available devices
   flutter run -d <device_id>         # Run on specific device
   ```

## Core Features Implemented

### Authentication
- Login/Register screens
- JWT token management
- Secure token storage
- Role-based navigation
- Automatic token refresh

### Dispatch Management (Dispatcher)
- View all dispatches
- Create new dispatch
- Assign drivers/ambulances
- Update dispatch status
- Real-time status updates

### Driver Interface
- View assigned dispatch
- Update dispatch status
- Location tracking
- Shift management

### Admin Interface
- System overview statistics
- User management
- Hospital management
- Ambulance management

## Features to Implement

### Priority 1 (Core)
- [ ] Maps integration with live ambulance tracking
- [ ] Real-time location streaming to backend
- [ ] SignalR connection for real-time updates
- [ ] Hospital selection UI with ML predictions
- [ ] Route visualization on map

### Priority 2 (Enhanced)
- [ ] Offline mode with local database (sqflite)
- [ ] Push notifications for new dispatches
- [ ] Call history and statistics
- [ ] Driver performance metrics
- [ ] Hospital occupancy display

### Priority 3 (Advanced)
- [ ] Voice communication integration
- [ ] Live video streaming
- [ ] Advanced analytics dashboard
- [ ] Document upload (patient records)
- [ ] Multi-language support

## API Integration

### Authentication Endpoints
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `GET /api/auth/me` - Get current user
- `POST /api/auth/logout` - Logout

### Dispatch Endpoints
- `GET /api/dispatch` - Get all dispatches
- `GET /api/dispatch/{id}` - Get dispatch by ID
- `POST /api/dispatch` - Create dispatch
- `PUT /api/dispatch/{id}/status/{status}` - Update status
- `PUT /api/dispatch/{id}/assign` - Assign to driver

### Driver Endpoints
- `GET /api/driver/{id}` - Get driver info
- `PUT /api/driver/{id}/location` - Update location
- `PUT /api/driver/{id}/status/{status}` - Update status

### ML Service Endpoints
- `POST /predict/hospital` - Get hospital prediction
- `POST /optimize/route` - Get optimized route

## SignalR Integration

### DispatchHub
- Subscribe to new dispatch notifications
- Get dispatch assignment events
- Receive status update broadcasts

### LocationHub
- Stream real-time location updates
- Receive ambulance location updates
- Get tracking data

## State Management with Provider

### AuthProvider
- Manages user authentication state
- Handles token storage and refresh
- Manages login/logout

### DispatchProvider
- Manages dispatch list state
- Handles dispatch CRUD operations
- Updates dispatch status

### LocationProvider
- Manages current location state
- Handles location tracking stream
- Updates location on server

## Testing

```bash
# Run all tests
flutter test

# Run specific test file
flutter test test/providers/auth_provider_test.dart

# Run with coverage
flutter test --coverage
```

## Building for Release

### Android
```bash
flutter build apk --release
# or for app bundle
flutter build appbundle --release
```

### iOS
```bash
flutter build ios --release
```

## Performance Optimization

1. **Location Updates**: Throttled to 10-second intervals
2. **Lazy Loading**: Dispatches loaded on-demand
3. **Image Caching**: Maps tiles cached locally
4. **Data Serialization**: JSON serialization for network efficiency

## Security Best Practices

1. JWT tokens stored in secure storage
2. HTTPS only for production
3. Sensitive data not logged
4. Permission requests at runtime
5. Input validation on all forms

## Troubleshooting

### Common Issues

**App won't connect to backend**
- Check API_BASE_URL in .env
- Verify backend is running
- Check Android emulator can reach host (10.0.2.2)

**Location permission denied**
- Request permission at runtime
- Check manifest/Info.plist
- Device location services enabled

**Maps not showing**
- Add Google Maps API key
- Check API key restrictions
- Platform-specific configuration

## Resources

- Flutter Documentation: https://flutter.dev/docs
- Provider Package: https://pub.dev/packages/provider
- Google Maps Flutter: https://pub.dev/packages/google_maps_flutter
- SignalR Client: https://pub.dev/packages/signalr_client
- Geolocator: https://pub.dev/packages/geolocator
