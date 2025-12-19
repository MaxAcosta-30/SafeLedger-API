using Microsoft.Extensions.Logging;
using SafeLedger.Application.DTOs;
using SafeLedger.Application.Interfaces;
using SafeLedger.Domain.Entities;
using SafeLedger.Domain.Interfaces;

namespace SafeLedger.Application.Services;

/// <summary>
/// Implementación del servicio para ejecutar transferencias financieras con detección de fraude, bloqueo pesimista y garantías ACID.
/// </summary>
public class TransferService(
    IUnitOfWork unitOfWork,
    IFraudDetectionService fraudService,
    ILogger<TransferService> logger
    ) : ITransferService
{
    /// <summary>
    /// Ejecuta una transferencia financiera con detección de fraude, bloqueo pesimista y garantías de transacción ACID.
    /// Utiliza hints SQL UPDLOCK/ROWLOCK para prevenir problemas de modificación concurrente.
    /// </summary>
    /// <param name="request">La solicitud de transferencia que contiene remitente, destinatario, monto y referencia.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Respuesta de transferencia con ID de transacción y marca de tiempo de procesamiento.</returns>
    /// <exception cref="UnauthorizedAccessException">Se lanza cuando se detecta fraude.</exception>
    /// <exception cref="KeyNotFoundException">Se lanza cuando no se encuentra la cuenta remitente o destinataria.</exception>
    /// <exception cref="InvalidOperationException">Se lanza cuando el remitente tiene fondos insuficientes.</exception>
    public async Task<TransferResponseDto> ExecuteTransferAsync(TransferRequestDto request, CancellationToken ct)
    {
        logger.LogInformation("Iniciando transferencia de {Amount} desde {SenderId}", request.Amount, request.SenderId);

        if (await fraudService.IsSuspiciousAsync(request, ct))
        {
            logger.LogWarning("Transferencia rechazada por posible fraude. Sender: {SenderId}", request.SenderId);
            throw new UnauthorizedAccessException("Operación rechazada por políticas de seguridad.");
        }

        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var sender = await unitOfWork.Accounts.GetByIdWithLockAsync(request.SenderId, ct);
            var receiver = await unitOfWork.Accounts.GetByIdWithLockAsync(request.ReceiverId, ct);

            if (sender is null) throw new KeyNotFoundException($"Cuenta origen {request.SenderId} no encontrada.");
            if (receiver is null) throw new KeyNotFoundException($"Cuenta destino {request.ReceiverId} no encontrada.");

            sender.Debit(request.Amount);
            receiver.Credit(request.Amount);

            var transactionRecord = new TransactionRecord(
                Guid.NewGuid(),
                sender.Id,
                receiver.Id,
                request.Amount,
                request.Reference
            );

            await unitOfWork.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            logger.LogInformation("Transferencia exitosa. ID: {TransactionId}", transactionRecord.Id);

            return new TransferResponseDto(transactionRecord.Id, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error en transferencia. Rollback ejecutado.");
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}