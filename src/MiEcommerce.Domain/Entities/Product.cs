using MiEcommerce.Domain.Common;
using MiEcommerce.Domain.Exceptions;

namespace MiEcommerce.Domain.Entities;

/// <summary>
/// Entidad de producto en el e-commerce.
/// </summary>
public class Product : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public bool IsActive { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }

    private Product(Guid id, string name, string description, decimal price, int stock, Guid categoryId)
        : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        IsActive = true;
        CategoryId = categoryId;
    }

    private Product(string name, string description, decimal price, int stock, Guid categoryId)
        : base()
    {
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        IsActive = true;
        CategoryId = categoryId;
    }

    public static Product Create(string name, string description, decimal price, int stock, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del producto no puede estar vacío.", nameof(name));

        if (price <= 0)
            throw new ArgumentException("El precio debe ser mayor a cero.", nameof(price));

        if (stock < 0)
            throw new ArgumentException("El stock no puede ser negativo.", nameof(stock));

        if (categoryId == Guid.Empty)
            throw new ArgumentException("El ID de categoría no puede estar vacío.", nameof(categoryId));

        return new Product(name, description, price, stock, categoryId);
    }

    public static Product CreateWithId(Guid id, string name, string description, decimal price, int stock, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del producto no puede estar vacío.", nameof(name));

        if (price <= 0)
            throw new ArgumentException("El precio debe ser mayor a cero.", nameof(price));

        if (stock < 0)
            throw new ArgumentException("El stock no puede ser negativo.", nameof(stock));

        if (categoryId == Guid.Empty)
            throw new ArgumentException("El ID de categoría no puede estar vacío.", nameof(categoryId));

        return new Product(id, name, description, price, stock, categoryId);
    }

    public void UpdateDetails(string name, string description, int stock, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del producto no puede estar vacío.", nameof(name));

        if (stock < 0)
            throw new ArgumentException("El stock no puede ser negativo.", nameof(stock));

        if (categoryId == Guid.Empty)
            throw new ArgumentException("El ID de categoría no puede estar vacío.", nameof(categoryId));

        Name = name;
        Description = description;
        Stock = stock;
        CategoryId = categoryId;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new ArgumentException("El precio debe ser mayor a cero.", nameof(newPrice));

        Price = newPrice;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(quantity));

        Stock += quantity;
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(quantity));

        if (quantity > Stock)
            throw new InsufficientStockException(quantity, Stock);

        Stock -= quantity;
    }

    public void Reserve(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(quantity));

        if (quantity > Stock)
            throw new InsufficientStockException(quantity, Stock);

        Stock -= quantity;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
