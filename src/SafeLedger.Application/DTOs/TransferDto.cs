namespace SafeLedger.Application.DTOs;

/// <summary>
/// DTO de solicitud para iniciar una transferencia financiera entre cuentas.
/// </summary>
/// <param name="SenderId">Identificador único de la cuenta remitente.</param>
/// <param name="ReceiverId">Identificador único de la cuenta destinataria.</param>
/// <param name="Amount">Monto de la transferencia (debe ser positivo y tener máximo 2 decimales).</param>
/// <param name="Reference">Referencia opcional o descripción de la transferencia.</param>
public record TransferRequestDto(
    Guid SenderId,
    Guid ReceiverId,
    decimal Amount,
    string Reference
);

/// <summary>
/// DTO de respuesta que contiene el resultado de una operación de transferencia exitosa.
/// </summary>
/// <param name="TransactionId">Identificador único de la transacción creada.</param>
/// <param name="ProcessedAtUtc">Marca de tiempo UTC cuando se procesó la transferencia.</param>
public record TransferResponseDto(
    Guid TransactionId,
    DateTime ProcessedAtUtc
);