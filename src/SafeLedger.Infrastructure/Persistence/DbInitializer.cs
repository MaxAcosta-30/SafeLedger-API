using SafeLedger.Domain.Entities;

namespace SafeLedger.Infrastructure.Persistence;

/// <summary>
/// Clase estática responsable de inicializar la base de datos con datos de prueba.
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Inicializa la base de datos, asegurando que exista y sembrando cuentas de prueba iniciales si está vacía.
    /// </summary>
    /// <param name="context">El contexto de base de datos a inicializar.</param>
    public static void Initialize(SafeLedgerDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Accounts.Any())
        {
            return;
        }

        var senderId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var receiverId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        var accounts = new Account[]
        {
            new Account(senderId, Guid.NewGuid(), "USD", 10000),
            new Account(receiverId, Guid.NewGuid(), "USD", 0)
        };

        context.Accounts.AddRange(accounts);
        context.SaveChanges();
    }
}