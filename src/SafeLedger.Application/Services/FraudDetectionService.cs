using SafeLedger.Application.DTOs;
using SafeLedger.Application.Interfaces;

namespace SafeLedger.Application.Services;

/// <summary>
/// Implementación del servicio para detectar operaciones de transferencia potencialmente fraudulentas.
/// </summary>
public class FraudDetectionService : IFraudDetectionService
{
    private const decimal SuspiciousAmountThreshold = 50000m;

    /// <summary>
    /// Analiza una solicitud de transferencia para determinar si presenta patrones sospechosos.
    /// </summary>
    /// <param name="request">La solicitud de transferencia a analizar.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>True si la transferencia se considera sospechosa, false en caso contrario.</returns>
    public Task<bool> IsSuspiciousAsync(TransferRequestDto request, CancellationToken ct)
    {
        if (request.Amount > SuspiciousAmountThreshold)
        {
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
}