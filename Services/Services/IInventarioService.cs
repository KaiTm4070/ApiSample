using Domain.Enums;
using Services.Dtos;

namespace Services
{
    public interface IInventarioService
    {
        Task<InventarioResponse> ObtenerInventarioAsync(DateOnly? fecha = null, TipoProducto? tipo = null, CancellationToken ct = default);
        Task<LoteResponse> ObtenerLoteAsync(int id, CancellationToken ct = default);
        Task<VencimientosResponse> ProcesarVencimientosAsync(DateOnly? fecha = null, CancellationToken ct = default);
    }
}
