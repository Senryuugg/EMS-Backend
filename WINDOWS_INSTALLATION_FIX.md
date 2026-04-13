# Windows Installation Troubleshooting Guide

## Problem: "Failed to build 'pandas'" Error

When running `pip install -r requirements.txt` on Windows, you may see an error about building pandas/numpy from source. This happens because:

1. **Windows doesn't have a C compiler by default**
2. **Python 3.12 may have limited pre-built wheels for some packages**
3. **Missing Visual Studio Build Tools**

---

## Solution 1: Use Python 3.11 (Recommended - Easiest)

This is the **fastest and easiest solution**. Python 3.11 has better pre-built wheel support.

### Steps:
1. Uninstall Python 3.12
   ```bash
   # In Windows, go to Settings > Apps > Installed Apps > Search "Python"
   # Or from command line:
   wmic product where name="Python 3.12*" call uninstall /nointeractive
   ```

2. Download Python 3.11
   - Visit https://www.python.org/downloads/
   - Download **Python 3.11.x** (latest 3.11 version)
   - **IMPORTANT**: Check "Add Python to PATH" during installation

3. Create a fresh virtual environment
   ```bash
   cd D:\EMS-Dispatcher-Application
   python --version  # Should show 3.11.x
   python -m venv venv
   venv\Scripts\activate
   ```

4. Upgrade pip (IMPORTANT!)
   ```bash
   python -m pip install --upgrade pip
   ```

5. Install requirements
   ```bash
   pip install -r requirements.txt
   ```

**Success indicators:**
- No errors during installation
- All packages listed when you run: `pip list`

---

## Solution 2: Install Visual Studio Build Tools (Manual Compilation)

If you prefer to stay on Python 3.12, install Visual Studio Build Tools.

### Steps:

1. **Download Visual Studio Build Tools**
   - Go to: https://visualstudio.microsoft.com/downloads/
   - Scroll down to "All Downloads"
   - Find "Tools for Visual Studio 2022"
   - Click "Build Tools for Visual Studio 2022"

2. **Run the installer**
   - Execute the downloaded `.exe` file
   - Click "Install"

3. **Select Components**
   - ✓ Check: "Desktop development with C++"
   - ✓ Check: "Python development" (if available)
   - Click "Install"
   - Wait 10-15 minutes for installation

4. **Upgrade pip**
   ```bash
   python -m pip install --upgrade pip
   ```

5. **Clear pip cache and reinstall**
   ```bash
   pip cache purge
   pip install -r requirements.txt
   ```

---

## Solution 3: Use Pre-built Wheels (Alternative)

If Solutions 1 & 2 don't work, download pre-built wheels.

### Steps:

1. **Go to**: https://www.lfd.uci.edu/~gohlke/pythonlibs/

2. **Download these wheels** (choose matching your Python version):
   - `numpy-1.26.4-cp312-cp312-win_amd64.whl` (for Python 3.12, 64-bit)
   - `pandas-2.1.4-cp312-cp312-win_amd64.whl` (for Python 3.12, 64-bit)

3. **Install wheels manually**
   ```bash
   cd D:\path\to\downloaded\wheels
   pip install numpy-1.26.4-cp312-cp312-win_amd64.whl
   pip install pandas-2.1.4-cp312-cp312-win_amd64.whl
   pip install -r requirements.txt
   ```

---

## Step-by-Step Installation Process (Windows)

### 1. **Create Virtual Environment**
```bash
cd D:\EMS-Dispatcher-Application
python -m venv venv
```

### 2. **Activate Virtual Environment**
```bash
# PowerShell
venv\Scripts\Activate.ps1

# CMD
venv\Scripts\activate.bat

# Git Bash
source venv/Scripts/activate
```

You should see `(venv)` at the start of your command line.

### 3. **Upgrade pip (CRITICAL!)**
```bash
python -m pip install --upgrade pip
```

### 4. **Install Requirements**
```bash
pip install -r requirements.txt
```

### 5. **Verify Installation**
```bash
# Should show all packages
pip list

# Test Python imports
python -c "import pandas; import numpy; import fastapi; print('All packages installed successfully!')"
```

---

## Common Errors & Fixes

