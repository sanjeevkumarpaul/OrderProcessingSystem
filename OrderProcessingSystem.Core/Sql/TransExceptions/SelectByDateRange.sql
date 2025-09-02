SELECT TransExceptionId, TransactionType, InputMessage, Reason, RunTime
FROM TransExceptions
WHERE RunTime BETWEEN @StartDate AND @EndDate
ORDER BY RunTime DESC
