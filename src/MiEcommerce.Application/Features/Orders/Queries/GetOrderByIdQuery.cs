using MediatR;
using MiEcommerce.Application.DTOs;
using MiEcommerce.Domain.Entities;
using MiEcommerce.Domain.Exceptions;
using MiEcommerce.Domain.Interfaces;

namespace MiEcommerce.Application.Features.Orders.Queries.GetById;

public record GetOrderByIdQuery(Guid Id) : IRequest<OrderByIdDto>;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderByIdDto>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderByIdDto> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(query.Id, cancellationToken);
        if (order is null)
            throw new Domain.Exceptions.NotFoundException(nameof(Order), query.Id);

        return new OrderByIdDto(
            order.Id,
            order.CustomerId,
            (int)order.Status,
            order.CreatedAt,
            order.Items.Select(i => new OrderItemDto(i.ProductId, i.Quantity, i.UnitPrice)).ToList(),
            order.GetTotal()
        );
    }
}

