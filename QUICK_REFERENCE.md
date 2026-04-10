# EMS Dispatcher - Quick Reference Checklist

## Project Setup Checklist

### Prerequisites Installation
- [ ] .NET 8.0 SDK installed
- [ ] Python 3.11+ installed with pip
- [ ] Flutter 3.0+ installed
- [ ] Docker & Docker Compose installed
- [ ] Git configured
- [ ] IDE installed (VS Code / Visual Studio / Android Studio)

### Initial Setup
- [ ] Clone repository
- [ ] Install backend dependencies: `dotnet restore`
- [ ] Install ML dependencies: `pip install -r requirements.txt`
- [ ] Install Flutter dependencies: `flutter pub get` (in ems_dispatch_mobile/)
- [ ] Configure environment variables from .env templates
- [ ] Start Docker Compose: `docker-compose up -d`

### Verification
- [ ] Backend runs: `dotnet run` from EmsDispatch.Backend/
- [ ] ML service runs: `python ml_api.py`
- [ ] Flutter builds: `flutter build web`
- [ ] MongoDB accessible at localhost:27017
- [ ] Can access Swagger at http://localhost:5000/swagger
- [ ] Can access ML docs at http://localhost:8000/docs

---

## Development Workflow

### Daily Development Start
```bash
# 1. Start services
docker-compose up -d

# 2. Verify all containers running
docker-compose ps

# 3. Check logs if issues
docker-compose logs -f

# 4. Backend: dotnet run
# 5. ML: python ml_api.py
# 6. Mobile: flutter run
```

### Backend Development
- [ ] Create model/entity if needed
- [ ] Create/update service for business logic
- [ ] Create/update controller for API endpoint
- [ ] Write unit tests
- [ ] Test with Swagger
- [ ] Check MongoDB data
- [ ] Verify SignalR if real-time feature
- [ ] Run full test suite: `dotnet test`
- [ ] Commit and push

### Mobile Development
- [ ] Update models if needed
- [ ] Update provider/state management
- [ ] Create/update screens
- [ ] Add navigation if new route
- [ ] Hot reload during development: Press 'r'
- [ ] Run tests: `flutter test`
- [ ] Build for Android: `flutter build apk --release`
- [ ] Build for iOS: `flutter build ipa --release`
- [ ] Commit and push

### ML Service Development
- [ ] Update model training if needed: `python train_model.py`
- [ ] Test inference locally
- [ ] Update FastAPI endpoints
- [ ] Test with curl or Postman
- [ ] Verify predictions accuracy
- [ ] Run pytest: `pytest`
- [ ] Rebuild Docker image if changed
- [ ] Commit and push

---

## API Testing

### Authentication
```bash
# Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@ems.local","password":"Test123!"}'

# Use returned token for authenticated requests
# Add header: Authorization: Bearer TOKEN
```

### Common Endpoints to Test
- [ ] POST /api/auth/login
- [ ] GET /api/dispatch
- [ ] POST /api/dispatch
- [ ] PUT /api/dispatch/{id}/status/{status}
- [ ] GET /api/driver/available
- [ ] PUT /api/driver/{id}/location
- [ ] GET /api/ambulance/available
- [ ] GET /api/session/active

### Using Swagger
1. Navigate to http://localhost:5000/swagger
2. Click "Authorize" button
3. Enter "Bearer YOUR_TOKEN"
4. Try endpoints directly from browser

---

## Database Management

### Connect to MongoDB
```bash
# Using MongoDB CLI
mongo -u admin -p password --authenticationDatabase admin

# or MongoDB Express UI
# http://localhost:8081
# Username: admin
# Password: password
```

