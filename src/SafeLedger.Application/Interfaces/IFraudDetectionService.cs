using SafeLedger.Application.DTOs;

namespace SafeLedger.Application.Interfaces;

/// <summary>
/// Servicio responsable de detectar operaciones de transferencia potencialmente fraudulentas.
/// </summary>
public interface IFraudDetectionService
{
    /// <summary>
    /// Analiza una solicitud de transferencia para determinar si presenta patrones sospechosos.
    /// </summary>
    /// <param name="request">La solicitud de transferencia a analizar.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>True si la transferencia se considera sospechosa, false en caso contrario.</returns>
    Task<bool> IsSuspiciousAsync(TransferRequestDto request, CancellationToken ct);
}