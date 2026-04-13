# Installation Checklist for EMS Dispatcher System

## Pre-Installation Check

- [ ] System has 50+ GB free disk space
- [ ] Internet connection available
- [ ] Administrator access to computer
- [ ] Note of what OS you're using (Windows/Mac/Linux)

---

## Step 1: Install Git (5 minutes)

- [ ] Download Git from https://git-scm.com/
- [ ] Run installer with default settings
- [ ] Verify: Open Command Prompt and run `git --version`

---

## Step 2: Install Docker (15 minutes)

- [ ] Download Docker Desktop from https://www.docker.com/products/docker-desktop
- [ ] Run installer
- [ ] Follow setup instructions
- [ ] **Sign in with Docker account or create one**
- [ ] Verify: Open Command Prompt and run `docker --version`
- [ ] Verify: Open Command Prompt and run `docker-compose --version`

---

## Step 3: Install .NET 8 SDK (10 minutes)

- [ ] Go to https://dotnet.microsoft.com/download
- [ ] Download **.NET 8.0 SDK** (NOT Runtime)
- [ ] Run installer
- [ ] Verify: Open Command Prompt and run `dotnet --version`

---

## Step 4: Install MongoDB (Optional if using Docker)

- [ ] If using Docker: Skip this, MongoDB runs in container
- [ ] If manual setup: Download from https://www.mongodb.com/try/download/community
- [ ] Install with default settings
- [ ] Verify: MongoDB should run as service

---

## Step 5: Install Python 3.11 (Important!)

- [ ] Go to https://www.python.org/downloads/
- [ ] Download **Python 3.11.x** (latest 3.11, NOT 3.12)
- [ ] Run installer
- [ ] **IMPORTANT**: Check ✓ "Add Python to PATH"
- [ ] **IMPORTANT**: Click "Disable path length limit" at end
- [ ] Verify: Open new Command Prompt and run `python --version`
- [ ] Verify: Run `pip --version`

---

## Step 6: Install Flutter SDK (Optional for mobile development)

- [ ] Go to https://flutter.dev/docs/get-started/install
- [ ] Download Flutter SDK for Windows
- [ ] Extract to `C:\src\flutter` (or your preferred location)
- [ ] Add to PATH: `C:\src\flutter\bin`
- [ ] Verify: Open new Command Prompt and run `flutter --version`

---

## Step 7: Install Android Studio (Optional for mobile development)

- [ ] Go to https://developer.android.com/studio
- [ ] Download Android Studio
- [ ] Run installer (leave defaults)
- [ ] Complete initial setup
- [ ] Install Android SDK (will prompt automatically)
- [ ] Verify: Run `flutter doctor` in Command Prompt

---

## Step 8: Install VS Code (Recommended for development)

- [ ] Go to https://code.visualstudio.com/
- [ ] Download for Windows
- [ ] Run installer (leave defaults)
- [ ] Open VS Code
- [ ] Install extensions:
  - [ ] C# (C# Dev Kit)
  - [ ] Dart
  - [ ] Flutter
  - [ ] MongoDB for VS Code
  - [ ] REST Client
  - [ ] Postman

---

## Step 9: Clone or Extract Project

- [ ] Open Command Prompt in desired location
- [ ] Run: `git clone https://github.com/Senryuugg/EMS-Backend.git`
- [ ] Or extract ZIP file if provided
- [ ] Verify: Project folder exists with all files

---

## Step 10: Setup Python Virtual Environment

- [ ] Open Command Prompt
- [ ] Navigate to project: `cd path\to\EMS-Backend`
- [ ] Create venv: `python -m venv venv`
- [ ] Activate venv: `venv\Scripts\activate`
- [ ] Verify: Prompt shows `(venv)` prefix

---

## Step 11: Install Python Requirements

- [ ] Ensure venv is activated (shows `(venv)`)
- [ ] Run: `python -m pip install --upgrade pip`
- [ ] Run: `pip install -r requirements.txt`
- [ ] Wait 3-5 minutes for completion
- [ ] Verify: `pip list` shows all packages

**If pandas/numpy error occurs:**
- [ ] See WINDOWS_INSTALLATION_FIX.md for solutions
- [ ] Most likely: Use Python 3.11 instead of 3.12

---

## Step 12: Setup .NET Backend

