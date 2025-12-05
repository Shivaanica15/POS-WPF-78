-- Fix MySQL Root User Authentication
-- Run this in MySQL to fix the root user password

-- Connect to MySQL first (you may need to use current working password)
-- mysql -h 127.0.0.1 -P 3307 -u root -p

-- Then run these commands:

-- Set password for root@localhost
ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY 'Shivaanica';
FLUSH PRIVILEGES;

-- Set password for root@% (all hosts)
ALTER USER 'root'@'%' IDENTIFIED WITH mysql_native_password BY 'Shivaanica';
FLUSH PRIVILEGES;

-- Grant all privileges
GRANT ALL PRIVILEGES ON *.* TO 'root'@'localhost' WITH GRANT OPTION;
GRANT ALL PRIVILEGES ON *.* TO 'root'@'%' WITH GRANT OPTION;
FLUSH PRIVILEGES;

-- Verify
SELECT user, host, plugin FROM mysql.user WHERE user='root';

