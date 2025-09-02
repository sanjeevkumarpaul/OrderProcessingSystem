INSERT INTO TransExceptions (TransactionType, InputMessage, Reason, RunTime) 
VALUES (@TransactionType, @InputMessage, @Reason, @RunTime); 
SELECT last_insert_rowid();
