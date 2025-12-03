# FinalPOS Installer Setup Guide

## Complete Folder Structure

```
Installer/
├── FinalPOS_Installer.iss          ← Main installer script
├── PrepareInstallerFiles.ps1        ← Helper script to prepare files
├── SETUP_GUIDE.md                   ← This file
└── InstallerFiles/
    ├── App/                         ← FinalPOS Release build files
    │   ├── README.txt               ← Instructions
    │   ├── FinalPOS.exe             ← REQUIRED
    │   ├── App.config               ← REQUIRED
    │   ├── *.dll                    ← REQUIRED (all DLLs)
    │   ├── *.rdlc                   ← REQUIRED (report files)
    │   └── Resources/               ← If exists
    │
    ├── XAMPP/                       ← XAMPP Portable extracted here
    │   ├── README.txt               ← Instructions
    │   ├── mysql/
    │   │   ├── bin/
    │   │   │   ├── mysqld.exe      ← REQUIRED
    │   │   │   └── my.ini           ← REQUIRED
    │   │   └── data/                ← REQUIRED (can be empty)
    │   └── ... (other XAMPP folders)
    │
    └── Tools/                       ← PowerShell scripts
        ├── FindAvailablePort.ps1    ← REQUIRED
        ├── ConfigureMySQL.ps1       ← REQUIRED
        ├── UpdateAppConfig.ps1      ← REQUIRED
        └── StartMySQL.ps1            ← REQUIRED
```

## Step-by-Step Setup Instructions

### Step 1: Build FinalPOS in Release Mode

