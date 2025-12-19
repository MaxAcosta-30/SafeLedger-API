using Microsoft.EntityFrameworkCore.Storage;
using SafeLedger.Domain.Interfaces;

namespace SafeLedger.Infrastructure.Adapters;

/// <summary>
/// Adaptador que envuelve IDbContextTransaction de Entity Framework Core para implementar la interfaz ITransaction del dominio.
/// Esto permite la inversión de dependencias manteniendo la capa de dominio independiente de EF Core.
/// </summary>
public class EfTransactionAdapter(IDbContextTransaction efTransaction) : ITransaction
{
    /// <summary>
    /// Confirma la transacción subyacente de Entity Framework.
    /// </summary>
    /// <param name="ct">Token de cancelación.</param>
    public async Task CommitAsync(CancellationToken ct = default)
    {
        await efTransaction.CommitAsync(ct);
    }

    /// <summary>
    /// Revierte la transacción subyacente de Entity Framework.
    /// </summary>
    /// <param name="ct">Token de cancelación.</param>
    public async Task RollbackAsync(CancellationToken ct = default)
    {
        await efTransaction.RollbackAsync(ct);
    }

    /// <summary>
    /// Libera la transacción subyacente de Entity Framework.
    /// </summary>
    public void Dispose()
    {
        efTransaction.Dispose();
    }

    /// <summary>
    /// Libera de forma asíncrona la transacción subyacente de Entity Framework.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await efTransaction.DisposeAsync();
    }
}