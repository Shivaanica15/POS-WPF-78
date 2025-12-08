@echo off
title FinalPOS - Prerequisites Checker
color 0A
echo.
echo ========================================
echo    FinalPOS Prerequisites Checker
echo ========================================
echo.

:: Check .NET Framework
echo [1/3] Checking .NET Framework...
reg query "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" /v Release >nul 2>&1
if %errorlevel% equ 0 (
    for /f "tokens=2 delims= " %%a in ('reg query "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" /v Release 2^>nul ^| findstr Release') do set release=%%a
    if !release! geq 461808 (
        echo    [OK] .NET Framework 4.7.2 or higher is installed
    ) else (
        echo    [WARNING] .NET Framework 4.7.2+ not found
        echo    Download from: https://dotnet.microsoft.com/download/dotnet-framework/net472
    )
) else (
    echo    [WARNING] .NET Framework not detected
    echo    Download from: https://dotnet.microsoft.com/download/dotnet-framework/net472
)

echo.

:: Check MySQL Service (including XAMPP)
echo [2/3] Checking MySQL Service...
set MYSQL_FOUND=0
set MYSQL_RUNNING=0

:: Check Windows MySQL Services
sc query MySQL >nul 2>&1
if %errorlevel% equ 0 (
    set MYSQL_FOUND=1
    sc query MySQL | findstr "RUNNING" >nul
    if %errorlevel% equ 0 (
        echo    [OK] MySQL service is running
        set MYSQL_RUNNING=1
        goto :mysql_check_done
    )
)

sc query MySQL80 >nul 2>&1
if %errorlevel% equ 0 (
    set MYSQL_FOUND=1
    sc query MySQL80 | findstr "RUNNING" >nul
    if %errorlevel% equ 0 (
        echo    [OK] MySQL80 service is running
        set MYSQL_RUNNING=1
        goto :mysql_check_done
    )
)

:: Check XAMPP MySQL (process-based, not service)
tasklist /FI "IMAGENAME eq mysqld.exe" 2>NUL | find /I /N "mysqld.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo    [OK] XAMPP MySQL (mysqld.exe) is running
    set MYSQL_RUNNING=1
    set MYSQL_FOUND=1
    goto :mysql_check_done
)

:: Check if MySQL is listening on port 3306
netstat -an | findstr ":3306" >nul 2>&1
if %errorlevel% equ 0 (
    echo    [OK] MySQL is listening on port 3306 (XAMPP or other)
    set MYSQL_RUNNING=1
    set MYSQL_FOUND=1
    goto :mysql_check_done
)

:mysql_check_done
if %MYSQL_FOUND% equ 0 (
    echo    [ERROR] MySQL not found
    echo    Please install MySQL Server or XAMPP
    echo    XAMPP: https://www.apachefriends.org/
    echo    MySQL: https://dev.mysql.com/downloads/installer/
) else if %MYSQL_RUNNING% equ 0 (
    echo    [WARNING] MySQL is installed but NOT running
    echo    If using XAMPP: Open XAMPP Control Panel and click Start next to MySQL
    echo    If using MySQL Server: Start MySQL service from Windows Services
)

echo.

:: Check if FinalPOS.exe exists
echo [3/3] Checking application files...
if exist "FinalPOS.exe" (
    echo    [OK] FinalPOS.exe found
    if exist "FinalPOS.exe.config" (
        echo    [OK] Configuration file found
    ) else (
        echo    [ERROR] FinalPOS.exe.config is missing!
    )
    if exist "MySql.Data.dll" (
        echo    [OK] Required DLL files found
    ) else (
        echo    [ERROR] Required DLL files are missing!
        echo    Make sure all files are extracted from the zip
    )
) else (
    echo    [ERROR] FinalPOS.exe not found!
    echo    Make sure you run this from the application folder
)

echo.
echo ========================================
echo    Check Complete
echo ========================================
echo.
echo If all checks passed, you can run FinalPOS.exe
echo If there are errors, please fix them before running the application
echo.
pause

