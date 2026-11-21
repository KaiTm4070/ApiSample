using Domain.Entities;
using Services.Dtos;

namespace Services.Mappers
{
    public static class VentaMapper
    {
        public static VentaResponse ToResponse(Venta venta) => new(
            venta.Id, venta.Fecha, venta.Tipo, venta.Cantidad, venta.PrecioUnitario, venta.Total,
            venta.Detalles
                .Select(d => new VentaDetalleResponse(d.LoteId, d.FechaProduccion, d.Cantidad))
                .ToList());
    }
}
