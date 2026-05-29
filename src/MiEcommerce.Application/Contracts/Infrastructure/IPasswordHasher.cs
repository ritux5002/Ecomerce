namespace MiEcommerce.Application.Contracts.Infrastructure;

/// <summary>
/// Contrato para el servicio de hasheado de contraseñas.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
