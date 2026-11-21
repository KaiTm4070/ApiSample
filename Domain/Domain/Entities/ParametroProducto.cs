using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities
{
    /// <summary>Rendimiento, vida útil y precio de un producto. Todo configurable, nada fijo en el código.</summary>
    public class ParametroProducto
    {
        public int Id { get; private set; }
        public int GranjaId { get; private set; }
        public TipoProducto Tipo { get; private set; }

        /// <summary>Litros de leche necesarios por unidad de producto (queso: 10 L = 1 kg; yogurt: 1 L = 1 L).</summary>
        public decimal LitrosPorUnidad { get; private set; }

        /// <summary>Días entre la producción y el vencimiento.</summary>
        public int VidaUtilDias { get; private set; }

        /// <summary>Precio de venta por unidad.</summary>
        public decimal Precio { get; private set; }

        private ParametroProducto() { }

        public ParametroProducto(TipoProducto tipo, decimal litrosPorUnidad, int vidaUtilDias, decimal precio)
        {
            Tipo = tipo;
            Actualizar(litrosPorUnidad, vidaUtilDias, precio);
        }

        public void Actualizar(decimal litrosPorUnidad, int vidaUtilDias, decimal precio)
        {
            if (litrosPorUnidad <= 0)
                throw new ValidacionDominioException("El rendimiento (litros por unidad) debe ser mayor que cero.");
            if (vidaUtilDias <= 0)
                throw new ValidacionDominioException("La vida útil debe ser de al menos un día.");
            if (precio < 0)
                throw new ValidacionDominioException("El precio no puede ser negativo.");

            LitrosPorUnidad = litrosPorUnidad;
            VidaUtilDias = vidaUtilDias;
            Precio = precio;
        }

        /// <summary>RN-03: cantidad de producto = litros usados ÷ rendimiento.</summary>
        public decimal CalcularCantidad(decimal litros) => litros / LitrosPorUnidad;

        /// <summary>RN-04: vencimiento = producción + vida útil.</summary>
        public DateOnly CalcularVencimiento(DateOnly fechaProduccion) => fechaProduccion.AddDays(VidaUtilDias);
    }
}
