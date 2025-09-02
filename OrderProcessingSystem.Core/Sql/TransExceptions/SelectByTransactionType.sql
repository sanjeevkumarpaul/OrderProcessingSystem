SELECT TransExceptionId, TransactionType, InputMessage, Reason, RunTime
FROM TransExceptions
WHERE TransactionType = @TransactionType
ORDER BY RunTime DESC
