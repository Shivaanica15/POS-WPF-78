-- FinalPOS MySQL Initialization Script
-- This script configures MySQL root user and creates the database

-- Set root password for localhost
ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY 'Shivaanica';

-- Set root password for remote connections
ALTER USER 'root'@'%' IDENTIFIED WITH mysql_native_password BY 'Shivaanica';

-- Flush privileges to apply changes
FLUSH PRIVILEGES;

-- Create the POS_NEXA_ERP database if it doesn't exist
CREATE DATABASE IF NOT EXISTS POS_NEXA_ERP CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Use the database
USE POS_NEXA_ERP;

-- Grant all privileges on the database to root user
GRANT ALL PRIVILEGES ON POS_NEXA_ERP.* TO 'root'@'localhost';
GRANT ALL PRIVILEGES ON POS_NEXA_ERP.* TO 'root'@'%';

-- Flush privileges again
FLUSH PRIVILEGES;

