# FinalPOS - Standalone Release Dependencies List

**Project:** FinalPOS (Point of Sale System)  
**Target Framework:** .NET Framework 4.7.2  
**Release Type:** Standalone (Without Installer)

---

## 📦 REQUIRED DEPENDENCIES

### 1. **Application Files** (Core Executable)

#### Main Application
- ✅ `FinalPOS.exe` - Main application executable
- ✅ `FinalPOS.exe.config` - Application configuration file (contains connection string)

#### Configuration Requirements
- Database Name: `POS_NEXA_ERP`
- MySQL Server: `localhost` (or `127.0.0.1`)
- Default Port: `3308` (configurable)
- MySQL User: `root`
- MySQL Password: (empty by default)

---

### 2. **.NET Framework Runtime**

#### Required Version
- **.NET Framework 4.7.2** or higher
- **Download:** https://dotnet.microsoft.com/download/dotnet-framework/net472

#### Alternative Minimum Versions
- .NET Framework 4.6.1 (minimum supported per App.config)
- .NET Framework 4.7.1
- .NET Framework 4.7.2 (recommended)

---

### 3. **DLL Dependencies** (26 DLLs + Resource DLLs)

#### Core Application DLLs
All DLLs must be in the same folder as `FinalPOS.exe`:

| DLL Name | Purpose | Version |
|----------|---------|---------|
| `BouncyCastle.Cryptography.dll` | Cryptography library | 2.6.2 |
| `Google.Protobuf.dll` | Protocol buffers | 3.32.0 |
| `K4os.Compression.LZ4.dll` | LZ4 compression | 1.3.8 |
| `K4os.Compression.LZ4.Streams.dll` | LZ4 stream compression | 1.3.8 |
| `K4os.Hash.xxHash.dll` | xxHash hashing | 1.0.8 |
| `MetroFramework.dll` | Metro UI framework | 1.4.0.0 |
| `MetroFramework.Design.dll` | Metro UI design components | 1.4.0.0 |
| `MetroFramework.Fonts.dll` | Metro UI fonts | 1.4.0.0 |
| `Microsoft.Bcl.AsyncInterfaces.dll` | Async interfaces | 5.0.0 |
| `Microsoft.ReportViewer.Common.dll` | Report viewer common | 15.0.0.0 |
| `Microsoft.ReportViewer.DataVisualization.dll` | Report data visualization | 15.0.0.0 |
| `Microsoft.ReportViewer.Design.dll` | Report viewer design | 15.0.0.0 |
| `Microsoft.ReportViewer.ProcessingObjectModel.dll` | Report processing | 15.0.0.0 |
| `Microsoft.ReportViewer.WinForms.dll` | Report viewer WinForms | 12.0.0.0 |
| `Microsoft.SqlServer.Types.dll` | SQL Server types | 14.0.0.0 |
| `MySql.Data.dll` | MySQL .NET connector | 9.5.0 |
| `System.Buffers.dll` | System buffers | 4.5.1 |
| `System.Configuration.ConfigurationManager.dll` | Configuration manager | 8.0.0 |
| `System.IO.Pipelines.dll` | IO pipelines | 5.0.2 |
| `System.Memory.dll` | System memory | 4.5.5 |
| `System.Numerics.Vectors.dll` | Numerics vectors | 4.5.0 |
| `System.Runtime.CompilerServices.Unsafe.dll` | Unsafe compiler services | 6.0.0 |
| `System.Threading.Tasks.Extensions.dll` | Task extensions | 4.5.4 |
| `Tulpep.NotificationWindow.dll` | Notification window | 1.1.37 |
| `ZstdSharp.dll` | Zstandard compression | 0.8.6 |
| `EnvDTE.dll` | Visual Studio automation | (included) |

#### Resource DLLs (Localization)
These folders contain localized resources for ReportViewer:

- `de\` - German resources
- `es\` - Spanish resources
- `fr\` - French resources
- `it\` - Italian resources
- `ja\` - Japanese resources
- `ko\` - Korean resources
- `pt\` - Portuguese resources
- `ru\` - Russian resources
- `zh-CHS\` - Simplified Chinese resources
- `zh-CHT\` - Traditional Chinese resources

Each folder contains:
- `Microsoft.ReportViewer.Common.resources.dll`
- `Microsoft.ReportViewer.DataVisualization.resources.dll`
- `Microsoft.ReportViewer.Design.resources.dll`

---

### 4. **MySQL Server** (Standalone/Portable)

#### Required Version
- **MySQL Server 8.0.40** (Windows ZIP Archive - No Installer)
- **Download URL:** https://dev.mysql.com/get/Downloads/MySQL-8.0/mysql-8.0.40-winx64.zip
- **Architecture:** x64 (64-bit)

#### Required MySQL Files Structure
```
mysql\
├── bin\
│   ├── mysqld.exe          (MySQL server daemon)
│   ├── mysql.exe           (MySQL client)
│   ├── mysqladmin.exe      (MySQL administration)
│   └── [other MySQL binaries]
├── data\                   (Database data directory - created during initialization)
├── lib\
├── share\
└── my.ini                  (MySQL configuration file - must be created)
```

#### MySQL Configuration (`my.ini`)
Required settings:
```ini
[mysqld]
port=3308
basedir="[MySQL_PATH]"
datadir="[MySQL_PATH]\data"
skip-grant-tables=0
skip-external-locking
sql-mode="NO_ENGINE_SUBSTITUTION"

