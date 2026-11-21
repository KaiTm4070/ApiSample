using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Services.Dtos
{
    public class ParametroProductoRequest
    {
        [EnumDataType(typeof(TipoProducto))]
        public TipoProducto Tipo { get; set; }

        /// <summary>Litros de leche por unidad de producto (queso: 10, yogurt: 1).</summary>
        [Range(typeof(decimal), "0.001", "1000000")]
        public decimal LitrosPorUnidad { get; set; }

        /// <summary>Días entre la producción y el vencimiento.</summary>
        [Range(1, 3650)]
        public int VidaUtilDias { get; set; }

        [Range(typeof(decimal), "0", "1000000000")]
        public decimal Precio { get; set; }
    }

    public class GranjaUpdateRequest
    {
        /// <summary>n: número de vacas.</summary>
        [Range(1, 100000)]
        public int NumeroVacas { get; set; }

        /// <summary>l: litros por vaca por día.</summary>
        [Range(typeof(decimal), "0.001", "1000000")]
        public decimal LitrosPorVaca { get; set; }

        /// <summary>Parámetros por producto. Si se omite, no se modifican.</summary>
        public List<ParametroProductoRequest>? Productos { get; set; }
    }

    public sealed record ParametroProductoResponse(
        TipoProducto Tipo, string Unidad, decimal LitrosPorUnidad, int VidaUtilDias, decimal Precio);

    public sealed record GranjaResponse(
        string Nombre,
        int NumeroVacas,
        decimal LitrosPorVaca,
        decimal ProduccionDiariaEsperada,
        IReadOnlyList<ParametroProductoResponse> Productos);

    public sealed record VacaResponse(int Id, string Nombre, string Raza, bool Activa);
}
