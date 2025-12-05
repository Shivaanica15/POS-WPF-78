-- ============================================================================
-- FinalPOS Database Schema
-- MySQL 8.0+ Compatible
-- ============================================================================
-- This schema creates all tables, views, and triggers for POS_NEXA_ERP
-- Database must already exist (created by init.sql)
-- ============================================================================

USE POS_NEXA_ERP;

-- ============================================================================
-- TABLES
-- ============================================================================

-- Brand table
CREATE TABLE IF NOT EXISTS `tbl_brand` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `brand` VARCHAR(120) NOT NULL,
    UNIQUE KEY `ux_tbl_brand_brand` (`brand`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Category table
CREATE TABLE IF NOT EXISTS `tbl_category` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `category` VARCHAR(150) NOT NULL,
    UNIQUE KEY `ux_tbl_category_category` (`category`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Store table
CREATE TABLE IF NOT EXISTS `tbl_store` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `store` VARCHAR(150) NOT NULL,
    `address` VARCHAR(255) NOT NULL,
    `vat` DECIMAL(5,2) NOT NULL DEFAULT 0.00,
    UNIQUE KEY `ux_tbl_store_store` (`store`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Vendor table
CREATE TABLE IF NOT EXISTS `tbl_vendor` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `vendor` VARCHAR(150) NOT NULL,
    `address` VARCHAR(255) DEFAULT '',
    `contactperson` VARCHAR(150) DEFAULT '',
    `phone` VARCHAR(60) DEFAULT '',
    `email` VARCHAR(120) DEFAULT '',
    UNIQUE KEY `ux_tbl_vendor_vendor` (`vendor`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Users table
CREATE TABLE IF NOT EXISTS `tbl_users` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `username` VARCHAR(50) NOT NULL,
    `password` VARCHAR(255) NOT NULL,
    `role` VARCHAR(50) NOT NULL,
    `name` VARCHAR(150) NOT NULL,
    `is_active` TINYINT(1) NOT NULL DEFAULT 1,
    UNIQUE KEY `ux_tbl_users_username` (`username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Products table
CREATE TABLE IF NOT EXISTS `tbl_products` (
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Cart table (transactions)
CREATE TABLE IF NOT EXISTS `tbl_cart` (
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Adjustment table
CREATE TABLE IF NOT EXISTS `tbl_adjustment` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `referenceno` VARCHAR(50) NOT NULL,
    `pcode` VARCHAR(30) NOT NULL,
    `qty` INT NOT NULL DEFAULT 0,
    `action` VARCHAR(30) NOT NULL,
    `remarks` VARCHAR(255) DEFAULT '',
    `sdate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `user` VARCHAR(120) NOT NULL,
    KEY `idx_adjustment_pcode` (`pcode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Stock In table
CREATE TABLE IF NOT EXISTS `tbl_stocks_in` (
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Cancel table
CREATE TABLE IF NOT EXISTS `tbl_cancel` (
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- VAT table
CREATE TABLE IF NOT EXISTS `tbl_vat` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `vat` DECIMAL(5,2) NOT NULL DEFAULT 0.00
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- VIEWS
-- ============================================================================

-- Critical items view (low stock)
CREATE OR REPLACE VIEW `viewcriticalitems` AS
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
    WHERE p.qty <= p.reorder;

-- Stock in view
CREATE OR REPLACE VIEW `viewstocks` AS
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
    LEFT JOIN tbl_vendor AS v ON v.id = si.vendorid;

-- Sold items view
CREATE OR REPLACE VIEW `viewsolditems` AS
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
    LEFT JOIN tbl_products AS p ON p.pcode = c.pcode;

-- Top 10 products view
CREATE OR REPLACE VIEW `viewtop10` AS
    SELECT  c.pcode,
            IFNULL(p.pdesc,'') AS pdesc,
            SUM(c.qty) AS qty,
            SUM(c.total) AS total
    FROM tbl_cart AS c
    LEFT JOIN tbl_products AS p ON p.pcode = c.pcode
    WHERE c.status = 'Sold'
    GROUP BY c.pcode, p.pdesc
    ORDER BY qty DESC, total DESC
    LIMIT 10;

-- Cancelled orders view
CREATE OR REPLACE VIEW `cancelledorder` AS
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
    LEFT JOIN tbl_products AS p ON p.pcode = ca.pcode;

-- ============================================================================
-- TRIGGERS
-- ============================================================================

-- Drop existing triggers if they exist (for re-installation)
DROP TRIGGER IF EXISTS `trg_cart_before_insert`;
DROP TRIGGER IF EXISTS `trg_cart_before_update`;
DROP TRIGGER IF EXISTS `trg_adjustment_before_insert`;
DROP TRIGGER IF EXISTS `trg_stockin_before_update`;
DROP TRIGGER IF EXISTS `trg_cancel_before_insert`;

-- Cart before insert trigger (calculate total)
DELIMITER //
CREATE TRIGGER `trg_cart_before_insert`
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
    END//
DELIMITER ;

-- Cart before update trigger (recalculate total)
DELIMITER //
CREATE TRIGGER `trg_cart_before_update`
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
    END//
DELIMITER ;

-- Adjustment before insert trigger (normalize action)
DELIMITER //
CREATE TRIGGER `trg_adjustment_before_insert`
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
    END//
DELIMITER ;

-- Stock in before update trigger (validate qty)
DELIMITER //
CREATE TRIGGER `trg_stockin_before_update`
    BEFORE UPDATE ON `tbl_stocks_in`
    FOR EACH ROW
    BEGIN
        IF NEW.qty < 0 THEN
            SET NEW.qty = 0;
        END IF;
        IF NEW.status IS NULL OR NEW.status = '' THEN
            SET NEW.status = OLD.status;
        END IF;
    END//
DELIMITER ;

-- Cancel before insert trigger (normalize data)
DELIMITER //
CREATE TRIGGER `trg_cancel_before_insert`
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
    END//
DELIMITER ;

-- ============================================================================
-- SEED DATA (Optional - only if tables are empty)
-- ============================================================================

-- Insert default admin user if no users exist
INSERT INTO `tbl_users` (`username`, `password`, `role`, `name`, `is_active`)
SELECT 'admin', 'admin', 'Admin', 'System Administrator', 1
WHERE NOT EXISTS (SELECT 1 FROM `tbl_users` LIMIT 1);

-- Insert default VAT if no VAT exists
INSERT INTO `tbl_vat` (`vat`)
SELECT 0.00
WHERE NOT EXISTS (SELECT 1 FROM `tbl_vat` LIMIT 1);

-- ============================================================================
-- SCHEMA IMPORT COMPLETE
-- ============================================================================
