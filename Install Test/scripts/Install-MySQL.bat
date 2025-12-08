@echo off
REM ========================================
REM MySQL Portable Installation Script
REM FinalPOS - Automatic Database Setup
REM ========================================

setlocal enabledelayedexpansion

echo.
echo ========================================
echo    FinalPOS MySQL Installation
echo ========================================
echo.

REM Get installation directory from parameter or use default
if "%~1"=="" (
    echo [ERROR] Installation directory not provided!
    exit /b 1
)

set INSTALLDIR=%~1
set MYSQLDIR=%INSTALLDIR%\mysql
set DATADIR=%MYSQLDIR%\data
set TMPDIR=%MYSQLDIR%\tmp
set CONFIGFILE=%MYSQLDIR%\my.ini
rem Normalize slashes for MySQL config to avoid escape sequences (e.g., \t)
set "INSTALLDIR_FWD=%INSTALLDIR:\=/%"

echo Installation Directory: %INSTALLDIR%
echo MySQL Directory: %MYSQLDIR%
echo Data Directory: %DATADIR%
echo.

REM Check if mysqld.exe exists
if not exist "%MYSQLDIR%\bin\mysqld.exe" (
    echo [ERROR] mysqld.exe not found in %MYSQLDIR%\bin\
    echo Please ensure MySQL files are correctly copied.
    exit /b 1
)

echo [Step 1] Creating required directories...
if not exist "%TMPDIR%" mkdir "%TMPDIR%"
if not exist "%DATADIR%" mkdir "%DATADIR%"
echo [OK] Directories created.
echo.

REM Update my.ini with actual installation directory
echo [Step 2] Configuring my.ini...
if exist "%CONFIGFILE%" (
    setlocal enabledelayedexpansion
    (for /f "usebackq delims=" %%a in ("%CONFIGFILE%") do (
        set "line=%%a"
        set "line=!line:{INSTALLDIR}=%INSTALLDIR_FWD%!"
        echo(!line!
    )) > "%CONFIGFILE%.tmp"
    move /y "%CONFIGFILE%.tmp" "%CONFIGFILE%" >nul
    endlocal
    echo [OK] Configuration file updated.
) else (
    echo [WARNING] my.ini not found, but continuing...
)
echo.

REM Check if data directory is already initialized (handle InnoDB layout)
echo [Step 3] Checking data directory...
if exist "%DATADIR%\ibdata1" (
    echo [INFO] Existing InnoDB data directory detected. Skipping initialization.
    goto :start_mysql
)

REM Initialize MySQL data directory
echo [Step 4] Initializing MySQL data directory...
echo This may take a moment...
"%MYSQLDIR%\bin\mysqld.exe" --defaults-file="%CONFIGFILE%" --initialize-insecure --datadir="%DATADIR%"
if errorlevel 1 (
    echo [ERROR] MySQL initialization failed!
    exit /b 1
)
echo [OK] MySQL initialized successfully.
echo.

:start_mysql
REM Start MySQL server
echo [Step 5] Starting MySQL server...
start /min "" "%MYSQLDIR%\bin\mysqld.exe" --defaults-file="%CONFIGFILE%"
if errorlevel 1 (
    echo [ERROR] Failed to start MySQL server!
    exit /b 1
)

REM Wait for MySQL to start
echo Waiting for MySQL to start...
timeout /t 5 /nobreak >nul

REM Check if MySQL is running
echo [Step 6] Verifying MySQL is running...
set RETRY_COUNT=0
:check_loop
netstat -an | findstr ":3310" >nul
if errorlevel 1 (
    set /a RETRY_COUNT+=1
    if !RETRY_COUNT! geq 10 (
        echo [ERROR] MySQL failed to start on port 3310!
        exit /b 1
    )
    timeout /t 2 /nobreak >nul
    goto :check_loop
)
echo [OK] MySQL is running on port 3310.
echo.

REM Wait a bit more for MySQL to be fully ready
timeout /t 3 /nobreak >nul

REM Create database and schema
echo [Step 7] Creating database and schema...
set SQLFILE=%INSTALLDIR%\scripts\schema.sql
if exist "%SQLFILE%" (
    "%MYSQLDIR%\bin\mysql.exe" --defaults-file="%CONFIGFILE%" -h127.0.0.1 -P3310 -uroot -e "CREATE DATABASE IF NOT EXISTS POS_NEXA_ERP DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"
    if errorlevel 1 (
        echo [WARNING] Database creation had issues, but continuing...
    )
    "%MYSQLDIR%\bin\mysql.exe" --defaults-file="%CONFIGFILE%" -h127.0.0.1 -P3310 -uroot POS_NEXA_ERP < "%SQLFILE%"
    if errorlevel 1 (
        echo [WARNING] Schema import had issues, but database may still work.
    ) else (
        echo [OK] Database schema imported successfully.
    )
) else (
    echo [WARNING] schema.sql not found. Database will be created on first run.
    "%MYSQLDIR%\bin\mysql.exe" --defaults-file="%CONFIGFILE%" -h127.0.0.1 -P3310 -uroot -e "CREATE DATABASE IF NOT EXISTS POS_NEXA_ERP DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"
)
echo.

REM Set root password (optional - keeping blank for simplicity)
REM You can uncomment this if you want to set a password:
REM echo [Step 8] Setting root password...
REM "%MYSQLDIR%\bin\mysql.exe" --defaults-file="%CONFIGFILE%" -h127.0.0.1 -P3310 -uroot -e "ALTER USER 'root'@'localhost' IDENTIFIED BY 'Shivaanica'; FLUSH PRIVILEGES;"

echo.
echo ========================================
echo    Installation Complete!
echo ========================================
echo.
echo MySQL is now running on localhost:3310
echo Database: POS_NEXA_ERP
echo User: root
echo Password: (none)
echo.
echo MySQL will start automatically when you run FinalPOS.
echo.

endlocal
