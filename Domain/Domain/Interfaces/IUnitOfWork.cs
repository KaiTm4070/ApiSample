namespace Domain.Interfaces
{
    /// <summary>Confirma en una sola transacción todos los cambios pendientes.</summary>
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
