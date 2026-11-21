using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Reglas;
using Services.Dtos;
using Services.Mappers;

namespace Services
{
    public class ReporteService : IReporteService
    {
        private readonly IVentaRepository _ventas;
        private readonly ILoteRepository _lotes;
        private readonly IProduccionRepository _producciones;

        public ReporteService(IVentaRepository ventas, ILoteRepository lotes, IProduccionRepository producciones)
        {
            _ventas = ventas;
            _lotes = lotes;
            _producciones = producciones;
        }

        public async Task<ReporteVentasResponse> VentasAsync(
            DateOnly? desde, DateOnly? hasta, TipoProducto? tipo, CancellationToken ct = default)
        {
            ValidarRango(desde, hasta);
            if (tipo.HasValue && !Enum.IsDefined(tipo.Value))
                throw new ValidacionDominioException("Tipo de producto no válido.");

            var ventas = await _ventas.ObtenerPorRangoAsync(desde, hasta, tipo, ct);
            var items = ventas
                .Select(v => new VentaReporteItem(
                    v.Id, v.Fecha, v.Tipo, UnidadProducto.De(v.Tipo), v.Cantidad, v.PrecioUnitario, v.Total))
                .ToList();

            return new ReporteVentasResponse(desde, hasta, tipo, items, items.Sum(i => i.Total));
        }

        public async Task<ReporteLecheDiariaResponse> LecheDiariaAsync(
            DateOnly? desde, DateOnly? hasta, CancellationToken ct = default)
        {
            ValidarRango(desde, hasta);
            var producciones = await _producciones.ObtenerPorRangoAsync(desde, hasta, ct);
            var lotes = await _lotes.ObtenerPorProduccionAsync(desde, hasta, null, ct);

            var dias = producciones
                .GroupBy(p => p.Fecha)
                .OrderBy(g => g.Key)
                .Select(g =>
                {
                    var producidos = g.Sum(p => p.Litros);
                    var transformados = lotes.Where(l => l.FechaProduccion == g.Key).Sum(l => l.LitrosUsados);
                    return new LecheDiariaItem(
                        g.Key, producidos, transformados, ReglasLeche.LitrosNoAprovechados(producidos, transformados));
                })
                .ToList();

            return new ReporteLecheDiariaResponse(
                desde, hasta, dias,
                dias.Sum(d => d.LitrosProducidos),
                dias.Sum(d => d.LitrosTransformados),
                dias.Sum(d => d.LitrosNoAprovechados));
        }

        public async Task<ReporteProductoDiarioResponse> ProductoDiarioAsync(
            DateOnly? desde, DateOnly? hasta, CancellationToken ct = default)
        {
            ValidarRango(desde, hasta);
            var lotes = await _lotes.ObtenerPorProduccionAsync(desde, hasta, null, ct);

            var items = lotes
                .GroupBy(l => (l.FechaProduccion, l.Tipo))
                .OrderBy(g => g.Key.FechaProduccion)
                .ThenBy(g => g.Key.Tipo)
                .Select(g => new ProductoDiarioItem(
                    g.Key.FechaProduccion, g.Key.Tipo, UnidadProducto.De(g.Key.Tipo),
                    g.Sum(l => l.CantidadInicial), g.Sum(l => l.LitrosUsados)))
                .ToList();

            return new ReporteProductoDiarioResponse(desde, hasta, items);
        }

        /// <summary>Por producto: producido, vendido, disponible y dañado (siempre aparecen queso y yogurt).</summary>
        public async Task<ReporteProductoTotalResponse> ProductoTotalAsync(CancellationToken ct = default)
        {
            var lotes = await _lotes.ObtenerPorProduccionAsync(null, null, null, ct);
            var ventas = await _ventas.ObtenerPorRangoAsync(null, null, null, ct);

            var productos = Enum.GetValues<TipoProducto>()
                .Select(t =>
                {
                    var delTipo = lotes.Where(l => l.Tipo == t).ToList();
                    return new ProductoTotalItem(
                        t, UnidadProducto.De(t),
                        delTipo.Sum(l => l.CantidadInicial),
                        ventas.Where(v => v.Tipo == t).Sum(v => v.Cantidad),
                        delTipo.Sum(l => l.CantidadDisponible),
                        delTipo.Sum(l => l.CantidadDanada));
                })
                .ToList();

            return new ReporteProductoTotalResponse(productos);
        }