### Useful MongoDB Commands
```javascript
// Show databases
show dbs

// Use ems_dispatch database
use ems_dispatch

// Show collections
show collections

// Count documents
db.users.countDocuments()

// Find all documents
db.dispatches.find().pretty()

// Find by ID
db.dispatches.findOne({ _id: ObjectId("...") })

// Update document
db.dispatches.updateOne(
  { _id: ObjectId("...") },
  { $set: { status: "Complete" } }
)

// Delete document
db.dispatches.deleteOne({ _id: ObjectId("...") })
```

---

## Debugging Guide

### Backend Debugging
- [ ] Set breakpoint in Visual Studio
- [ ] Press F5 to start debugging
- [ ] API pauses at breakpoint
- [ ] Step through code
- [ ] Check variables in debug console

### Database Debugging
- [ ] Use MongoDB Express UI
- [ ] Query database directly
- [ ] Check indexes: `db.dispatches.getIndexes()`
- [ ] Monitor connections: `db.currentOp()`

### Mobile Debugging
- [ ] Run with verbose: `flutter run -v`
- [ ] Check console logs: `flutter logs`
- [ ] Toggle debug overlay: Press 'p'
- [ ] Open DevTools: `flutter pub global run devtools`

### ML Service Debugging
- [ ] Check logs in terminal
- [ ] Use `logging` module for debug output
- [ ] Test predictions manually: `python predict_hospital.py`
- [ ] Verify model files exist in models/ directory

---

## Common Tasks

### Add New API Endpoint
1. [ ] Create model in Models/ if needed
2. [ ] Add service method in Services/
3. [ ] Add controller method in Controllers/
4. [ ] Add DTO in DTOs/ if needed
5. [ ] Register route in Program.cs
6. [ ] Test with Swagger
7. [ ] Add to API_REFERENCE.md

### Add New Flutter Screen
1. [ ] Create screen file in lib/screens/
2. [ ] Add provider if needed in lib/providers/
3. [ ] Add model in lib/models/
4. [ ] Update navigation in main.dart
5. [ ] Test navigation
6. [ ] Add to documentation

### Fix Production Bug
1. [ ] Create feature branch: `git checkout -b bugfix/issue-name`
2. [ ] Reproduce locally
3. [ ] Write test that fails
4. [ ] Fix the bug
5. [ ] Verify test passes
6. [ ] Commit: `git commit -m "Fix: description"`
7. [ ] Push and create PR

---

## Testing Checklist

### Unit Tests
- [ ] Backend: `dotnet test`
- [ ] Mobile: `flutter test`
- [ ] Python: `pytest`

### Integration Tests
- [ ] Test with real database
- [ ] Test SignalR connections
- [ ] Test API chains (login → create dispatch → update status)

### Manual Testing
- [ ] Test all user roles
- [ ] Test error scenarios
- [ ] Test edge cases
- [ ] Test performance under load
- [ ] Test on real devices (iOS/Android)

---

## Deployment Checklist

### Pre-Deployment
- [ ] All tests passing
- [ ] Code reviewed
- [ ] Documentation updated
- [ ] Version numbers bumped
- [ ] Database migrations ready
- [ ] Environment variables set
- [ ] Backup taken
- [ ] Rollback plan ready

### Deployment
- [ ] Build Docker images
- [ ] Push to registry
- [ ] Deploy to staging
- [ ] Run smoke tests
- [ ] Deploy to production
- [ ] Verify all services up
- [ ] Monitor logs
- [ ] Test critical paths

### Post-Deployment
- [ ] Verify all features working
- [ ] Check error logs
- [ ] Monitor performance
- [ ] Alert if issues
- [ ] Document any issues
- [ ] Notify team

---

## Git Workflow

### Committing Changes
```bash
# Check status
git status

# Stage changes
git add .

# Commit with descriptive message
git commit -m "type: description"

# Types: feat, fix, docs, style, refactor, test, chore
# Example: "feat: add real-time location tracking"

# Push to remote
git push origin branch-name

# Create pull request on GitHub
```

### Branch Naming
- Feature: `feature/description`
- Bugfix: `bugfix/issue-name`
- Hotfix: `hotfix/critical-issue`
- Documentation: `docs/update-type`

