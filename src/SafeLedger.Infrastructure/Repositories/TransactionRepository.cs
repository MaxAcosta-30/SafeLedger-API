using Microsoft.EntityFrameworkCore;
using SafeLedger.Domain.Entities;
using SafeLedger.Domain.Interfaces;
using SafeLedger.Infrastructure.Persistence;

namespace SafeLedger.Infrastructure.Repositories;

/// <summary>
/// Implementación de Entity Framework del repositorio de transacciones.
/// Proporciona operaciones de persistencia de registros de transacciones.
/// </summary>
public class TransactionRepository(SafeLedgerDbContext context) : ITransactionRepository
{
    /// <summary>
    /// Agrega un nuevo registro de transacción al repositorio.
    /// </summary>
    /// <param name="transactionRecord">El registro de transacción a agregar.</param>
    /// <param name="ct">Token de cancelación.</param>
    public async Task AddAsync(TransactionRecord transactionRecord, CancellationToken ct)
    {
        await context.Transactions.AddAsync(transactionRecord, ct);
    }

    /// <summary>
    /// Obtiene un registro de transacción por su identificador único.
    /// </summary>
    /// <param name="id">El identificador de la transacción.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>El registro de transacción si se encuentra, null en caso contrario.</returns>
    public async Task<TransactionRecord?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await context.Transactions.FindAsync([id], cancellationToken: ct);
    }
}

