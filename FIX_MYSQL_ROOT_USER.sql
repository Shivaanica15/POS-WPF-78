-- ========================================
-- Fix MySQL Root User Authentication
-- Run this in MySQL to fix authentication issues
-- ========================================

-- Step 1: Connect to MySQL (run this command in terminal first)
-- mysql -h localhost -P 3307 -u root -p
-- (Enter your current password)

-- Step 2: Run these SQL commands:

-- Option A: Change to mysql_native_password (Recommended for compatibility)
ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY 'Shivaanica';

-- Option B: If Option A doesn't work, try resetting password
-- ALTER USER 'root'@'localhost' IDENTIFIED BY 'Shivaanica';

-- Option C: If root@localhost doesn't exist, create it
-- CREATE USER 'root'@'localhost' IDENTIFIED BY 'Shivaanica';
-- GRANT ALL PRIVILEGES ON *.* TO 'root'@'localhost' WITH GRANT OPTION;

-- Step 3: Flush privileges
FLUSH PRIVILEGES;

-- Step 4: Verify the user
SELECT user, host, plugin FROM mysql.user WHERE user='root';

-- Step 5: Test connection
SELECT 'Connection successful!' AS Status;

-- ========================================
-- Expected Result:
-- User: root
-- Host: localhost
-- Plugin: mysql_native_password (or caching_sha2_password)
-- ========================================

