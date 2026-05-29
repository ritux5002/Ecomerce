using System.Text.RegularExpressions;

namespace MiEcommerce.Domain.ValueObjects;

/// <summary>
/// Value Object que representa un correo electrónico.
/// Inmutable, con validación de formato y igualdad por valor.
/// </summary>
public sealed class Email : IEquatable<Email>
{
    public string Value { get; }

    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled);

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El correo no puede estar vacío.", nameof(value));

        if (!EmailRegex.IsMatch(value))
            throw new ArgumentException("El formato del correo no es válido.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Email);
    }

    public bool Equals(Email? other)
    {
        return other is not null && Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(Email? left, Email? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Email? left, Email? right)
    {
        return !Equals(left, right);
    }

    public override string ToString()
    {
        return Value;
    }
}
