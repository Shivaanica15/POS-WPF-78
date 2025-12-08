@echo off
title FinalPOS - Installation Wizard
color 0B
echo.
echo ========================================
echo    FinalPOS Installation Wizard
echo ========================================
echo.
echo This will help you set up FinalPOS on your computer.
echo.
pause

:: Check current directory
if not exist "FinalPOS.exe" (
    echo [ERROR] Please run this installer from the FinalPOS folder!
    echo Make sure FinalPOS.exe is in the same folder as this installer.
    pause
    exit /b
)

echo.
echo [Step 1/3] Checking prerequisites...
echo.

:: Check .NET Framework
echo Checking .NET Framework...
reg query "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" /v Release >nul 2>&1
if %errorlevel% equ 0 (
    echo    [OK] .NET Framework is installed
) else (
    echo    [WARNING] .NET Framework not detected
    echo.
    echo    .NET Framework is required to run FinalPOS.
    echo    It's usually pre-installed on Windows 10/11.
    echo.
    echo    If you get an error when running FinalPOS, download it from:
    echo    https://dotnet.microsoft.com/download/dotnet-framework/net472
    echo.
    pause
)

echo.
echo [Step 2/3] Checking MySQL...
echo.

set MYSQL_FOUND=0
set MYSQL_RUNNING=0

:: Check Windows MySQL Services
sc query MySQL >nul 2>&1
if %errorlevel% equ 0 (
    set MYSQL_FOUND=1
    sc query MySQL | findstr "RUNNING" >nul
    if %errorlevel% equ 0 set MYSQL_RUNNING=1
)

sc query MySQL80 >nul 2>&1
if %errorlevel% equ 0 (
    set MYSQL_FOUND=1
    sc query MySQL80 | findstr "RUNNING" >nul
    if %errorlevel% equ 0 set MYSQL_RUNNING=1
)

:: Check XAMPP MySQL
tasklist /FI "IMAGENAME eq mysqld.exe" 2>NUL | find /I /N "mysqld.exe">NUL
if "%ERRORLEVEL%"=="0" (
    set MYSQL_FOUND=1
    set MYSQL_RUNNING=1
)

:: Check port 3306
netstat -an | findstr ":3306" >nul 2>&1
if %errorlevel% equ 0 (
    set MYSQL_FOUND=1
    set MYSQL_RUNNING=1
)

if %MYSQL_FOUND% equ 0 (
    echo    [ERROR] MySQL not found!
    echo.
    echo    Please install MySQL first:
    echo.
    echo    Option 1 - XAMPP (Easiest):
    echo       Download from: https://www.apachefriends.org/
    echo       Install and start MySQL from XAMPP Control Panel
    echo.
    echo    Option 2 - MySQL Server:
    echo       Download from: https://dev.mysql.com/downloads/installer/
    echo.
    pause
    exit /b
) else if %MYSQL_RUNNING% equ 0 (
    echo    [WARNING] MySQL is installed but NOT running
    echo.
    echo    Please start MySQL:
    echo    - XAMPP: Open XAMPP Control Panel, click Start next to MySQL
    echo    - MySQL Server: Start MySQL service from Windows Services
    echo.
    pause
) else (
    echo    [OK] MySQL is running
)

echo.
echo [Step 3/3] Finalizing installation...
echo.

:: Create desktop shortcut
set DESKTOP=%USERPROFILE%\Desktop
if exist "%DESKTOP%\FinalPOS.lnk" (
    echo    Desktop shortcut already exists
) else (
    echo    Creating desktop shortcut...
    powershell -Command "$WshShell = New-Object -ComObject WScript.Shell; $Shortcut = $WshShell.CreateShortcut('%DESKTOP%\FinalPOS.lnk'); $Shortcut.TargetPath = '%CD%\FinalPOS.exe'; $Shortcut.WorkingDirectory = '%CD%'; $Shortcut.Description = 'FinalPOS Point of Sale System'; $Shortcut.Save()" >nul 2>&1
    if %errorlevel% equ 0 (
        echo    [OK] Desktop shortcut created
    ) else (
        echo    [INFO] Could not create shortcut (not critical)
    )
)

echo.
echo ========================================
echo    Installation Complete!
echo ========================================
echo.
echo FinalPOS is ready to use!
echo.
echo To start the application:
echo   - Double-click FinalPOS.exe
echo   - OR use the desktop shortcut
echo   - OR run START_FinalPOS.bat
echo.
echo Default login credentials:
echo   Username: admin
echo   Password: admin
echo.
echo IMPORTANT: Change password after first login!
echo.
pause

