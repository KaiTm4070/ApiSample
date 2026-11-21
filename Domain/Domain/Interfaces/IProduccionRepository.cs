using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IProduccionRepository : IRepository<ProduccionLeche>
    {
        Task<bool> ExisteAsync(int vacaId, DateOnly fecha, CancellationToken ct = default);
        Task<decimal> LitrosProducidosAsync(DateOnly fecha, CancellationToken ct = default);

        /// <summary>Producción del período con la vaca cargada (para los reportes de leche y de vacas).</summary>
        Task<IReadOnlyList<ProduccionLeche>> ObtenerPorRangoAsync(
            DateOnly? desde, DateOnly? hasta, CancellationToken ct = default);
    }
}
