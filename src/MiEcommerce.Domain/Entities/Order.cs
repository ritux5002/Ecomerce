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
    public string? TransactionId { get; private set; }
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

    /// <summary>
    /// Agrega un producto a la orden. Devuelve el <see cref="OrderItem"/> recién creado
    /// cuando el producto no estaba en la orden, o null cuando se fusionó con un ítem
    /// existente (aumentando su cantidad) — el caller usa ese valor para saber si debe
    /// registrar explícitamente el nuevo ítem en el repositorio.
    /// </summary>
    public OrderItem? AddItem(Guid productId, int quantity, decimal unitPrice)
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
            return null;
        }

        var item = OrderItem.Create(Id, productId, quantity, unitPrice);
        Items.Add(item);
        return item;
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
        if (Status == OrderStatus.Cancelled || Status == OrderStatus.Delivered || Status == OrderStatus.Paid)
            throw new DomainRuleException("No se puede cancelar una orden que ya fue cancelada, entregada o pagada.");

        Status = OrderStatus.Cancelled;
    }

    public void MarkAsPaid(string transactionId)
    {
        if (Status != OrderStatus.Confirmed)
            throw new DomainRuleException("Solo se pueden marcar como pagadas las órdenes en estado Confirmed.");

        Status = OrderStatus.Paid;
        TransactionId = transactionId;
    }

    public void MarkPaymentRejected(string? transactionId)
    {
        if (Status != OrderStatus.Confirmed)
            throw new DomainRuleException("Solo se puede rechazar el pago de órdenes en estado Confirmed.");

        Status = OrderStatus.PaymentRejected;
        TransactionId = transactionId;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Paid)
            throw new DomainRuleException("Solo se pueden enviar órdenes en estado Paid.");

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
