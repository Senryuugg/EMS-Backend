PYTHON 3.12 WINDOWS PIP INSTALL FIX
====================================

ERROR YOU ENCOUNTERED:
- NumPy/Pandas trying to build from source on Windows
- "Unknown compiler(s)" error - No C compiler found
- Solution: Use pre-built wheels

SOLUTION 1: UPGRADE PIP (RECOMMENDED - 2 MINUTES)
================================================

This is the easiest fix. Updated pip can find pre-built wheels for Python 3.12:

1. Open Command Prompt/PowerShell in your project directory
2. Activate virtual environment:
   venv\Scripts\activate

3. Upgrade pip:
   python -m pip install --upgrade pip

4. Clear pip cache:
   pip cache purge

5. Try installing again:
   pip install -r requirements.txt

This should work because the updated pip has wheels for Python 3.12.


SOLUTION 2: INSTALL VISUAL C++ BUILD TOOLS (ADVANCED - 30 MINUTES)
=================================================================

If Solution 1 doesn't work, install Microsoft C++ compiler:

1. Download Visual Studio Build Tools:
   https://visualstudio.microsoft.com/downloads/
   
2. Run installer
3. Select "Desktop development with C++"
4. Install (takes ~15 minutes)
5. Restart your computer
6. Clear pip cache: pip cache purge
7. Try: pip install -r requirements.txt

After installing, NumPy can compile from source.


SOLUTION 3: USE PYTHON 3.11 (IF OTHERS FAIL - 15 MINUTES)
========================================================

Easiest fallback if both above don't work:

1. Uninstall Python 3.12
2. Download Python 3.11 from python.org
3. Check "Add Python 3.11 to PATH" during installation
4. Delete old venv: rmdir /s venv
5. Create new venv: python -m venv venv
6. Activate: venv\Scripts\activate
7. Install: pip install -r requirements.txt

Python 3.11 has pre-built wheels for older NumPy versions.


TROUBLESHOOTING IF STILL FAILING
================================

If you still get errors after trying Solution 1:

1. Check pip version:
   pip --version
   (Should be 24.0 or higher)

2. Check Python version:
   python --version
   (Should be 3.12.x)

3. Try updating setuptools and wheel:
   pip install --upgrade setuptools wheel

4. Install with no-build-isolation:
   pip install --no-build-isolation -r requirements.txt

5. If that fails, use Solution 3 (Python 3.11)


QUICK COMMAND SUMMARY
====================

# Activate virtual environment
venv\Scripts\activate

# Try this first (5 min)
python -m pip install --upgrade pip
pip cache purge
pip install -r requirements.txt

# If that fails, try this (10 min)
pip install --upgrade setuptools wheel
pip cache purge
pip install -r requirements.txt

# If still fails, Switch to Python 3.11 (15 min total)
# Or install Visual C++ Build Tools (30 min)


VERIFY INSTALLATION
===================

After successful install, verify with:

python -c "import pandas; import numpy; import scikit-learn; print('✓ All packages installed successfully!')"

You should see: ✓ All packages installed successfully!
