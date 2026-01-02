-- Database First - SQL Script to create tables if they don't exist
-- Run this script on your AICodeChallenge2025 database

USE AICodeChallenge2025;
GO

-- Create Vehicle table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Vehicle' AND xtype='U')
BEGIN
    CREATE TABLE Vehicle (
        Id                  BIGINT PRIMARY KEY IDENTITY(1,1),
        Make                VARCHAR(50),
        Model               VARCHAR(100),
        Year                INT NOT NULL,
        Trim                VARCHAR(100),
        VIN                 VARCHAR(17) NOT NULL,
        CreatedAt           DATETIME2 DEFAULT GETUTCDATE(),
        LastModifiedAt      DATETIME2 DEFAULT GETUTCDATE()
    );
    PRINT 'Vehicle table created successfully.';
END
ELSE
BEGIN
    PRINT 'Vehicle table already exists.';
END
GO

-- Create Offer table  
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Offer' AND xtype='U')
BEGIN
    CREATE TABLE Offer (
        OfferId             BIGINT PRIMARY KEY IDENTITY(1,1),
        SellerId            BIGINT NOT NULL,
        VIN                 VARCHAR(17) NOT NULL,
        OfferAmount         DECIMAL(12,2),
        City                VARCHAR(50),
        State               VARCHAR(50),
        Country             VARCHAR(50),
        Status              VARCHAR(20), -- OPEN, SOLD, CANCELLED
        CreatedAt           DATETIME2 DEFAULT GETUTCDATE(),
        VehicleId           INT NOT NULL,
        UpdatedAt           DATETIME2 DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Offer_Vehicle FOREIGN KEY (VehicleId) REFERENCES Vehicle(Id)
    );
    PRINT 'Offer table created successfully.';
END
ELSE
BEGIN
    PRINT 'Offer table already exists.';
END
GO

-- Create indexes for better performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Vehicle_VIN')
BEGIN
    CREATE INDEX IX_Vehicle_VIN ON Vehicle(VIN);
    PRINT 'Index IX_Vehicle_VIN created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Vehicle_YearMakeModel')
BEGIN
    CREATE INDEX IX_Vehicle_YearMakeModel ON Vehicle(Year, Make, Model, Trim, VIN);
    PRINT 'Index IX_Vehicle_YearMakeModel created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Offer_VehicleId')
BEGIN
    CREATE INDEX IX_Offer_VehicleId ON Offer(VehicleId);
    PRINT 'Index IX_Offer_VehicleId created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Offer_SellerId')
BEGIN
    CREATE INDEX IX_Offer_SellerId ON Offer(SellerId);
    PRINT 'Index IX_Offer_SellerId created.';
END

PRINT 'Database setup completed successfully.';
GO
