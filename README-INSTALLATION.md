# FinalPOS - Installation Guide

## Overview
FinalPOS is a Point of Sale system with integrated portable MySQL database that works completely offline.

## Installation Structure

```
FinalPOS/
├── mysql/                    # Portable MySQL database files
│   ├── bin/                  # MySQL executables (mysqld.exe, mysql.exe, etc.)
│   ├── data/                 # Database data directory
│   ├── lib/                  # MySQL libraries
│   ├── share/                # MySQL shared files
│   └── include/              # MySQL header files
├── InstallerFiles/
│   ├── config/
│   │   └── my.ini            # MySQL configuration template
│   └── scripts/
│       ├── Install-MySQL.bat # Database initialization script
│       ├── Start-MySQL.bat   # Start MySQL server
│       ├── Stop-MySQL.bat    # Stop MySQL server
│       ├── Check-MySQL.bat   # Check MySQL status
│       └── schema.sql        # Database schema
├── FinalPOS/                 # Application source code
└── InnoSetupScript.iss       # Inno Setup installer script
```

## Installation Process

### Automated Installation (Recommended)

1. **Build the installer:**
   - Open `InnoSetupScript.iss` in Inno Setup Compiler
   - Click "Build > Compile" or press F9
   - The installer will be created in `Installer/` directory

2. **Run the installer:**
   - Execute `FinalPOS-Setup.exe`
   - Follow the installation wizard
   - The installer will:
     - Copy application files
     - Copy MySQL portable database
     - Configure MySQL settings
     - Initialize database schema
     - Create desktop shortcuts

3. **Launch the application:**
   - MySQL starts automatically when you launch FinalPOS
   - Database is initialized on first run if needed
   - Default credentials:
     - Username: `admin`
     - Password: `admin`

### Manual Installation

If you need to install manually:

1. **Copy files:**
   ```
   Copy FinalPOS\bin\Release\* to C:\Program Files\FinalPOS\
   Copy mysql\* to C:\Program Files\FinalPOS\mysql\
   Copy InstallerFiles\config\my.ini to C:\Program Files\FinalPOS\mysql\
   Copy InstallerFiles\scripts\* to C:\Program Files\FinalPOS\scripts\
   ```

2. **Configure MySQL:**
   - Edit `C:\Program Files\FinalPOS\mysql\my.ini`
   - Replace `{INSTALLDIR}` with actual installation path
   - Ensure port is set to `3310`

3. **Initialize database:**
   ```
   Run: C:\Program Files\FinalPOS\scripts\Install-MySQL.bat "C:\Program Files\FinalPOS"
   ```

4. **Start MySQL:**
   ```
   Run: C:\Program Files\FinalPOS\scripts\Start-MySQL.bat "C:\Program Files\FinalPOS"
   ```

5. **Launch application:**
   ```
   Run: C:\Program Files\FinalPOS\FinalPOS.exe
   ```

## Database Configuration

- **Port:** 3310
- **Host:** localhost
- **Database:** POS_NEXA_ERP
- **User:** root
- **Password:** (blank/empty by default)

## MySQL Management Scripts

### Start MySQL
```batch
scripts\Start-MySQL.bat [INSTALL_DIR]
```

### Stop MySQL
```batch
scripts\Stop-MySQL.bat [INSTALL_DIR]
```

### Check MySQL Status
```batch
scripts\Check-MySQL.bat [INSTALL_DIR]
```

### Install/Initialize Database
```batch
scripts\Install-MySQL.bat [INSTALL_DIR]
```

## Troubleshooting

### MySQL Won't Start
1. Check if port 3310 is already in use:
   ```batch
   netstat -an | findstr ":3310"
   ```
2. Verify `my.ini` has correct installation directory paths
3. Check MySQL error log:
   ```
   mysql\data\mysql-error.log
   ```

### Database Connection Failed
1. Ensure MySQL is running:
   ```batch
   scripts\Check-MySQL.bat
   ```
2. Verify connection string in `App.config`:
   ```
   Server=localhost;Port=3310;Database=POS_NEXA_ERP;Uid=root;Pwd=;
   ```
3. Try manually connecting:
   ```batch
   mysql\bin\mysql.exe -h127.0.0.1 -P3310 -uroot
   ```

### Application Won't Start
1. Check if MySQL started successfully
2. Verify database schema is initialized
3. Check application error messages
4. Review MySQL error logs

## Uninstallation

When uninstalling:
- The installer automatically stops MySQL
- Database files remain in `mysql\data\` (for backup purposes)
- To fully remove, manually delete the installation directory

## Features

- ✅ Portable MySQL (no Windows service required)
- ✅ Automatic database initialization
- ✅ Offline operation
- ✅ Auto-start MySQL on application launch
- ✅ Auto-stop MySQL on application close
- ✅ No manual configuration needed

## Support

For issues or questions:
1. Check the error logs in `mysql\data\mysql-error.log`
2. Verify all files are in correct locations
3. Ensure scripts have correct paths
4. Test MySQL connection manually using Check-MySQL.bat

