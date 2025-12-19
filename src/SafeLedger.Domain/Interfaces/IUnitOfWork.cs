namespace SafeLedger.Domain.Interfaces;

/// <summary>
/// Interfaz del patrón Unit of Work para gestionar transacciones y coordinar operaciones de repositorio.
/// Garantiza propiedades ACID en múltiples operaciones de repositorio.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Obtiene el repositorio de cuentas.
    /// </summary>
    IAccountRepository Accounts { get; }
    
    /// <summary>
    /// Guarda todos los cambios realizados en el contexto de unidad de trabajo actual.
    /// </summary>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>El número de entidades guardadas.</returns>
    Task<int> SaveChangesAsync(CancellationToken ct);
    
    /// <summary>
    /// Inicia una nueva transacción de base de datos.
    /// </summary>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Un objeto de transacción que puede ser confirmado o revertido.</returns>
    Task<ITransaction> BeginTransactionAsync(CancellationToken ct);
}