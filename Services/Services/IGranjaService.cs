using Services.Dtos;

namespace Services
{
    public interface IGranjaService
    {
        Task<GranjaResponse> ObtenerAsync(CancellationToken ct = default);
        Task<GranjaResponse> ActualizarAsync(GranjaUpdateRequest request, CancellationToken ct = default);
        Task<IReadOnlyList<VacaResponse>> ListarVacasAsync(CancellationToken ct = default);
    }
}
