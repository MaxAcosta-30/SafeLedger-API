using SafeLedger.Domain.Entities;

namespace SafeLedger.Domain.Interfaces;

/// <summary>
/// Interfaz del repositorio para operaciones de persistencia de registros de transacciones.
/// </summary>
public interface ITransactionRepository
{
    /// <summary>
    /// Agrega un nuevo registro de transacción al repositorio.
    /// </summary>
    /// <param name="transactionRecord">El registro de transacción a agregar.</param>
    /// <param name="ct">Token de cancelación.</param>
    Task AddAsync(TransactionRecord transactionRecord, CancellationToken ct);

    /// <summary>
    /// Obtiene un registro de transacción por su identificador único.
    /// </summary>
    /// <param name="id">El identificador de la transacción.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>El registro de transacción si se encuentra, null en caso contrario.</returns>
    Task<TransactionRecord?> GetByIdAsync(Guid id, CancellationToken ct);
}

