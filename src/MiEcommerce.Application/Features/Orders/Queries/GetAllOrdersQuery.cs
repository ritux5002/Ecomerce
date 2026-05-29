using MediatR;
using MiEcommerce.Application.DTOs;
using MiEcommerce.Domain.Interfaces;

namespace MiEcommerce.Application.Features.Orders.Queries.GetAll;

public record GetAllOrdersQuery : IRequest<IEnumerable<OrderDto>>;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, IEnumerable<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetAllOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IEnumerable<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);
        return orders.Select(o => new OrderDto(
            o.Id,
            o.CustomerId,
            (int)o.Status,
            o.CreatedAt,
            o.GetTotal()
        )).ToList();
    }
}

