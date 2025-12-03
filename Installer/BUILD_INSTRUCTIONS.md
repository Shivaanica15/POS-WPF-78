# FinalPOS Installer Build Instructions

## Quick Start

1. **Install Inno Setup** (if not already installed)
   - Download: https://jrsoftware.org/isdl.php
   - Version 6.2.0 or later recommended

2. **Prepare InstallerFiles Folder**

   ```powershell
   # Navigate to project root
   cd C:\Users\shiva\Documents\GitHub\POS-WPF-78
   
   # Create folder structure
   New-Item -ItemType Directory -Force -Path "Installer\InstallerFiles\App"
   New-Item -ItemType Directory -Force -Path "Installer\InstallerFiles\XAMPP"
   New-Item -ItemType Directory -Force -Path "Installer\InstallerFiles\Tools"
   ```

3. **Build FinalPOS in Release Mode**

   ```powershell
   # Open Visual Studio
   # Build → Configuration Manager → Set to Release
   # Build → Build Solution (or press Ctrl+Shift+B)
   ```

4. **Copy Application Files**

   ```powershell
   # Copy all Release build files
   Copy-Item "FinalPOS\bin\Release\*" "Installer\InstallerFiles\App\" -Recurse -Force
   
   # Verify essential files are present:
   # - FinalPOS.exe
   # - App.config
   # - All .dll files
   # - All .rdlc files
   # - Resources folder
   ```

5. **Prepare XAMPP**

   - Download XAMPP Portable for Windows
   - Extract entire XAMPP folder to `Installer\InstallerFiles\XAMPP\`
   - Structure should be:
     ```
     InstallerFiles\XAMPP\
     ├── mysql\
     │   ├── bin\
     │   │   ├── mysqld.exe
     │   │   └── my.ini
     │   └── data\
     └── ... (other XAMPP folders)
     ```

6. **Verify PowerShell Scripts**

   Ensure these files exist:
   - `Installer\InstallerFiles\Tools\FindAvailablePort.ps1`
   - `Installer\InstallerFiles\Tools\ConfigureMySQL.ps1`
   - `Installer\InstallerFiles\Tools\UpdateAppConfig.ps1`
   - `Installer\InstallerFiles\Tools\StartMySQL.ps1`

7. **Build Installer**

   - Open Inno Setup Compiler
   - File → Open → Select `Installer\FinalPOS_Installer.iss`
   - Build → Compile (or press F9)
   - Installer will be created in `Installer\Output\FinalPOS_Setup.exe`

## Detailed Steps

### Step 1: Verify Prerequisites

- [ ] Inno Setup installed
- [ ] Visual Studio installed
- [ ] .NET Framework 4.7.2 or later installed
- [ ] XAMPP Portable downloaded

### Step 2: Build Application

1. Open `FinalPOS.sln` in Visual Studio
2. Select **Release** configuration
3. Build → Build Solution
4. Verify `FinalPOS\bin\Release\` contains:
   - FinalPOS.exe
   - All required DLLs
   - App.config
   - Report files (.rdlc)
   - Resources folder

### Step 3: Prepare InstallerFiles

```powershell
# Create directory structure
$basePath = "Installer\InstallerFiles"
New-Item -ItemType Directory -Force -Path "$basePath\App"
New-Item -ItemType Directory -Force -Path "$basePath\XAMPP"
New-Item -ItemType Directory -Force -Path "$basePath\Tools"

# Copy application files
Copy-Item "FinalPOS\bin\Release\*" "$basePath\App\" -Recurse -Force

# Copy PowerShell scripts (already in place)
# Copy-Item "Installer\InstallerFiles\Tools\*.ps1" "$basePath\Tools\" -Force

# Extract XAMPP to XAMPP folder
# (Manual step - extract downloaded XAMPP portable)
```

### Step 4: Verify XAMPP Structure

After extracting XAMPP, verify:
```
InstallerFiles\XAMPP\
├── mysql\
│   ├── bin\
│   │   ├── mysqld.exe          ← Must exist
│   │   ├── mysql.exe
│   │   └── my.ini               ← Must exist
│   ├── data\                    ← Must exist (can be empty)
│   └── ...
├── apache\                      ← Optional
└── ...
```

### Step 5: Test PowerShell Scripts

Before building installer, test scripts:

```powershell
# Test port detection
.\Installer\InstallerFiles\Tools\FindAvailablePort.ps1

