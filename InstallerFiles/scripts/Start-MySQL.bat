@echo off
REM FinalPOS MySQL Start Script
REM Starts MySQL server on port 3307

setlocal enabledelayedexpansion

set MYSQL_DIR=C:\FinalPOS-MySQL
set MYSQL_CONFIG=%MYSQL_DIR%\my.ini
set MYSQLD=%MYSQL_DIR%\bin\mysqld.exe

REM Check if MySQL directory exists
if not exist "%MYSQL_DIR%" (
    echo ERROR: MySQL directory not found: %MYSQL_DIR%
    echo Please ensure FinalPOS-MySQL is installed correctly.
    pause
    exit /b 1
)

REM Check if mysqld.exe exists
if not exist "%MYSQLD%" (
    echo ERROR: MySQL server executable not found: %MYSQLD%
    echo Please ensure MySQL is installed correctly.
    pause
    exit /b 1
)

REM Check if MySQL is already running
tasklist /FI "IMAGENAME eq mysqld.exe" 2>NUL | find /I /N "mysqld.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo MySQL server is already running.
    echo Checking port 3307...
    netstat -an | findstr ":3307" >nul
    if "%ERRORLEVEL%"=="0" (
        echo MySQL is listening on port 3307.
    ) else (
        echo WARNING: MySQL process found but not listening on port 3307.
    )
    pause
    exit /b 0
)

echo Starting MySQL server...
echo MySQL Directory: %MYSQL_DIR%
echo MySQL Config: %MYSQL_CONFIG%
echo.

REM Start MySQL server in background
start "" /MIN "%MYSQLD%" --defaults-file="%MYSQL_CONFIG%" --console

REM Wait a few seconds for MySQL to start
timeout /t 5 /nobreak >nul

REM Check if MySQL started successfully
timeout /t 2 /nobreak >nul
netstat -an | findstr ":3307" >nul
if "%ERRORLEVEL%"=="0" (
    echo.
    echo SUCCESS: MySQL server started successfully on port 3307!
    echo.
) else (
    echo.
    echo WARNING: MySQL server may still be starting...
    echo Please wait a few more seconds and check again.
    echo.
)

pause

