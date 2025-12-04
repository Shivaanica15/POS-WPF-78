# FinalPOS Installer - Complete Setup Guide

## Overview

This installer creates a completely isolated MySQL + phpMyAdmin environment for your POS system. Everything runs from your application directory and does not interfere with any existing MySQL installations.

## Quick Start

1. **Compile the installer** using Inno Setup Compiler
2. **Run the installer** - it will automatically:
   - Download MySQL 8.0.x
   - Download phpMyAdmin
   - Detect available ports
   - Configure everything
   - Create database
   - Update App.config

## Prerequisites

Before compiling the installer, ensure you have:

1. **PHP Portable Runtime** in `PHP\` folder:
   ```
   PHP\
   ├── php.exe
   ├── php.ini
   └── ext\ (extensions)
   ```

2. **FinalPOS Release Build** in `FinalPOS\bin\Release\`

3. **Inno Setup Compiler** (version 6.0 or later)

## Folder Structure Before Compilation

```
ProjectRoot\
├── FinalPOS_Installer.iss
├── FinalPOS\
│   └── bin\Release\ (your compiled application)
├── PHP\
│   ├── php.exe
│   ├── php.ini
│   └── ext\
└── Output\ (created by Inno Setup)
```

## Installation Process Details

### Phase 1: Port Detection
- Checks port 3308 for MySQL (increments if occupied)
- Checks port 8000 for phpMyAdmin (increments if occupied)
- Logs detected ports to `install-log.txt`

### Phase 2: MySQL Installation
1. Downloads MySQL ZIP from official MySQL website
2. Extracts to temporary directory
3. Copies to `{app}\mysql\`
4. Creates `my.ini` configuration file
5. Initializes data directory with `--initialize-insecure`
6. Starts MySQL server
7. Creates `POS_NEXA_ERP` database

### Phase 3: phpMyAdmin Installation
1. Downloads phpMyAdmin ZIP
2. Extracts to `{app}\phpmyadmin\`
3. Creates `config.inc.php` with MySQL connection details
4. Starts PHP built-in server
5. Opens browser to phpMyAdmin

### Phase 4: Application Configuration
1. Updates `FinalPOS.exe.config` with detected MySQL port
2. Creates `StartMySQL.bat` and `StopMySQL.bat`
3. Logs all configuration details

## Configuration Files Generated

### MySQL Configuration (`{app}\mysql\my.ini`)
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

### phpMyAdmin Configuration (`{app}\phpmyadmin\config.inc.php`)
```php
<?php
$cfg['blowfish_secret'] = '{TIMESTAMP}';
$i = 0;
$i++;
$cfg['Servers'][$i]['auth_type'] = 'config';
$cfg['Servers'][$i]['host'] = '127.0.0.1';
$cfg['Servers'][$i]['port'] = '{DETECTED_MYSQL_PORT}';
$cfg['Servers'][$i]['user'] = 'root';
$cfg['Servers'][$i]['password'] = '';
?>
```

### App.config Connection String
```xml
<add name="FinalPOS.Properties.Settings.NewOneConnectionString" 
     connectionString="Server=127.0.0.1;Port={DETECTED_PORT};Database=POS_NEXA_ERP;Uid=root;Pwd=;" 
     providerName="MySql.Data.MySqlClient" />