[client]
port=3308
default-character-set=utf8mb4
```

#### MySQL Initialization
- Must run: `mysqld.exe --initialize-insecure --datadir="[path]\data"`
- Creates default database structure
- Root user has no password (empty password)

---

### 5. **PHP Runtime** (For phpMyAdmin)

#### Required Version
- **PHP 7.4+** or **PHP 8.x** (Portable/Thread-Safe)
- **Architecture:** x64 (64-bit)
- **Thread Safety:** Thread-Safe (TS) version

#### Required PHP Files Structure
```
PHP\
├── php.exe                 (PHP executable)
├── php.ini                 (PHP configuration - must be configured)
├── php8ts.dll             (PHP runtime DLL)
├── ext\                    (PHP extensions folder)
│   ├── php_mysqli.dll     (MySQLi extension - REQUIRED)
│   ├── php_openssl.dll     (OpenSSL extension - REQUIRED)
│   ├── php_mbstring.dll    (Multibyte string - REQUIRED)
│   ├── php_pdo_mysql.dll   (PDO MySQL - REQUIRED)
│   └── php_curl.dll        (cURL extension - REQUIRED)
└── [other PHP runtime files]
```

#### PHP Configuration (`php.ini`)
Required settings:
```ini
extension_dir = ".\ext"
extension=mysqli
extension=openssl
extension=mbstring
extension=pdo_mysql
extension=curl
file_uploads = On
session.auto_start = Off
max_execution_time = 300
upload_max_filesize = 50M
post_max_size = 50M
short_open_tag = Off
```

---

### 6. **phpMyAdmin** (Database Management Interface)

#### Required Version
- **phpMyAdmin 5.2.1** (All Languages)
- **Download URL:** https://files.phpmyadmin.net/phpMyAdmin/5.2.1/phpMyAdmin-5.2.1-all-languages.zip

#### Required phpMyAdmin Files Structure
```
phpmyadmin\
├── index.php
├── config.inc.php          (Configuration file - must be created)
├── libraries\
├── themes\
└── [other phpMyAdmin files]
```

#### phpMyAdmin Configuration (`config.inc.php`)
Required settings:
```php
<?php
$cfg['blowfish_secret'] = '[random_string]';
$i = 0;
$i++;
$cfg['Servers'][$i]['auth_type'] = 'config';
$cfg['Servers'][$i]['host'] = '127.0.0.1';
$cfg['Servers'][$i]['port'] = '3308';
$cfg['Servers'][$i]['user'] = 'root';
$cfg['Servers'][$i]['password'] = '';
?>
```

---

### 7. **Management Scripts** (Batch Files)

#### StartMySQL.bat
```batch
@echo off
echo Starting MySQL Server...
cd /d "%~dp0"
start /B "" "[PATH]\mysql\bin\mysqld.exe" --defaults-file="[PATH]\mysql\my.ini" --standalone --console
timeout /t 3 /nobreak >nul
echo MySQL Server started on port 3308
```

#### StopMySQL.bat
```batch
@echo off
echo Stopping MySQL Server...
cd /d "%~dp0"
"[PATH]\mysql\bin\mysqladmin.exe" -u root -h 127.0.0.1 -P 3308 shutdown
timeout /t 2 /nobreak >nul
taskkill /F /IM mysqld.exe 2>nul
echo MySQL Server stopped
```

---

## 📋 COMPLETE FOLDER STRUCTURE

```
FinalPOS_Release\
├── FinalPOS.exe
├── FinalPOS.exe.config
├── [26 DLL files]
├── [10 localization folders: de, es, fr, it, ja, ko, pt, ru, zh-CHS, zh-CHT]
│
├── mysql\
│   ├── bin\
│   │   ├── mysqld.exe
│   │   ├── mysql.exe
│   │   └── mysqladmin.exe
│   ├── data\              (created during initialization)
│   ├── lib\
│   ├── share\
│   └── my.ini
│
├── PHP\
│   ├── php.exe
│   ├── php.ini
│   ├── php8ts.dll
│   └── ext\
│       ├── php_mysqli.dll
│       ├── php_openssl.dll
│       ├── php_mbstring.dll
│       ├── php_pdo_mysql.dll
│       └── php_curl.dll
│
├── phpmyadmin\
│   ├── index.php
│   ├── config.inc.php
│   ├── libraries\
│   └── [other files]
│
├── StartMySQL.bat
├── StopMySQL.bat
│
└── [Optional: Database initialization scripts]
```

---

## 🔧 SETUP REQUIREMENTS

### Pre-Installation Steps

1. **Extract MySQL Server**
   - Download MySQL 8.0.40 ZIP archive
   - Extract to `mysql\` folder
   - Create `my.ini` configuration file

2. **Extract PHP Runtime**
   - Download PHP portable (Thread-Safe)
   - Extract to `PHP\` folder
   - Rename `php.ini-development` to `php.ini`
   - Configure `php.ini` (enable extensions)

3. **Extract phpMyAdmin**
   - Download phpMyAdmin 5.2.1
   - Extract to `phpmyadmin\` folder
   - Create `config.inc.php` configuration file

4. **Initialize MySQL Database**
   - Run: `mysql\bin\mysqld.exe --initialize-insecure --datadir="mysql\data"`
   - Wait for initialization to complete
   - Start MySQL server
   - Create database: `CREATE DATABASE POS_NEXA_ERP DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;`

5. **Update App.config**
   - Set correct MySQL port in connection string
   - Format: `Server=127.0.0.1;Port=3308;Database=POS_NEXA_ERP;Uid=root;Pwd=;`

---

## ⚠️ SYSTEM REQUIREMENTS

### Operating System
- **Windows 7 SP1** or higher
- **Windows 8/8.1**
- **Windows 10**
- **Windows 11**
- **Windows Server 2012 R2** or higher

### Architecture
- **64-bit (x64)** required for MySQL and PHP

### Prerequisites
- **.NET Framework 4.7.2** (or higher)
- **Visual C++ Redistributable** (for MySQL and PHP)
  - Visual C++ 2015-2022 Redistributable (x64)
  - Download: https://aka.ms/vs/17/release/vc_redist.x64.exe

### Disk Space
- **Minimum:** 500 MB (application + DLLs)
- **MySQL:** ~200 MB (server + data)
- **PHP:** ~50 MB
- **phpMyAdmin:** ~30 MB
- **Total:** ~800 MB (minimum)
- **Recommended:** 2 GB (for database growth)

### Memory (RAM)
- **Minimum:** 2 GB
- **Recommended:** 4 GB or more
- MySQL requires ~512 MB RAM minimum

---

## 📝 DEPENDENCY SUMMARY TABLE

| Category | Component | Version | Size (approx) | Required |
|----------|-----------|---------|---------------|----------|
| **Runtime** | .NET Framework | 4.7.2+ | ~50 MB | ✅ Yes |
| **Runtime** | Visual C++ Redistributable | 2015-2022 | ~20 MB | ✅ Yes |
| **Application** | FinalPOS.exe | 1.0 | ~5 MB | ✅ Yes |
| **Application** | DLLs (26 files) | Various | ~50 MB | ✅ Yes |
| **Database** | MySQL Server | 8.0.40 | ~200 MB | ✅ Yes |
| **Database** | MySQL Data | (initialized) | ~50 MB | ✅ Yes |
| **Web Server** | PHP Runtime | 8.x | ~50 MB | ✅ Yes |
| **Web Interface** | phpMyAdmin | 5.2.1 | ~30 MB | ✅ Yes |
| **Scripts** | Batch Files | N/A | <1 MB | ✅ Yes |

---

## 🚀 QUICK DEPLOYMENT CHECKLIST

- [ ] Copy `FinalPOS.exe` and `FinalPOS.exe.config`
- [ ] Copy all 26 DLL files to same folder
- [ ] Copy all localization folders (de, es, fr, etc.)
- [ ] Extract MySQL Server to `mysql\` folder
- [ ] Create `mysql\my.ini` configuration file
- [ ] Initialize MySQL data directory
- [ ] Extract PHP to `PHP\` folder
- [ ] Configure `PHP\php.ini` (enable extensions)
- [ ] Extract phpMyAdmin to `phpmyadmin\` folder
- [ ] Create `phpmyadmin\config.inc.php`
- [ ] Create `StartMySQL.bat` script
- [ ] Create `StopMySQL.bat` script
- [ ] Update `FinalPOS.exe.config` with correct MySQL port
- [ ] Test MySQL server startup
- [ ] Test PHP server startup
- [ ] Test application connection to database
- [ ] Verify phpMyAdmin access

---

## 📞 SUPPORT & TROUBLESHOOTING

### Common Issues

1. **"MySQL Server failed to start"**
   - Check if port 3308 is available
   - Verify `my.ini` configuration
   - Check Windows Firewall settings

2. **"Cannot connect to database"**
   - Verify MySQL server is running
   - Check connection string in `FinalPOS.exe.config`
   - Verify database `POS_NEXA_ERP` exists

3. **"phpMyAdmin not accessible"**
   - Verify PHP server is running
   - Check `config.inc.php` settings
   - Verify PHP extensions are enabled

4. **"Missing DLL errors"**
   - Ensure all DLLs are in same folder as `FinalPOS.exe`
   - Verify .NET Framework 4.7.2+ is installed
   - Check Visual C++ Redistributable is installed

---

**Document Version:** 1.0  
**Last Updated:** 2025-01-XX  
**Maintained By:** FinalPOS Development Team

