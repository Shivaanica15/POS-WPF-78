@echo off
REM ========================================
REM Check MySQL Server Status
REM FinalPOS
REM ========================================

setlocal

REM Get installation directory
set INSTALLDIR=%~dp0..
set INSTALLDIR=%INSTALLDIR:~0,-1%
if "%~1" neq "" set INSTALLDIR=%~1

set MYSQLDIR=%INSTALLDIR%\mysql
set CONFIGFILE=%MYSQLDIR%\my.ini

echo.
echo Checking MySQL Status...
echo.

REM Check if port 3310 is in use
netstat -an | findstr ":3310" >nul
if errorlevel 1 (
    echo Status: NOT RUNNING
    echo Port 3310 is not in use.
    exit /b 1
) else (
    echo Status: RUNNING
    echo Port 3310 is active.
)

REM Try to connect to MySQL
if exist "%MYSQLDIR%\bin\mysql.exe" (
    "%MYSQLDIR%\bin\mysql.exe" --defaults-file="%CONFIGFILE%" -h127.0.0.1 -P3310 -uroot -e "SELECT VERSION() AS 'MySQL Version', DATABASE() AS 'Current Database';" 2>nul
    if errorlevel 1 (
        echo [WARNING] MySQL is running but connection test failed.
        exit /b 1
    ) else (
        echo [OK] Connection successful!
    )
) else (
    echo [INFO] mysql.exe not found, cannot test connection.
)

endlocal

