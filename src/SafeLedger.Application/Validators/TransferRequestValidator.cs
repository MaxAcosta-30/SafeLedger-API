using FluentValidation;
using SafeLedger.Application.DTOs;

namespace SafeLedger.Application.Validators;

/// <summary>
/// Validador para TransferRequestDto que garantiza la integridad financiera y el cumplimiento de reglas de negocio.
/// </summary>
public class TransferRequestValidator : AbstractValidator<TransferRequestDto>
{
    /// <summary>
    /// Inicializa las reglas de validación para solicitudes de transferencia.
    /// </summary>
    public TransferRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a 0.")
            .PrecisionScale(18, 2, false).WithMessage("El monto no puede tener más de 2 decimales y 18 dígitos en total.");

        RuleFor(x => x.SenderId)
            .NotEmpty().WithMessage("El ID del remitente es obligatorio.");

        RuleFor(x => x.ReceiverId)
            .NotEmpty().WithMessage("El ID del destinatario es obligatorio.")
            .NotEqual(x => x.SenderId).WithMessage("No puedes enviarte dinero a ti mismo.");

        RuleFor(x => x.Reference)
            .MaximumLength(100).WithMessage("La referencia es demasiado larga.");
    }
}