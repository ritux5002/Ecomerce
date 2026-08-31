using MediatR;
using MiEcommerce.Application.Contracts.Infrastructure;
using MiEcommerce.Application.DTOs;
using MiEcommerce.Domain.Entities;
using MiEcommerce.Domain.Exceptions;
using MiEcommerce.Domain.Interfaces;

namespace MiEcommerce.Application.Features.Orders.Commands.ConfirmOrder;

public record ConfirmOrderCommand(Guid OrderId) : IRequest<ConfirmOrderResponse>;

public class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, ConfirmOrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IPaymentServiceClient _paymentServiceClient;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IPaymentServiceClient paymentServiceClient,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _paymentServiceClient = paymentServiceClient;
        _unitOfWork = unitOfWork;
    }

    public async Task<ConfirmOrderResponse> Handle(ConfirmOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken);
        if (order is null)
            throw new Domain.Exceptions.NotFoundException(nameof(Order), command.OrderId);

        // 1) Validar stock y reservarlo para cada ítem
        foreach (var item in order.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product is null)
                throw new Domain.Exceptions.NotFoundException(nameof(Product), item.ProductId);

            if (product.Stock < item.Quantity)
                throw new Domain.Exceptions.InsufficientStockException(item.Quantity, product.Stock);

            product.Reserve(item.Quantity);
            await _productRepository.UpdateAsync(product, cancellationToken);
        }

        order.Confirm();

        // 2) Procesar el pago contra el microservicio externo PaymentService
        var total = order.GetTotal();
        var paymentResult = await _paymentServiceClient.ProcessPaymentAsync(order.Id, total, cancellationToken);

        // 3) Actuar según la respuesta del PaymentService
        if (string.Equals(paymentResult.Status, "Approved", StringComparison.OrdinalIgnoreCase))
        {
            order.MarkAsPaid(paymentResult.TransactionId);
        }
        else
        {
            order.MarkPaymentRejected(paymentResult.TransactionId);

            // El pago no se concretó: liberar el stock reservado en el paso 1
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

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ConfirmOrderResponse(order.Id, order.Status.ToString(), order.TransactionId);
    }
}
