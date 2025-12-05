# FinalPOS Installation System

Complete installer package for FinalPOS Point of Sale System with integrated portable MySQL server.

## 📦 Package Contents

```
InstallerFiles/
├── mysql/              # Portable MySQL 8.0.44 (ZIP version)
│   ├── bin/            # MySQL executables
│   ├── data/           # Database files (created during install)
│   ├── lib/            # MySQL libraries
│   └── ...
├── app/                # FinalPOS application files
│   ├── FinalPOS.exe    # Main application executable
│   ├── *.dll           # Required DLLs
│   └── ...
├── config/
│   └── my.ini          # MySQL configuration file
├── scripts/
│   ├── init.sql        # Database initialization script
│   ├── schema.sql      # Database schema (to be provided)
│   ├── Start-MySQL.bat # Start MySQL server
│   ├── Stop-MySQL.bat  # Stop MySQL server
│   ├── Check-MySQL.bat # Check MySQL status
│   └── Install-MySQL.bat # Initialize MySQL (used during install)
└── InnoSetupScript.iss # Inno Setup installer script
```

## 🚀 Installation Process

### Prerequisites

1. **Inno Setup Compiler** (version 6.0 or later)
   - Download from: https://jrsoftware.org/isdl.php
   - Install Inno Setup Compiler

2. **Portable MySQL 8.0.44**
   - Download MySQL 8.0.44 ZIP version from: https://dev.mysql.com/downloads/mysql/
   - Extract the ZIP file contents into `InstallerFiles/mysql/` directory
   - Ensure the structure is: `InstallerFiles/mysql/bin/mysqld.exe`

3. **FinalPOS Application Files**
   - Build your FinalPOS application in Release mode
   - Copy all files from `FinalPOS/bin/Release/` to `InstallerFiles/app/`
   - Include all DLLs, config files, and resources

4. **Database Schema**
   - Place your complete `schema.sql` file in `InstallerFiles/scripts/schema.sql`
   - This will be automatically imported during installation

### Building the Installer

1. Open `InnoSetupScript.iss` in Inno Setup Compiler
2. Click **Build** → **Compile** (or press F9)
3. The installer `FinalPOS-Setup.exe` will be created in the same directory

### Installation Steps (What the Installer Does)

1. **Stops any running MySQL instances**
   - Checks for existing mysqld.exe processes
   - Gracefully shuts down MySQL if running

2. **Installs MySQL to C:\FinalPOS-MySQL**
   - Copies all MySQL files
   - Copies `my.ini` configuration file
   - Copies all scripts

3. **Initializes MySQL Database**
   - Creates data directory structure
   - Initializes MySQL with `--initialize-insecure`
   - Starts MySQL server on port 3307

4. **Configures MySQL**
   - Sets root password to `Shivaanica`
   - Creates database `POS_NEXA_ERP`
   - Grants necessary privileges

5. **Imports Database Schema**
   - Executes `init.sql` for user configuration
   - Imports `schema.sql` to create tables and structure

6. **Installs FinalPOS Application**
   - Copies application files to `C:\Program Files\FinalPOS`
   - Creates Start Menu shortcuts
   - Creates desktop shortcut (optional)
   - Creates shortcuts for MySQL management scripts

7. **Creates Management Shortcuts**
   - Start MySQL Server
   - Stop MySQL Server
   - Check MySQL Status

## 🔧 Configuration

### MySQL Configuration (my.ini)

- **Port**: 3307
- **Root Password**: Shivaanica
- **Database**: POS_NEXA_ERP
- **SQL Mode**: NO_ENGINE_SUBSTITUTION,STRICT_TRANS_TABLES
- **Data Directory**: C:\FinalPOS-MySQL\data
- **Base Directory**: C:\FinalPOS-MySQL

### Connection String

The application uses the following connection parameters:
- **Server**: localhost
- **Port**: 3307
- **Database**: POS_NEXA_ERP
- **User**: root
- **Password**: Shivaanica

## 📝 Scripts Reference

### Start-MySQL.bat
Starts the MySQL server on port 3307. Checks if MySQL is already running before starting.

