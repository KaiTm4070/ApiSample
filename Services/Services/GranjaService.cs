using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Services.Dtos;
using Services.Extensions;
using Services.Mappers;

namespace Services
{
    public class GranjaService : IGranjaService
    {
        private readonly IGranjaRepository _granjas;
        private readonly IUnitOfWork _uow;

        public GranjaService(IGranjaRepository granjas, IUnitOfWork uow)
        {
            _granjas = granjas;
            _uow = uow;
        }

        public async Task<GranjaResponse> ObtenerAsync(CancellationToken ct = default)
            => GranjaMapper.ToResponse(await _granjas.ObtenerGranjaAsync(ct));

        public async Task<IReadOnlyList<VacaResponse>> ListarVacasAsync(CancellationToken ct = default)
        {
            var granja = await _granjas.ObtenerGranjaAsync(ct);
            return granja.Vacas.OrderBy(v => v.Id).Select(GranjaMapper.ToResponse).ToList();
        }

        public async Task<GranjaResponse> ActualizarAsync(GranjaUpdateRequest request, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var productos = request.Productos ?? new List<ParametroProductoRequest>();
            if (productos.Any(p => !Enum.IsDefined(p.Tipo)))
                throw new ValidacionDominioException("Tipo de producto no válido.");
            if (productos.GroupBy(p => p.Tipo).Any(g => g.Count() > 1))
                throw new ValidacionDominioException("Cada producto solo puede aparecer una vez.");

            var granja = await _granjas.ObtenerGranjaAsync(ct);

            // Las reglas (valores positivos, sincronizar vacas) viven en el dominio y lanzan 400 si algo no cumple.
            granja.Actualizar(request.NumeroVacas, request.LitrosPorVaca);
            foreach (var p in productos)
                granja.ConfigurarProducto(p.Tipo, p.LitrosPorUnidad, p.VidaUtilDias, p.Precio);

            await _uow.SaveChangesAsync(ct);
            return GranjaMapper.ToResponse(granja);
        }
    }
}
