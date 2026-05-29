using MediatR;
using MiEcommerce.Application.DTOs;
using MiEcommerce.Domain.Entities;
using MiEcommerce.Domain.Exceptions;
using MiEcommerce.Domain.Interfaces;

namespace MiEcommerce.Application.Features.Products.Queries.GetById;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductByIdDto>;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductByIdDto>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductByIdDto> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(query.Id, cancellationToken);
        if (product is null)
            throw new NotFoundException(nameof(Product), query.Id);

        return new ProductByIdDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.IsActive,
            product.CategoryId
        );
    }
}

