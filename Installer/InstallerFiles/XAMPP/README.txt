===========================================
XAMPP PORTABLE INSTALLATION
===========================================

This folder MUST contain the extracted XAMPP Portable installation.

REQUIRED STRUCTURE:
-------------------
XAMPP\
├── mysql\
│   ├── bin\
│   │   ├── mysqld.exe      ← REQUIRED
│   │   ├── mysql.exe
│   │   └── my.ini          ← REQUIRED
│   └── data\               ← REQUIRED (can be empty)
├── apache\                 ← Optional
└── ... (other XAMPP folders)

HOW TO POPULATE THIS FOLDER:
-----------------------------
1. Download XAMPP Portable for Windows from:
   https://www.apachefriends.org/download.html

2. Extract the entire XAMPP folder contents HERE
   (Not the XAMPP folder itself, but its contents)

3. Ensure MySQL is included in the download

VERIFICATION:
-------------
After extraction, verify these paths exist:
✓ XAMPP\mysql\bin\mysqld.exe
✓ XAMPP\mysql\bin\my.ini
✓ XAMPP\mysql\data\ (folder exists)

IMPORTANT:
----------
- Use XAMPP Portable (NOT the installer version)
- Extract ALL contents directly into this XAMPP folder
- Do NOT create an extra nested XAMPP folder
- MySQL must be included in the XAMPP package

===========================================
NOTE: This folder cannot be empty for the installer to compile!
===========================================

