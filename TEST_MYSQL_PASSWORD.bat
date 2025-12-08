@echo off
title Test MySQL Password
color 0B
echo.
echo ========================================
echo    Testing MySQL Password
echo ========================================
echo.
echo Testing connection with password: Shivaanica
echo.

:: Try to connect with password
echo Attempting connection...
mysql -h 127.0.0.1 -P 3310 -u root -pShivaanica -e "SELECT 'Connection OK!' AS Status, USER() AS CurrentUser;" 2>&1

if %errorlevel% equ 0 (
    echo.
    echo [SUCCESS] Password is correct!
) else (
    echo.
    echo [FAILED] Password is incorrect or user doesn't exist
    echo.
    echo Solutions:
    echo 1. Check if password is actually "Shivaanica"
    echo 2. Run FIX_MYSQL_ROOT_PASSWORD.sql to reset password
    echo 3. Check MySQL user configuration
)

echo.
pause


