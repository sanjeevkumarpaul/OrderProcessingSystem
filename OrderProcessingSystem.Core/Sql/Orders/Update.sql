UPDATE Orders 
SET CustomerId = @CustomerId, Status = @Status, SupplierId = @SupplierId, Total = @Total 
WHERE OrderId = @OrderId
