@echo off
REM ========================================
REM Stop Portable MySQL Server
REM FinalPOS
REM ========================================

setlocal

REM Get installation directory
set INSTALLDIR=%~dp0..
set INSTALLDIR=%INSTALLDIR:~0,-1%
if "%~1" neq "" set INSTALLDIR=%~1

set MYSQLDIR=%INSTALLDIR%\mysql
set CONFIGFILE=%MYSQLDIR%\my.ini

echo Stopping MySQL server...

REM Try to shutdown gracefully using mysqladmin
if exist "%MYSQLDIR%\bin\mysqladmin.exe" (
    "%MYSQLDIR%\bin\mysqladmin.exe" --defaults-file="%CONFIGFILE%" -h127.0.0.1 -P3310 -uroot shutdown >nul 2>&1
    timeout /t 2 /nobreak >nul
)

REM Check if still running
netstat -an | findstr ":3310" >nul
if errorlevel 1 (
    echo MySQL stopped successfully.
    exit /b 0
)

REM If still running, try to kill the process
echo Waiting for graceful shutdown...
timeout /t 3 /nobreak >nul

netstat -an | findstr ":3310" >nul
if errorlevel 1 (
    echo MySQL stopped successfully.
    exit /b 0
)

REM Force kill mysqld.exe processes
echo Force stopping MySQL...
taskkill /F /IM mysqld.exe >nul 2>&1
timeout /t 1 /nobreak >nul

REM Verify it's stopped
netstat -an | findstr ":3310" >nul
if errorlevel 1 (
    echo MySQL stopped successfully.
) else (
    echo [WARNING] MySQL may still be running. Please check manually.
    exit /b 1
)

endlocal

