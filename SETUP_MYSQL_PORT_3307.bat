@echo off
title Setup MySQL Port 3310 - FinalPOS
color 0B
echo.
echo ========================================
echo    MySQL Port 3310 Configuration Tool
echo ========================================
echo.
echo This script will help you configure MySQL to run on port 3310
echo.

:: Check if running as administrator
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] This script must be run as Administrator!
    echo Right-click and select "Run as administrator"
    pause
    exit /b
)

echo [Step 1] Checking MySQL installation...
echo.

:: Find MySQL configuration file
set MYSQL_INI=""
set XAMPP_INI="C:\xampp\mysql\bin\my.ini"
set MYSQL_SERVER_INI="C:\ProgramData\MySQL\MySQL Server 8.0\my.ini"

if exist %XAMPP_INI% (
    set MYSQL_INI=%XAMPP_INI%
    echo [FOUND] XAMPP MySQL configuration: %XAMPP_INI%
    goto :found_ini
)

if exist %MYSQL_SERVER_INI% (
    set MYSQL_INI=%MYSQL_SERVER_INI%
    echo [FOUND] MySQL Server configuration: %MYSQL_SERVER_INI%
    goto :found_ini
)

echo [ERROR] MySQL configuration file not found!
echo Please locate my.ini manually and edit port=3310
pause
exit /b

:found_ini
echo.
echo [Step 2] Checking current port configuration...
echo.

findstr /C:"port=" %MYSQL_INI% | findstr /V "^;" | findstr /V "^#"
echo.

echo [Step 3] Updating port to 3310...
echo.

:: Stop MySQL first
echo Stopping MySQL service...
net stop MySQL >nul 2>&1
net stop MySQL80 >nul 2>&1
timeout /t 2 /nobreak >nul

:: Backup original file
copy %MYSQL_INI% "%MYSQL_INI%.backup" >nul 2>&1
echo [OK] Backup created: %MYSQL_INI%.backup

:: Update port in my.ini
powershell -Command "(Get-Content %MYSQL_INI%) -replace '^port\s*=\s*\d+', 'port=3310' | Set-Content %MYSQL_INI%"
powershell -Command "(Get-Content %MYSQL_INI%) -replace '^port\s*=\s*3306', 'port=3310' | Set-Content %MYSQL_INI%"

:: Add port if not exists under [mysqld]
findstr /C:"[mysqld]" %MYSQL_INI% >nul
if %errorlevel% equ 0 (
    findstr /C:"port=" %MYSQL_INI% >nul
    if %errorlevel% neq 0 (
        echo Adding port=3310 under [mysqld] section...
        powershell -Command "$content = Get-Content %MYSQL_INI%; $newContent = @(); $found = $false; foreach($line in $content) { $newContent += $line; if($line -match '\[mysqld\]' -and -not $found) { $newContent += 'port=3307'; $found = $true } }; $newContent | Set-Content %MYSQL_INI%"
    )
)

echo.
echo [Step 4] Starting MySQL service...
echo.

:: Start MySQL
net start MySQL >nul 2>&1
if %errorlevel% equ 0 (
    echo [OK] MySQL service started
) else (
    net start MySQL80 >nul 2>&1
    if %errorlevel% equ 0 (
        echo [OK] MySQL80 service started
    ) else (
        echo [WARNING] Could not start MySQL service automatically
        echo Please start MySQL manually from XAMPP Control Panel or Services
    )
)

echo.
echo [Step 5] Verifying port 3310...
echo.

timeout /t 3 /nobreak >nul
netstat -an | findstr ":3310" >nul
if %errorlevel% equ 0 (
    echo [SUCCESS] MySQL is now running on port 3310!
    netstat -an | findstr ":3310"
) else (
    echo [WARNING] MySQL might not be listening on port 3310 yet
    echo Please restart MySQL manually and check again
)

echo.
echo ========================================
echo    Configuration Complete!
echo ========================================
echo.
echo Next steps:
echo 1. Test connection: mysql -h localhost -P 3310 -u root -pShivaanica
echo 2. Run FinalPOS application
echo.
pause


