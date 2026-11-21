using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Reglas;
using Services.Dtos;
using Services.Extensions;
using Services.Mappers;

namespace Services
{
    public class ProduccionService : IProduccionService
    {
        private readonly IGranjaRepository _granjas;
        private readonly IProduccionRepository _producciones;
        private readonly ILoteRepository _lotes;
        private readonly IUnitOfWork _uow;
        private readonly IClock _clock;

        public ProduccionService(
            IGranjaRepository granjas, IProduccionRepository producciones,
            ILoteRepository lotes, IUnitOfWork uow, IClock clock)
        {
            _granjas = granjas;
            _producciones = producciones;
            _lotes = lotes;
            _uow = uow;
            _clock = clock;
        }

        /// <summary>RN-01: registra la leche del día, una sola vez por fecha y por vaca. Todo o nada.</summary>
        public async Task<ProduccionResponse> RegistrarProduccionAsync(ProduccionRequest request, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            var fecha = request.Fecha ?? _clock.Today;
            var granja = await _granjas.ObtenerGranjaAsync(ct);

            var entradas = ResolverEntradas(granja, request);

            // Se valida todo antes de agregar nada: si una vaca ya produjo ese día, no se registra ninguna.
            foreach (var (vaca, _) in entradas)
            {
                if (await _producciones.ExisteAsync(vaca.Id, fecha, ct))
                    throw new ProduccionDuplicadaException(vaca.Id, fecha);
            }

            var registros = new List<ProduccionLeche>();
            foreach (var (vaca, litros) in entradas)
            {
                var produccion = new ProduccionLeche(vaca.Id, fecha, litros);
                await _producciones.AgregarAsync(produccion, ct);
                registros.Add(produccion);
            }

            await _uow.SaveChangesAsync(ct);

            return new ProduccionResponse(
                fecha,
                entradas.Sum(e => e.Litros),
                entradas.Select(e => new ProduccionVacaResponse(e.Vaca.Id, e.Vaca.Nombre, e.Litros)).ToList());
        }

        /// <summary>RN-02, RN-03 y RN-04: convierte litros de la leche de una fecha en un lote de queso o yogurt.</summary>
        public async Task<TransformacionResponse> TransformarAsync(TransformacionRequest request, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            if (!Enum.IsDefined(request.TipoProducto))
                throw new ValidacionDominioException("Tipo de producto no válido.");

            var fecha = request.Fecha ?? _clock.Today;
            var granja = await _granjas.ObtenerGranjaAsync(ct);
            var parametro = granja.ObtenerParametro(request.TipoProducto);

            var producidos = await _producciones.LitrosProducidosAsync(fecha, ct);
            var transformados = await _lotes.LitrosTransformadosAsync(fecha, ct);
            ReglasLeche.ValidarTransformacion(fecha, producidos, transformados, request.Litros);

            var lote = LoteProducto.Crear(parametro, fecha, request.Litros);
            await _lotes.AgregarAsync(lote, ct);
            await _uow.SaveChangesAsync(ct);

            return InventarioMapper.ToTransformacion(lote, producidos - transformados - request.Litros);
        }

        private static List<(Vaca Vaca, decimal Litros)> ResolverEntradas(Granja granja, ProduccionRequest request)
        {
            // Sin detalle: cada vaca activa produce l litros, o sea n × l en total.
            if (request.Vacas is null || request.Vacas.Count == 0)
            {
                return granja.Vacas
                    .Where(v => v.Activa)
                    .OrderBy(v => v.Id)
                    .Select(v => (v, granja.LitrosPorVaca))
                    .ToList();
            }

            if (request.Vacas.GroupBy(x => x.VacaId).Any(g => g.Count() > 1))
                throw new ValidacionDominioException("Una vaca no puede aparecer dos veces en la misma producción.");

            var porId = granja.Vacas.ToDictionary(v => v.Id);
            var entradas = new List<(Vaca, decimal)>();
            foreach (var item in request.Vacas)
            {
                if (!porId.TryGetValue(item.VacaId, out var vaca))
                    throw new EntidadNoEncontradaException("la vaca", item.VacaId);
                if (!vaca.Activa)
                    throw new ValidacionDominioException($"La vaca {vaca.Id} está inactiva y no puede producir.");
                if (item.Litros < 0)
                    throw new ValidacionDominioException("Los litros producidos no pueden ser negativos.");
                entradas.Add((vaca, item.Litros));
            }
            return entradas;
        }
    }
}
