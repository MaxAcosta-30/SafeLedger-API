using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeLedger.Domain.Entities;

namespace SafeLedger.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad Account.
/// Define la estructura de la tabla, precisión para montos financieros y control de concurrencia optimista.
/// </summary>
public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    /// <summary>
    /// Configura el mapeo y las propiedades de la entidad Account.
    /// </summary>
    /// <param name="builder">El constructor de tipo de entidad.</param>
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Balance)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(a => a.Currency)
            .HasMaxLength(3)
            .IsRequired();
            
        builder.Property(a => a.RowVersion)
            .IsRowVersion();
    }
}