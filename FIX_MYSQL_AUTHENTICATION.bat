@echo off
title Fix MySQL Root Password - FinalPOS
color 0B
echo.
echo ========================================
echo    Fix MySQL Root User Password
echo ========================================
echo.
echo This script will help you fix MySQL root password
echo.

echo [Step 1] Testing current connection...
echo.

mysql -h 127.0.0.1 -P 3307 -u root -pShivaanica -e "SELECT 'Password OK!' AS Status;" 2>&1
if %errorlevel% equ 0 (
    echo.
    echo [SUCCESS] Password is already correct!
    echo Your MySQL root password is already set to "Shivaanica"
    pause
    exit /b
)

echo.
echo [FAILED] Password is incorrect or user doesn't exist
echo.
echo [Step 2] You need to fix MySQL root password manually
echo.
echo Choose one of these options:
echo.
echo OPTION 1: If you know the current MySQL root password
echo ----------------------------------------
echo 1. Run: mysql -h 127.0.0.1 -P 3307 -u root -p
echo 2. Enter your current password
echo 3. Run these commands:
echo    ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY 'Shivaanica';
echo    ALTER USER 'root'@'%%' IDENTIFIED WITH mysql_native_password BY 'Shivaanica';
echo    FLUSH PRIVILEGES;
echo.
echo OPTION 2: If you DON'T know the password (XAMPP)
echo ----------------------------------------
echo 1. Open XAMPP Control Panel
echo 2. Stop MySQL
echo 3. Edit: C:\xampp\mysql\bin\my.ini
echo 4. Under [mysqld] section, add: skip-grant-tables
echo 5. Start MySQL
echo 6. Run: mysql -h 127.0.0.1 -P 3307 -u root
echo 7. Run: UPDATE mysql.user SET authentication_string=PASSWORD('Shivaanica') WHERE User='root';
echo 8. Run: FLUSH PRIVILEGES;
echo 9. Stop MySQL, remove skip-grant-tables, Start MySQL
echo.
echo OPTION 3: Use SQL file
echo ----------------------------------------
echo 1. Open MySQL Workbench or command line
echo 2. Connect to MySQL (use current password)
echo 3. Run: FIX_MYSQL_ROOT_PASSWORD.sql
echo.
pause

