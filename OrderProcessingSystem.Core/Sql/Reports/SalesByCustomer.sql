SELECT
    o.CustomerId as CustomerId,
    COALESCE(c.Name, '') as CustomerName,
    SUM(o.Total) as TotalSales,
    COUNT(1) as OrderCount
FROM Orders o
LEFT JOIN Customers c ON c.CustomerId = o.CustomerId
WHERE (@CustomerId IS NULL OR o.CustomerId = @CustomerId)
GROUP BY o.CustomerId, c.Name
ORDER BY TotalSales DESC
