using Microsoft.EntityFrameworkCore.Storage;
using SafeLedger.Domain.Interfaces;
using SafeLedger.Infrastructure.Adapters;
using SafeLedger.Infrastructure.Persistence;

namespace SafeLedger.Infrastructure.Repositories;

/// <summary>
/// Implementación de Entity Framework del patrón Unit of Work.
/// Coordina las operaciones de repositorio y gestiona las transacciones de base de datos.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly SafeLedgerDbContext context;

    public IAccountRepository Accounts { get; }

    /// <summary>
    /// Inicializa una nueva instancia de la clase UnitOfWork.
    /// </summary>
    /// <param name="context">El contexto de base de datos.</param>
    public UnitOfWork(SafeLedgerDbContext context)
    {
        this.context = context;
        Accounts = new AccountRepository(context);
    }

    /// <summary>
    /// Inicia una nueva transacción de base de datos envuelta en un adaptador de transacción de dominio.
    /// </summary>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Un objeto de transacción que puede ser confirmado o revertido.</returns>
    public async Task<ITransaction> BeginTransactionAsync(CancellationToken ct)
    {
        var efTransaction = await context.Database.BeginTransactionAsync(ct);
        return new EfTransactionAdapter(efTransaction);
    }

    /// <summary>
    /// Guarda todos los cambios realizados en el contexto de unidad de trabajo actual.
    /// </summary>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>El número de entidades guardadas.</returns>
    public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return await context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Libera el contexto de base de datos subyacente.
    /// </summary>
    public void Dispose()
    {
        context.Dispose();
    }
}