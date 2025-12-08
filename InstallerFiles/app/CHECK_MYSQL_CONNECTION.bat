@echo off
title Check MySQL Connection - Port 3310
color 0E
echo.
echo ========================================
echo    MySQL Connection Diagnostic Tool
echo ========================================
echo.
echo Checking MySQL on port 3310...
echo.

:: Check if MySQL is listening on port 3310
netstat -an | findstr ":3310" >nul 2>&1
if %errorlevel% equ 0 (
    echo [OK] MySQL is listening on port 3310
    netstat -an | findstr ":3310"
) else (
    echo [ERROR] MySQL is NOT listening on port 3310!
    echo.
    echo Checking other ports...
    netstat -an | findstr ":3306" >nul 2>&1
    if %errorlevel% equ 0 (
        echo [WARNING] MySQL is running on port 3306 (not 3310)
        echo You need to change MySQL port to 3310
    ) else (
        echo [ERROR] MySQL is not running on port 3306 either!
        echo MySQL server is not running or not accessible.
    )
)

echo.
echo ========================================
echo    Testing MySQL Connection
echo ========================================
echo.

:: Test connection
mysql -h localhost -P 3310 -u root -pShivaanica -e "SELECT 'Connection OK!' AS Status;" 2>nul
if %errorlevel% equ 0 (
    echo [SUCCESS] MySQL connection works!
    echo Your credentials are correct.
) else (
    echo [FAILED] Cannot connect to MySQL
    echo.
    echo Possible issues:
    echo 1. MySQL is not running
    echo 2. MySQL is not on port 3310
    echo 3. Password is incorrect
    echo 4. MySQL service is not started
    echo.
    echo Solutions:
    echo - Start MySQL service
    echo - Check MySQL port configuration
    echo - Verify password: Shivaanica
)

echo.
echo ========================================
echo.
pause



