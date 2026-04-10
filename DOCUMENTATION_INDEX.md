# EMS Dispatcher - Documentation Index

## Quick Start
- Start here: [README.md](./README.md)
- Setup backend: [EMS_SETUP.md](./EmsDispatch.Backend/appsettings.json)
- Setup mobile: [FLUTTER_SETUP.md](./ems_dispatch_mobile/FLUTTER_SETUP.md)
- Quick commands: [QUICK_REFERENCE.md](./QUICK_REFERENCE.md)

---

## Documentation by Role

### For Project Managers
1. [README.md](./README.md) - Project overview and features
2. [PROJECT_COMPLETION.md](./PROJECT_COMPLETION.md) - What's been built
3. [SYSTEM_SUMMARY.md](./SYSTEM_SUMMARY.md) - Architecture overview
4. [DEPLOYMENT_GUIDE.md](./DEPLOYMENT_GUIDE.md) - Deployment status

### For Backend Developers
1. [EMS_SETUP.md](./EmsDispatch.Backend/appsettings.json) - Backend setup
2. [API_REFERENCE.md](./API_REFERENCE.md) - API endpoints
3. [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md) - Development workflow
4. [QUICK_REFERENCE.md](./QUICK_REFERENCE.md) - Common tasks

### For Mobile Developers
1. [FLUTTER_SETUP.md](./ems_dispatch_mobile/FLUTTER_SETUP.md) - Mobile setup
2. [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md) - Testing & debugging
3. [QUICK_REFERENCE.md](./QUICK_REFERENCE.md) - Common tasks

### For ML Engineers
1. [EMS_SETUP.md](./EmsDispatch.Backend/appsettings.json) - ML service setup
2. [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md) - Python debugging
3. [API_REFERENCE.md](./API_REFERENCE.md) - ML endpoints

### For DevOps Engineers
1. [DEPLOYMENT_GUIDE.md](./DEPLOYMENT_GUIDE.md) - Production deployment
2. [docker-compose.yml](./docker-compose.yml) - Local orchestration
3. [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md) - Local development
4. [QUICK_REFERENCE.md](./QUICK_REFERENCE.md) - Emergency procedures

---

## Documentation by Topic