```

## Batch Files Created

### StartMySQL.bat
```batch
@echo off
echo Starting MySQL Server...
cd /d "%~dp0"
start /B "" "{app}\mysql\bin\mysqld.exe" --defaults-file="{app}\mysql\my.ini" --standalone --console
timeout /t 3 /nobreak >nul
echo MySQL Server started on port {PORT}
```

### StopMySQL.bat
```batch
@echo off
echo Stopping MySQL Server...
cd /d "%~dp0"
"{app}\mysql\bin\mysqladmin.exe" -u root -h 127.0.0.1 -P {PORT} shutdown
timeout /t 2 /nobreak >nul
taskkill /F /IM mysqld.exe 2>nul
echo MySQL Server stopped
```

## Customization

### Change MySQL Base Port
Edit line 12 in `FinalPOS_Installer.iss`:
```pascal
#define MySQLBasePort 3308  // Change to your preferred port
```

### Change phpMyAdmin Base Port
Edit line 13 in `FinalPOS_Installer.iss`:
```pascal
#define phpMyAdminBasePort 8000  // Change to your preferred port
```

### Change MySQL Version
Edit lines 16-18 in `FinalPOS_Installer.iss`:
```pascal
#define MySQLVersion "8.0.40"
#define MySQLDownloadURL "https://dev.mysql.com/get/Downloads/MySQL-8.0/mysql-{#MySQLVersion}-winx64.zip"
#define MySQLZipName "mysql-{#MySQLVersion}-winx64.zip"
```

### Change phpMyAdmin Version
Edit lines 21-23 in `FinalPOS_Installer.iss`:
```pascal
#define phpMyAdminVersion "5.2.1"
#define phpMyAdminDownloadURL "https://files.phpmyadmin.net/phpMyAdmin/{#phpMyAdminVersion}/phpMyAdmin-{#phpMyAdminVersion}-all-languages.zip"
#define phpMyAdminZipName "phpMyAdmin-{#phpMyAdminVersion}-all-languages.zip"
```

## Troubleshooting

### Installation Fails at Download Step
- **Check internet connection**
- **Verify download URLs are accessible**
- **Check firewall/antivirus settings**
- **Review `{tmp}\install-log.txt`**

### MySQL Won't Start
- **Check port availability**: `netstat -an | findstr "3308"`
- **Check MySQL error log**: `{app}\mysql\data\*.err`
- **Verify my.ini exists and is correct**
- **Check disk space**

### phpMyAdmin Won't Start
- **Verify PHP exists**: `{app}\PHP\php.exe`
- **Check PHP port**: `netstat -an | findstr "8000"`
- **Verify config.inc.php exists**

### App Can't Connect to Database
- **Verify MySQL is running**: `tasklist | findstr mysqld`
- **Check App.config port matches MySQL port**
- **Test connection manually**: `{app}\mysql\bin\mysql.exe -u root -h 127.0.0.1 -P {PORT}`

### Port Detection Issues
- **Check PowerShell is available**
- **Verify netstat works**: `netstat -an`
- **Review port detection logic in installer script**

## Manual Database Schema Import

If you need to import a custom schema.sql file, add this function to the installer:

```pascal
function ImportSchema: Boolean;
var
  MySQLExe: String;
  SchemaFile: String;
  ResultCode: Integer;
begin
  Result := False;
  MySQLExe := ExpandConstant('{app}\mysql\bin\mysql.exe');
  SchemaFile := ExpandConstant('{app}\schema.sql');
  
  if FileExists(SchemaFile) then
  begin
    if Exec(MySQLExe, '-u root -h 127.0.0.1 -P ' + IntToStr(DetectedMySQLPort) + ' {#DatabaseName} < "' + SchemaFile + '"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
    begin
      Result := (ResultCode = 0);
    end;
  end;
end;
```

Then call it in `CurStepChanged` after `CreateDatabase`.

## Uninstallation

The uninstaller automatically:
1. Stops MySQL server (via StopMySQL.bat)
2. Stops PHP server
3. Removes all files
4. Removes registry entries

**Note**: MySQL data directory is removed during uninstall. Backup data if needed!

## Security Considerations

- **Root password is blank** - Consider adding password protection for production
- **MySQL runs on localhost only** - Not accessible from network
- **phpMyAdmin runs on localhost only** - Not accessible from network
- **No Windows service** - MySQL runs as user process

## Performance Notes

- **First startup**: MySQL initialization takes 2-5 minutes
- **Subsequent startups**: MySQL starts in 5-10 seconds
- **Memory usage**: ~100-200 MB for MySQL + phpMyAdmin
- **Disk space**: ~500 MB for MySQL + phpMyAdmin

## Support

For issues or questions:
1. Check `{app}\install-log.txt`
2. Check `{app}\uninstall-log.txt` (if uninstalling)
3. Review MySQL error logs in `{app}\mysql\data\`
4. Test components individually (MySQL, PHP, phpMyAdmin)
