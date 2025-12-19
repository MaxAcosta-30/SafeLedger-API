using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SafeLedger.Domain.Entities;

namespace SafeLedger.Infrastructure.Persistence;

/// <summary>
/// Contexto de base de datos de Entity Framework Core para la aplicación SafeLedger.
/// Gestiona las conexiones de base de datos y las configuraciones de entidades.
/// </summary>
public class SafeLedgerDbContext(DbContextOptions<SafeLedgerDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<TransactionRecord> Transactions { get; set; }

    /// <summary>
    /// Configura el modelo aplicando todas las configuraciones de entidades del ensamblado actual.
    /// </summary>
    /// <param name="modelBuilder">La instancia del constructor de modelos.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}