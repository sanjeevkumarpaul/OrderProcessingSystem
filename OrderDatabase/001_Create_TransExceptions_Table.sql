-- Create TransExceptions table for Order Processing System
-- This table stores exceptions that occur during order processing transactions

CREATE TABLE IF NOT EXISTS TransExceptions (
    TransExceptionId INTEGER PRIMARY KEY AUTOINCREMENT,
    TransactionType NVARCHAR(50) NOT NULL, -- ORDERCREATION or ORDERCANCELLATION
    InputMessage TEXT NOT NULL, -- JSON message that was passed
    Reason NVARCHAR(1000) NOT NULL, -- Reason for being caught at TransException table  
    RunTime DATETIME NOT NULL DEFAULT (datetime('now')) -- Date and Time with default constraint
);

-- Create indexes for better query performance
CREATE INDEX IF NOT EXISTS IX_TransExceptions_TransactionType ON TransExceptions (TransactionType);
CREATE INDEX IF NOT EXISTS IX_TransExceptions_RunTime ON TransExceptions (RunTime);
CREATE INDEX IF NOT EXISTS IX_TransExceptions_TransactionType_RunTime ON TransExceptions (TransactionType, RunTime);

-- Sample query to verify table creation
-- SELECT name FROM sqlite_master WHERE type='table' AND name='TransExceptions';
