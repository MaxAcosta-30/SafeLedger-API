namespace SafeLedger.Domain.Entities;

/// <summary>
/// Representa un registro de auditoría inmutable de una transacción financiera entre dos cuentas.
/// </summary>
public class TransactionRecord
{
    private TransactionRecord() { }

    /// <summary>
    /// Inicializa una nueva instancia de la entidad TransactionRecord.
    /// </summary>
    /// <param name="id">Identificador único de la transacción.</param>
    /// <param name="senderId">Identificador único de la cuenta remitente.</param>
    /// <param name="receiverId">Identificador único de la cuenta destinataria.</param>
    /// <param name="amount">Monto de la transacción (siempre positivo).</param>
    /// <param name="reference">Referencia opcional o descripción de la transacción.</param>
    public TransactionRecord(Guid id, Guid senderId, Guid receiverId, decimal amount, string reference)
    {
        Id = id;
        SenderId = senderId;
        ReceiverId = receiverId;
        Amount = amount;
        Reference = reference;
        DateUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid SenderId { get; private set; }
    public Guid ReceiverId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime DateUtc { get; private set; }
    public string? Reference { get; private set; }
}