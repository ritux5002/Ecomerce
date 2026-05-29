using MiEcommerce.Domain.Common;
using MiEcommerce.Domain.ValueObjects;

namespace MiEcommerce.Domain.Entities;

/// <summary>
/// Entidad de cliente en el e-commerce.
/// </summary>
public class Customer : BaseEntity
{
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Customer(Guid id, string name, Email email) : base(id)
    {
        Name = name;
        Email = email;
        CreatedAt = DateTime.UtcNow;
    }

    private Customer(string name, Email email) : base()
    {
        Name = name;
        Email = email;
        CreatedAt = DateTime.UtcNow;
    }

    public static Customer Create(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del cliente no puede estar vacío.", nameof(name));

        var emailVo = new Email(email);
        return new Customer(name, emailVo);
    }

    public static Customer CreateWithId(Guid id, string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del cliente no puede estar vacío.", nameof(name));

        var emailVo = new Email(email);
        return new Customer(id, name, emailVo);
    }
}
