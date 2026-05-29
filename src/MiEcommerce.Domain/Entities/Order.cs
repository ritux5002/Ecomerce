using MiEcommerce.Domain.Common;
using MiEcommerce.Domain.Enums;
using MiEcommerce.Domain.Exceptions;

namespace MiEcommerce.Domain.Entities;

/// <summary>
/// Entidad de orden de compra.
/// </summary>
public class Order : BaseEntity
{
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ICollection<OrderItem> Items { get; private set; }

    private Order(Guid id, Guid customerId) : base(id)
    {
        CustomerId = customerId;
        Status = OrderStatus.Draft;
        CreatedAt = DateTime.UtcNow;
        Items = new List<OrderItem>();
    }

    private Order(Guid customerId) : base()
    {
        CustomerId = customerId;
        Status = OrderStatus.Draft;
        CreatedAt = DateTime.UtcNow;
        Items = new List<OrderItem>();
    }

    public static Order Create(Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("El ID del cliente no puede estar vacío.", nameof(customerId));

        return new Order(customerId);
    }

    public static Order CreateWithId(Guid id, Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("El ID del cliente no puede estar vacío.", nameof(customerId));

        return new Order(id, customerId);
    }

    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        if (Status != OrderStatus.Draft)
            throw new DomainRuleException("No se pueden agregar ítems a una orden que no está en estado Draft.");

        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(quantity));

        if (unitPrice <= 0)
            throw new ArgumentException("El precio debe ser mayor a cero.", nameof(unitPrice));

        var existingItem = Items.FirstOrDefault(x => x.ProductId == productId);
        if (existingItem is not null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var item = OrderItem.Create(Id, productId, quantity, unitPrice);
            Items.Add(item);
        }
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Draft)
            throw new DomainRuleException("Solo se pueden confirmar órdenes en estado Draft.");

        if (!Items.Any())
            throw new DomainRuleException("No se puede confirmar una orden sin ítems.");

        Status = OrderStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Cancelled || Status == OrderStatus.Delivered)
            throw new DomainRuleException("No se puede cancelar una orden que ya fue cancelada o entregada.");

        Status = OrderStatus.Cancelled;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Confirmed)
            throw new DomainRuleException("Solo se pueden enviar órdenes en estado Confirmed.");

        Status = OrderStatus.Shipped;
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new DomainRuleException("Solo se pueden entregar órdenes en estado Shipped.");

        Status = OrderStatus.Delivered;
    }

    public decimal GetTotal()
    {
        return Items.Sum(x => x.Quantity * x.UnitPrice);
    }
}
