using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IGranjaRepository : IRepository<Granja>
    {
        /// <summary>La granja (es única) con sus vacas y parámetros de producto cargados.</summary>
        Task<Granja?> ObtenerAsync(CancellationToken ct = default);
    }
}
