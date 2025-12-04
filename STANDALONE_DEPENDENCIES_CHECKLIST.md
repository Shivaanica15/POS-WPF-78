# FinalPOS.exe - Standalone Dependencies Checklist

**Purpose:** Complete list of files/folders needed to package with `FinalPOS.exe` for standalone deployment  
**Target:** Create installable application WITHOUT using installer tools  
**Date:** 2025-01-XX

---

## ✅ QUICK CHECKLIST

### 🔴 REQUIRED (For FinalPOS.exe to Run)

- [ ] **FinalPOS.exe** (main application)
- [ ] **FinalPOS.exe.config** (configuration file)
- [ ] **26 DLL files** (see list below)
- [ ] **MySQL Server** (extracted and configured)
- [ ] **StartMySQL.bat** (management script)
- [ ] **StopMySQL.bat** (management script)

### 🟢 OPTIONAL (Only if you want phpMyAdmin web interface)

- [ ] **PHP Runtime** (extracted and configured) - **ONLY NEEDED FOR PHPMYADMIN**
- [ ] **phpMyAdmin** (extracted and configured) - **ONLY NEEDED FOR WEB DATABASE MANAGEMENT**
- [ ] **10 localization folders** (optional, see below)

---

## 📦 DETAILED FILE LIST

### 1. APPLICATION FILES (Required)

#### Core Executable
```
✅ FinalPOS.exe                    (Main application - REQUIRED)
✅ FinalPOS.exe.config             (Configuration - REQUIRED)
```

#### DLL Files (26 files - ALL REQUIRED)

**Copy ALL these DLLs to the same folder as FinalPOS.exe:**

```
✅ BouncyCastle.Cryptography.dll
✅ EnvDTE.dll
✅ Google.Protobuf.dll
✅ K4os.Compression.LZ4.dll
✅ K4os.Compression.LZ4.Streams.dll
✅ K4os.Hash.xxHash.dll
✅ MetroFramework.Design.dll
✅ MetroFramework.dll
✅ MetroFramework.Fonts.dll
✅ Microsoft.Bcl.AsyncInterfaces.dll
✅ Microsoft.ReportViewer.Common.dll
✅ Microsoft.ReportViewer.DataVisualization.dll
✅ Microsoft.ReportViewer.Design.dll
✅ Microsoft.ReportViewer.ProcessingObjectModel.dll
✅ Microsoft.ReportViewer.WinForms.dll
✅ Microsoft.SqlServer.Types.dll
✅ MySql.Data.dll
✅ System.Buffers.dll
✅ System.Configuration.ConfigurationManager.dll
✅ System.IO.Pipelines.dll
✅ System.Memory.dll
✅ System.Numerics.Vectors.dll
✅ System.Runtime.CompilerServices.Unsafe.dll
✅ System.Threading.Tasks.Extensions.dll
✅ Tulpep.NotificationWindow.dll
✅ ZstdSharp.dll
```

**Total DLL Size:** ~50 MB

#### Localization Folders (Optional - Only if needed)

**Copy these folders ONLY if you need multi-language support:**

```
📁 de\          (German - ~500 KB)
📁 es\          (Spanish - ~500 KB)
📁 fr\          (French - ~500 KB)
📁 it\          (Italian - ~500 KB)
📁 ja\          (Japanese - ~500 KB)
📁 ko\          (Korean - ~500 KB)
📁 pt\          (Portuguese - ~500 KB)
📁 ru\          (Russian - ~500 KB)
📁 zh-CHS\      (Simplified Chinese - ~500 KB)
📁 zh-CHT\      (Traditional Chinese - ~500 KB)
```

**Each folder contains:**
- Microsoft.ReportViewer.Common.resources.dll
- Microsoft.ReportViewer.DataVisualization.resources.dll
- Microsoft.ReportViewer.Design.resources.dll

**Note:** If you only need English, skip all localization folders (saves ~5 MB)

---

### 2. DATABASE SERVER (MySQL - Required)

#### MySQL Server Structure

