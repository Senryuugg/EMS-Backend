# EMS Dispatcher System - Complete Installation Requirements

This document lists everything you need to install and configure to run the entire EMS Dispatcher system.

## 1. System Requirements

### Minimum Hardware
- **CPU**: 4 cores
- **RAM**: 8 GB minimum (16 GB recommended)
- **Storage**: 50 GB free space
- **Network**: Stable internet connection

### Supported Operating Systems
- Windows 10/11 (Pro or higher)
- macOS 11+ (Intel or Apple Silicon)
- Ubuntu 20.04 LTS or newer
- Any Linux distribution with Docker support

---

## 2. Core Prerequisites

### 2.1 Git
**Purpose**: Version control and repository management

**Installation**:
```bash
# Windows (using Chocolatey)
choco install git

# macOS (using Homebrew)
brew install git

# Ubuntu/Debian
sudo apt-get install git

# Verify installation
git --version
```

**Download**: https://git-scm.com/download

---

### 2.2 Docker & Docker Compose
**Purpose**: Container management for running all services

**Installation**:

**Windows**:
1. Download Docker Desktop: https://www.docker.com/products/docker-desktop
2. Run installer and follow setup
3. Restart computer after installation

**macOS**:
```bash
brew install docker docker-compose
# Or download Docker Desktop from: https://www.docker.com/products/docker-desktop
```

**Ubuntu/Linux**:
```bash
sudo apt-get update
sudo apt-get install docker.io docker-compose

# Add user to docker group
sudo usermod -aG docker $USER
newgrp docker
```

**Verify Installation**:
```bash
docker --version
docker-compose --version
docker run hello-world
```

**Minimum Versions**:
- Docker: 20.10+
- Docker Compose: 1.29+

---

## 3. Backend Development Setup

### 3.1 .NET SDK 8.0
**Purpose**: ASP.NET Core backend development

**Installation**:

**Windows**:
1. Download: https://dotnet.microsoft.com/download/dotnet/8.0
2. Run installer
3. Restart terminal

**macOS**:
```bash
# Using Homebrew
brew tap isen-ng/dotnet-sdk-versions
brew install dotnet-sdk8

# Or download directly from Microsoft
```

**Ubuntu/Linux**:
```bash
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

**Verify Installation**:
```bash
dotnet --version
dotnet --info
```

**Required Version**: .NET 8.0 SDK or higher

---

### 3.2 MongoDB
**Purpose**: NoSQL database for the system

**Option A: Docker (Recommended)**
```bash
# Runs automatically with docker-compose up
docker-compose up -d mongodb
```

**Option B: Local Installation**

**Windows**:
1. Download MongoDB Community: https://www.mongodb.com/try/download/community
2. Run installer
3. MongoDB will run as a Windows Service

**macOS**:
```bash
brew tap mongodb/brew
brew install mongodb-community
brew services start mongodb-community
```

**Ubuntu/Linux**:
```bash
curl -fsSL https://www.mongodb.org/static/pgp/server-5.0.asc | sudo apt-key add -
echo "deb [ arch=amd64,arm64 ] https://repo.mongodb.org/apt/ubuntu focal/mongodb-org/5.0 multiverse" | sudo tee /etc/apt/sources.list.d/mongodb-org-5.0.list
sudo apt-get update
sudo apt-get install mongodb-org
sudo systemctl start mongod
```

**Verify Installation**:
```bash
mongosh  # Open MongoDB shell
```

**Required Version**: MongoDB 5.0+

---

### 3.3 MongoDB GUI Tools (Optional but Recommended)

**Option A: MongoDB Compass (Official GUI)**
- Download: https://www.mongodb.com/products/tools/compass
- Great for database visualization

**Option B: MongoDB Express (Runs in Docker)**
```bash
# Automatically included in docker-compose.yml
# Access at http://localhost:8081
# Default credentials: admin/password
```

---

## 4. Mobile Development Setup

### 4.1 Flutter SDK
**Purpose**: Cross-platform mobile app development

**Installation**:

1. Download: https://flutter.dev/docs/get-started/install
2. Unzip to desired location
3. Add to PATH

**Windows**:
```bash
# Add Flutter to PATH
setx PATH "%PATH%;C:\path\to\flutter\bin"
```

**macOS/Linux**:
```bash
export PATH="$PATH:~/flutter/bin"
# Add to ~/.bashrc or ~/.zshrc for persistence
```

**Verify Installation**:
```bash
flutter --version
flutter doctor
```

**Required Version**: Flutter 3.0+

---

### 4.2 Android Development
**Purpose**: Building Android version of the app

**Installation**:

1. Download Android Studio: https://developer.android.com/studio
2. Install Android SDK
3. Configure in Flutter:

```bash
flutter config --android-studio-path="<path-to-android-studio>"
flutter doctor
```

**Required Components**:
- Android SDK API 31+ (minimum)
- Android Build Tools
- Android Emulator (optional, can use physical device)

---

### 4.3 iOS Development (macOS Only)
**Purpose**: Building iOS version of the app

**Installation**:

```bash
# Install Xcode Command Line Tools
xcode-select --install

