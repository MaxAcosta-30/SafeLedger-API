using SafeLedger.Domain.Entities;

namespace SafeLedger.Domain.Interfaces;

/// <summary>
/// Interfaz del repositorio para operaciones de persistencia de cuentas.
/// </summary>
public interface IAccountRepository
{
    /// <summary>
    /// Obtiene una cuenta por su identificador único.
    /// </summary>
    /// <param name="id">El identificador de la cuenta.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>La cuenta si se encuentra, null en caso contrario.</returns>
    Task<Account?> GetByIdAsync(Guid id, CancellationToken ct);
    
    /// <summary>
    /// Obtiene una cuenta por su identificador único con bloqueo pesimista (UPDLOCK, ROWLOCK).
    /// Se utiliza para prevenir modificaciones concurrentes durante transacciones financieras.
    /// </summary>
    /// <param name="id">El identificador de la cuenta.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>La cuenta si se encuentra, null en caso contrario.</returns>
    Task<Account?> GetByIdWithLockAsync(Guid id, CancellationToken ct);
    
    /// <summary>
    /// Agrega una nueva cuenta al repositorio.
    /// </summary>
    /// <param name="account">La cuenta a agregar.</param>
    /// <param name="ct">Token de cancelación.</param>
    Task AddAsync(Account account, CancellationToken ct);
    
    /// <summary>
    /// Actualiza una cuenta existente en el repositorio.
    /// </summary>
    /// <param name="account">La cuenta a actualizar.</param>
    /// <param name="ct">Token de cancelación.</param>
    Task UpdateAsync(Account account, CancellationToken ct);
}