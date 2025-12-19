using SafeLedger.Application.DTOs;

namespace SafeLedger.Application.Interfaces;

/// <summary>
/// Servicio responsable de ejecutar transferencias financieras entre cuentas.
/// </summary>
public interface ITransferService
{
    /// <summary>
    /// Ejecuta una transferencia financiera con detección de fraude, bloqueo pesimista y garantías de transacción ACID.
    /// </summary>
    /// <param name="request">La solicitud de transferencia que contiene remitente, destinatario, monto y referencia.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Respuesta de transferencia con ID de transacción y marca de tiempo de procesamiento.</returns>
    /// <exception cref="UnauthorizedAccessException">Se lanza cuando se detecta fraude.</exception>
    /// <exception cref="KeyNotFoundException">Se lanza cuando no se encuentra la cuenta remitente o destinataria.</exception>
    /// <exception cref="InvalidOperationException">Se lanza cuando el remitente tiene fondos insuficientes.</exception>
    Task<TransferResponseDto> ExecuteTransferAsync(TransferRequestDto request, CancellationToken ct);
}