# Verify
flutter doctor
```

**Requirements**:
- macOS 11+ 
- Xcode 12+
- iOS 12+ deployment target

---

### 4.4 IDE for Flutter Development

**Option A: Android Studio**
- Download: https://developer.android.com/studio
- Includes emulator and debugging tools

**Option B: VS Code**
```bash
# Install Flutter and Dart extensions
# Extensions:
#   - Flutter (by Dart Code)
#   - Dart (by Dart Code)
```

---

## 5. Python ML Service Setup

### 5.1 Python 3.11+
**Purpose**: Machine learning service using FastAPI

**Installation**:

**Windows**:
1. Download: https://www.python.org/downloads/
2. Run installer (check "Add Python to PATH")

**macOS**:
```bash
brew install python@3.11
```

**Ubuntu/Linux**:
```bash
sudo apt-get update
sudo apt-get install python3.11 python3.11-venv python3-pip
```

**Verify Installation**:
```bash
python --version
pip --version
```

**Required Version**: Python 3.11 or higher

---

### 5.2 Python Virtual Environment
**Purpose**: Isolated Python environment for ML service

```bash
# Navigate to project directory
cd /path/to/ems-dispatcher

# Create virtual environment
python -m venv venv

# Activate virtual environment
# Windows
venv\Scripts\activate

# macOS/Linux
source venv/bin/activate

# Verify activation (should show (venv) in terminal)
```

---

### 5.3 Python Dependencies
**Purpose**: Required Python packages

```bash
# Ensure pip is updated
pip install --upgrade pip

# Install all dependencies
pip install -r requirements.txt

# Verify installations
pip list
```

**Key Packages** (in requirements.txt):
- FastAPI - Web framework
- uvicorn - ASGI server
- numpy - Numerical computing
- pandas - Data manipulation
- scikit-learn - Machine learning
- requests - HTTP client

---

## 6. IDE and Code Editors

### 6.1 Visual Studio Code (Recommended for all)
**Download**: https://code.visualstudio.com/

**Required Extensions**:
```
C# Dev Kit (for .NET)
Flutter (for mobile)
Dart
Python
MongoDB for VS Code
REST Client (for API testing)
```

**Installation**:
```bash
# macOS
brew install visual-studio-code

# Ubuntu
sudo apt-get install code
```

---

### 6.2 Visual Studio 2022 Community (Optional, for .NET)
**Download**: https://visualstudio.microsoft.com/vs/community/

**Workloads to Install**:
- ASP.NET and web development
- .NET desktop development

---

### 6.3 Jetbrains Rider (Optional, for .NET)
**Download**: https://www.jetbrains.com/rider/

---

## 7. Package Managers

### 7.1 npm (Node.js)
**Purpose**: JavaScript package management (if needed for web dashboard)

```bash
# macOS
brew install node

# Windows
choco install nodejs

