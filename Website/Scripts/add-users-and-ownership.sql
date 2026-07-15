-- Adds login/ownership support: a Users table, and a CreatedByUserId column
-- on each log's header table so edits can be restricted to the creator (or master).

CREATE TABLE Users (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    IsMaster BIT NOT NULL DEFAULT 0
);

INSERT INTO Users (Name, IsMaster) VALUES ('Admin', 1);

ALTER TABLE QCTestLogHeaders ADD CreatedByUserId INT NULL;
ALTER TABLE OperatorAuditHeaders ADD CreatedByUserId INT NULL;
ALTER TABLE QCAuditLogs ADD CreatedByUserId INT NULL, LoggedBy NVARCHAR(200) NULL;