# Should create %TEMP%\mysql_port.txt with port number
```

### Step 6: Build Installer

1. Open Inno Setup Compiler
2. File → Open → `Installer\FinalPOS_Installer.iss`
3. Review script (check paths are correct)
4. Build → Compile
5. Check `Installer\Output\` for `FinalPOS_Setup.exe`

### Step 7: Test Installation

1. Run `FinalPOS_Setup.exe` on test machine
2. Verify installation completes successfully
3. Check:
   - Files installed to `C:\Program Files\FinalPOS\`
   - XAMPP installed to `C:\FinalPOS-XAMPP\`
   - MySQL starts automatically
   - FinalPOS launches and connects to database
   - Admin/admin login works

## Troubleshooting Build Issues

### Issue: "File not found" errors

**Solution:**
- Verify all paths in `.iss` file are correct
- Check that `InstallerFiles` folder structure matches script
- Ensure all required files exist

### Issue: PowerShell scripts not executing

**Solution:**
- Check PowerShell execution policy:
  ```powershell
  Get-ExecutionPolicy
  # Should be: RemoteSigned or Bypass
  ```
- If restricted, run:
  ```powershell
  Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
  ```

### Issue: MySQL won't start during installation

**Solution:**
- Verify `my.ini` exists in XAMPP
- Check that paths in `ConfigureMySQL.ps1` are correct
- Test MySQL manually:
  ```cmd
  cd C:\FinalPOS-XAMPP\mysql\bin
  mysqld.exe --console
  ```

### Issue: Connection string not updated

**Solution:**
- Verify `App.config` exists in `InstallerFiles\App\`
- Check `UpdateAppConfig.ps1` for correct XML node name
- Test script manually:
  ```powershell
  .\UpdateAppConfig.ps1 -AppConfigPath "path\to\App.config" -PortFile "path\to\mysql_port.txt"
  ```

## Pre-Build Checklist

Before building installer, verify:

- [ ] FinalPOS builds successfully in Release mode
- [ ] All DLLs are included in Release folder
- [ ] App.config is present and valid
- [ ] XAMPP extracted to correct location
- [ ] PowerShell scripts are in Tools folder
- [ ] Folder structure matches `.iss` file
- [ ] No missing file errors in Inno Setup compiler

## Post-Build Checklist

After building installer:

- [ ] Installer file created (`FinalPOS_Setup.exe`)
- [ ] File size is reasonable (should include XAMPP)
- [ ] Test installation on clean machine
- [ ] Verify MySQL starts correctly
- [ ] Verify FinalPOS connects to database
- [ ] Verify admin/admin login works
- [ ] Check `mysql_port.txt` contains correct port

## Distribution

When distributing installer:

1. **File Size:** Installer will be large (includes XAMPP)
   - Typical size: 100-200 MB
   - Consider compression options

2. **System Requirements:**
   - Windows 7 or later (64-bit)
   - .NET Framework 4.7.2 or later
   - Administrator privileges for installation

3. **Installation Instructions:**
   - Provide to end users
   - Include troubleshooting steps
   - Document default credentials (admin/admin)

## Advanced Customization

### Change Installation Paths

Edit `FinalPOS_Installer.iss`:

```iss
; Application path
DefaultDirName={pf}\{#MyAppName}  ; Change to {localappdata}\{#MyAppName} for user install

; XAMPP path
DestDir: "C:\FinalPOS-XAMPP"  ; Change to desired path
```

### Add Custom Installation Steps

Add to `[Run]` section:

```iss
Filename: "powershell.exe"; Parameters: "-Command ""Your custom script here"""; StatusMsg: "Custom step..."; Flags: runhidden waituntilterminated
```

### Modify Port Range

Edit `FindAvailablePort.ps1`:

```powershell
# Change port range
for ($port = 3306; $port -le 3320; $port++) {  # Extended range
```

## Support

For build issues:
1. Check Inno Setup compiler output window
2. Review PowerShell script outputs
3. Test scripts individually
4. Verify file paths and permissions

