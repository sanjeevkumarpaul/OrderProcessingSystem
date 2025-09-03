-- Create UserLog table for Order Processing System
-- This table stores user activity logs for audit and monitoring purposes

CREATE TABLE IF NOT EXISTS UserLogs (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EventDate DATETIME NOT NULL,
    Event NVARCHAR(200) NOT NULL, -- Description of the event/action
    EventFlag NVARCHAR(50) NOT NULL, -- Type/category flag for the event
    UserId NVARCHAR(255) NOT NULL, -- User identifier (email address)
    UserName NVARCHAR(100) NOT NULL -- User name
);

-- Create indexes for better query performance
CREATE INDEX IF NOT EXISTS IX_UserLogs_EventDate ON UserLogs (EventDate);
CREATE INDEX IF NOT EXISTS IX_UserLogs_UserId ON UserLogs (UserId);
CREATE INDEX IF NOT EXISTS IX_UserLogs_EventFlag ON UserLogs (EventFlag);
CREATE INDEX IF NOT EXISTS IX_UserLogs_UserId_EventDate ON UserLogs (UserId, EventDate);
CREATE INDEX IF NOT EXISTS IX_UserLogs_EventFlag_EventDate ON UserLogs (EventFlag, EventDate);

-- Sample query to verify table creation
-- SELECT name FROM sqlite_master WHERE type='table' AND name='UserLogs';
