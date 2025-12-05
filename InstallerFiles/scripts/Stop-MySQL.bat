@echo off
REM FinalPOS MySQL Stop Script
REM Safely stops MySQL server

setlocal enabledelayedexpansion

set MYSQL_DIR=C:\FinalPOS-MySQL
set MYSQL_CLIENT=%MYSQL_DIR%\bin\mysql.exe

echo Stopping MySQL server...
echo.

REM Check if MySQL is running
tasklist /FI "IMAGENAME eq mysqld.exe" 2>NUL | find /I /N "mysqld.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo MySQL server process found.
    echo.
    
    REM Try graceful shutdown using mysqladmin
    if exist "%MYSQL_CLIENT%" (
        echo Attempting graceful shutdown...
        "%MYSQL_DIR%\bin\mysqladmin.exe" -u root -pShivaanica --port=3307 shutdown 2>nul
        timeout /t 3 /nobreak >nul
        
        REM Check if still running
        tasklist /FI "IMAGENAME eq mysqld.exe" 2>NUL | find /I /N "mysqld.exe">NUL
        if "%ERRORLEVEL%"=="0" (
            echo Graceful shutdown failed. Forcing termination...
            taskkill /F /IM mysqld.exe /T >nul 2>&1
            timeout /t 2 /nobreak >nul
        ) else (
            echo MySQL server stopped gracefully.
        )
    ) else (
        echo mysqladmin not found. Forcing termination...
        taskkill /F /IM mysqld.exe /T >nul 2>&1
        timeout /t 2 /nobreak >nul
    )
    
    REM Verify MySQL is stopped
    tasklist /FI "IMAGENAME eq mysqld.exe" 2>NUL | find /I /N "mysqld.exe">NUL
    if "%ERRORLEVEL%"=="0" (
        echo ERROR: Failed to stop MySQL server.
        pause
        exit /b 1
    ) else (
        echo.
        echo SUCCESS: MySQL server stopped successfully.
    )
) else (
    echo MySQL server is not running.
)

echo.
pause

