namespace MiEcommerce.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando no se puede contactar al PaymentService externo,
/// o este responde con un error. No es una regla de negocio violada, sino una
/// falla de un servicio del que depende el caso de uso.
/// </summary>
public sealed class PaymentServiceUnavailableException : Exception
{
    public PaymentServiceUnavailableException(string message) : base(message)
    {
    }

    public PaymentServiceUnavailableException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
