@echo off
title MySQL Connection Diagnostic Tool
color 0B
echo.
echo ========================================
echo    MySQL Connection Diagnostic Tool
echo ========================================
echo.

echo [Step 1] Checking if MySQL is running on port 3310...
netstat -an | findstr ":3310" >nul 2>&1
if %errorlevel% equ 0 (
    echo [OK] MySQL is listening on port 3310
    netstat -an | findstr ":3310" | findstr "LISTENING"
) else (
    echo [ERROR] MySQL is NOT listening on port 3310!
    echo MySQL server is not running or not configured for port 3310.
    echo.
    echo Please:
    echo 1. Start MySQL service
    echo 2. Configure MySQL to use port 3310
    echo 3. Restart MySQL
    pause
    exit /b
)

echo.
echo [Step 2] Checking App.config connection string...
if exist "FinalPOS\App.config" (
    echo [OK] App.config found
    echo.
    echo Current connection string:
    findstr /C:"connectionString" "FinalPOS\App.config"
    echo.
    findstr /C:"Pwd=" "FinalPOS\App.config" | findstr /C:"connectionString"
) else (
    echo [WARNING] App.config not found in FinalPOS folder
)

echo.
echo [Step 3] Testing connection string format...
echo.
echo Expected format:
echo Server=localhost;Port=3310;Database=POS_NEXA_ERP;Uid=root;Pwd=Shivaanica;AllowPublicKeyRetrieval=True;
echo.

echo [Step 4] Checking for common issues...
echo.

:: Check if password is empty
findstr /C:"Pwd=;" "FinalPOS\App.config" >nul 2>&1
if %errorlevel% equ 0 (
    echo [ERROR] Password is EMPTY in App.config!
    echo The connection string has Pwd=; (empty password)
    echo It should be: Pwd=Shivaanica;
    echo.
    echo Fix: Edit FinalPOS\App.config and change Pwd=; to Pwd=Shivaanica;
)

:: Check if port is correct
findstr /C:"Port=3310" "FinalPOS\App.config" >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] Port is NOT 3310 in App.config!
    echo The connection string should have Port=3310
)

echo.
echo ========================================
echo    Diagnostic Complete
echo ========================================
echo.
echo If MySQL is running on port 3310 but connection fails:
echo 1. Check App.config has Pwd=Shivaanica (not Pwd=;)
echo 2. Check App.config has Port=3310
echo 3. Rebuild the project in Visual Studio
echo 4. Run FinalPOS.exe again
echo.
pause


