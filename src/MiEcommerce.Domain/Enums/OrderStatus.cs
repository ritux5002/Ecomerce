namespace MiEcommerce.Domain.Enums;

/// <summary>
/// Estados posibles de una orden.
/// </summary>
public enum OrderStatus
{
    Draft = 1,
    Confirmed = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5,
    Paid = 6,
    PaymentRejected = 7
}
