using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace FinalPOS
{
    public static class DatabaseInitializer
    {
        private const string DatabaseName = "POS_NEXA_ERP";
        
        private static string GetServerConnectionString()
        {
            try
            {
                // Try to read from App.config connection string
                var connectionString = ConfigurationManager.ConnectionStrings["FinalPOS.Properties.Settings.NewOneConnectionString"]?.ConnectionString;
                if (!string.IsNullOrEmpty(connectionString))
                {
                    // Extract port if present, otherwise use default
                    var parts = connectionString.Split(';');
                    string server = "localhost";
                    string port = "3310";
                    string uid = "root";
                    string pwd = "Shivaanica";
                    
                    foreach (var part in parts)
                    {
                        var keyValue = part.Split('=');
                        if (keyValue.Length == 2)
                        {
                            var key = keyValue[0].Trim();
                            var value = keyValue[1].Trim();
                            
                            if (key.Equals("Server", StringComparison.OrdinalIgnoreCase))
                                server = value;
                            else if (key.Equals("Port", StringComparison.OrdinalIgnoreCase))
                                port = value;
                            else if (key.Equals("Uid", StringComparison.OrdinalIgnoreCase))
                                uid = value;
                            else if (key.Equals("Pwd", StringComparison.OrdinalIgnoreCase))
                                pwd = value;
                        }
                    }
                    
                    // Build connection string with port if specified
                    // For MySQL 8.0+, explicitly set Pwd= (empty) if no password, and add compatibility parameters
                    string pwdPart = string.IsNullOrEmpty(pwd) ? "Pwd=;" : $"Pwd={pwd};";
                    string additionalParams = "AllowPublicKeyRetrieval=True;";
                    if (!string.IsNullOrEmpty(port))
                    {
                        return $"Server={server};Port={port};Uid={uid};{pwdPart}{additionalParams}";
                    }
                    else
                    {
                        return $"Server={server};Uid={uid};{pwdPart}{additionalParams}";
                    }
                }
            }
            catch
            {
                // Fall back to default if reading config fails
            }
            
            // Default connection string (uses portable MySQL Server on port 3310, no password)
            return "Server=localhost;Port=3310;Uid=root;Pwd=;AllowPublicKeyRetrieval=True;";
        }
        
        private static string _cachedServerConnectionString = null;
        
        private static string ServerConnectionString 
        { 
            get 
            {
                if (_cachedServerConnectionString == null)
                {
                    _cachedServerConnectionString = GetServerConnectionString();
                }
                return _cachedServerConnectionString;
            }
        }
        
        public static void RefreshConnectionString()
        {
            _cachedServerConnectionString = null;
        }

        private static readonly string[] TableStatements = new[]
        {
            @"CREATE TABLE IF NOT EXISTS `tbl_brand` (
                `id` INT AUTO_INCREMENT PRIMARY KEY,
                `brand` VARCHAR(120) NOT NULL,
                UNIQUE KEY `ux_tbl_brand_brand` (`brand`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",

            @"CREATE TABLE IF NOT EXISTS `tbl_category` (
                `id` INT AUTO_INCREMENT PRIMARY KEY,
                `category` VARCHAR(150) NOT NULL,
                UNIQUE KEY `ux_tbl_category_category` (`category`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",

            @"CREATE TABLE IF NOT EXISTS `tbl_store` (
                `id` INT AUTO_INCREMENT PRIMARY KEY,
                `store` VARCHAR(150) NOT NULL,
                `address` VARCHAR(255) NOT NULL,
                `vat` DECIMAL(5,2) NOT NULL DEFAULT 0.00,
                UNIQUE KEY `ux_tbl_store_store` (`store`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",

            @"CREATE TABLE IF NOT EXISTS `tbl_vendor` (
                `id` INT AUTO_INCREMENT PRIMARY KEY,
                `vendor` VARCHAR(150) NOT NULL,
                `address` VARCHAR(255) DEFAULT '',
                `contactperson` VARCHAR(150) DEFAULT '',
                `phone` VARCHAR(60) DEFAULT '',
                `email` VARCHAR(120) DEFAULT '',
                UNIQUE KEY `ux_tbl_vendor_vendor` (`vendor`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",

            @"CREATE TABLE IF NOT EXISTS `tbl_users` (
                `id` INT AUTO_INCREMENT PRIMARY KEY,
                `username` VARCHAR(50) NOT NULL,
                `password` VARCHAR(255) NOT NULL,
                `role` VARCHAR(50) NOT NULL,
                `name` VARCHAR(150) NOT NULL,
                `is_active` TINYINT(1) NOT NULL DEFAULT 1,
                UNIQUE KEY `ux_tbl_users_username` (`username`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",

            @"CREATE TABLE IF NOT EXISTS `tbl_products` (
                `pcode` VARCHAR(30) NOT NULL,
                `barcode` VARCHAR(80) DEFAULT NULL,
                `pdesc` VARCHAR(255) NOT NULL,
                `bid` INT NOT NULL,
                `cid` INT NOT NULL,
                `price` DECIMAL(12,2) NOT NULL DEFAULT 0.00,
                `qty` INT NOT NULL DEFAULT 0,
                `reorder` INT NOT NULL DEFAULT 0,
                PRIMARY KEY (`pcode`),
                KEY `idx_products_barcode` (`barcode`),
                KEY `idx_products_desc` (`pdesc`),
                KEY `idx_products_bid` (`bid`),
                KEY `idx_products_cid` (`cid`),
                CONSTRAINT `fk_products_brand` FOREIGN KEY (`bid`) REFERENCES `tbl_brand` (`id`) ON UPDATE CASCADE ON DELETE RESTRICT,
                CONSTRAINT `fk_products_category` FOREIGN KEY (`cid`) REFERENCES `tbl_category` (`id`) ON UPDATE CASCADE ON DELETE RESTRICT
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",

            @"CREATE TABLE IF NOT EXISTS `tbl_cart` (
                `id` INT AUTO_INCREMENT PRIMARY KEY,
                `transno` VARCHAR(30) NOT NULL,
                `pcode` VARCHAR(30) NOT NULL,
                `price` DECIMAL(12,2) NOT NULL DEFAULT 0.00,
                `qty` INT NOT NULL DEFAULT 0,
                `disc` DECIMAL(12,2) NOT NULL DEFAULT 0.00,
                `disc_per` DECIMAL(12,2) NOT NULL DEFAULT 0.00,
                `total` DECIMAL(14,2) NOT NULL DEFAULT 0.00,
                `sdate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                `cashier` VARCHAR(120) DEFAULT NULL,
                `status` VARCHAR(20) NOT NULL DEFAULT 'Pending',
                KEY `idx_cart_transno` (`transno`),
                KEY `idx_cart_pcode` (`pcode`),
                KEY `idx_cart_status` (`status`),
                CONSTRAINT `fk_cart_product` FOREIGN KEY (`pcode`) REFERENCES `tbl_products` (`pcode`) ON UPDATE CASCADE
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",

            @"CREATE TABLE IF NOT EXISTS `tbl_adjustment` (
                `id` INT AUTO_INCREMENT PRIMARY KEY,
                `referenceno` VARCHAR(50) NOT NULL,
                `pcode` VARCHAR(30) NOT NULL,
                `qty` INT NOT NULL DEFAULT 0,
                `action` VARCHAR(30) NOT NULL,
                `remarks` VARCHAR(255) DEFAULT '',
                `sdate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                `user` VARCHAR(120) NOT NULL,
                KEY `idx_adjustment_pcode` (`pcode`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",

            @"CREATE TABLE IF NOT EXISTS `tbl_stocks_in` (
                `id` INT AUTO_INCREMENT PRIMARY KEY,
                `refno` VARCHAR(50) NOT NULL,
                `pcode` VARCHAR(30) NOT NULL,
                `qty` INT NOT NULL DEFAULT 0,
                `sdate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                `stockinby` VARCHAR(120) NOT NULL,
                `status` VARCHAR(20) NOT NULL DEFAULT 'Pending',
                `vendorid` INT DEFAULT NULL,
                KEY `idx_stockin_refno` (`refno`),
                KEY `idx_stockin_status` (`status`),
                CONSTRAINT `fk_stockin_product` FOREIGN KEY (`pcode`) REFERENCES `tbl_products` (`pcode`) ON UPDATE CASCADE,
                CONSTRAINT `fk_stockin_vendor` FOREIGN KEY (`vendorid`) REFERENCES `tbl_vendor` (`id`) ON UPDATE CASCADE ON DELETE SET NULL
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",

            @"CREATE TABLE IF NOT EXISTS `tbl_cancel` (
                `id` INT AUTO_INCREMENT PRIMARY KEY,
                `transno` VARCHAR(30) NOT NULL,
                `pcode` VARCHAR(30) NOT NULL,
                `price` DECIMAL(12,2) NOT NULL DEFAULT 0.00,
                `qty` INT NOT NULL DEFAULT 0,
                `sdate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                `voidby` VARCHAR(120) NOT NULL,
                `cancelledby` VARCHAR(120) NOT NULL,
                `reason` VARCHAR(255) DEFAULT '',
                `action` VARCHAR(50) NOT NULL,
                KEY `idx_cancel_transno` (`transno`),
                KEY `idx_cancel_pcode` (`pcode`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",

            @"CREATE TABLE IF NOT EXISTS `tbl_vat` (
                `id` INT AUTO_INCREMENT PRIMARY KEY,
                `vat` DECIMAL(5,2) NOT NULL DEFAULT 0.00
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;"
        };

        private static readonly string[] ViewStatements = new[]
        {
            @"CREATE OR REPLACE VIEW `viewcriticalitems` AS
                SELECT  p.pcode,
                        p.barcode,
                        p.pdesc,
                        b.brand,
                        c.category,
                        p.price,
                        p.qty,
                        p.reorder
                FROM tbl_products AS p
                INNER JOIN tbl_brand AS b ON b.id = p.bid
                INNER JOIN tbl_category AS c ON c.id = p.cid
                WHERE p.qty <= p.reorder;",

            @"CREATE OR REPLACE VIEW `viewstocks` AS
                SELECT  si.id,
                        si.refno,
                        si.pcode,
                        IFNULL(p.pdesc,'') AS pdesc,
                        si.qty,
                        si.sdate,
                        si.stockinby,
                        si.status,
                        IFNULL(v.vendor,'') AS vendor
                FROM tbl_stocks_in AS si
                LEFT JOIN tbl_products AS p ON p.pcode = si.pcode
                LEFT JOIN tbl_vendor AS v ON v.id = si.vendorid;",

            @"CREATE OR REPLACE VIEW `viewsolditems` AS
                SELECT  c.id,
                        c.transno,
                        c.pcode,
                        IFNULL(p.pdesc,'') AS pdesc,
                        c.price,
                        c.qty,
                        c.disc,
                        c.total,
                        c.sdate,
                        c.status,
                        c.cashier
                FROM tbl_cart AS c
                LEFT JOIN tbl_products AS p ON p.pcode = c.pcode;",

            @"CREATE OR REPLACE VIEW `viewtop10` AS
                SELECT  c.pcode,
                        IFNULL(p.pdesc,'') AS pdesc,
                        SUM(c.qty) AS qty,
                        SUM(c.total) AS total
                FROM tbl_cart AS c
                LEFT JOIN tbl_products AS p ON p.pcode = c.pcode
                WHERE c.status = 'Sold'
                GROUP BY c.pcode, p.pdesc
                ORDER BY qty DESC, total DESC
                LIMIT 10;",

            @"CREATE OR REPLACE VIEW `cancelledorder` AS
                SELECT  ca.id,
                        ca.transno,
                        ca.pcode,
                        IFNULL(p.pdesc,'') AS pdesc,
                        ca.price,
                        ca.qty,
                        (ca.price * ca.qty) AS total,
                        ca.sdate,
                        ca.voidby,
                        ca.cancelledby,
                        ca.reason,
                        ca.action
                FROM tbl_cancel AS ca
                LEFT JOIN tbl_products AS p ON p.pcode = ca.pcode;"
        };

        private static readonly string[] TriggerStatements = new[]
        {
            "DROP TRIGGER IF EXISTS `trg_cart_before_insert`;",
            @"CREATE TRIGGER `trg_cart_before_insert`
                BEFORE INSERT ON `tbl_cart`
                FOR EACH ROW
                BEGIN
                    IF NEW.disc IS NULL THEN SET NEW.disc = 0; END IF;
                    IF NEW.disc_per IS NULL THEN SET NEW.disc_per = 0; END IF;
                    IF NEW.qty IS NULL THEN SET NEW.qty = 0; END IF;
                    SET NEW.total = (NEW.price * NEW.qty) - NEW.disc;
                    IF NEW.status IS NULL OR NEW.status = '' THEN
                        SET NEW.status = 'Pending';
                    END IF;
                    IF NEW.sdate IS NULL THEN
                        SET NEW.sdate = NOW();
                    END IF;
                END;",

            "DROP TRIGGER IF EXISTS `trg_cart_before_update`;",
            @"CREATE TRIGGER `trg_cart_before_update`
                BEFORE UPDATE ON `tbl_cart`
                FOR EACH ROW
                BEGIN
                    IF NEW.disc IS NULL THEN SET NEW.disc = 0; END IF;
                    IF NEW.disc_per IS NULL THEN SET NEW.disc_per = 0; END IF;
                    IF NEW.qty IS NULL THEN SET NEW.qty = 0; END IF;
                    SET NEW.total = (NEW.price * NEW.qty) - NEW.disc;
                    IF NEW.status IS NULL OR NEW.status = '' THEN
                        SET NEW.status = 'Pending';
                    END IF;
                END;",

            "DROP TRIGGER IF EXISTS `trg_adjustment_before_insert`;",
            @"CREATE TRIGGER `trg_adjustment_before_insert`
                BEFORE INSERT ON `tbl_adjustment`
                FOR EACH ROW
                BEGIN
                    IF NEW.action IS NULL OR NEW.action = '' THEN
                        SET NEW.action = 'ADJUST';
                    ELSE
                        SET NEW.action = UPPER(NEW.action);
                    END IF;
                    IF NEW.qty < 0 THEN
                        SET NEW.qty = ABS(NEW.qty);
                    END IF;
                END;",

            "DROP TRIGGER IF EXISTS `trg_stockin_before_update`;",
            @"CREATE TRIGGER `trg_stockin_before_update`
                BEFORE UPDATE ON `tbl_stocks_in`
                FOR EACH ROW
                BEGIN
                    IF NEW.qty < 0 THEN
                        SET NEW.qty = 0;
                    END IF;
                    IF NEW.status IS NULL OR NEW.status = '' THEN
                        SET NEW.status = OLD.status;
                    END IF;
                END;",

            "DROP TRIGGER IF EXISTS `trg_cancel_before_insert`;",
            @"CREATE TRIGGER `trg_cancel_before_insert`
                BEFORE INSERT ON `tbl_cancel`
                FOR EACH ROW
                BEGIN
                    IF NEW.qty < 0 THEN
                        SET NEW.qty = ABS(NEW.qty);
                    END IF;
                    IF NEW.reason IS NULL THEN
                        SET NEW.reason = '';
                    END IF;
                    IF NEW.action IS NULL OR NEW.action = '' THEN
                        SET NEW.action = 'RETURN';
                    END IF;
                END;"
        };

        public static string ConnectionString => $"{ServerConnectionString}Database={DatabaseName};";

        public static void EnsureDatabaseSetup()
        {
            int maxRetries = 3;
            int retryCount = 0;
            string savedPassword = "";

            while (retryCount < maxRetries)
            {
                try
                {
                    // Try to create database and setup
                    CreateDatabaseIfMissing();
                    using (var connection = new MySqlConnection(ConnectionString))
                    {
                        connection.Open();
                        ExecuteStatements(connection, TableStatements);
                        ExecuteStatements(connection, ViewStatements);
                        ExecuteStatements(connection, TriggerStatements);
                        EnsureMissingColumns(connection);
                        EnsureSeedData(connection);
                    }
                    // Success - exit the retry loop
                    return;
                }
                catch (MySqlException mysqlEx)
                {
                    // Check if it's an authentication error
                    if (mysqlEx.Number == 1045 || mysqlEx.Message.Contains("Access denied") || mysqlEx.Message.Contains("using password"))
                    {
                        // Authentication failed - prompt for password
                        if (retryCount == 0)
                        {
                            // First attempt - show password prompt
                            var passwordForm = new frmDatabasePassword();
                            if (passwordForm.ShowDialog() == DialogResult.OK && passwordForm.PasswordProvided)
                            {
                                savedPassword = passwordForm.DatabasePassword;
                                // Update connection string with new password
                                UpdateConnectionStringWithPassword(savedPassword);
                                RefreshConnectionString(); // Refresh cached connection string
                                retryCount++;
                                continue;
                            }
                            else
                            {
                                // User cancelled or didn't provide password
                                throw new Exception("Database connection cancelled. MySQL password is required.");
                            }
                        }
                        else
                        {
                            // Password was wrong, try again
                            var passwordForm = new frmDatabasePassword();
                            if (passwordForm.ShowDialog() == DialogResult.OK && passwordForm.PasswordProvided)
                            {
                                savedPassword = passwordForm.DatabasePassword;
                                UpdateConnectionStringWithPassword(savedPassword);
                                RefreshConnectionString(); // Refresh cached connection string
                                retryCount++;
                                continue;
                            }
                            else
                            {
                                throw new Exception("Invalid MySQL password. Please check your password and try again.");
                            }
                        }
                    }
                    else
                    {
                        // Other MySQL errors
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    // Non-authentication errors
                    if (retryCount == 0)
                    {
                        MessageBox.Show($"Database initialization failed: {ex.Message}", "FinalPOS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    throw;
                }
            }

            throw new Exception("Failed to connect to database after multiple attempts.");
        }

        private static void UpdateConnectionStringWithPassword(string password)
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
                
                if (connectionStringsSection != null)
                {
                    var connectionString = $"Server=localhost;Port=3310;Database=POS_NEXA_ERP;Uid=root;Pwd={password};AllowPublicKeyRetrieval=True;";
                    
                    if (connectionStringsSection.ConnectionStrings["FinalPOS.Properties.Settings.NewOneConnectionString"] != null)
                    {
                        connectionStringsSection.ConnectionStrings["FinalPOS.Properties.Settings.NewOneConnectionString"].ConnectionString = connectionString;
                    }
                    else
                    {
                        connectionStringsSection.ConnectionStrings.Add(
                            new ConnectionStringSettings("FinalPOS.Properties.Settings.NewOneConnectionString", connectionString, "MySql.Data.MySqlClient"));
                    }
                    
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("connectionStrings");
                }
            }
            catch
            {
                // If we can't save to config, continue with in-memory password
            }
        }

        private static void CreateDatabaseIfMissing()
        {
            using (var connection = new MySqlConnection(ServerConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS `{DatabaseName}` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void ExecuteStatements(MySqlConnection connection, IEnumerable<string> statements)
        {
            foreach (var statement in statements)
            {
                if (string.IsNullOrWhiteSpace(statement))
                {
                    continue;
                }

                using (var command = new MySqlCommand(statement, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void EnsureMissingColumns(MySqlConnection connection)
        {
            try
            {
                // Check if tbl_users table exists and add missing columns
                using (var checkTable = new MySqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tbl_users'", connection))
                {
                    var tableExists = Convert.ToInt64(checkTable.ExecuteScalar()) > 0;
                    if (tableExists)
                    {
                        // Check if is_active column exists in tbl_users
                        using (var checkColumn = new MySqlCommand(@"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                            WHERE TABLE_SCHEMA = DATABASE() 
                            AND TABLE_NAME = 'tbl_users' 
                            AND COLUMN_NAME = 'is_active'", connection))
                        {
                            var columnExists = Convert.ToInt64(checkColumn.ExecuteScalar()) > 0;
                            if (!columnExists)
                            {
                                // Add is_active column to existing tbl_users table
                                using (var addColumn = new MySqlCommand(@"ALTER TABLE `tbl_users` 
                                    ADD COLUMN `is_active` TINYINT(1) NOT NULL DEFAULT 1 AFTER `name`", connection))
                                {
                                    addColumn.ExecuteNonQuery();
                                }
                                
                                // Update existing records to be active by default
                                using (var updateExisting = new MySqlCommand("UPDATE `tbl_users` SET `is_active` = 1 WHERE `is_active` IS NULL OR `is_active` = 0", connection))
                                {
                                    updateExisting.ExecuteNonQuery();
                                }
                            }
                        }

                        // Check if id column exists in tbl_users (for old databases)
                        using (var checkIdColumn = new MySqlCommand(@"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                            WHERE TABLE_SCHEMA = DATABASE() 
                            AND TABLE_NAME = 'tbl_users' 
                            AND COLUMN_NAME = 'id'", connection))
                        {
                            var idColumnExists = Convert.ToInt64(checkIdColumn.ExecuteScalar()) > 0;
                            if (!idColumnExists)
                            {
                                // Check if there's already a primary key
                                using (var checkPrimaryKey = new MySqlCommand(@"SELECT COUNT(*) FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
                                    WHERE TABLE_SCHEMA = DATABASE() 
                                    AND TABLE_NAME = 'tbl_users' 
                                    AND CONSTRAINT_NAME = 'PRIMARY'", connection))
                                {
                                    var hasPrimaryKey = Convert.ToInt64(checkPrimaryKey.ExecuteScalar()) > 0;
                                    if (!hasPrimaryKey)
                                    {
                                        // Add id column as primary key only if no primary key exists
                                        using (var addIdColumn = new MySqlCommand(@"ALTER TABLE `tbl_users` 
                                            ADD COLUMN `id` INT AUTO_INCREMENT PRIMARY KEY FIRST", connection))
                                        {
                                            addIdColumn.ExecuteNonQuery();
                                        }
                                    }
                                    else
                                    {
                                        // Just add id column without primary key constraint
                                        using (var addIdColumn = new MySqlCommand(@"ALTER TABLE `tbl_users` 
                                            ADD COLUMN `id` INT AUTO_INCREMENT FIRST", connection))
                                        {
                                            addIdColumn.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log but don't fail initialization if column migration fails
                // This allows the application to continue even if migration has issues
                System.Diagnostics.Debug.WriteLine($"Column migration warning: {ex.Message}");
            }
        }

        private static void EnsureSeedData(MySqlConnection connection)
        {
            using (var countCommand = new MySqlCommand("SELECT COUNT(*) FROM `tbl_users`;", connection))
            {
                var userCount = Convert.ToInt64(countCommand.ExecuteScalar());
                if (userCount == 0)
                {
                    using (var insertCommand = new MySqlCommand(@"INSERT INTO `tbl_users`
                        (username, password, role, name, is_active)
                        VALUES ('admin', 'admin', 'Admin', 'System Administrator', 1);", connection))
                    {
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }

            using (var vatCount = new MySqlCommand("SELECT COUNT(*) FROM `tbl_vat`;", connection))
            {
                var existing = Convert.ToInt64(vatCount.ExecuteScalar());
                if (existing == 0)
                {
                    using (var insertVat = new MySqlCommand("INSERT INTO `tbl_vat` (vat) VALUES (0.00);", connection))
                    {
                        insertVat.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
