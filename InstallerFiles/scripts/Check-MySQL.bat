@echo off
REM FinalPOS MySQL Status Check Script
REM Checks if MySQL server is running on port 3307

setlocal enabledelayedexpansion

set MYSQL_DIR=C:\FinalPOS-MySQL
set MYSQL_CLIENT=%MYSQL_DIR%\bin\mysql.exe

echo ========================================
echo FinalPOS MySQL Status Check
echo ========================================
echo.

REM Check if MySQL directory exists
if not exist "%MYSQL_DIR%" (
    echo [ERROR] MySQL directory not found: %MYSQL_DIR%
    echo Please ensure FinalPOS-MySQL is installed correctly.
    echo.
    pause
    exit /b 1
)

REM Check if MySQL process is running
echo Checking MySQL process...
tasklist /FI "IMAGENAME eq mysqld.exe" 2>NUL | find /I /N "mysqld.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [OK] MySQL server process is running.
    
    REM Get process details
    for /f "tokens=2" %%a in ('tasklist /FI "IMAGENAME eq mysqld.exe" /FO LIST ^| findstr "PID"') do (
        set PID=%%a
    )
    echo     Process ID: !PID!
) else (
    echo [FAIL] MySQL server process is NOT running.
    echo.
    echo Please start MySQL using Start-MySQL.bat
    echo.
    pause
    exit /b 1
)

echo.

REM Check if port 3307 is listening
echo Checking port 3307...
netstat -an | findstr ":3307" >nul
if "%ERRORLEVEL%"=="0" (
    echo [OK] MySQL is listening on port 3307.
    netstat -an | findstr ":3307"
) else (
    echo [FAIL] MySQL is NOT listening on port 3307.
    echo.
    pause
    exit /b 1
)

echo.

REM Try to connect to MySQL
if exist "%MYSQL_CLIENT%" (
    echo Testing MySQL connection...
    "%MYSQL_CLIENT%" -u root -pShivaanica --port=3307 -e "SELECT VERSION();" >nul 2>&1
    if "%ERRORLEVEL%"=="0" (
        echo [OK] MySQL connection successful!
        echo.
        echo MySQL Version:
        "%MYSQL_CLIENT%" -u root -pShivaanica --port=3307 -e "SELECT VERSION();" 2>nul
        echo.
        echo Database Status:
        "%MYSQL_CLIENT%" -u root -pShivaanica --port=3307 -e "SHOW DATABASES;" 2>nul
    ) else (
        echo [WARNING] Could not connect to MySQL.
        echo This may be normal if the server is still starting.
    )
) else (
    echo [INFO] MySQL client not found. Skipping connection test.
)

echo.
echo ========================================
echo Status check complete.
echo ========================================
echo.
pause