        /// <summary>El período se aplica a la fecha de vencimiento del lote, que es cuando se daña.</summary>
        public async Task<ReporteProductoDanadoResponse> ProductoDanadoAsync(
            DateOnly? desde, DateOnly? hasta, CancellationToken ct = default)
        {
            ValidarRango(desde, hasta);
            var lotes = (await _lotes.ObtenerPorProduccionAsync(null, null, null, ct))
                .Where(l => l.CantidadDanada > 0)
                .Where(l => !desde.HasValue || l.FechaVencimiento >= desde.Value)
                .Where(l => !hasta.HasValue || l.FechaVencimiento <= hasta.Value)
                .OrderBy(l => l.FechaVencimiento)
                .ThenBy(l => l.Id)
                .ToList();

            var items = lotes
                .Select(l => new ProductoDanadoItem(
                    l.Id, l.Tipo, UnidadProducto.De(l.Tipo), l.FechaProduccion, l.FechaVencimiento, l.CantidadDanada))
                .ToList();

            var totales = lotes
                .GroupBy(l => l.Tipo)
                .OrderBy(g => g.Key)
                .Select(g => new ProductoDanadoTotal(g.Key, UnidadProducto.De(g.Key), g.Sum(l => l.CantidadDanada)))
                .ToList();

            return new ReporteProductoDanadoResponse(desde, hasta, items, totales);
        }

        /// <summary>
        /// Por vaca: total de litros, días con registro y promedio diario (total ÷ días). El promedio general
        /// es el promedio de esos promedios. Solo cuentan las vacas con producción en el período.
        /// </summary>
        public async Task<ReporteVacasResponse> VacasAsync(
            DateOnly? desde, DateOnly? hasta, CancellationToken ct = default)
        {
            ValidarRango(desde, hasta);
            var producciones = await _producciones.ObtenerPorRangoAsync(desde, hasta, ct);

            var crudas = producciones
                .GroupBy(p => p.VacaId)
                .Select(g =>
                {
                    var total = g.Sum(p => p.Litros);
                    var dias = g.Select(p => p.Fecha).Distinct().Count();
                    return (VacaId: g.Key, Nombre: g.First().Vaca.Nombre, Total: total, Dias: dias, Promedio: total / dias);
                })
                .OrderBy(v => v.VacaId)
                .ToList();

            if (crudas.Count == 0)
                return new ReporteVacasResponse(desde, hasta, 0, null, null, [], []);

            var promedioGeneral = crudas.Average(v => v.Promedio);

            VacaProduccionItem Item((int VacaId, string Nombre, decimal Total, int Dias, decimal Promedio) v)
                => new(v.VacaId, v.Nombre, v.Total, v.Dias, Math.Round(v.Promedio, 2));

            var mayor = crudas.MaxBy(v => v.Promedio);
            var menor = crudas.MinBy(v => v.Promedio);

            return new ReporteVacasResponse(
                desde, hasta,
                Math.Round(promedioGeneral, 2),
                Item(mayor),
                Item(menor),
                crudas.Where(v => v.Promedio < promedioGeneral).Select(Item).ToList(),
                crudas.Select(Item).ToList());
        }

        private static void ValidarRango(DateOnly? desde, DateOnly? hasta)
        {
            if (desde.HasValue && hasta.HasValue && desde > hasta)
                throw new ValidacionDominioException("La fecha 'desde' no puede ser posterior a 'hasta'.");
        }
    }
}
