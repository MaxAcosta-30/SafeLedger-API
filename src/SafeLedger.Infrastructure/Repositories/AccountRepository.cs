using Microsoft.EntityFrameworkCore;
using SafeLedger.Domain.Entities;
using SafeLedger.Domain.Interfaces;
using SafeLedger.Infrastructure.Persistence;

namespace SafeLedger.Infrastructure.Repositories;

/// <summary>
/// Implementación de Entity Framework del repositorio de cuentas.
/// Proporciona operaciones de persistencia de cuentas incluyendo bloqueo pesimista para integridad financiera.
/// </summary>
public class AccountRepository(SafeLedgerDbContext context) : IAccountRepository
{
    /// <summary>
    /// Obtiene una cuenta por su identificador único.
    /// </summary>
    /// <param name="id">El identificador de la cuenta.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>La cuenta si se encuentra, null en caso contrario.</returns>
    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await context.Accounts.FindAsync([id], cancellationToken: ct);
    }

    /// <summary>
    /// Obtiene una cuenta por su identificador único con bloqueo pesimista usando hints SQL UPDLOCK y ROWLOCK.
    /// UPDLOCK: Indica intención de actualizar, previniendo que otras transacciones modifiquen la fila.
    /// ROWLOCK: Bloquea solo la fila específica, no toda la tabla, para mejor rendimiento.
    /// </summary>
    /// <param name="id">El identificador de la cuenta.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>La cuenta si se encuentra, null en caso contrario.</returns>
    public async Task<Account?> GetByIdWithLockAsync(Guid id, CancellationToken ct)
    {
        return await context.Accounts
            .FromSqlInterpolated($"SELECT * FROM Accounts WITH (UPDLOCK, ROWLOCK) WHERE Id = {id}")
            .FirstOrDefaultAsync(ct);
    }

    /// <summary>
    /// Agrega una nueva cuenta al repositorio.
    /// </summary>
    /// <param name="account">La cuenta a agregar.</param>
    /// <param name="ct">Token de cancelación.</param>
    public async Task AddAsync(Account account, CancellationToken ct)
    {
        await context.Accounts.AddAsync(account, ct);
    }

    /// <summary>
    /// Actualiza una cuenta existente en el repositorio.
    /// </summary>
    /// <param name="account">La cuenta a actualizar.</param>
    /// <param name="ct">Token de cancelación.</param>
    public Task UpdateAsync(Account account, CancellationToken ct)
    {
        context.Accounts.Update(account);
        return Task.CompletedTask;
    }
}