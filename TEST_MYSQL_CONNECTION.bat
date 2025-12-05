@echo off
title Test MySQL Connection
color 0E
echo.
echo ========================================
echo    Testing MySQL Connection
echo ========================================
echo.
echo Testing connection with:
echo   Host: localhost
echo   Port: 3307
echo   User: root
echo   Password: Shivaanica
echo.
echo ========================================
echo.

:: Test connection using mysql command line
mysql -h localhost -P 3307 -u root -pShivaanica -e "SELECT VERSION();" 2>nul
if %errorlevel% equ 0 (
    echo [SUCCESS] MySQL connection works!
    echo.
    echo Your MySQL credentials are correct.
    echo The application should work now.
    echo.
) else (
    echo [FAILED] MySQL connection failed!
    echo.
    echo Possible issues:
    echo 1. Password is incorrect
    echo 2. MySQL is not running on port 3307
    echo 3. Root user authentication plugin issue
    echo.
    echo Try manual connection:
    echo   mysql -h localhost -P 3307 -u root -p
    echo   (Enter password when prompted)
    echo.
)

echo ========================================
echo.
pause