**Usage**: Double-click or run from command line

### Stop-MySQL.bat
Safely stops the MySQL server. Attempts graceful shutdown first, then forces termination if needed.

**Usage**: Double-click or run from command line

### Check-MySQL.bat
Checks MySQL server status:
- Verifies MySQL process is running
- Checks if port 3307 is listening
- Tests database connection
- Shows MySQL version and databases

**Usage**: Double-click or run from command line

### Install-MySQL.bat
Initializes MySQL data directory and runs initialization scripts. This is automatically called during installation but can be run manually if needed.

**Usage**: Run manually only if re-initialization is needed

## 🗑️ Uninstallation

The uninstaller will:
1. Stop the FinalPOS application if running
2. Stop MySQL server
3. Remove application files from `C:\Program Files\FinalPOS`
4. Remove MySQL directory `C:\FinalPOS-MySQL`
5. Remove all shortcuts (Start Menu, Desktop)
6. Remove registry entries

## 🔍 Troubleshooting

### MySQL Won't Start

1. Check if port 3307 is already in use:
   ```cmd
   netstat -an | findstr ":3307"
   ```

2. Verify MySQL files exist:
   ```cmd
   dir C:\FinalPOS-MySQL\bin\mysqld.exe
   ```

3. Check MySQL error log:
   ```cmd
   type C:\FinalPOS-MySQL\data\*.err
   ```

4. Run Check-MySQL.bat to diagnose issues

### Database Connection Errors

1. Ensure MySQL is running (use Check-MySQL.bat)
2. Verify connection string in application config
3. Check root password is correct: `Shivaanica`
4. Verify database exists:
   ```cmd
   C:\FinalPOS-MySQL\bin\mysql.exe -u root -pShivaanica --port=3307 -e "SHOW DATABASES;"
   ```

### Installation Fails

1. Run installer as Administrator
2. Check Windows Event Viewer for errors
3. Review installation log: `%TEMP%\FinalPOS-Install.log`
4. Ensure no antivirus is blocking MySQL files
5. Verify sufficient disk space (at least 500MB free)

### Schema Import Fails

1. Ensure `schema.sql` exists in `InstallerFiles/scripts/`
2. Check SQL syntax is valid
3. Verify MySQL is running before import
4. Manually import schema:
   ```cmd
   C:\FinalPOS-MySQL\bin\mysql.exe -u root -pShivaanica --port=3307 POS_NEXA_ERP < C:\FinalPOS-MySQL\scripts\schema.sql
   ```

## 📋 Manual Installation Steps (If Needed)

If the installer fails, you can install manually:

1. **Extract MySQL**:
   ```cmd
   xcopy /E /I mysql C:\FinalPOS-MySQL
   copy config\my.ini C:\FinalPOS-MySQL\
   xcopy /E /I scripts C:\FinalPOS-MySQL\scripts
   ```

2. **Initialize MySQL**:
   ```cmd
   C:\FinalPOS-MySQL\bin\mysqld.exe --defaults-file=C:\FinalPOS-MySQL\my.ini --initialize-insecure
   ```

3. **Start MySQL**:
   ```cmd
   C:\FinalPOS-MySQL\scripts\Start-MySQL.bat
   ```

4. **Run Initialization**:
   ```cmd
   C:\FinalPOS-MySQL\scripts\Install-MySQL.bat
   ```

5. **Install Application**:
   ```cmd
   xcopy /E /I app "C:\Program Files\FinalPOS"
   ```

## 🔐 Security Notes

- The default root password `Shivaanica` should be changed in production
- MySQL is configured to listen only on localhost (127.0.0.1)
- Consider setting up a dedicated MySQL user for the application
- Regularly backup the `C:\FinalPOS-MySQL\data` directory

## 📞 Support

For issues or questions:
1. Check the troubleshooting section above
2. Review MySQL error logs
3. Check application logs
4. Verify all prerequisites are met

## 📄 License

[Your License Information Here]

---

**Version**: 1.0.0  
**Last Updated**: 2024  
**MySQL Version**: 8.0.44  
**Application**: FinalPOS

