using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Services.Dtos
{
    public class ProduccionVacaRequest
    {
        [Range(1, int.MaxValue)]
        public int VacaId { get; set; }

        [Range(typeof(decimal), "0", "1000000")]
        public decimal Litros { get; set; }
    }

    public class ProduccionRequest
    {
        /// <summary>Fecha de la producción. Si se omite se usa la fecha actual (IClock).</summary>
        public DateOnly? Fecha { get; set; }

        /// <summary>
        /// Litros por vaca. Si se omite, cada vaca activa produce l litros (n × l en total).
        /// </summary>
        public List<ProduccionVacaRequest>? Vacas { get; set; }
    }

    public sealed record ProduccionVacaResponse(int VacaId, string Nombre, decimal Litros);

    public sealed record ProduccionResponse(
        DateOnly Fecha, decimal TotalLitros, IReadOnlyList<ProduccionVacaResponse> Vacas);

    public class TransformacionRequest
    {
        /// <summary>Fecha de la leche que se transforma. Si se omite se usa la fecha actual.</summary>
        public DateOnly? Fecha { get; set; }

        [EnumDataType(typeof(TipoProducto))]
        public TipoProducto TipoProducto { get; set; }

        [Range(typeof(decimal), "0.001", "1000000")]
        public decimal Litros { get; set; }
    }

    public sealed record TransformacionResponse(
        int LoteId,
        TipoProducto TipoProducto,
        DateOnly FechaProduccion,
        DateOnly FechaVencimiento,
        decimal LitrosUsados,
        decimal Cantidad,
        string Unidad,
        decimal LitrosSinTransformarDelDia);
}
