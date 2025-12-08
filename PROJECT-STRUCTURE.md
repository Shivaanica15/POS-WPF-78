# FinalPOS - Project Structure & Implementation

## Overview
This document describes the complete structure and implementation of the FinalPOS portable MySQL deployment system.

## Directory Structure

```
POS-WPF-78/
├── FinalPOS/                          # Main application project
│   ├── bin/
│   │   ├── Debug/                     # Debug build output
│   │   └── Release/                   # Release build output (for installer)
│   ├── Program.cs                     # Application entry point (auto-starts MySQL)
│   ├── DatabaseInitializer.cs         # Database schema initialization
│   ├── DbConnection.cs                # Database connection handling
│   ├── App.config                     # Application configuration (connection string)
│   └── [Other application files]
│
├── mysql/                             # Portable MySQL installation
│   ├── bin/                           # MySQL executables
│   │   ├── mysqld.exe                 # MySQL server (REQUIRED)
│   │   ├── mysql.exe                  # MySQL client
│   │   ├── mysqladmin.exe             # MySQL administration
│   │   └── [other MySQL utilities]
│   ├── data/                          # Database data directory
│   │   ├── mysql/                     # MySQL system database
│   │   ├── performance_schema/        # Performance schema
│   │   └── [application databases]
│   ├── lib/                           # MySQL libraries
│   ├── share/                         # MySQL shared files
│   └── include/                       # MySQL headers
│
├── InstallerFiles/                    # Installer resources
│   ├── config/
│   │   └── my.ini                     # MySQL configuration template
│   └── scripts/
│       ├── Install-MySQL.bat          # Database initialization script
│       ├── Start-MySQL.bat            # Start MySQL server
│       ├── Stop-MySQL.bat             # Stop MySQL server
│       ├── Check-MySQL.bat            # Check MySQL status
│       └── schema.sql                 # Database schema SQL
│
├── InnoSetupScript.iss                # Inno Setup installer script
├── README-INSTALLATION.md             # Installation guide
└── PROJECT-STRUCTURE.md               # This file
```

## Key Components

### 1. MySQL Configuration (my.ini)

**Location:** `InstallerFiles/config/my.ini`

**Features:**
- Port: 3310 (non-standard to avoid conflicts with MySQL80 service)
- Portable paths using `{INSTALLDIR}` placeholder
- Optimized for single-user desktop use
- UTF8MB4 character set
- Minimal logging for performance

**Path Replacement:**
- `{INSTALLDIR}` is replaced with actual installation directory during installation
- Done by Inno Setup script and Install-MySQL.bat

### 2. Installation Script (Install-MySQL.bat)

**Location:** `InstallerFiles/scripts/Install-MySQL.bat`

**Functions:**
1. Creates required directories (data, tmp)
2. Updates my.ini with installation directory
3. Initializes MySQL data directory (if needed)
4. Starts MySQL server
5. Creates POS_NEXA_ERP database
6. Imports schema.sql

**Usage:**
```batch
Install-MySQL.bat "C:\Program Files\FinalPOS"
```

### 3. MySQL Management Scripts

#### Start-MySQL.bat
- Starts MySQL server in background
- Checks if already running
- Waits for server to be ready

#### Stop-MySQL.bat
- Gracefully shuts down MySQL
- Falls back to force kill if needed
- Verifies shutdown

#### Check-MySQL.bat
- Checks if MySQL is running on port 3310
- Tests database connection
- Reports status

### 4. Database Schema (schema.sql)

**Location:** `InstallerFiles/scripts/schema.sql`

**Contents:**
- All table definitions
- Views for reporting
- Triggers for data integrity
- Seed data (admin user, VAT record)

### 5. Application Integration

#### Program.cs
- Auto-starts MySQL on application launch
- Stops MySQL on application exit
- Handles database initialization

#### DatabaseInitializer.cs
- Creates database if missing
- Creates tables, views, triggers
- Handles schema migration
- Seeds initial data

#### App.config
- Connection string: `Server=localhost;Port=3310;Database=POS_NEXA_ERP;Uid=root;Pwd=;`
- No password by default (can be configured)

