using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Services.Dtos
{
    public class VentaRequest
    {
        [EnumDataType(typeof(TipoProducto))]
        public TipoProducto TipoProducto { get; set; }

        [Range(typeof(decimal), "0.001", "1000000")]
        public decimal Cantidad { get; set; }

        /// <summary>Fecha de la venta. Si se omite se usa la fecha actual (IClock).</summary>
        public DateOnly? Fecha { get; set; }
    }

    public sealed record VentaDetalleResponse(int LoteId, DateOnly FechaProduccion, decimal Cantidad);

    public sealed record VentaResponse(
        int Id,
        DateOnly Fecha,
        TipoProducto TipoProducto,
        decimal Cantidad,
        decimal PrecioUnitario,
        decimal Total,
        IReadOnlyList<VentaDetalleResponse> Detalle);
}
