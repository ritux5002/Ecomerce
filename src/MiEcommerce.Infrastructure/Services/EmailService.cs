using MiEcommerce.Application.Contracts.Infrastructure;

namespace MiEcommerce.Infrastructure.Services;

/// <summary>
/// Implementación básica del servicio de envío de emails.
/// En producción, conectar con SendGrid, AWS SES, etc.
/// </summary>
public class EmailService : IEmailService
{
    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        // TODO: Implementar envío real de emails
        return Task.CompletedTask;
    }
}
