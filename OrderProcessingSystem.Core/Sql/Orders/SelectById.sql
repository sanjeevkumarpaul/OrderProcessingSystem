SELECT o.OrderId, o.CustomerId, o.SupplierId, o.Total, o.Status,
       s.SupplierId, s.Name, s.Country,
       c.CustomerId, c.Name
FROM Orders o
LEFT JOIN Suppliers s ON s.SupplierId = o.SupplierId
LEFT JOIN Customers c ON c.CustomerId = o.CustomerId
WHERE o.OrderId = @Id
