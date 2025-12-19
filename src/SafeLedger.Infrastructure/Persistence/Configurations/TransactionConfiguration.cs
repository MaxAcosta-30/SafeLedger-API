using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeLedger.Domain.Entities;

namespace SafeLedger.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad TransactionRecord.
/// Define la estructura de la tabla y precisión para montos de transacciones financieras.
/// </summary>
public class TransactionConfiguration : IEntityTypeConfiguration<TransactionRecord>
{
    /// <summary>
    /// Configura el mapeo y las propiedades de la entidad TransactionRecord.
    /// </summary>
    /// <param name="builder">El constructor de tipo de entidad.</param>
    public void Configure(EntityTypeBuilder<TransactionRecord> builder)
    {
        builder.ToTable("Transactions");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(t => t.Reference)
            .HasMaxLength(100);
    }
}