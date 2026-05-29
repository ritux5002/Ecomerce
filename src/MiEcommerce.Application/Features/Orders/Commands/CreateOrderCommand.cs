using FluentValidation;
using MediatR;
using MiEcommerce.Application.DTOs;
using MiEcommerce.Domain.Entities;
using MiEcommerce.Domain.Interfaces;

namespace MiEcommerce.Application.Features.Orders.Commands.CreateOrder;

public record CreateOrderCommand(Guid CustomerId) : IRequest<CreateOrderResponse>;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
    }
}

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateOrderResponse> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(command.CustomerId, cancellationToken);
        if (customer is null)
            throw new Domain.Exceptions.NotFoundException(nameof(Customer), command.CustomerId);

        var order = Order.Create(command.CustomerId);
        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateOrderResponse(order.Id);
    }
}

