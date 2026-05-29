using MiEcommerce.Domain.Common;

namespace MiEcommerce.Domain.Entities;

/// <summary>
/// Entidad de categoría de productos.
/// </summary>
public class Category : BaseEntity
{
    public string Name { get; private set; }
    public ICollection<Product> Products { get; private set; }

    private Category(Guid id, string name) : base(id)
    {
        Name = name;
        Products = new List<Product>();
    }

    private Category(string name) : base()
    {
        Name = name;
        Products = new List<Product>();
    }

    public static Category Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre de la categoría no puede estar vacío.", nameof(name));

        return new Category(name);
    }

    public static Category CreateWithId(Guid id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre de la categoría no puede estar vacío.", nameof(name));

        return new Category(id, name);
    }
}
