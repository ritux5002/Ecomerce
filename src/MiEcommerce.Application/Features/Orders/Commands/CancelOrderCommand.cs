using MediatR;
using MiEcommerce.Domain.Entities;
using MiEcommerce.Domain.Enums;
using MiEcommerce.Domain.Exceptions;
using MiEcommerce.Domain.Interfaces;

namespace MiEcommerce.Application.Features.Orders.Commands.CancelOrder;

public record CancelOrderCommand(Guid OrderId) : IRequest<Unit>;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken);
        if (order is null)
            throw new Domain.Exceptions.NotFoundException(nameof(Order), command.OrderId);

        // Si la orden fue confirmada, restaurar stock
        var orderStatus = (Domain.Enums.OrderStatus)order.Status;
        if (orderStatus == Domain.Enums.OrderStatus.Confirmed)
        {
            foreach (var item in order.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
                if (product is not null)
                {
                    product.AddStock(item.Quantity);
                    await _productRepository.UpdateAsync(product, cancellationToken);
                }
            }
        }

        order.Cancel();
        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
