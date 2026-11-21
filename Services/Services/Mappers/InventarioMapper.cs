using Domain.Entities;
using Services.Dtos;

namespace Services.Mappers
{
    public static class InventarioMapper
    {
        public static LoteResponse ToResponse(LoteProducto lote) => new(
            lote.Id, lote.Tipo, lote.FechaProduccion, lote.FechaVencimiento,
            lote.CantidadInicial, lote.CantidadDisponible, lote.CantidadDanada,
            UnidadProducto.De(lote.Tipo), lote.Estado);

        public static TransformacionResponse ToTransformacion(LoteProducto lote, decimal litrosSinTransformar) => new(
            lote.Id, lote.Tipo, lote.FechaProduccion, lote.FechaVencimiento,
            lote.LitrosUsados, lote.CantidadInicial, UnidadProducto.De(lote.Tipo), litrosSinTransformar);
    }
}
