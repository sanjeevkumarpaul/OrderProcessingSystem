INSERT INTO Suppliers (Name, Country) 
VALUES (@Name, @Country); 
SELECT last_insert_rowid();
