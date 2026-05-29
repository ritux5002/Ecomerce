namespace MiEcommerce.Application.DTOs;

public record ProductDto(Guid Id, string Name, string Description, decimal Price, int Stock, bool IsActive, Guid CategoryId);
public record ProductByIdDto(Guid Id, string Name, string Description, decimal Price, int Stock, bool IsActive, Guid CategoryId);
public record CreateProductResponse(Guid Id);
