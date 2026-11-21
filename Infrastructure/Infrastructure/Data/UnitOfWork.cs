using Domain.Interfaces;

namespace Infrastructure.Data
{
    /// <summary>
    /// SaveChangesAsync ejecuta todos los cambios pendientes del DbContext en una sola transacción:
    /// la venta nueva y el descuento de sus lotes se confirman juntos o no se confirma nada.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context) => _context = context;

        public Task<int> SaveChangesAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);
    }
}
