namespace Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<T>> ListarAsync(CancellationToken ct = default);
        Task AgregarAsync(T entidad, CancellationToken ct = default);
    }
}
