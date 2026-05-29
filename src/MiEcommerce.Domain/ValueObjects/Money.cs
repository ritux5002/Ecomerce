namespace MiEcommerce.Domain.ValueObjects;

/// <summary>
/// Value Object que representa dinero (monto + moneda).
/// Inmutable, con operación de suma.
/// </summary>
public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency = "ARS")
    {
        if (amount < 0)
            throw new ArgumentException("El monto no puede ser negativo.", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("La moneda no puede estar vacía.", nameof(currency));

        Amount = amount;
        Currency = currency;
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"No se pueden sumar monedas diferentes: {Currency} vs {other.Currency}");

        return new Money(Amount + other.Amount, Currency);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Money);
    }

    public bool Equals(Money? other)
    {
        return other is not null && Amount == other.Amount && Currency == other.Currency;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Amount, Currency);
    }

    public static bool operator ==(Money? left, Money? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Money? left, Money? right)
    {
        return !Equals(left, right);
    }

    public override string ToString()
    {
        return $"{Amount:F2} {Currency}";
    }
}
