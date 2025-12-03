# FinalPOS Installer Documentation

## Overview

This installer creates a production-ready installation package for FinalPOS that includes:
- FinalPOS application files
- Bundled XAMPP with MySQL
- Automatic MySQL port conflict detection and resolution
- Automatic configuration of connection strings
- Silent, automated setup process

## Folder Structure

```
Installer/
├── FinalPOS_Installer.iss          # Main Inno Setup script
├── InstallerFiles/
│   ├── App/                        # FinalPOS application files
│   │   ├── FinalPOS.exe
│   │   ├── *.dll                   # All required DLLs
│   │   ├── App.config
│   │   ├── *.rdlc                  # Report files
│   │   └── Resources/              # Application resources
│   ├── XAMPP/                      # Portable XAMPP installation
│   │   ├── mysql/
│   │   │   ├── bin/
│   │   │   │   ├── mysqld.exe
│   │   │   │   └── my.ini
│   │   │   └── data/
│   │   └── ... (other XAMPP files)
│   └── Tools/                      # Helper PowerShell scripts
│       ├── FindAvailablePort.ps1
│       ├── ConfigureMySQL.ps1
│       └── UpdateAppConfig.ps1
└── README.md                       # This file
```

## How It Works

### 1. Port Detection (`FindAvailablePort.ps1`)

- Scans ports 3306 through 3315
- Uses `Test-NetConnection` and `netstat` to check port availability
- Writes the first available port to `%TEMP%\mysql_port.txt`

### 2. MySQL Configuration (`ConfigureMySQL.ps1`)

- Reads port from temp file
- Updates `my.ini` with:
  - Port number in `[client]` and `[mysqld]` sections
  - Correct `basedir` path (C:\FinalPOS-XAMPP\mysql)
  - Correct `datadir` path (C:\FinalPOS-XAMPP\mysql\data)
- Creates backup of original `my.ini`

### 3. App.config Update (`UpdateAppConfig.ps1`)

- Reads port from temp file
- Updates `App.config` connection string:
  - Adds or updates `Port=XXXX` parameter
  - Preserves other connection string parameters
- Creates backup of original `App.config`

### 4. MySQL Startup

- Starts MySQL using the configured port
- Waits 5 seconds for MySQL to initialize
- Runs in background (hidden window)

### 5. Application Launch

- Launches FinalPOS.exe
- DatabaseInitializer automatically creates database on first run
- Default login: admin / admin

## Installation Process Flow

```
1. User runs installer
   ↓
2. Files copied to:
   - C:\Program Files\FinalPOS\ (application)
   - C:\FinalPOS-XAMPP\ (XAMPP)
   ↓
3. FindAvailablePort.ps1 executes
   - Detects available port (3306-3315)
   - Saves to temp file
   ↓
4. ConfigureMySQL.ps1 executes
   - Updates my.ini with port and paths
   ↓
5. UpdateAppConfig.ps1 executes
   - Updates App.config connection string
   ↓
6. MySQL started with configured port
   ↓
7. Port saved to C:\Program Files\FinalPOS\mysql_port.txt
   ↓
8. FinalPOS.exe launched
   ↓
9. DatabaseInitializer creates database automatically
```

## Building the Installer

### Prerequisites

1. **Inno Setup** (6.2.0 or later)
   - Download from: https://jrsoftware.org/isdl.php
   - Install with default options

