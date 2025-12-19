namespace SafeLedger.Domain.Interfaces;

/// <summary>
/// Representa una abstracción de transacción de base de datos que soporta disposición síncrona y asíncrona.
/// </summary>
public interface ITransaction : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Confirma la transacción, haciendo permanentes todos los cambios.
    /// </summary>
    /// <param name="ct">Token de cancelación.</param>
    Task CommitAsync(CancellationToken ct = default);
    
    /// <summary>
    /// Revierte la transacción, descartando todos los cambios.
    /// </summary>
    /// <param name="ct">Token de cancelación.</param>
    Task RollbackAsync(CancellationToken ct = default);
}