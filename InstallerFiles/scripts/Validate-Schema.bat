@echo off
REM FinalPOS Schema Validation Script
REM Validates that schema.sql was imported correctly

setlocal enabledelayedexpansion

set MYSQL_DIR=C:\FinalPOS-MySQL
set MYSQL_CLIENT=%MYSQL_DIR%\bin\mysql.exe

echo ========================================
echo FinalPOS Schema Validation
echo ========================================
echo.

REM Check if MySQL directory exists
if not exist "%MYSQL_DIR%" (
    echo [ERROR] MySQL directory not found: %MYSQL_DIR%
    echo Please ensure FinalPOS-MySQL is installed correctly.
    echo.
    pause
    exit /b 1
)

REM Check if MySQL client exists
if not exist "%MYSQL_CLIENT%" (
    echo [ERROR] MySQL client not found: %MYSQL_CLIENT%
    echo.
    pause
    exit /b 1
)

REM Check if MySQL is running
tasklist /FI "IMAGENAME eq mysqld.exe" 2>NUL | find /I /N "mysqld.exe">NUL
if "%ERRORLEVEL%" neq "0" (
    echo [ERROR] MySQL server is not running.
    echo Please start MySQL using Start-MySQL.bat
    echo.
    pause
    exit /b 1
)

REM Check if port 3307 is listening
netstat -an | findstr ":3307" >nul
if "%ERRORLEVEL%" neq "0" (
    echo [ERROR] MySQL is not listening on port 3307.
    echo.
    pause
    exit /b 1
)

echo Testing database connection...
"%MYSQL_CLIENT%" -u root -pShivaanica --port=3307 -e "USE POS_NEXA_ERP; SELECT 'Connection OK' AS Status;" >nul 2>&1
if "%ERRORLEVEL%" neq "0" (
    echo [ERROR] Cannot connect to database POS_NEXA_ERP
    echo.
    pause
    exit /b 1
)

echo [OK] Database connection successful
echo.

echo Validating tables...
echo.

REM Check each required table
set TABLES=tbl_brand tbl_category tbl_store tbl_vendor tbl_users tbl_products tbl_cart tbl_adjustment tbl_stocks_in tbl_cancel tbl_vat
set TABLE_COUNT=0
set MISSING_TABLES=

for %%T in (%TABLES%) do (
    "%MYSQL_CLIENT%" -u root -pShivaanica --port=3307 POS_NEXA_ERP -e "SELECT COUNT(*) FROM %%T;" >nul 2>&1
    if "!ERRORLEVEL!"=="0" (
        echo [OK] Table %%T exists
        set /a TABLE_COUNT+=1
    ) else (
        echo [FAIL] Table %%T is missing or inaccessible
        set MISSING_TABLES=!MISSING_TABLES! %%T
    )
)

echo.
echo Validating views...
echo.

REM Check views
set VIEWS=viewcriticalitems viewstocks viewsolditems viewtop10 cancelledorder
set VIEW_COUNT=0

for %%V in (%VIEWS%) do (
    "%MYSQL_CLIENT%" -u root -pShivaanica --port=3307 POS_NEXA_ERP -e "SELECT COUNT(*) FROM %%V LIMIT 1;" >nul 2>&1
    if "!ERRORLEVEL!"=="0" (
        echo [OK] View %%V exists
        set /a VIEW_COUNT+=1
    ) else (
        echo [FAIL] View %%V is missing or inaccessible
    )
)

echo.
echo Validating triggers...
echo.

REM Check triggers
"%MYSQL_CLIENT%" -u root -pShivaanica --port=3307 POS_NEXA_ERP -e "SHOW TRIGGERS;" >nul 2>&1
if "%ERRORLEVEL%"=="0" (
    echo [OK] Triggers exist
    "%MYSQL_CLIENT%" -u root -pShivaanica --port=3307 POS_NEXA_ERP -e "SHOW TRIGGERS;" 2>nul | findstr /C:"trg_" >nul
    if "%ERRORLEVEL%"=="0" (
        echo [OK] Trigger validation passed
    )
) else (
    echo [WARNING] Could not verify triggers
)

echo.
echo ========================================
echo Validation Summary
echo ========================================
echo Tables found: %TABLE_COUNT% / 11
echo Views found: %VIEW_COUNT% / 5
echo.

if "%TABLE_COUNT%"=="11" (
    echo [SUCCESS] All tables are present!
) else (
    echo [WARNING] Some tables are missing: %MISSING_TABLES%
)

echo.
echo ========================================
echo Validation complete.
echo ========================================
echo.
pause

