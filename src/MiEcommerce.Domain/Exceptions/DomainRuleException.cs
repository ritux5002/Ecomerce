namespace MiEcommerce.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando se viola una regla de negocio del dominio.
/// </summary>
public sealed class DomainRuleException : DomainException
{
    public DomainRuleException(string message) : base(message)
    {
    }
}
