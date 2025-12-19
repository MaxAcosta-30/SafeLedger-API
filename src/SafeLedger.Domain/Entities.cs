namespace SafeLedger.Domain.Entities;

/// <summary>
/// Representa una cuenta financiera con gestión de saldo y lógica de negocio orientada al dominio.
/// Refuerza la encapsulación al prevenir la modificación externa del saldo sin usar métodos de dominio.
/// </summary>
public class Account
{
    private Account() { }

    /// <summary>
    /// Inicializa una nueva instancia de la entidad Account.
    /// </summary>
    /// <param name="id">Identificador único de la cuenta.</param>
    /// <param name="ownerId">Identificador único del propietario de la cuenta.</param>
    /// <param name="currency">Código de moneda (ej: USD, EUR). Se normalizará a mayúsculas.</param>
    /// <param name="initialBalance">Saldo inicial (por defecto 0).</param>
    /// <exception cref="ArgumentNullException">Se lanza cuando la moneda es null o está en blanco.</exception>
    /// <exception cref="ArgumentException">Se lanza cuando el saldo inicial es negativo.</exception>
    public Account(Guid id, Guid ownerId, string currency, decimal initialBalance = 0)
    {
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentNullException(nameof(currency));
        if (initialBalance < 0) throw new ArgumentException("El saldo inicial no puede ser negativo.");

        Id = id;
        OwnerId = ownerId;
        Currency = currency.ToUpper();
        Balance = initialBalance;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid OwnerId { get; private set; }
    public decimal Balance { get; private set; }
    public string Currency { get; private set; } = null!;
    public DateTime CreatedAtUtc { get; private set; }
    public byte[]? RowVersion { get; private set; }

    /// <summary>
    /// Acredita (suma) el monto especificado al saldo de la cuenta.
    /// </summary>
    /// <param name="amount">El monto a acreditar (debe ser positivo).</param>
    /// <exception cref="ArgumentException">Se lanza cuando el monto no es positivo.</exception>
    public void Credit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("El monto a acreditar debe ser positivo.");
        
        Balance += amount;
    }

    /// <summary>
    /// Debita (resta) el monto especificado del saldo de la cuenta.
    /// </summary>
    /// <param name="amount">El monto a debitar (debe ser positivo).</param>
    /// <exception cref="ArgumentException">Se lanza cuando el monto no es positivo.</exception>
    /// <exception cref="InvalidOperationException">Se lanza cuando la cuenta tiene fondos insuficientes.</exception>
    public void Debit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("El monto a debitar debe ser positivo.");
        
        if (Balance - amount < 0)
        {
            throw new InvalidOperationException($"Fondos insuficientes. Saldo actual: {Balance}, Intento de retiro: {amount}");
        }

        Balance -= amount;
    }
}