using FluentValidation;
using MediatR;
using MiEcommerce.Domain.Entities;
using MiEcommerce.Domain.Exceptions;
using MiEcommerce.Domain.Interfaces;

namespace MiEcommerce.Application.Features.Orders.Commands.AddItemToOrder;

public record AddItemToOrderCommand(
    Guid OrderId,
    Guid ProductId,
    int Quantity
) : IRequest<Unit>;

public class AddItemToOrderCommandValidator : AbstractValidator<AddItemToOrderCommand>
{
    public AddItemToOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}

public class AddItemToOrderCommandHandler : IRequestHandler<AddItemToOrderCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddItemToOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(AddItemToOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken);
        if (order is null)
            throw new Domain.Exceptions.NotFoundException(nameof(Order), command.OrderId);

        var product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken);
        if (product is null)
            throw new Domain.Exceptions.NotFoundException(nameof(Product), command.ProductId);

        order.AddItem(command.ProductId, command.Quantity, product.Price);
        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
