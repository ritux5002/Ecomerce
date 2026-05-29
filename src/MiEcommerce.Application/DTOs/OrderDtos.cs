namespace MiEcommerce.Application.DTOs;

public record OrderDto(Guid Id, Guid CustomerId, int Status, DateTime CreatedAt, decimal Total);
public record OrderItemDto(Guid ProductId, int Quantity, decimal UnitPrice);
public record OrderByIdDto(Guid Id, Guid CustomerId, int Status, DateTime CreatedAt, List<OrderItemDto> Items, decimal Total);
public record CreateOrderResponse(Guid Id);