- [ ] Open Command Prompt
- [ ] Navigate to: `cd path\to\EMS-Backend\EmsDispatch.Backend`
- [ ] Run: `dotnet restore`
- [ ] Run: `dotnet build`
- [ ] Verify: "Build succeeded" message

---

## Step 13: Setup Flutter App (Optional)

- [ ] Open Command Prompt
- [ ] Navigate to: `cd path\to\EMS-Backend\ems_dispatch_mobile`
- [ ] Run: `flutter pub get`
- [ ] Run: `flutter doctor` to check setup
- [ ] Fix any issues shown (usually Android SDK related)

---

## Step 14: Start Services with Docker

- [ ] Open Command Prompt in project root
- [ ] Run: `docker-compose up -d`
- [ ] Wait 1-2 minutes for containers to start
- [ ] Verify services:
  - [ ] MongoDB: `http://localhost:27017` (should not give error in browser)
  - [ ] MongoDB Express: `http://localhost:8081` (login with admin/password)

---

## Step 15: Test Backend API

- [ ] Open new Command Prompt in project root
- [ ] Run: `cd EmsDispatch.Backend && dotnet run`
- [ ] Wait for message: "Now listening on..."
- [ ] Open browser: `http://localhost:5000/swagger`
- [ ] Verify: Swagger UI loads with all endpoints

---

## Step 16: Test ML API

- [ ] Open new Command Prompt in project root
- [ ] Activate venv: `venv\Scripts\activate`
- [ ] Run: `python ml_api.py`
- [ ] Open browser: `http://localhost:8000/docs`
- [ ] Verify: Swagger UI loads with ML endpoints

---

## Step 17: Test Flutter App (Optional)

- [ ] Connect Android phone via USB or start Android emulator
- [ ] Open Command Prompt in `ems_dispatch_mobile` folder
- [ ] Run: `flutter run`
- [ ] Wait 2-3 minutes for build
- [ ] App should open on device/emulator

---

## Post-Installation Verification

Run these commands to verify everything:

```bash
# Check all versions
git --version
docker --version
dotnet --version
python --version
flutter --version  # If installed

# Check services are running
docker ps  # Should show 3 containers: mongodb, mongo-express, backend (optional: ml-service)

# Check ports are accessible
# Browser tests:
# - http://localhost:5000/swagger (Backend)
# - http://localhost:8000/docs (ML API)
# - http://localhost:8081 (MongoDB Express)
```

---

## Troubleshooting Quick Links

| Issue | Solution |
|-------|----------|
| Python pandas error on Windows | See: WINDOWS_INSTALLATION_FIX.md |
| Docker fails to start | Restart Docker Desktop |
| Port already in use | Check TROUBLESHOOTING.md |
| API won't connect | Check DEVELOPMENT_GUIDE.md |
| MongoDB connection error | Restart Docker: `docker-compose restart mongodb` |

---

## Time Estimates

| Step | Time |
|------|------|
| Git | 5 min |
| Docker | 15 min |
| .NET 8 | 10 min |
| Python 3.11 | 5 min |
| Flutter (optional) | 20 min |
| Android Studio (optional) | 30 min |
| Project Setup | 10 min |
| Python Requirements | 5 min |
| .NET Setup | 5 min |
| Docker Services | 2 min |
| Testing | 5 min |
| **TOTAL** | **~80-90 minutes** |

---

## What You'll Have After Installation

✓ Git version control system  
✓ Docker containerization  
✓ .NET 8.0 backend runtime  
✓ Python 3.11 ML environment  
✓ Flutter mobile development kit (optional)  
✓ Running MongoDB database  
✓ Running Backend API at :5000  
✓ Running ML Service at :8000  
✓ Running MongoDB Express at :8081  

---

## Next: Start Development

Once everything is installed and verified:

1. Read: `DEVELOPMENT_GUIDE.md` for coding standards
2. Read: `API_REFERENCE.md` for endpoint documentation
3. Start: `docker-compose up -d` to run all services
4. Test: Visit Swagger UIs to explore APIs
5. Build: Start implementing features

---

## Need Help?

1. Check the error message carefully
2. Search in relevant guide file
3. See troubleshooting section
4. Check GitHub issues
5. Contact development team

**Remember**: Installation takes time on first run. Be patient, especially with Docker and pip install!
