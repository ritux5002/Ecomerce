namespace MiEcommerce.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando no hay suficiente stock para una operación.
/// </summary>
public sealed class InsufficientStockException : DomainException
{
    public InsufficientStockException(int requested, int available)
        : base($"Cannot reserve {requested} units — only {available} available")
    {
    }
}
