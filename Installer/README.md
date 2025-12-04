# FinalPOS Installer - MySQL + phpMyAdmin Setup

This installer automatically downloads and configures an isolated MySQL Server and phpMyAdmin environment for the FinalPOS application.

## Features

- **Standalone MySQL Server**: Downloads and installs MySQL 8.0.x in portable mode
- **Isolated Installation**: MySQL runs from `{app}\mysql\` - does not interfere with system MySQL
- **Custom Port**: Uses port 3308 (or next available port if occupied)
- **Auto Port Detection**: Automatically finds available ports for MySQL and phpMyAdmin
- **phpMyAdmin**: Downloads and configures phpMyAdmin to connect to the embedded MySQL
- **Database Auto-Creation**: Creates `POS_NEXA_ERP` database automatically
- **App.config Integration**: Updates connection string with detected MySQL port

## Installation Process

1. **Port Detection**: Detects available ports starting from 3308 (MySQL) and 8000 (phpMyAdmin)
2. **MySQL Download**: Downloads MySQL 8.0.x Windows ZIP archive
3. **MySQL Installation**: Extracts MySQL to `{app}\mysql\`
4. **MySQL Configuration**: Creates `my.ini` with detected port and paths
5. **MySQL Initialization**: Initializes data directory with `--initialize-insecure` (blank root password)
6. **MySQL Startup**: Starts MySQL server on detected port
7. **Database Creation**: Creates `POS_NEXA_ERP` database
8. **phpMyAdmin Download**: Downloads phpMyAdmin
9. **phpMyAdmin Installation**: Extracts phpMyAdmin to `{app}\phpmyadmin\`
10. **phpMyAdmin Configuration**: Creates `config.inc.php` with MySQL connection details
11. **App.config Update**: Updates `FinalPOS.exe.config` with detected MySQL port
12. **Batch Files**: Creates `StartMySQL.bat` and `StopMySQL.bat` for manual control

## File Structure

After installation:

```
{app}/
├── FinalPOS.exe
├── FinalPOS.exe.config (updated with MySQL port)
├── StartMySQL.bat
├── StopMySQL.bat
├── mysql/
│   ├── bin/
│   │   ├── mysqld.exe
│   │   ├── mysql.exe
│   │   └── mysqladmin.exe
│   ├── data/ (MySQL data directory)
│   └── my.ini (MySQL configuration)
├── phpmyadmin/
│   ├── index.php
│   └── config.inc.php (auto-generated)
└── PHP/ (required for phpMyAdmin)
    └── php.exe
```

## MySQL Configuration

The `my.ini` file is created with:

```ini
[mysqld]
port={DETECTED_PORT}
basedir={app}\mysql
datadir={app}\mysql\data
skip-grant-tables=0
skip-external-locking
sql-mode="NO_ENGINE_SUBSTITUTION"

[client]
port={DETECTED_PORT}
default-character-set=utf8mb4
```

## Connection String

The installer updates `App.config` with:

```
Server=127.0.0.1;Port={DETECTED_PORT};Database=POS_NEXA_ERP;Uid=root;Pwd=;
```

## Manual MySQL Control

### Start MySQL
Run `StartMySQL.bat` or manually:
```batch
cd {app}
mysql\bin\mysqld.exe --defaults-file=mysql\my.ini --standalone --console
```

### Stop MySQL
Run `StopMySQL.bat` or manually:
```batch
cd {app}
mysql\bin\mysqladmin.exe -u root -h 127.0.0.1 -P {PORT} shutdown
```

## phpMyAdmin Access

After installation, phpMyAdmin is accessible at:
```
http://localhost:{DETECTED_PHPMYADMIN_PORT}
```

Default credentials:
- **Host**: 127.0.0.1
- **Port**: {DETECTED_MYSQL_PORT}
- **User**: root
- **Password**: (blank)

## Port Detection Logic

The installer uses PowerShell to check if ports are in use:

```pascal
function FindAvailablePort(BasePort: Integer): Integer
```

- Starts checking from base port (3308 for MySQL, 8000 for phpMyAdmin)
- Increments until finding an available port
- Maximum 100 attempts

## Requirements

- Windows x64
- Administrator privileges
- Internet connection (for downloading MySQL and phpMyAdmin)
- PHP portable runtime (for phpMyAdmin)

## Notes

- MySQL runs as a standalone process (not a Windows service)
- MySQL data is stored in `{app}\mysql\data\`
- Uninstallation stops MySQL cleanly before removing files
- Port information is logged to `{app}\install-log.txt`

## Troubleshooting

### MySQL won't start
1. Check `{app}\install-log.txt` for errors
2. Verify port is not in use: `netstat -an | findstr "3308"`
3. Check MySQL error log in `{app}\mysql\data\`

### phpMyAdmin won't start
1. Verify PHP is installed in `{app}\PHP\`
2. Check if port 8000 (or detected port) is available
3. Verify `config.inc.php` exists and has correct MySQL port

### Connection issues
1. Verify MySQL is running: `tasklist | findstr mysqld`
2. Check `App.config` has correct port number
3. Test connection: `mysql\bin\mysql.exe -u root -h 127.0.0.1 -P {PORT}`