---

## Performance Optimization

### Backend Optimization
- [ ] Add database indexes
- [ ] Cache frequently accessed data
- [ ] Use connection pooling
- [ ] Implement pagination
- [ ] Monitor query performance
- [ ] Profile memory usage

### Mobile Optimization
- [ ] Lazy load screens
- [ ] Cache API responses
- [ ] Optimize image sizes
- [ ] Use efficient layouts
- [ ] Monitor memory usage
- [ ] Test on low-end devices

### ML Service Optimization
- [ ] Cache model in memory
- [ ] Batch predictions if possible
- [ ] Monitor inference time
- [ ] Use optimized libraries
- [ ] Consider quantization for models

---

## Security Checklist

### Before Production
- [ ] Remove debug logs
- [ ] Enable HTTPS
- [ ] Set secure CORS
- [ ] Validate all inputs
- [ ] Sanitize database queries
- [ ] Hash passwords with bcrypt
- [ ] Use environment variables for secrets
- [ ] Enable rate limiting
- [ ] Add API authentication
- [ ] Implement audit logging
- [ ] Security testing done
- [ ] Penetration testing completed

---

## Documentation

### Keep Updated
- [ ] README.md with latest info
- [ ] API_REFERENCE.md with new endpoints
- [ ] DEVELOPMENT_GUIDE.md with tips
- [ ] Code comments for complex logic
- [ ] Inline documentation for APIs

### New Features Document
- [ ] Architecture overview
- [ ] API changes
- [ ] Database changes
- [ ] Mobile changes
- [ ] Deployment changes
- [ ] Security implications

---

## Useful Links

### Documentation
- Backend: `./EMS_SETUP.md`
- Mobile: `./ems_dispatch_mobile/FLUTTER_SETUP.md`
- API: `./API_REFERENCE.md`
- Deployment: `./DEPLOYMENT_GUIDE.md`
- Development: `./DEVELOPMENT_GUIDE.md`

### Local Services
- Swagger UI: http://localhost:5000/swagger
- ML Docs: http://localhost:8000/docs
- MongoDB Express: http://localhost:8081
- Backend: http://localhost:5000
- ML Service: http://localhost:8000

### External Resources
- .NET Docs: https://docs.microsoft.com/dotnet
- Flutter Docs: https://flutter.dev/docs
- MongoDB Docs: https://docs.mongodb.com
- FastAPI Docs: https://fastapi.tiangolo.com
- Docker Docs: https://docs.docker.com

---

## Emergency Procedures

### Service Down
1. Check Docker containers: `docker-compose ps`
2. Check logs: `docker-compose logs -f SERVICE_NAME`
3. Restart service: `docker-compose restart SERVICE_NAME`
4. If persistent: `docker-compose down && docker-compose up -d`

### Database Connection Lost
1. Verify MongoDB running: `docker ps | grep mongo`
2. Check connection string in appsettings.json
3. Verify network: `docker network inspect ems_network`
4. Restart MongoDB: `docker-compose restart mongodb`

### API Not Responding
1. Check if running: `curl http://localhost:5000/health`
2. Check logs: `dotnet run` (if running locally)
3. Check firewall: Verify port 5000 open
4. Restart: `docker-compose restart backend`

### SignalR Connection Issues
1. Verify WebSocket support enabled
2. Check firewall for port 5000
3. Verify SignalR hub routes in Program.cs
4. Check CORS configuration
5. Restart backend service

---

## Contact & Support

- **Backend Issues**: Check DEVELOPMENT_GUIDE.md
- **Mobile Issues**: Check FLUTTER_SETUP.md
- **Deployment Issues**: Check DEPLOYMENT_GUIDE.md
- **General Questions**: See README.md
- **API Help**: Visit http://localhost:5000/swagger

---

**Last Updated**: January 2024
**Version**: 1.0.0
