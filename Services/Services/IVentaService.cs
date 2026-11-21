using Services.Dtos;

namespace Services
{
    public interface IVentaService
    {
        Task<VentaResponse> RegistrarVentaAsync(VentaRequest request, CancellationToken ct = default);
        Task<VentaResponse> ObtenerVentaAsync(int id, CancellationToken ct = default);
    }
}
