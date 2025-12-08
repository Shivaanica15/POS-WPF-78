@echo off
REM ========================================
REM Start Portable MySQL Server
REM FinalPOS
REM ========================================

setlocal

REM Get installation directory
set INSTALLDIR=%~dp0..
set INSTALLDIR=%INSTALLDIR:~0,-1%
if "%~1" neq "" set INSTALLDIR=%~1

set MYSQLDIR=%INSTALLDIR%\mysql
set CONFIGFILE=%MYSQLDIR%\my.ini

REM Check if MySQL is already running
netstat -an | findstr ":3310" >nul
if %errorlevel% equ 0 (
    echo MySQL is already running on port 3310.
    exit /b 0
)

REM Check if mysqld.exe exists
if not exist "%MYSQLDIR%\bin\mysqld.exe" (
    echo [ERROR] MySQL not found in %MYSQLDIR%\bin\
    exit /b 1
)

REM Check if config file exists
if not exist "%CONFIGFILE%" (
    echo [ERROR] Configuration file not found: %CONFIGFILE%
    exit /b 1
)

REM Start MySQL in background
echo Starting MySQL server...
start /min "" "%MYSQLDIR%\bin\mysqld.exe" --defaults-file="%CONFIGFILE%"

REM Wait for MySQL to start
timeout /t 3 /nobreak >nul

REM Verify it started
set RETRY_COUNT=0
:check_loop
netstat -an | findstr ":3310" >nul
if errorlevel 1 (
    set /a RETRY_COUNT+=1
    if %RETRY_COUNT% geq 10 (
        echo [ERROR] MySQL failed to start!
        exit /b 1
    )
    timeout /t 1 /nobreak >nul
    goto :check_loop
)

echo MySQL started successfully on port 3310.

endlocal
