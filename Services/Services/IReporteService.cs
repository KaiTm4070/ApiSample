using Domain.Enums;
using Services.Dtos;

namespace Services
{
    public interface IReporteService
    {
        Task<ReporteVentasResponse> VentasAsync(DateOnly? desde, DateOnly? hasta, TipoProducto? tipo, CancellationToken ct = default);
        Task<ReporteLecheDiariaResponse> LecheDiariaAsync(DateOnly? desde, DateOnly? hasta, CancellationToken ct = default);
        Task<ReporteProductoDiarioResponse> ProductoDiarioAsync(DateOnly? desde, DateOnly? hasta, CancellationToken ct = default);
        Task<ReporteProductoTotalResponse> ProductoTotalAsync(CancellationToken ct = default);
        Task<ReporteProductoDanadoResponse> ProductoDanadoAsync(DateOnly? desde, DateOnly? hasta, CancellationToken ct = default);
        Task<ReporteVacasResponse> VacasAsync(DateOnly? desde, DateOnly? hasta, CancellationToken ct = default);
    }
}