1. Open `FinalPOS.sln` in Visual Studio
2. Select **Release** configuration (top toolbar)
3. Build → Build Solution (or press `Ctrl+Shift+B`)
4. Verify build succeeds
5. Check that `FinalPOS\bin\Release\` contains files

### Step 2: Copy Release Files to InstallerFiles\App

**Option A: Using PowerShell Script (Recommended)**
```powershell
cd C:\Users\shiva\Documents\GitHub\POS-WPF-78\Installer
.\PrepareInstallerFiles.ps1 -CopyReleaseFiles
```

**Option B: Manual Copy**
1. Navigate to: `FinalPOS\bin\Release\`
2. Select ALL files and folders
3. Copy (Ctrl+C)
4. Navigate to: `Installer\InstallerFiles\App\`
5. Paste (Ctrl+V)

**Required Files in App Folder:**
- ✅ FinalPOS.exe
- ✅ FinalPOS.exe.config (or App.config)
- ✅ All .dll files
- ✅ All .rdlc files (Report1.rdlc, Report2.rdlc, etc.)
- ✅ Resources folder (if exists)

### Step 3: Extract XAMPP Portable

1. Download XAMPP Portable for Windows:
   - URL: https://www.apachefriends.org/download.html
   - Choose: **XAMPP for Windows** (Portable version)
   - Ensure MySQL is included

2. Extract XAMPP:
   - Extract the downloaded ZIP file
   - Copy ALL contents from the extracted XAMPP folder
   - Paste into: `Installer\InstallerFiles\XAMPP\`
   - **IMPORTANT:** Copy the CONTENTS, not the XAMPP folder itself

3. Verify XAMPP Structure:
   ```
   InstallerFiles\XAMPP\
   ├── mysql\
   │   ├── bin\
   │   │   ├── mysqld.exe      ← Must exist
   │   │   └── my.ini          ← Must exist
   │   └── data\                ← Must exist
   └── ... (other folders)
   ```

### Step 4: Verify PowerShell Scripts

Check that these files exist in `Installer\InstallerFiles\Tools\`:
- ✅ FindAvailablePort.ps1
- ✅ ConfigureMySQL.ps1
- ✅ UpdateAppConfig.ps1
- ✅ StartMySQL.ps1

If missing, they should already be in the Tools folder. If not, copy them from the repository.

### Step 5: Verify Folder Structure

Run the preparation script to check everything:
```powershell
cd C:\Users\shiva\Documents\GitHub\POS-WPF-78\Installer
.\PrepareInstallerFiles.ps1
```

Expected output:
```
✓ App folder: X files
✓ XAMPP folder: X files/folders
✓ Tools folder: All scripts present
```

### Step 6: Build Installer

1. **Install Inno Setup** (if not installed):
   - Download: https://jrsoftware.org/isdl.php
   - Install with default options

2. **Open Inno Setup Compiler**

3. **Open Script:**
   - File → Open
   - Navigate to: `Installer\FinalPOS_Installer.iss`
   - Click Open

4. **Compile:**
   - Build → Compile (or press F9)
   - Wait for compilation to complete

5. **Check Output:**
   - Installer will be created in: `Installer\Output\FinalPOS_Setup.exe`
   - Check for any errors in the compiler output

## Troubleshooting

### Error: "No files found matching InstallerFiles\App\*"

**Cause:** App folder is empty or doesn't exist

**Solution:**
1. Build FinalPOS in Release mode
2. Copy files from `FinalPOS\bin\Release\*` to `Installer\InstallerFiles\App\`
3. Verify App folder contains FinalPOS.exe and DLLs

### Error: "No files found matching InstallerFiles\XAMPP\*"

**Cause:** XAMPP folder is empty or doesn't exist

**Solution:**
1. Download XAMPP Portable
2. Extract contents to `Installer\InstallerFiles\XAMPP\`
3. Verify `XAMPP\mysql\bin\mysqld.exe` exists

### Error: "Unknown constant" in .iss file

**Cause:** Curly braces not escaped in PowerShell commands

**Solution:**
- The script is already fixed with proper escaping
- If you see this error, ensure you're using the latest `FinalPOS_Installer.iss`

### Error: PowerShell scripts not found

**Cause:** Scripts missing from Tools folder

**Solution:**
1. Verify scripts exist in `Installer\InstallerFiles\Tools\`
2. Check file names match exactly (case-sensitive)

## File Checklist

Before compiling installer, verify:

### InstallerFiles\App\
- [ ] FinalPOS.exe exists
- [ ] App.config (or FinalPOS.exe.config) exists
- [ ] At least 10+ .dll files present
- [ ] .rdlc files present (Report1.rdlc, etc.)
- [ ] Folder is not empty

### InstallerFiles\XAMPP\
- [ ] mysql\bin\mysqld.exe exists
- [ ] mysql\bin\my.ini exists
- [ ] mysql\data\ folder exists
- [ ] Folder contains multiple subfolders

### InstallerFiles\Tools\
- [ ] FindAvailablePort.ps1 exists
- [ ] ConfigureMySQL.ps1 exists
- [ ] UpdateAppConfig.ps1 exists
- [ ] StartMySQL.ps1 exists
- [ ] All 4 scripts present

## Quick Reference

**BasePath Define:**
```iss
#define BasePath "C:\\Users\\shiva\\Documents\\GitHub\\POS-WPF-78\\Installer\\InstallerFiles"
```

**Source Paths (in .iss file):**
```iss
Source: "{#BasePath}\\App\\*";           → InstallerFiles\App\*
Source: "{#BasePath}\\XAMPP\\*";         → InstallerFiles\XAMPP\*
Source: "{#BasePath}\\Tools\\*.ps1";     → InstallerFiles\Tools\*.ps1
```

**Important Notes:**
- Double backslashes (`\\`) are required in BasePath define
- Double backslashes (`\\`) are required in Source paths
- Curly braces in PowerShell commands are escaped as `{{` and `}}`
- All paths use forward slashes in Source: lines after BasePath expansion

## Next Steps After Building Installer

1. Test installer on clean machine
2. Verify MySQL starts correctly
3. Verify FinalPOS connects to database
4. Test admin/admin login
5. Verify all features work

---

**Last Updated:** 2025-12-03
**Script Version:** 1.0