**Download:** MySQL 8.0.40 Windows ZIP (no installer)  
**URL:** https://dev.mysql.com/get/Downloads/MySQL-8.0/mysql-8.0.40-winx64.zip  
**Size:** ~200 MB (compressed), ~250 MB (extracted)

**Required Folder Structure:**
```
📁 mysql\
   ├── 📁 bin\
   │   ├── ✅ mysqld.exe          (REQUIRED - MySQL server)
   │   ├── ✅ mysql.exe           (REQUIRED - MySQL client)
   │   ├── ✅ mysqladmin.exe      (REQUIRED - MySQL admin)
   │   └── [other MySQL binaries]
   │
   ├── 📁 data\                   (Created during initialization)
   │   └── [database files - created automatically]
   │
   ├── 📁 lib\                    (MySQL libraries)
   ├── 📁 share\                  (MySQL shared files)
   │
   └── ✅ my.ini                  (REQUIRED - Configuration file)
```

#### MySQL Configuration File (my.ini)

**Create this file:** `mysql\my.ini`

```ini
[mysqld]
port=3308
basedir="C:\YourAppPath\mysql"
datadir="C:\YourAppPath\mysql\data"
skip-grant-tables=0
skip-external-locking
sql-mode="NO_ENGINE_SUBSTITUTION"

[client]
port=3308
default-character-set=utf8mb4
```

**Important:** Replace `C:\YourAppPath` with your actual application path!

#### MySQL Initialization Steps

**Before first run, you MUST initialize MySQL:**

1. Create `mysql\data` folder (if not exists)
2. Run this command:
   ```
   mysql\bin\mysqld.exe --defaults-file="mysql\my.ini" --initialize-insecure --datadir="mysql\data"
   ```
3. Wait for initialization to complete (1-5 minutes)
4. Start MySQL server (see StartMySQL.bat below)

---

### 3. PHP RUNTIME (⚠️ OPTIONAL - Only Required if Using phpMyAdmin)

#### PHP Runtime Structure

**⚠️ IMPORTANT:** PHP is **NOT required** for FinalPOS.exe to run.  
PHP is **ONLY needed** if you want to use phpMyAdmin (web-based database management tool).

**If you don't need phpMyAdmin, skip PHP entirely!**

**Download:** PHP 8.x Thread-Safe (TS) x64 Portable  
**URL:** https://windows.php.net/download/  
**Size:** ~50 MB (compressed), ~50 MB (extracted)

**Required Folder Structure:**
```
📁 PHP\
   ├── ✅ php.exe                 (REQUIRED - PHP executable)
   ├── ✅ php.ini                 (REQUIRED - PHP configuration)
   ├── ✅ php8ts.dll             (REQUIRED - PHP runtime DLL)
   │
   ├── 📁 ext\                    (PHP extensions folder)
   │   ├── ✅ php_mysqli.dll     (REQUIRED - MySQL extension)
   │   ├── ✅ php_openssl.dll     (REQUIRED - SSL support)
   │   ├── ✅ php_mbstring.dll    (REQUIRED - Unicode support)
   │   ├── ✅ php_pdo_mysql.dll   (REQUIRED - PDO MySQL)
   │   └── ✅ php_curl.dll         (REQUIRED - cURL support)
   │
   └── [other PHP runtime files]
```

#### PHP Configuration File (php.ini)

**Create/Edit:** `PHP\php.ini`

**Required Settings:**
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

**Note:** If `php.ini` doesn't exist, rename `php.ini-development` to `php.ini`

---

### 4. PHPMYADMIN (Optional - Database Management Tool)

#### phpMyAdmin Structure

**Download:** phpMyAdmin 5.2.1 All Languages  
**URL:** https://files.phpmyadmin.net/phpMyAdmin/5.2.1/phpMyAdmin-5.2.1-all-languages.zip  
**Size:** ~30 MB (compressed), ~30 MB (extracted)

**Required Folder Structure:**
```
📁 phpmyadmin\
   ├── ✅ index.php               (REQUIRED - Main entry point)
   ├── ✅ config.inc.php          (REQUIRED - Configuration - CREATE THIS)
   ├── 📁 libraries\
   ├── 📁 themes\
   └── [other phpMyAdmin files]
```

