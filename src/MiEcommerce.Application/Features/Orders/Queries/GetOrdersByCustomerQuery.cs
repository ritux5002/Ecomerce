using FluentValidation;
using MediatR;
using MiEcommerce.Application.DTOs;
using MiEcommerce.Domain.Exceptions;
using MiEcommerce.Domain.Interfaces;

namespace MiEcommerce.Application.Features.Orders.Queries.GetByCustomer;

public record GetOrdersByCustomerQuery(Guid CustomerId) : IRequest<IEnumerable<OrderDto>>;

public class GetOrdersByCustomerQueryValidator : AbstractValidator<GetOrdersByCustomerQuery>
{
    public GetOrdersByCustomerQueryValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
    }
}

public class GetOrdersByCustomerQueryHandler : IRequestHandler<GetOrdersByCustomerQuery, IEnumerable<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;

    public GetOrdersByCustomerQueryHandler(IOrderRepository orderRepository, ICustomerRepository customerRepository)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<OrderDto>> Handle(GetOrdersByCustomerQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
            throw new NotFoundException(nameof(Domain.Entities.Customer), request.CustomerId);

        var orders = await _orderRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);

        return orders.Select(o => new OrderDto(
            o.Id,
            o.CustomerId,
            (int)o.Status,
            o.CreatedAt,
            o.Items.Sum(i => i.Quantity * i.UnitPrice)
        )).ToList();
    }
}
