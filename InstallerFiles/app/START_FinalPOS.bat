@echo off
title FinalPOS - Starting Application
color 0B

echo.
echo ========================================
echo    Starting FinalPOS Application
echo ========================================
echo.

:: Check if MySQL is running (multiple methods)
set MYSQL_RUNNING=0

:: Method 1: Check Windows MySQL Services
sc query MySQL 2>nul | findstr "RUNNING" >nul
if %errorlevel% equ 0 (
    set MYSQL_RUNNING=1
    goto :mysql_found
)

sc query MySQL80 2>nul | findstr "RUNNING" >nul
if %errorlevel% equ 0 (
    set MYSQL_RUNNING=1
    goto :mysql_found
)

:: Method 2: Check XAMPP MySQL (process-based)
tasklist /FI "IMAGENAME eq mysqld.exe" 2>NUL | find /I /N "mysqld.exe">NUL
if "%ERRORLEVEL%"=="0" (
    set MYSQL_RUNNING=1
    goto :mysql_found
)

:: Method 3: Check if MySQL is listening on port 3306
netstat -an | findstr ":3306" >nul 2>&1
if %errorlevel% equ 0 (
    set MYSQL_RUNNING=1
    goto :mysql_found
)

:mysql_found
if %MYSQL_RUNNING% equ 0 (
    echo [WARNING] MySQL is not running!
    echo.
    echo Please start MySQL first:
    echo.
    echo Option 1 - XAMPP (Recommended):
    echo   1. Open XAMPP Control Panel
    echo   2. Click "Start" button next to MySQL
    echo.
    echo Option 2 - Windows Services:
    echo   1. Press Win+R, type: services.msc
    echo   2. Find "MySQL" or "MySQL80"
    echo   3. Right-click and select "Start"
    echo.
    pause
    exit /b
)

:: Check if FinalPOS.exe exists
if not exist "FinalPOS.exe" (
    echo [ERROR] FinalPOS.exe not found!
    echo Make sure you run this from the application folder
    pause
    exit /b
)

echo [OK] MySQL is running
echo [OK] Starting FinalPOS...
echo.

:: Start the application
start "" "FinalPOS.exe"

:: Wait a moment and check if it started
timeout /t 2 /nobreak >nul

echo Application started!
echo.
echo If you see a password prompt, enter your MySQL root password
echo Default login: admin / admin
echo.
timeout /t 3 /nobreak >nul

