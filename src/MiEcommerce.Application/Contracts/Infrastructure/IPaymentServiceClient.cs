namespace MiEcommerce.Application.Contracts.Infrastructure;

/// <summary>
/// Puerto hacia el microservicio externo PaymentService (proyecto independiente,
/// se comunica por HTTP). La implementación concreta vive en Infrastructure.
/// </summary>
public interface IPaymentServiceClient
{
    Task<PaymentResult> ProcessPaymentAsync(Guid orderId, decimal amount, CancellationToken cancellationToken);
}

public record PaymentResult(string Status, string TransactionId);