#### phpMyAdmin Configuration File (config.inc.php)

**Create this file:** `phpmyadmin\config.inc.php`

```php
<?php
$cfg['blowfish_secret'] = 'a8b7c6d5e4f3g2h1i0j9k8l7m6n5o4';

$i = 0;
$i++;
$cfg['Servers'][$i]['auth_type'] = 'config';
$cfg['Servers'][$i]['host'] = '127.0.0.1';
$cfg['Servers'][$i]['port'] = '3308';
$cfg['Servers'][$i]['user'] = 'root';
$cfg['Servers'][$i]['password'] = '';
?>
```

**Important:** Change `port` to match your MySQL port (default: 3308)

---

### 5. MANAGEMENT SCRIPTS (Required)

#### ⚠️ IMPORTANT: Why PHP is Needed

**phpMyAdmin is a PHP web application** - it's written in PHP language. To run phpMyAdmin, you need:
1. **PHP Runtime** - Executes PHP scripts (like `index.php` in phpMyAdmin)
2. **PHP Web Server** - Serves phpMyAdmin web pages (PHP has built-in web server)
3. **PHP Extensions** - Connect to MySQL database

**Without PHP:**
- phpMyAdmin files are just text files
- They cannot execute
- You cannot access phpMyAdmin in browser

**With PHP:**
- PHP executes phpMyAdmin scripts
- PHP serves web pages on a port (e.g., http://localhost:8000)
- You can access phpMyAdmin in your browser

#### StartMySQL.bat

**Create this file:** `StartMySQL.bat` (in root folder)

```batch
@echo off
echo Starting MySQL Server...
cd /d "%~dp0"
start /B "" "mysql\bin\mysqld.exe" --defaults-file="mysql\my.ini" --standalone --console
timeout /t 3 /nobreak >nul
echo MySQL Server started on port 3308
pause
```

#### StopMySQL.bat

**Create this file:** `StopMySQL.bat` (in root folder)

```batch
@echo off
echo Stopping MySQL Server...
cd /d "%~dp0"
mysql\bin\mysqladmin.exe -u root -h 127.0.0.1 -P 3308 shutdown
timeout /t 2 /nobreak >nul
taskkill /F /IM mysqld.exe 2>nul
echo MySQL Server stopped
pause
```

#### StartphpMyAdmin.bat

**Create this file:** `StartphpMyAdmin.bat` (in root folder)

**This script starts PHP's built-in web server to run phpMyAdmin:**

```batch
@echo off
echo Starting phpMyAdmin Web Server...
cd /d "%~dp0"

REM Check if MySQL is running first
echo Checking MySQL connection...
mysql\bin\mysql.exe -u root -h 127.0.0.1 -P 3308 -e "SELECT 1" >nul 2>&1
if errorlevel 1 (
    echo ERROR: MySQL is not running!
    echo Please start MySQL first using StartMySQL.bat
    pause
    exit /b 1
)

REM Start PHP built-in web server
echo Starting PHP web server on http://localhost:8000
start "phpMyAdmin Server" "PHP\php.exe" -S localhost:8000 -t "phpmyadmin"

timeout /t 2 /nobreak >nul
echo.
echo phpMyAdmin is now running!
echo Open your browser and go to: http://localhost:8000
echo.
echo Press any key to stop the server...
pause >nul

REM Stop PHP server
taskkill /F /IM php.exe 2>nul
echo phpMyAdmin server stopped.
```

#### StopphpMyAdmin.bat

**Create this file:** `StopphpMyAdmin.bat` (in root folder)

```batch
@echo off
echo Stopping phpMyAdmin Server...
taskkill /F /IM php.exe 2>nul
if errorlevel 1 (
    echo phpMyAdmin server was not running.
) else (
    echo phpMyAdmin server stopped successfully.
)
pause
```

---

## 📋 COMPLETE FOLDER STRUCTURE

```
YourAppFolder\
│
├── ✅ FinalPOS.exe
├── ✅ FinalPOS.exe.config
│
├── ✅ [26 DLL files listed above]
│
├── 📁 de\              (Optional - German)
├── 📁 es\              (Optional - Spanish)
├── 📁 fr\              (Optional - French)
├── 📁 it\              (Optional - Italian)
├── 📁 ja\              (Optional - Japanese)
├── 📁 ko\              (Optional - Korean)
├── 📁 pt\              (Optional - Portuguese)
├── 📁 ru\              (Optional - Russian)
├── 📁 zh-CHS\          (Optional - Simplified Chinese)
├── 📁 zh-CHT\          (Optional - Traditional Chinese)
│
├── 📁 mysql\
│   ├── 📁 bin\
│   │   ├── mysqld.exe
│   │   ├── mysql.exe
│   │   └── mysqladmin.exe
│   ├── 📁 data\        (Created during initialization)
│   ├── 📁 lib\
│   ├── 📁 share\
│   └── my.ini
│
├── 📁 PHP\                    (⚠️ OPTIONAL - Only if using phpMyAdmin)
│   ├── php.exe
│   ├── php.ini
│   ├── php8ts.dll
│   └── 📁 ext\
│       ├── php_mysqli.dll
│       ├── php_openssl.dll
│       ├── php_mbstring.dll
│       ├── php_pdo_mysql.dll
│       └── php_curl.dll
│
├── 📁 phpmyadmin\             (⚠️ OPTIONAL - Only if using phpMyAdmin)
│   ├── index.php
│   ├── config.inc.php
│   └── [other files]
│
├── ✅ StartMySQL.bat
├── ✅ StopMySQL.bat
├── ✅ StartphpMyAdmin.bat      (Starts PHP web server for phpMyAdmin)
└── ✅ StopphpMyAdmin.bat        (Stops PHP web server)
```

---

## 🔧 SETUP INSTRUCTIONS

### Step 1: Copy Application Files
1. Copy `FinalPOS.exe` and `FinalPOS.exe.config` to your deployment folder
2. Copy all 26 DLL files to the same folder
3. (Optional) Copy localization folders if needed

### Step 2: Setup MySQL
1. Download MySQL 8.0.40 ZIP archive
2. Extract to `mysql\` folder
3. Create `mysql\my.ini` configuration file (use template above)
4. **Update paths in my.ini** to match your actual folder path
5. Initialize MySQL data directory:
   ```
   mysql\bin\mysqld.exe --defaults-file="mysql\my.ini" --initialize-insecure --datadir="mysql\data"
   ```

### Step 3: Setup PHP (⚠️ OPTIONAL - Skip if you don't need phpMyAdmin)
**Note:** PHP is ONLY needed for phpMyAdmin. If you don't need web-based database management, skip this step entirely.

1. Download PHP 8.x Thread-Safe x64 Portable
2. Extract to `PHP\` folder
3. Rename `php.ini-development` to `php.ini`
4. Edit `php.ini` and enable required extensions (see template above)

### Step 4: Setup phpMyAdmin (⚠️ OPTIONAL - Skip if you don't need web database management)
1. Download phpMyAdmin 5.2.1 All Languages
2. Extract to `phpmyadmin\` folder
3. Create `phpmyadmin\config.inc.php` (use template above)
4. Update port number in config.inc.php if different from 3308

### Step 5: Create Management Scripts
1. Create `StartMySQL.bat` (use template above)
2. Create `StopMySQL.bat` (use template above)
3. Update paths in batch files if needed

### Step 6: Update Configuration
1. Edit `FinalPOS.exe.config`
2. Update connection string:
   ```xml
   <add name="FinalPOS.Properties.Settings.NewOneConnectionString" 
        connectionString="Server=127.0.0.1;Port=3308;Database=POS_NEXA_ERP;Uid=root;Pwd=;" 
        providerName="MySql.Data.MySqlClient" />
   ```
3. Change `Port=3308` if you used a different port

### Step 7: Initialize Database
1. Start MySQL: Run `StartMySQL.bat`
2. Wait 5-10 seconds for MySQL to start
3. Create database:
   ```
   mysql\bin\mysql.exe -u root -h 127.0.0.1 -P 3308 -e "CREATE DATABASE IF NOT EXISTS POS_NEXA_ERP DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"
   ```
4. Run FinalPOS.exe - it will create tables automatically

### Step 8: Run phpMyAdmin (Optional)
1. Make sure MySQL is running (use StartMySQL.bat)
2. Run `StartphpMyAdmin.bat`
3. Open browser and go to: `http://localhost:8000`
4. You should see phpMyAdmin login page
5. Login with:
   - **Server:** 127.0.0.1:3308
   - **Username:** root
   - **Password:** (leave empty)

---

## ⚠️ SYSTEM REQUIREMENTS

### Operating System
- ✅ Windows 7 SP1 (64-bit) or higher
- ✅ Windows 8/8.1 (64-bit)
- ✅ Windows 10 (64-bit) - **Recommended**
- ✅ Windows 11 (64-bit) - **Recommended**
- ✅ Windows Server 2012 R2+ (64-bit)

### Prerequisites (Must Install First)

#### 1. .NET Framework 4.7.2 or Higher
- **Download:** https://dotnet.microsoft.com/download/dotnet-framework/net472
- **Size:** ~60 MB
- **Check if installed:** Control Panel → Programs → .NET Framework
- **Critical:** Application will NOT run without this

#### 2. Visual C++ Redistributable 2015-2022 (x64)
- **Download:** https://aka.ms/vs/17/release/vc_redist.x64.exe
- **Size:** ~20 MB
- **Critical:** MySQL and PHP will NOT run without this

### Hardware Requirements
- **CPU:** x64 (64-bit) processor
- **RAM:** Minimum 2 GB, Recommended 4 GB+
- **Disk Space:** Minimum 1 GB free space
- **Network:** Not required (all local)

---

## 📊 FILE SIZE SUMMARY

| Component | Size | Required |
|-----------|------|----------|
| FinalPOS.exe + Config | ~10 MB | ✅ Yes |
| DLL Files (26) | ~50 MB | ✅ Yes |
| Localization Folders | ~5 MB | ⚠️ Optional |
| MySQL Server | ~250 MB | ✅ Yes |
| PHP Runtime | ~50 MB | ✅ Yes |
| phpMyAdmin | ~30 MB | ⚠️ Optional |
| **TOTAL (Required)** | **~360 MB** | - |
| **TOTAL (With Optional)** | **~395 MB** | - |

---

## ✅ FINAL CHECKLIST

Before distributing your application, verify:

- [ ] FinalPOS.exe is in root folder
- [ ] FinalPOS.exe.config is in root folder
- [ ] All 26 DLL files are in root folder
- [ ] MySQL folder exists with all files
- [ ] MySQL my.ini is configured correctly
- [ ] MySQL data folder is initialized
- [ ] PHP folder exists with php.exe and php.ini
- [ ] PHP extensions are enabled in php.ini
- [ ] phpMyAdmin folder exists (if using)
- [ ] phpMyAdmin config.inc.php is created
- [ ] StartMySQL.bat exists and works
- [ ] StopMySQL.bat exists and works
- [ ] StartphpMyAdmin.bat exists (if using phpMyAdmin)
- [ ] StopphpMyAdmin.bat exists (if using phpMyAdmin)
- [ ] Connection string in FinalPOS.exe.config is correct
- [ ] Tested MySQL startup
- [ ] Tested application connection to database
- [ ] Tested phpMyAdmin (if included)

---

## 🚀 DISTRIBUTION OPTIONS

### Option 1: ZIP Archive
1. Create folder with all files
2. Compress to ZIP file
3. User extracts and runs

### Option 2: Self-Extracting Archive
1. Use 7-Zip SFX or WinRAR SFX
2. Create self-extracting executable
3. User runs and extracts automatically

### Option 3: Portable Application
1. Package as portable app
2. Include launcher script
3. User runs from any location

---

## 📝 NOTES

1. **Paths:** All paths in configuration files must match actual folder structure
2. **Ports:** Default MySQL port is 3308 (change if needed)
3. **First Run:** MySQL must be initialized before first use
4. **Permissions:** User needs write access to MySQL data folder
5. **Antivirus:** May need to add exceptions for MySQL and PHP folders

---

**Document Version:** 1.0  
**Last Updated:** 2025-01-XX