### Error: "The system cannot find the file specified"
**Cause**: Missing C compiler  
**Fix**: Use Solution 1 (Python 3.11) or Solution 2 (Build Tools)

### Error: "No module named 'numpy'"
**Cause**: Virtual environment not activated  
**Fix**: Run `venv\Scripts\activate` first

### Error: "pip is not recognized as an internal or external command"
**Cause**: Python not in PATH  
**Fix**: 
```bash
# Use python -m pip instead
python -m pip install --upgrade pip
python -m pip install -r requirements.txt
```

### Error: "Permission denied"
**Cause**: Running PowerShell script execution policy  
**Fix**: 
```bash
# In PowerShell as Administrator
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Error: "Building wheel for pandas failed"
**Cause**: Old pip version  
**Fix**:
```bash
python -m pip install --upgrade pip setuptools wheel
pip install -r requirements.txt
```

---

## Verification Checklist

After installation, verify everything is working:

```bash
# 1. Check Python version
python --version
# Should be 3.11.x or 3.12.x

# 2. Check pip version
pip --version
# Should be recent version (>= 23.0)

# 3. List all installed packages
pip list
# Should show: fastapi, uvicorn, pandas, numpy, scikit-learn, etc.

# 4. Test imports
python -c "import fastapi, uvicorn, pandas, numpy, sklearn; print('✓ All imports successful')"

# 5. Test ML model loading
python -c "import pickle; pickle.load(open('hospital_selection_model.pkl', 'rb')); print('✓ Model loaded successfully')"

# 6. Test API startup (should start without errors, then Ctrl+C to stop)
python ml_api.py
# Should show: "Uvicorn running on http://0.0.0.0:8000"
```

---

## Full Installation Walkthrough (Video Guide Text)

### For Python 3.11 (Easiest):

1. Uninstall Python 3.12
2. Install Python 3.11 from python.org
3. Open Command Prompt in project folder
4. `python -m venv venv`
5. `venv\Scripts\activate`
6. `python -m pip install --upgrade pip`
7. `pip install -r requirements.txt`
8. Wait 2-5 minutes
9. `pip list` to verify
10. Done!

### For Python 3.12 + Build Tools:

1. Install Visual Studio Build Tools
2. Select "Desktop development with C++"
3. Open Command Prompt in project folder
4. `python -m venv venv`
5. `venv\Scripts\activate`
6. `pip cache purge`
7. `python -m pip install --upgrade pip`
8. `pip install -r requirements.txt`
9. Wait 5-10 minutes
10. `pip list` to verify
11. Done!

---

## Recommended System Specifications for Windows

| Component | Requirement | Why |
|-----------|-------------|-----|
| **OS** | Windows 10/11 | Latest security patches |
| **RAM** | 8 GB minimum | Docker + services need memory |
| **Disk Space** | 50+ GB SSD | Database, Docker images, dependencies |
| **Python Version** | 3.11 or 3.12 | Tested versions |
| **Build Tools** | Visual C++ 14.0+ | Compile C extensions |
| **Terminal** | Windows Terminal (recommended) | Better than default CMD |

---

## Next Steps After Installation

Once `pip list` shows all packages:

1. **Test ML API**
   ```bash
   python ml_api.py
   # Open browser: http://localhost:8000/docs
   ```

2. **Test Backend API**
   ```bash
   cd EmsDispatch.Backend
   dotnet run
   # Open browser: http://localhost:5000/swagger
   ```

3. **Start All Services**
   ```bash
   docker-compose up -d
   ```

---

## Getting Help

If you're still stuck:

1. Copy the **full error message**
2. Check that you're using **Python 3.11** (easier first attempt)
3. Make sure **virtual environment is activated** (should see `(venv)` in terminal)
4. Try **Solution 1 first** before attempting Solution 2

---

## Updated Requirements

The `requirements.txt` has been updated to use the most compatible versions:

- **pandas**: 2.1.4 (instead of 2.1.3)
- **numpy**: 1.26.4 (instead of 1.26.2)
- **Added**: folium, python-dotenv, httpx for additional features

These versions have pre-built wheels for both Python 3.11 and 3.12 on Windows.
