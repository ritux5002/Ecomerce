using MiEcommerce.Domain.Common;

namespace MiEcommerce.Domain.Entities;

/// <summary>
/// Entidad de usuario para autenticación y autorización.
/// </summary>
public class User : BaseEntity
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Role { get; private set; }

    private User(Guid id, string name, string email, string passwordHash, string role) : base(id)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }

    private User(string name, string email, string passwordHash, string role) : base()
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }

    public static User Create(string name, string email, string passwordHash, string role = "Customer")
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El correo no puede estar vacío.", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("El hash de contraseña no puede estar vacío.", nameof(passwordHash));

        if (role != "Admin" && role != "Customer")
            throw new ArgumentException("El rol debe ser 'Admin' o 'Customer'.", nameof(role));

        return new User(name, email, passwordHash, role);
    }

    public static User CreateWithId(Guid id, string name, string email, string passwordHash, string role = "Customer")
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El correo no puede estar vacío.", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("El hash de contraseña no puede estar vacío.", nameof(passwordHash));

        if (role != "Admin" && role != "Customer")
            throw new ArgumentException("El rol debe ser 'Admin' o 'Customer'.", nameof(role));

        return new User(id, name, email, passwordHash, role);
    }
}
