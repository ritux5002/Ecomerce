namespace MiEcommerce.Application.DTOs;

public record CategoryDto(Guid Id, string Name);
public record CreateCategoryResponse(Guid Id);
