namespace MiEcommerce.Application.DTOs;

public record CustomerByIdDto(Guid Id, string Name, string Email, DateTime CreatedAt);
public record RegisterCustomerResponse(Guid Id);
