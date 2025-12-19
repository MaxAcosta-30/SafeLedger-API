using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SafeLedger.Domain.Interfaces;
using SafeLedger.Infrastructure.Persistence;
using SafeLedger.Infrastructure.Repositories;

namespace SafeLedger.Infrastructure;

/// <summary>
/// Métodos de extensión para configurar las dependencias de la capa de Infraestructura.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra todos los servicios de la capa de Infraestructura incluyendo Entity Framework Core, repositorios y Unit of Work.
    /// </summary>
    /// <param name="services">La colección de servicios a configurar.</param>
    /// <param name="configuration">Configuración de la aplicación.</param>
    /// <returns>La colección de servicios configurada.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SafeLedgerDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(SafeLedgerDbContext).Assembly.FullName)));

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}