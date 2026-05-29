using MiEcommerce.Domain.Common;

namespace MiEcommerce.Domain.Entities;

/// <summary>
/// Entidad de ítem en una orden.
/// </summary>
public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public Order? Order { get; private set; }
    public Product? Product { get; private set; }

    private OrderItem(Guid id, Guid orderId, Guid productId, int quantity, decimal unitPrice) : base(id)
    {
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    private OrderItem(Guid orderId, Guid productId, int quantity, decimal unitPrice) : base()
    {
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public static OrderItem Create(Guid orderId, Guid productId, int quantity, decimal unitPrice)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("El ID de la orden no puede estar vacío.", nameof(orderId));

        if (productId == Guid.Empty)
            throw new ArgumentException("El ID del producto no puede estar vacío.", nameof(productId));

        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(quantity));

        if (unitPrice <= 0)
            throw new ArgumentException("El precio unitario debe ser mayor a cero.", nameof(unitPrice));

        return new OrderItem(orderId, productId, quantity, unitPrice);
    }

    public static OrderItem CreateWithId(Guid id, Guid orderId, Guid productId, int quantity, decimal unitPrice)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("El ID de la orden no puede estar vacío.", nameof(orderId));

        if (productId == Guid.Empty)
            throw new ArgumentException("El ID del producto no puede estar vacío.", nameof(productId));

        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(quantity));

        if (unitPrice <= 0)
            throw new ArgumentException("El precio unitario debe ser mayor a cero.", nameof(unitPrice));

        return new OrderItem(id, orderId, productId, quantity, unitPrice);
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(newQuantity));

        Quantity = newQuantity;
    }
}
