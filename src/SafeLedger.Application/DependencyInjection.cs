using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SafeLedger.Application.Interfaces;
using SafeLedger.Application.Services;

namespace SafeLedger.Application;

/// <summary>
/// Métodos de extensión para configurar las dependencias de la capa de Aplicación.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra todos los servicios, validadores y dependencias de la capa de Aplicación.
    /// </summary>
    /// <param name="services">La colección de servicios a configurar.</param>
    /// <returns>La colección de servicios configurada.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITransferService, TransferService>();
        services.AddScoped<IFraudDetectionService, FraudDetectionService>();
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}