### Architecture & Design
- [SYSTEM_SUMMARY.md](./SYSTEM_SUMMARY.md) - Complete system architecture
- [README.md](./README.md#-system-architecture) - Architecture diagram
- [API_REFERENCE.md](./API_REFERENCE.md) - API design patterns

### Setup & Installation
- [README.md](./README.md#-quick-start) - Quick start guide
- [EMS_SETUP.md](./EMS_SETUP.md) - Detailed backend setup
- [FLUTTER_SETUP.md](./ems_dispatch_mobile/FLUTTER_SETUP.md) - Mobile app setup
- [DEPLOYMENT_GUIDE.md](./DEPLOYMENT_GUIDE.md) - Production setup

### Development
- [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md) - Complete dev guide
- [API_REFERENCE.md](./API_REFERENCE.md) - API documentation
- [QUICK_REFERENCE.md](./QUICK_REFERENCE.md) - Quick reference
- [v0_plans/pragmatic-implementation.md](./v0_plans/pragmatic-implementation.md) - Implementation plan

### Deployment
- [DEPLOYMENT_GUIDE.md](./DEPLOYMENT_GUIDE.md) - Deployment guide
- [docker-compose.yml](./docker-compose.yml) - Local deployment
- [EmsDispatch.Backend/Dockerfile](./EmsDispatch.Backend/Dockerfile) - Backend container
- [Dockerfile.ml](./Dockerfile.ml) - ML service container

### API Reference
- [API_REFERENCE.md](./API_REFERENCE.md) - Complete API endpoints
- [Swagger UI](http://localhost:5000/swagger) - Interactive API docs
- [ML Docs](http://localhost:8000/docs) - ML service endpoints

### Troubleshooting
- [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md#debugging) - Debugging guide
- [QUICK_REFERENCE.md](./QUICK_REFERENCE.md#emergency-procedures) - Emergency procedures
- [README.md](./README.md#-troubleshooting) - Common issues

---

## File Structure

```
EMS-Backend/
├── README.md                              # Main project overview
├── QUICK_REFERENCE.md                     # Developer quick reference
├── PROJECT_COMPLETION.md                  # What's been built
├── SYSTEM_SUMMARY.md                      # Architecture overview
├── EMS_SETUP.md                          # Backend setup guide
├── API_REFERENCE.md                       # API documentation
├── DEVELOPMENT_GUIDE.md                   # Development workflow
├── DEPLOYMENT_GUIDE.md                    # Production deployment
├── docker-compose.yml                     # Local orchestration
├── Dockerfile.ml                          # ML service image
├── requirements.txt                       # Python dependencies
│
├── EmsDispatch.Backend/                   # ASP.NET Core API
│   ├── Program.cs                         # App configuration
│   ├── appsettings.json                   # Settings
│   ├── EmsDispatch.Backend.csproj         # Project file
│   ├── Dockerfile                         # Backend image
│   ├── Models/                            # Data models
│   ├── Services/                          # Business logic
│   ├── Controllers/                       # API endpoints
│   ├── Hubs/                              # SignalR real-time
│   ├── DTOs/                              # Data transfer objects
│   └── Configuration/                     # App configuration
│
├── ems_dispatch_mobile/                   # Flutter app
│   ├── pubspec.yaml                       # Dependencies
│   ├── FLUTTER_SETUP.md                   # Mobile setup
│   ├── .env                               # Configuration
│   └── lib/
│       ├── main.dart                      # Entry point
│       ├── config/                        # Configuration
│       ├── models/                        # Data models
│       ├── providers/                     # State management
│       ├── screens/                       # UI screens
│       ├── services/                      # API/SignalR
│       └── widgets/                       # Reusable widgets
│
├── ml_api.py                              # FastAPI service
├── ml_service.py                          # ML logic
├── models/                                # Trained models
├── datasets/                              # Training data
└── v0_plans/
    └── pragmatic-implementation.md        # Implementation plan
```

---

## How to Navigate This Documentation

### If you want to...

**Get started quickly**
1. Read [README.md](./README.md) - 5 minutes
2. Follow "Quick Start" section
3. Access [QUICK_REFERENCE.md](./QUICK_REFERENCE.md) - keep open while developing

**Understand the system**
1. Read [SYSTEM_SUMMARY.md](./SYSTEM_SUMMARY.md) - architecture overview
2. Check [README.md#-system-architecture](./README.md#-system-architecture) - visual diagram
3. Review [PROJECT_COMPLETION.md](./PROJECT_COMPLETION.md) - what's built

**Set up locally**
1. [EMS_SETUP.md](./EmsDispatch.Backend/appsettings.json) - Backend
2. [FLUTTER_SETUP.md](./ems_dispatch_mobile/FLUTTER_SETUP.md) - Mobile
3. [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md) - Full workflow

**Build a new feature**
1. Check [API_REFERENCE.md](./API_REFERENCE.md) - existing endpoints
2. Follow [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md#testing-strategy)
3. Refer to [QUICK_REFERENCE.md](./QUICK_REFERENCE.md#add-new-api-endpoint)

**Deploy to production**
1. Read [DEPLOYMENT_GUIDE.md](./DEPLOYMENT_GUIDE.md) - complete guide
2. Follow deployment checklist in [QUICK_REFERENCE.md](./QUICK_REFERENCE.md#deployment-checklist)
3. Verify in [DEPLOYMENT_GUIDE.md#post-deployment-checklist](./DEPLOYMENT_GUIDE.md#post-deployment-checklist)

**Fix a bug**
1. Check [DEVELOPMENT_GUIDE.md#debugging](./DEVELOPMENT_GUIDE.md#debugging)
2. See [QUICK_REFERENCE.md#common-tasks](./QUICK_REFERENCE.md#common-tasks)
3. Follow [QUICK_REFERENCE.md#git-workflow](./QUICK_REFERENCE.md#git-workflow)

**Understand an API**
1. Visit [API_REFERENCE.md](./API_REFERENCE.md) - complete reference
2. Try in Swagger: http://localhost:5000/swagger
3. Check examples in [DEVELOPMENT_GUIDE.md#api-testing](./DEVELOPMENT_GUIDE.md#api-testing)

**Handle an emergency**
1. Check [QUICK_REFERENCE.md#emergency-procedures](./QUICK_REFERENCE.md#emergency-procedures)
2. Look up service in [DEPLOYMENT_GUIDE.md#troubleshooting](./DEPLOYMENT_GUIDE.md#troubleshooting)
3. Monitor logs and health checks

---

## Key Files Overview

### Configuration Files
- `appsettings.json` - Backend configuration
- `appsettings.Development.json` - Dev-specific settings
- `.env` - Mobile app configuration
- `docker-compose.yml` - Multi-service orchestration
- `pubspec.yaml` - Flutter dependencies

### Documentation Files
| File | Purpose | Audience | Read Time |
|------|---------|----------|-----------|
| README.md | Project overview | Everyone | 10 min |
| PROJECT_COMPLETION.md | What's built | PMs, Leads | 15 min |
| SYSTEM_SUMMARY.md | Architecture | Developers | 15 min |
| API_REFERENCE.md | API docs | Developers | 20 min |
| DEVELOPMENT_GUIDE.md | Dev workflow | Developers | 20 min |
| DEPLOYMENT_GUIDE.md | Production | DevOps | 20 min |
| EMS_SETUP.md | Backend setup | Backend devs | 15 min |
| FLUTTER_SETUP.md | Mobile setup | Mobile devs | 15 min |
| QUICK_REFERENCE.md | Checklists | Everyone | 5 min |

### Source Code Structure

**Backend** (C#/.NET)
- Models: Domain entities
- Services: Business logic
- Controllers: REST endpoints
- Hubs: SignalR real-time
- DTOs: Data contracts

**Frontend** (Flutter/Dart)
- Screens: UI pages
- Providers: State management
- Services: API/SignalR clients
- Models: Data structures
- Config: Environment setup

**ML Service** (Python)
- ml_api.py: FastAPI server
- ml_service.py: Model logic
- models/: Trained models
- datasets/: Training data

---

## External Resources

### Official Documentation
- [.NET 8.0 Documentation](https://docs.microsoft.com/dotnet)
- [Flutter Documentation](https://flutter.dev/docs)
- [MongoDB Documentation](https://docs.mongodb.com)
- [FastAPI Documentation](https://fastapi.tiangolo.com)
- [Docker Documentation](https://docs.docker.com)
- [SignalR Documentation](https://learn.microsoft.com/aspnet/core/signalr)

### Tools & Services
- **Swagger UI**: http://localhost:5000/swagger
- **ML API Docs**: http://localhost:8000/docs
- **MongoDB Express**: http://localhost:8081
- **Docker Hub**: https://hub.docker.com
- **GitHub**: https://github.com/Senryuugg/EMS-Backend

---

## Versioning & Updates

**Current Version**: 1.0.0
**Last Updated**: January 2024
**Status**: Production Ready

### Documentation Updates
- API changes → Update [API_REFERENCE.md](./API_REFERENCE.md)
- Architecture changes → Update [SYSTEM_SUMMARY.md](./SYSTEM_SUMMARY.md)
- Setup changes → Update relevant setup guide
- New features → Update [README.md](./README.md)

---

## Getting Help

### For Questions About...

**Backend/API Development**
- Read: [API_REFERENCE.md](./API_REFERENCE.md)
- Try: http://localhost:5000/swagger
- Guide: [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md)

**Mobile App Development**
- Read: [FLUTTER_SETUP.md](./ems_dispatch_mobile/FLUTTER_SETUP.md)
- Guide: [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md#flutter-development)
- Quick: [QUICK_REFERENCE.md](./QUICK_REFERENCE.md)

**ML Service**
- Read: [EMS_SETUP.md](./EMS_SETUP.md)
- Try: http://localhost:8000/docs
- Guide: [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md#python-ml-service-development)

**Deployment**
- Read: [DEPLOYMENT_GUIDE.md](./DEPLOYMENT_GUIDE.md)
- Checklist: [QUICK_REFERENCE.md](./QUICK_REFERENCE.md#deployment-checklist)

**Troubleshooting**
- Guide: [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md#debugging)
- Emergency: [QUICK_REFERENCE.md](./QUICK_REFERENCE.md#emergency-procedures)
- Issues: [README.md](./README.md#-troubleshooting)

---

**Navigation Complete!** 
Choose a document above or start with [README.md](./README.md).
