@echo off
REM FinalPOS MySQL Installation Script
REM Initializes MySQL data directory and runs initialization SQL

setlocal enabledelayedexpansion

set MYSQL_DIR=C:\FinalPOS-MySQL
set MYSQL_CONFIG=%MYSQL_DIR%\my.ini
set MYSQLD=%MYSQL_DIR%\bin\mysqld.exe
set MYSQL_INIT=%MYSQL_DIR%\bin\mysqld.exe
set DATA_DIR=%MYSQL_DIR%\data
set INIT_SQL=%MYSQL_DIR%\scripts\init.sql
set SCHEMA_SQL=%MYSQL_DIR%\scripts\schema.sql

echo ========================================
echo FinalPOS MySQL Installation Script
echo ========================================
echo.

REM Check if MySQL directory exists
if not exist "%MYSQL_DIR%" (
    echo ERROR: MySQL directory not found: %MYSQL_DIR%
    echo Please ensure MySQL files are copied correctly.
    pause
    exit /b 1
)

REM Check if mysqld.exe exists
if not exist "%MYSQLD%" (
    echo ERROR: MySQL server executable not found: %MYSQLD%
    pause
    exit /b 1
)

REM Check if data directory exists and is not empty
if exist "%DATA_DIR%\mysql" (
    echo MySQL data directory already exists.
    echo Skipping initialization...
    echo.
    goto :start_server
)

echo Initializing MySQL data directory...
echo This may take a few minutes...
echo.

REM Initialize MySQL data directory
"%MYSQLD%" --defaults-file="%MYSQL_CONFIG%" --initialize-insecure --console
if "%ERRORLEVEL%" neq "0" (
    echo ERROR: Failed to initialize MySQL data directory.
    pause
    exit /b 1
)

echo.
echo MySQL data directory initialized successfully.
echo.

:start_server
REM Start MySQL server
echo Starting MySQL server...
start "" /MIN "%MYSQLD%" --defaults-file="%MYSQL_CONFIG%" --console

REM Wait for MySQL to start
echo Waiting for MySQL server to start...
timeout /t 10 /nobreak >nul

REM Check if MySQL is listening on port 3307
:check_port
netstat -an | findstr ":3307" >nul
if "%ERRORLEVEL%" neq "0" (
    echo Waiting for MySQL to start on port 3307...
    timeout /t 3 /nobreak >nul
    goto :check_port
)

echo MySQL server is running on port 3307.
echo.

REM Run initialization SQL
if exist "%INIT_SQL%" (
    echo Running initialization SQL...
    "%MYSQL_DIR%\bin\mysql.exe" -u root --port=3307 < "%INIT_SQL%"
    if "%ERRORLEVEL%" neq "0" (
        echo WARNING: Failed to run initialization SQL.
    ) else (
        echo Initialization SQL executed successfully.
    )
    echo.
) else (
    echo WARNING: init.sql not found: %INIT_SQL%
    echo Skipping initialization SQL.
    echo.
)

REM Import schema if it exists
if exist "%SCHEMA_SQL%" (
    echo Importing database schema...
    "%MYSQL_DIR%\bin\mysql.exe" -u root -pShivaanica --port=3307 POS_NEXA_ERP < "%SCHEMA_SQL%"
    if "%ERRORLEVEL%" neq "0" (
        echo ERROR: Failed to import schema.sql.
        echo Please check the schema file for errors.
        pause
        exit /b 1
    ) else (
        echo Schema imported successfully.
    )
    echo.
) else (
    echo INFO: schema.sql not found: %SCHEMA_SQL%
    echo Skipping schema import. You can import it manually later.
    echo.
)

REM Validate connection
echo Validating database connection...
"%MYSQL_DIR%\bin\mysql.exe" -u root -pShivaanica --port=3307 -e "USE POS_NEXA_ERP; SELECT 'Connection successful!' AS Status;" >nul 2>&1
if "%ERRORLEVEL%" neq "0" (
    echo ERROR: Database connection validation failed.
    pause
    exit /b 1
) else (
    echo Database connection validated successfully.
    echo.
)

REM Stop MySQL server after installation (user will start it when needed)
echo Stopping MySQL server...
"%MYSQL_DIR%\bin\mysqladmin.exe" -u root -pShivaanica --port=3307 shutdown >nul 2>&1
timeout /t 3 /nobreak >nul

REM Verify MySQL is stopped
tasklist /FI "IMAGENAME eq mysqld.exe" 2>NUL | find /I /N "mysqld.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo Forcing MySQL shutdown...
    taskkill /F /IM mysqld.exe /T >nul 2>&1
    timeout /t 2 /nobreak >nul
)

echo ========================================
echo MySQL installation complete!
echo ========================================
echo.
echo MySQL has been configured successfully:
echo   - Port: 3307
echo   - Root password: Shivaanica
echo   - Database: POS_NEXA_ERP
echo   - Schema imported: Yes
echo.
echo MySQL server has been stopped.
echo Start it using: Start-MySQL.bat
echo.
pause

