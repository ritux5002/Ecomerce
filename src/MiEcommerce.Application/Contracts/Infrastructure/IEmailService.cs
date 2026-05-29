namespace MiEcommerce.Application.Contracts.Infrastructure;

/// <summary>
/// Contrato para el servicio de envío de correos electrónicos.
/// </summary>
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}
