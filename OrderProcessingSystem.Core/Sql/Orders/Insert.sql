INSERT INTO Orders (CustomerId, Status, SupplierId, Total) 
VALUES (@CustomerId, @Status, @SupplierId, @Total); 
SELECT last_insert_rowid();
