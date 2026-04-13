# START HERE - EMS Dispatcher Installation & Setup

## You've Hit an Issue? Here's Your Quick Answer

### The Error You're Seeing:
```
ERROR: Failed to build 'pandas' when installing build dependencies for pandas
```

### The Problem:
- You're on **Python 3.12**
- Windows doesn't have a C compiler installed
- pandas/numpy are trying to compile from source

### The Solution (Pick One):

#### ✅ **Solution 1: Use Python 3.11 (EASIEST - Do This First)**
```bash
# 1. Uninstall Python 3.12
# 2. Download & Install Python 3.11 from https://python.org
# 3. Make sure to CHECK "Add Python to PATH"
# 4. Open NEW Command Prompt
# 5. Run these:
python -m venv venv
venv\Scripts\activate
python -m pip install --upgrade pip
pip install -r requirements.txt
```
**Takes: 15 minutes**  
**Success Rate: 99%**

---

#### ⚙️ **Solution 2: Install Visual Studio Build Tools**
```bash
# If you want to stay on Python 3.12:
# 1. Go to https://visualstudio.microsoft.com/downloads/
# 2. Download "Build Tools for Visual Studio 2022"
# 3. Install and select "Desktop development with C++"
# 4. Then:
pip cache purge
pip install -r requirements.txt
```
**Takes: 25 minutes**  
**Success Rate: 85%**

---

## Installation Steps (Choose Your Path)

### Path A: Fresh Installation (Recommended)
1. Install Git → https://git-scm.com/
2. Install Docker → https://docker.com/
3. Install .NET 8 → https://dotnet.microsoft.com/download
4. **Install Python 3.11** → https://python.org/ (NOT 3.12!)
5. Clone project: `git clone https://github.com/Senryuugg/EMS-Backend.git`
6. Setup Python:
   ```bash
   cd EMS-Backend
   python -m venv venv
   venv\Scripts\activate
   python -m pip install --upgrade pip
   pip install -r requirements.txt
   ```
7. Setup .NET:
   ```bash
   cd EmsDispatch.Backend
   dotnet restore
   dotnet build
   ```
8. Start services:
   ```bash
   docker-compose up -d
   ```

**Total Time: 60-90 minutes**

---

### Path B: You Already Have Python 3.12
1. Remove Python 3.12
2. Install Python 3.11 from https://python.org
3. Open a NEW Command Prompt (important!)
4. Follow the 6 bash commands from Path A, Step 6

**Total Time: 30 minutes**

---

## Verification: Are You Set Up Correctly?

Run these commands to check:

```bash
# 1. Python version (should be 3.11.x)
python --version

# 2. All packages installed
pip list

# 3. All packages work
python -c "import pandas, numpy, fastapi; print('✓ Success')"

# 4. Backend API starts
cd EmsDispatch.Backend
dotnet run
# Should say: "Now listening on http://localhost:5000"
# Press Ctrl+C to stop

# 5. ML API starts (in new Command Prompt)
python ml_api.py
# Should say: "Uvicorn running on http://0.0.0.0:8000"
# Press Ctrl+C to stop

# 6. Docker running
docker ps
# Should show containers running
```

---

## If Installation Still Fails

| Error | Step 1 | Step 2 |
|-------|--------|--------|
| "Failed to build 'pandas'" | Use Python 3.11 | Or install Build Tools |
| "Module not found" | Activate venv: `venv\Scripts\activate` | Run `pip install -r requirements.txt` again |
| "Command not found: python" | Reinstall Python 3.11 | Check PATH is set (restart terminal) |
| "Docker error" | Restart Docker Desktop | Run `docker-compose restart` |
| Port 5000 already in use | Check what's running: `netstat -ano` | Use different port in appsettings.json |

**See full troubleshooting**: `WINDOWS_INSTALLATION_FIX.md`

---

## What Gets Installed

When you run `pip install -r requirements.txt`, you get:

| Package | Purpose |
|---------|---------|
| **fastapi** | Python web framework for ML API |
| **uvicorn** | ASGI server to run FastAPI |
| **pandas** | Data manipulation (hospital data) |
| **numpy** | Numerical computing |
| **scikit-learn** | Machine learning models |
| **pydantic** | Data validation |
| **requests** | HTTP calls to backend |

**Total**: ~8 packages, ~100 MB

---

## Full Installation List (All Components)

### Required:
- [ ] Git (version control)
- [ ] Docker Desktop (containers - runs MongoDB)
- [ ] .NET 8.0 SDK (backend API)
- [ ] **Python 3.11** (ML service & data processing)

### Optional but Recommended:
- [ ] VS Code (code editor)
- [ ] Postman (API testing)
- [ ] MongoDB Compass (database GUI)
- [ ] Flutter SDK (mobile app)
- [ ] Android Studio (mobile testing)

### Total Install Time:
- **Minimum**: 60-90 minutes (just required)
- **With optional**: 2-3 hours

---

## Starting Your First Development Session

Once installed:

```bash
# Terminal 1: Start Docker services
docker-compose up -d

# Terminal 2: Start Backend API
cd EmsDispatch.Backend
dotnet run
# Visit: http://localhost:5000/swagger

# Terminal 3: Start ML API
python ml_api.py
# Visit: http://localhost:8000/docs

# Terminal 4: Optional - Start Flutter
cd ems_dispatch_mobile
flutter run
```

---

## Quick Reference

### MongoDB Admin
- URL: http://localhost:8081
- Username: admin
- Password: password

### Backend API
- URL: http://localhost:5000
- Swagger: http://localhost:5000/swagger

### ML API
- URL: http://localhost:8000
- Docs: http://localhost:8000/docs

### Database
- Connection: `mongodb://localhost:27017`
- Database: `ems_dispatch`

---

## Documentation Map

Start with these in order:

1. **This file** ← You are here
2. **INSTALLATION_CHECKLIST.md** - Step-by-step checklist
3. **WINDOWS_INSTALLATION_FIX.md** - If you have pandas error
4. **DEVELOPMENT_GUIDE.md** - Start coding
5. **API_REFERENCE.md** - API endpoints
6. **DEPLOYMENT_GUIDE.md** - Going to production

---

## Common Questions

**Q: Should I use Python 3.11 or 3.12?**  
A: Use Python 3.11. It has better pre-built wheels for Windows.

**Q: Do I need to install MongoDB separately?**  
A: No, it runs in Docker. Just do `docker-compose up -d`.

**Q: Can I use Python 3.10?**  
A: Technically yes, but 3.11 is tested and recommended.

**Q: What if I'm on Mac or Linux?**  
A: See `INSTALLATION_REQUIREMENTS.md` for Mac/Linux specific steps.

**Q: Can I skip Flutter/Android Studio?**  
A: Yes, you only need them if building the mobile app.

---

## Still Stuck?

1. **Read**: Check the error message carefully
2. **Search**: Look in WINDOWS_INSTALLATION_FIX.md
3. **Check**: Run verification commands above
4. **Retry**: Follow Solution 1 step-by-step again
5. **Help**: Contact team with full error output

---

## Next Step

👉 **Go to**: `INSTALLATION_CHECKLIST.md` for detailed step-by-step guide
