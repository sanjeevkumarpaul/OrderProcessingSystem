SELECT TransExceptionId, TransactionType, InputMessage, Reason, RunTime
FROM TransExceptions
WHERE TransExceptionId = @Id
