namespace MiEcommerce.Domain.Common;

/// <summary>
/// Clase base para todas las entidades del dominio.
/// Genera automáticamente un GUID como identificador único.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; private set; }

    protected BaseEntity(Guid id)
    {
        Id = id;
    }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
    }

    public override bool Equals(object? obj)
    {
        return obj is BaseEntity entity && entity.Id == Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(BaseEntity? left, BaseEntity? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(BaseEntity? left, BaseEntity? right)
    {
        return !Equals(left, right);
    }
}