# Ubuntu
sudo apt-get install nodejs npm

# Verify
node --version
npm --version
```

---

### 7.2 pip (Python)
**Already included with Python installation**
```bash
pip --version
```

---

### 7.3 Homebrew (macOS only)
```bash
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
```

---

## 8. API Testing Tools

### 8.1 Postman
**Purpose**: Testing REST APIs

**Download**: https://www.postman.com/downloads/

**Installation**:
```bash
# macOS
brew install postman

# Or download directly
```

---

### 8.2 REST Client VS Code Extension
```bash
# Install in VS Code
# Command: ext install humao.rest-client
```

---

### 8.3 Thunderclient (VS Code)
**vs code Extension ID**: rangav.vscode-thunder-client

---

## 9. Database Tools

### 9.1 MongoDB Shell (mongosh)
**Included with MongoDB, verify with**:
```bash
mongosh --version
```

---

### 9.2 Robo 3T (Optional GUI)
**Download**: https://robomongo.org/

---

## 10. CLI Tools

### 10.1 Dotnet CLI (Included with .NET SDK)
```bash
dotnet --version
dotnet --info
```

---

### 10.2 Flutter CLI (Included with Flutter SDK)
```bash
flutter --version
flutter doctor -v
```

---

## 11. Optional But Recommended Tools

### 11.1 Insomnia
REST API testing client
**Download**: https://insomnia.rest/

---

### 11.2 Git GUI Tools

**GitHub Desktop**:
```bash
brew install github
```

**Sourcetree**:
```bash
brew install sourcetree
```

---

### 11.3 Terminal Emulators

**Windows**:
- Windows Terminal (built-in on Windows 11)
- Git Bash

**macOS/Linux**:
- iTerm2 (macOS)
- Zsh or Fish shell

---

## 12. Required API Keys & Credentials

### 12.1 OpenRouteService (for route optimization)
1. Visit: https://openrouteservice.org/
2. Create account
3. Generate API key
4. Add to `ml_api.py`

```python
ORS_API_KEY = "your_key_here"
```

---

### 12.2 Google Maps (for mobile app)
1. Create project on Google Cloud Console
2. Enable Maps API
3. Generate API key
4. Add to Flutter `android/app/src/main/AndroidManifest.xml`

```xml
<meta-data
    android:name="com.google.android.geo.API_KEY"
    android:value="YOUR_API_KEY"/>
```

---

### 12.3 Firebase (for push notifications - optional)
1. Create Firebase project: https://firebase.google.com/
2. Add Android and iOS apps
3. Download configuration files
4. Add to Flutter project

---

## 13. Complete Installation Checklist

Use this checklist to track your installation progress:

### Essential (Required)
- [ ] Git
- [ ] Docker & Docker Compose
- [ ] .NET 8.0 SDK
- [ ] MongoDB 5.0+
- [ ] Flutter SDK 3.0+
- [ ] Python 3.11+
- [ ] VS Code or IDE

### Backend Development
- [ ] .NET 8.0 verified
- [ ] MongoDB running
- [ ] Project dependencies installed (`dotnet restore`)

### Mobile Development
- [ ] Flutter SDK verified
- [ ] Android SDK configured
- [ ] iOS tools configured (macOS only)
- [ ] Emulator/device setup

### Python ML Service
- [ ] Python 3.11+ verified
- [ ] Virtual environment created
- [ ] Requirements installed (`pip install -r requirements.txt`)

### Tools & Utilities
- [ ] Postman installed
- [ ] MongoDB Compass installed (optional)
- [ ] Git configured

### API Keys (if needed)
- [ ] OpenRouteService key obtained
- [ ] Google Maps API key obtained
- [ ] Firebase project created (optional)

---

## 14. Installation Verification Script

Run this script to verify all installations:

```bash
#!/bin/bash

echo "=== EMS Dispatcher System - Installation Verification ==="
echo ""

echo "1. Checking Git..."
git --version

echo ""
echo "2. Checking Docker..."
docker --version
docker-compose --version

