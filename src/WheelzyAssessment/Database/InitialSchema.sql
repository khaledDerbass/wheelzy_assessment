-- Wheelzy Assessment Database Schema
-- SQL Server Implementation

-- Car makes table to avoid repetition
CREATE TABLE Makes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);

-- Car models table
CREATE TABLE Models (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MakeId INT NOT NULL,
    Name NVARCHAR(50) NOT NULL,
    CONSTRAINT FK_Models_Makes FOREIGN KEY (MakeId) REFERENCES Makes(Id),
    CONSTRAINT UK_Models_Make_Name UNIQUE(MakeId, Name)
);

-- Car submodels table
CREATE TABLE SubModels (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ModelId INT NOT NULL,
    Name NVARCHAR(50) NOT NULL,
    CONSTRAINT FK_SubModels_Models FOREIGN KEY (ModelId) REFERENCES Models(Id),
    CONSTRAINT UK_SubModels_Model_Name UNIQUE(ModelId, Name)
);

-- Buyers table
CREATE TABLE Buyers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Cars table
CREATE TABLE Cars (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Year INT NOT NULL,
    MakeId INT NOT NULL,
    ModelId INT NOT NULL,
    SubModelId INT NULL,
    ZipCode NVARCHAR(10) NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Cars_Makes FOREIGN KEY (MakeId) REFERENCES Makes(Id),
    CONSTRAINT FK_Cars_Models FOREIGN KEY (ModelId) REFERENCES Models(Id),
    CONSTRAINT FK_Cars_SubModels FOREIGN KEY (SubModelId) REFERENCES SubModels(Id)
);

-- Quotes table
CREATE TABLE Quotes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CarId INT NOT NULL,
    BuyerId INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    IsCurrent BIT NOT NULL DEFAULT 0,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Quotes_Cars FOREIGN KEY (CarId) REFERENCES Cars(Id),
    CONSTRAINT FK_Quotes_Buyers FOREIGN KEY (BuyerId) REFERENCES Buyers(Id)
);

-- Status definitions
CREATE TABLE Statuses (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE,
    RequiresDate BIT NOT NULL DEFAULT 0
);

-- Car status history
CREATE TABLE CarStatusHistory (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CarId INT NOT NULL,
    StatusId INT NOT NULL,
    IsCurrent BIT NOT NULL DEFAULT 0,
    StatusDate DATETIME2 NULL,
    ChangedBy NVARCHAR(100) NULL,
    ChangedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_CarStatusHistory_Cars FOREIGN KEY (CarId) REFERENCES Cars(Id),
    CONSTRAINT FK_CarStatusHistory_Statuses FOREIGN KEY (StatusId) REFERENCES Statuses(Id)
);

-- Buyer coverage areas
CREATE TABLE BuyerZipCodes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    BuyerId INT NOT NULL,
    ZipCode NVARCHAR(10) NOT NULL,
    CONSTRAINT FK_BuyerZipCodes_Buyers FOREIGN KEY (BuyerId) REFERENCES Buyers(Id),
    CONSTRAINT UK_BuyerZipCodes_Buyer_Zip UNIQUE(BuyerId, ZipCode)
);

-- Customers table (for Questions 3 & 4)
CREATE TABLE Customers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Invoices table (for Question 3)
CREATE TABLE Invoices (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NULL,
    Total DECIMAL(10,2) NOT NULL,
    InvoiceDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Invoices_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
);

-- Orders table (for Question 4)
CREATE TABLE Orders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderDate DATETIME2 NOT NULL,
    CustomerId INT NOT NULL,
    StatusId INT NOT NULL,
    Total DECIMAL(10,2) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
    CONSTRAINT FK_Orders_Statuses FOREIGN KEY (StatusId) REFERENCES Statuses(Id)
);

-- Create indexes for performance
CREATE INDEX IX_Cars_ZipCode ON Cars(ZipCode);
CREATE INDEX IX_Cars_Year ON Cars(Year);
CREATE INDEX IX_Quotes_IsCurrent ON Quotes(IsCurrent);
CREATE INDEX IX_CarStatusHistory_IsCurrent ON CarStatusHistory(IsCurrent);
CREATE INDEX IX_Orders_OrderDate ON Orders(OrderDate);
CREATE INDEX IX_Orders_CustomerId ON Orders(CustomerId);
CREATE INDEX IX_Orders_IsActive ON Orders(IsActive);

-- Sample data for testing
INSERT INTO Makes (Name) VALUES ('Toyota'), ('Honda'), ('Ford');
INSERT INTO Models (MakeId, Name) VALUES (1, 'Camry'), (1, 'Corolla'), (2, 'Civic'), (3, 'F-150');
INSERT INTO SubModels (ModelId, Name) VALUES (1, 'LE'), (1, 'XLE'), (3, 'LX'), (4, 'SuperCrew');

INSERT INTO Statuses (Name, RequiresDate) VALUES 
    ('Pending Acceptance', 0),
    ('Accepted', 0),
    ('Picked Up', 1),
    ('Completed', 0);

INSERT INTO Buyers (Name) VALUES ('Buyer ABC'), ('Premium Auto'), ('Quick Cash Cars');

-- Main query from Question 1
SELECT 
    c.Year,
    m.Name AS Make,
    md.Name AS Model,
    sm.Name AS SubModel,
    c.ZipCode,
    b.Name AS CurrentBuyerName,
    q.Amount AS CurrentQuote,
    s.Name AS CurrentStatusName,
    csh.StatusDate
FROM Cars c
    INNER JOIN Makes m ON c.MakeId = m.Id
    INNER JOIN Models md ON c.ModelId = md.Id
    LEFT JOIN SubModels sm ON c.SubModelId = sm.Id
    LEFT JOIN Quotes q ON c.Id = q.CarId AND q.IsCurrent = 1
    LEFT JOIN Buyers b ON q.BuyerId = b.Id
    LEFT JOIN CarStatusHistory csh ON c.Id = csh.CarId AND csh.IsCurrent = 1
    LEFT JOIN Statuses s ON csh.StatusId = s.Id;
