namespace MiEcommerce.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando una entidad no existe en la base de datos.
/// </summary>
public sealed class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object id)
        : base($"{entityName} with id {id} was not found")
    {
    }
}