2. **XAMPP Portable**
   - Download XAMPP Portable (Windows)
   - Extract to `InstallerFiles\XAMPP\`
   - Ensure MySQL is included

3. **FinalPOS Release Build**
   - Build FinalPOS in Release mode
   - Copy all files from `bin\Release\` to `InstallerFiles\App\`
   - Include:
     - FinalPOS.exe
     - All DLLs
     - App.config
     - All .rdlc files
     - Resources folder

### Build Steps

1. **Prepare InstallerFiles folder:**
   ```powershell
   # Create folder structure
   mkdir Installer\InstallerFiles\App
   mkdir Installer\InstallerFiles\XAMPP
   mkdir Installer\InstallerFiles\Tools
   
   # Copy application files
   Copy-Item "FinalPOS\bin\Release\*" "Installer\InstallerFiles\App\" -Recurse
   
   # Copy XAMPP (extract portable XAMPP here)
   # Copy PowerShell scripts to Tools folder
   ```

2. **Open Inno Setup Compiler:**
   - File → Open → Select `FinalPOS_Installer.iss`

3. **Build:**
   - Build → Compile (or press F9)
   - Installer will be created in `Installer\Output\`

## Testing the Installer

### Test Scenarios

1. **Clean Installation:**
   - Install on clean Windows machine
   - Verify MySQL starts on port 3306 (or next available)
   - Verify FinalPOS connects successfully
   - Verify admin/admin login works

2. **Port Conflict Test:**
   - Start MySQL on port 3306 manually
   - Run installer
   - Verify installer uses port 3307
   - Verify FinalPOS connects to port 3307

3. **Multiple Port Conflicts:**
   - Block ports 3306-3308
   - Verify installer uses port 3309

4. **Uninstall Test:**
   - Install application
   - Uninstall via Control Panel
   - Verify files removed
   - Verify shortcuts removed

## Troubleshooting

### MySQL Won't Start

1. Check `C:\FinalPOS-XAMPP\mysql\bin\my.ini`:
   - Verify port is set correctly
   - Verify paths are correct
   - Check for syntax errors

2. Check Windows Event Viewer:
   - Look for MySQL errors

3. Manual MySQL start:
   ```cmd
   cd C:\FinalPOS-XAMPP\mysql\bin
   mysqld.exe --console
   ```

### Connection String Issues

1. Check `C:\Program Files\FinalPOS\App.config`:
   - Verify `Port=XXXX` is present
   - Verify connection string format

2. Check `C:\Program Files\FinalPOS\mysql_port.txt`:
   - Verify port matches App.config

3. Verify DatabaseInitializer.cs:
   - Ensure it reads port from App.config
   - Check connection string parsing logic

### Port Detection Fails

1. Check PowerShell execution policy:
   ```powershell
   Get-ExecutionPolicy
   # Should be: RemoteSigned or Bypass
   ```

2. Run FindAvailablePort.ps1 manually:
   ```powershell
   .\FindAvailablePort.ps1
   ```

3. Check firewall:
   - Ensure ports 3306-3315 are not blocked

## Customization

### Change Default Port Range

Edit `FindAvailablePort.ps1`:
```powershell
# Change this line:
for ($port = 3306; $port -le 3315; $port++) {
# To:
for ($port = 3306; $port -le 3320; $port++) {
```

### Change Installation Paths

Edit `FinalPOS_Installer.iss`:
```iss
DefaultDirName={pf}\{#MyAppName}  ; Application path
Source: "InstallerFiles\XAMPP\*"; DestDir: "C:\FinalPOS-XAMPP";  ; XAMPP path
```

### Add Custom Tasks

Edit `[Tasks]` section in `.iss` file:
```iss
[Tasks]
Name: "customtask"; Description: "Custom task description"; GroupDescription: "Custom"
```

## Security Considerations

1. **MySQL Root Password:**
   - Currently set to empty (Pwd=;)
   - For production, consider:
     - Setting a password during installation
     - Storing password securely
     - Updating connection strings accordingly

2. **File Permissions:**
   - Installer requires admin privileges
   - Application files in Program Files require elevation
   - XAMPP in C:\ root requires admin access

3. **Firewall:**
   - MySQL port should be accessible locally
   - Consider blocking external access if not needed

## Support

For issues or questions:
1. Check `C:\Program Files\FinalPOS\mysql_port.txt` for port information
2. Review installer logs in `%TEMP%\`
3. Check Windows Event Viewer for MySQL errors
4. Review PowerShell script outputs

## Version History

- **1.0** - Initial release
  - Automatic port detection
  - XAMPP bundling
  - App.config auto-configuration
  - MySQL auto-start

