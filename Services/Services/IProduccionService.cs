using Services.Dtos;

namespace Services
{
    public interface IProduccionService
    {
        Task<ProduccionResponse> RegistrarProduccionAsync(ProduccionRequest request, CancellationToken ct = default);
        Task<TransformacionResponse> TransformarAsync(TransformacionRequest request, CancellationToken ct = default);
    }
}