### 6. Inno Setup Installer

**Location:** `InnoSetupScript.iss`

**Features:**
- Copies application files
- Copies MySQL portable database
- Copies configuration and scripts
- Runs Install-MySQL.bat during installation
- Updates my.ini paths
- Stops MySQL during uninstall
- Creates desktop shortcuts

## Installation Flow

1. **User runs installer**
   - Inno Setup extracts files
   - Copies application to installation directory
   - Copies MySQL to `{app}\mysql\`
   - Copies scripts to `{app}\scripts\`
   - Copies my.ini to `{app}\mysql\`

2. **Installer runs Install-MySQL.bat**
   - Script updates my.ini with installation path
   - Initializes MySQL data directory (if needed)
   - Starts MySQL server
   - Creates POS_NEXA_ERP database
   - Imports schema.sql

3. **Application launches**
   - Program.cs checks if MySQL is running
   - If not running, starts MySQL automatically
   - DatabaseInitializer ensures schema is up-to-date
   - Application connects and runs

4. **Application closes**
   - Program.cs stops MySQL gracefully
   - Clean shutdown ensures data integrity

## Database Configuration

### Default Settings
- **Port:** 3310
- **Host:** localhost (127.0.0.1)
- **Database:** POS_NEXA_ERP
- **User:** root
- **Password:** (blank/empty)

### Connection String
```
Server=localhost;Port=3310;Database=POS_NEXA_ERP;Uid=root;Pwd=;AllowPublicKeyRetrieval=True;
```

## Security Considerations

1. **Local-only binding:** MySQL only listens on 127.0.0.1
2. **No network access:** skip-networking=0 but bind-address=127.0.0.1
3. **No password:** Acceptable for local-only, single-user desktop application
4. **No Windows service:** Runs as regular process, stops when app closes

## Troubleshooting

### MySQL Won't Start
- Check port 3310 availability
- Verify my.ini paths are correct
- Check mysql-error.log

### Database Connection Failed
- Ensure MySQL is running (Check-MySQL.bat)
- Verify connection string in App.config
- Test manual connection with mysql.exe

### Schema Issues
- DatabaseInitializer auto-creates missing tables
- Check schema.sql for reference
- Verify database exists (POS_NEXA_ERP)

## Build & Deploy Process

1. **Build Application:**
   ```
   Build FinalPOS in Release mode
   Output: FinalPOS\bin\Release\FinalPOS.exe
   ```

2. **Prepare Installer Files:**
   - Ensure mysql/ folder contains all required files
   - Verify InstallerFiles/ scripts are correct
   - Check my.ini template

3. **Build Installer:**
   ```
   Open InnoSetupScript.iss in Inno Setup Compiler
   Build > Compile (F9)
   Output: Installer\FinalPOS-Setup.exe
   ```

4. **Test Installation:**
   - Install on clean system
   - Verify MySQL starts
   - Test database connection
   - Verify application works

## Maintenance

### Updating Database Schema
1. Update schema.sql with new changes
2. Update DatabaseInitializer.cs with new CREATE statements
3. Application will auto-migrate on next run

### Updating MySQL Version
1. Replace mysql/ folder with new version
2. Update my.ini if configuration syntax changes
3. Test initialization process

### Changing Port
1. Update my.ini port setting
2. Update App.config connection string
3. Update all scripts that reference port 3310

## Validation Checklist

- [x] MySQL folder contains all required subdirectories (bin, data, lib, share, include)
- [x] mysqld.exe exists in mysql/bin/
- [x] my.ini template has {INSTALLDIR} placeholders
- [x] All batch scripts use correct paths
- [x] schema.sql contains complete database structure
- [x] Inno Setup script copies all required files
- [x] Install-MySQL.bat replaces {INSTALLDIR} correctly
- [x] Application auto-starts MySQL
- [x] Application auto-stops MySQL on exit
- [x] Connection string uses correct port (3310)
- [x] Database initialization works correctly

## Notes

- MySQL runs as a regular process, not a Windows service
- All paths are relative to installation directory
- Database files persist after uninstall (for backup)
- No internet connection required
- Works completely offline

