using MediatR;
using MiEcommerce.Domain.Entities;
using MiEcommerce.Domain.Exceptions;
using MiEcommerce.Domain.Interfaces;

namespace MiEcommerce.Application.Features.Orders.Commands.DeliverOrder;

public record DeliverOrderCommand(Guid OrderId) : IRequest<Unit>;

public class DeliverOrderCommandHandler : IRequestHandler<DeliverOrderCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeliverOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeliverOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken);
        if (order is null)
            throw new Domain.Exceptions.NotFoundException(nameof(Order), command.OrderId);

        order.Deliver();
        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
