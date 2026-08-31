-- Conexión: (localdb)\MSSQLLocalDB | Base de datos: MiEcommerceDb

-- Categorías
SELECT * FROM Categories;

-- Productos
SELECT p.Id, p.Name, p.Description, p.Price, p.Stock, p.IsActive, c.Name AS Category
FROM Products p
JOIN Categories c ON p.CategoryId = c.Id;

-- Clientes
SELECT * FROM Customers;

-- Órdenes
SELECT o.Id, c.Name AS Customer, o.Status, o.CreatedAt
FROM Orders o
JOIN Customers c ON o.CustomerId = c.Id;

-- Items de órdenes
SELECT oi.Id, o.Id AS OrderId, p.Name AS Product, oi.Quantity, oi.UnitPrice
FROM OrderItems oi
JOIN Orders o ON oi.OrderId = o.Id
JOIN Products p ON oi.ProductId = p.Id;

-- Usuarios
SELECT Id, Name, Email, Role FROM Users;
