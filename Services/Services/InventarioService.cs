using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Services.Dtos;
using Services.Mappers;

namespace Services
{
    public class InventarioService : IInventarioService
    {
        private readonly ILoteRepository _lotes;
        private readonly IUnitOfWork _uow;
        private readonly IClock _clock;

        public InventarioService(ILoteRepository lotes, IUnitOfWork uow, IClock clock)
        {
            _lotes = lotes;
            _uow = uow;
            _clock = clock;
        }

        /// <summary>Existencias vendibles a esa fecha: con cantidad disponible y sin vencer.</summary>
        public async Task<InventarioResponse> ObtenerInventarioAsync(
            DateOnly? fecha = null, TipoProducto? tipo = null, CancellationToken ct = default)
        {
            var dia = fecha ?? _clock.Today;
            if (tipo.HasValue && !Enum.IsDefined(tipo.Value))
                throw new ValidacionDominioException("Tipo de producto no válido.");

            var lotes = (await _lotes.ObtenerPorProduccionAsync(null, dia, tipo, ct))
                .Where(l => l.EsVendible(dia))
                .ToList();

            var tipos = tipo.HasValue ? new[] { tipo.Value } : Enum.GetValues<TipoProducto>();
            var existencias = tipos
                .Select(t => new ExistenciaResponse(
                    t, UnidadProducto.De(t), lotes.Where(l => l.Tipo == t).Sum(l => l.CantidadDisponible)))
                .ToList();

            return new InventarioResponse(dia, existencias, lotes.Select(InventarioMapper.ToResponse).ToList());
        }

        public async Task<LoteResponse> ObtenerLoteAsync(int id, CancellationToken ct = default)
        {
            var lote = await _lotes.ObtenerPorIdAsync(id, ct)
                       ?? throw new EntidadNoEncontradaException("el lote", id);
            return InventarioMapper.ToResponse(lote);
        }

        /// <summary>RN-08: los lotes vencidos pasan lo que no se vendió de disponible a dañado.</summary>
        public async Task<VencimientosResponse> ProcesarVencimientosAsync(DateOnly? fecha = null, CancellationToken ct = default)
        {
            var dia = fecha ?? _clock.Today;
            var vencidos = await _lotes.ObtenerVencidosAsync(dia, ct);

            var procesados = new List<VencimientoLoteResponse>();
            foreach (var lote in vencidos)
            {
                var perdida = lote.ProcesarVencimiento(dia);
                if (perdida > 0)
                {
                    procesados.Add(new VencimientoLoteResponse(
                        lote.Id, lote.Tipo, UnidadProducto.De(lote.Tipo),
                        lote.FechaProduccion, lote.FechaVencimiento, perdida));
                }
            }

            if (procesados.Count > 0)
                await _uow.SaveChangesAsync(ct);

            return new VencimientosResponse(dia, procesados.Count, procesados);
        }
    }
}
