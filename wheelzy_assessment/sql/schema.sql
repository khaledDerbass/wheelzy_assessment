-- Wheelzy - SQL Server schema (normalized) and sample query
-- Run on SQL Server

-- Reference data
CREATE TABLE dbo.Make (
  MakeId       int IDENTITY PRIMARY KEY,
  Name         varchar(100) NOT NULL UNIQUE
);

CREATE TABLE dbo.Model (
  ModelId      int IDENTITY PRIMARY KEY,
  MakeId       int NOT NULL REFERENCES dbo.Make(MakeId),
  Name         varchar(100) NOT NULL,
  CONSTRAINT UQ_Model UNIQUE (MakeId, Name)
);

CREATE TABLE dbo.Submodel (
  SubmodelId   int IDENTITY PRIMARY KEY,
  ModelId      int NOT NULL REFERENCES dbo.Model(ModelId),
  Name         varchar(100) NOT NULL,
  CONSTRAINT UQ_Submodel UNIQUE (ModelId, Name)
);

CREATE TABLE dbo.ZipCode (
  Zip          char(5) NOT NULL PRIMARY KEY,
  City         varchar(80) NULL,
  State        char(2) NULL
);

CREATE TABLE dbo.Car (
  CarId        int IDENTITY PRIMARY KEY,
  Year         smallint NOT NULL CHECK (Year BETWEEN 1900 AND 2100),
  MakeId       int NOT NULL REFERENCES dbo.Make(MakeId),
  ModelId      int NOT NULL REFERENCES dbo.Model(ModelId),
  SubmodelId   int NULL REFERENCES dbo.Submodel(SubmodelId),
  Zip          char(5) NOT NULL REFERENCES dbo.ZipCode(Zip),
  CreatedUtc   datetime2(0) NOT NULL CONSTRAINT DF_Car_CreatedUtc DEFAULT (SYSUTCDATETIME())
);

CREATE TABLE dbo.Buyer (
  BuyerId      int IDENTITY PRIMARY KEY,
  Name         varchar(150) NOT NULL UNIQUE,
  IsActive     bit NOT NULL CONSTRAINT DF_Buyer_IsActive DEFAULT (1)
);

CREATE TABLE dbo.BuyerZipQuote (
  BuyerZipQuoteId int IDENTITY PRIMARY KEY,
  BuyerId      int NOT NULL REFERENCES dbo.Buyer(BuyerId),
  Zip          char(5) NOT NULL REFERENCES dbo.ZipCode(Zip),
  QuoteAmount  money NOT NULL CHECK (QuoteAmount >= 0),
  CONSTRAINT UQ_BuyerZip UNIQUE (BuyerId, Zip)
);

CREATE TABLE dbo.CarQuote (
  CarQuoteId   int IDENTITY PRIMARY KEY,
  CarId        int NOT NULL REFERENCES dbo.Car(CarId),
  BuyerId      int NOT NULL REFERENCES dbo.Buyer(BuyerId),
  Amount       money NOT NULL CHECK (Amount >= 0),
  CreatedUtc   datetime2(0) NOT NULL CONSTRAINT DF_CarQuote_CreatedUtc DEFAULT (SYSUTCDATETIME()),
  IsCurrent    bit NOT NULL CONSTRAINT DF_CarQuote_IsCurrent DEFAULT (0)
);

CREATE UNIQUE INDEX IX_CarQuote_UqCurrent ON dbo.CarQuote (CarId) WHERE IsCurrent = 1;

CREATE TABLE dbo.Status (
  StatusId     int IDENTITY PRIMARY KEY,
  Name         varchar(60) NOT NULL UNIQUE,
  RequiresDate bit NOT NULL CONSTRAINT DF_Status_RequiresDate DEFAULT (0)
);

CREATE TABLE dbo.CarStatus (
  CarStatusId  int IDENTITY PRIMARY KEY,
  CarId        int NOT NULL REFERENCES dbo.Car(CarId),
  StatusId     int NOT NULL REFERENCES dbo.Status(StatusId),
  StatusDate   datetime2(0) NULL,
  ChangedBy    varchar(100) NULL,
  CreatedUtc   datetime2(0) NOT NULL CONSTRAINT DF_CarStatus_CreatedUtc DEFAULT (SYSUTCDATETIME()),
  IsCurrent    bit NOT NULL CONSTRAINT DF_CarStatus_IsCurrent DEFAULT (0)
);

CREATE UNIQUE INDEX IX_CarStatus_UqCurrent ON dbo.CarStatus (CarId) WHERE IsCurrent = 1;

-- Sample query (Car info + current buyer + current status)
SELECT
  c.CarId,
  c.Year,
  mk.Name  AS Make,
  mo.Name  AS Model,
  sm.Name  AS Submodel,
  c.Zip,
  b.Name   AS CurrentBuyer,
  cq.Amount AS CurrentQuote,
  s.Name   AS CurrentStatus,
  cs.StatusDate
FROM dbo.Car AS c
JOIN dbo.Make  AS mk ON mk.MakeId = c.MakeId
JOIN dbo.Model AS mo ON mo.ModelId = c.ModelId
LEFT JOIN dbo.Submodel AS sm ON sm.SubmodelId = c.SubmodelId
LEFT JOIN dbo.CarQuote AS cq ON cq.CarId = c.CarId AND cq.IsCurrent = 1
LEFT JOIN dbo.Buyer    AS b  ON b.BuyerId = cq.BuyerId
LEFT JOIN dbo.CarStatus AS cs ON cs.CarId = c.CarId AND cs.IsCurrent = 1
LEFT JOIN dbo.Status     AS s  ON s.StatusId = cs.StatusId;