echo ""
echo "3. Checking .NET..."
dotnet --version

echo ""
echo "4. Checking MongoDB..."
mongosh --version 2>/dev/null || echo "MongoDB not in PATH, but Docker will handle it"

echo ""
echo "5. Checking Python..."
python --version
pip --version

echo ""
echo "6. Checking Flutter..."
flutter --version
flutter doctor

echo ""
echo "7. Checking Node.js (optional)..."
node --version 2>/dev/null || echo "Node.js not installed (optional)"
npm --version 2>/dev/null || echo "npm not installed (optional)"

echo ""
echo "=== Verification Complete ==="
```

Save as `verify_installation.sh` and run:
```bash
chmod +x verify_installation.sh
./verify_installation.sh
```

---

## 15. Quick Start Installation Order

Follow this order for a smooth setup:

1. **Install Git** - needed for cloning the project
2. **Install Docker** - for running all services
3. **Install .NET SDK** - for backend development
4. **Install Python** - for ML service
5. **Install Flutter** - for mobile app
6. **Install IDE** - VS Code recommended
7. **Clone the project** - `git clone <repo-url>`
8. **Install dependencies** - dotnet restore, pip install
9. **Configure API keys** - OpenRouteService, Google Maps
10. **Run Docker Compose** - `docker-compose up -d`

---

## 16. Troubleshooting Installation

### Issue: Docker daemon not running
**Solution**:
```bash
# Windows/macOS: Start Docker Desktop application
# Linux
sudo systemctl start docker
```

### Issue: MongoDB connection refused
**Solution**:
```bash
# Check if MongoDB is running
docker ps | grep mongodb

# Or restart Docker Compose
docker-compose restart mongodb
```

### Issue: .NET SDK not found
**Solution**:
```bash
# Verify installation
dotnet --info

# Reinstall if needed
# Download from https://dotnet.microsoft.com/download/dotnet/8.0
```

### Issue: Flutter doctor showing errors
**Solution**:
```bash
flutter doctor -v
# Follow the suggestions to install missing components
```

### Issue: Python module not found
**Solution**:
```bash
# Ensure virtual environment is activated
source venv/bin/activate  # macOS/Linux
venv\Scripts\activate     # Windows

# Reinstall requirements
pip install -r requirements.txt
```

---

## 17. Environment Variables Setup

Create `.env` file in root directory:

```env
# MongoDB
MONGODB_URI=mongodb://admin:password@localhost:27017/ems_dispatch?authSource=admin

# JWT
JWT_SECRET_KEY=your-super-secret-key-change-this-in-production
JWT_ISSUER=ems-dispatch-api
JWT_AUDIENCE=ems-dispatch-app

# API
API_BASE_URL=http://localhost:5000
SIGNALR_URL=http://localhost:5000/hubs

# ML Service
ML_API_URL=http://localhost:8000
ORS_API_KEY=your-openrouteservice-key

# Google Maps (for Flutter)
GOOGLE_MAPS_API_KEY=your-google-maps-key

# Firebase (optional)
FIREBASE_PROJECT_ID=your-firebase-project
```

---

## 18. Getting Help

If installation issues occur:

1. **Check the error message** - often indicates what's missing
2. **Review Troubleshooting section** above
3. **Check individual tool documentation**:
   - Docker: https://docs.docker.com/
   - .NET: https://docs.microsoft.com/dotnet
   - Flutter: https://flutter.dev/docs
   - MongoDB: https://docs.mongodb.com/
4. **Search GitHub issues** - many problems already solved
5. **Community forums**:
   - Stack Overflow
   - Flutter community
   - .NET community

---

## Summary

**Minimum Required Installations**:
- Git
- Docker & Docker Compose
- .NET 8.0 SDK
- Python 3.11+
- Flutter SDK
- MongoDB (via Docker)
- VS Code

**Total Disk Space**: ~50-100 GB (depending on optional tools)

**Total Installation Time**: 1-2 hours

Once all installations are complete, proceed to `NEXT_STEPS.md` to start the system.
