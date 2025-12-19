using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SafeLedger.Application.DTOs;
using SafeLedger.Application.Interfaces;

namespace SafeLedger.API.Controllers;

/// <summary>
/// Controlador responsable de manejar las operaciones de transferencia financiera.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TransfersController(ITransferService transferService, IValidator<TransferRequestDto> validator) : ControllerBase
{
    /// <summary>
    /// Ejecuta una transferencia financiera entre dos cuentas.
    /// </summary>
    /// <param name="request">Solicitud de transferencia que contiene remitente, destinatario, monto y referencia.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Respuesta de transferencia con ID de transacción y marca de tiempo de procesamiento.</returns>
    /// <response code="200">Transferencia completada exitosamente.</response>
    /// <response code="400">Datos de solicitud inválidos o fondos insuficientes.</response>
    /// <response code="403">Transferencia rechazada debido a detección de fraude.</response>
    /// <response code="404">Cuenta no encontrada.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpPost]
    public async Task<IActionResult> Transfer([FromBody] TransferRequestDto request, CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }

        try
        {
            var result = await transferService.ExecuteTransferAsync(request, ct);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", detail = ex.Message });
        }
    }
}