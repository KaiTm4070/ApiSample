using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities
{
    /// <summary>
    /// Lote de producto terminado (RN-04). Las reglas de vencimiento y descuento
    /// viven aquí y no en el servicio.
    /// </summary>
    public class LoteProducto
    {
        public int Id { get; private set; }
        public TipoProducto Tipo { get; private set; }
        public DateOnly FechaProduccion { get; private set; }
        public DateOnly FechaVencimiento { get; private set; }

        /// <summary>Litros de leche de la fecha de producción que se transformaron en este lote.</summary>
        public decimal LitrosUsados { get; private set; }

        public decimal CantidadInicial { get; private set; }
        public decimal CantidadDisponible { get; private set; }
        public decimal CantidadDanada { get; private set; }

        public EstadoLote Estado =>
            CantidadDisponible > 0 ? EstadoLote.Disponible
            : CantidadDanada > 0 ? EstadoLote.Vencido
            : EstadoLote.Agotado;

        private LoteProducto() { }

        /// <summary>RN-03 y RN-04: calcula cantidad y vencimiento a partir de los parámetros del producto.</summary>
        public static LoteProducto Crear(ParametroProducto parametro, DateOnly fechaProduccion, decimal litrosUsados)
        {
            if (litrosUsados <= 0)
                throw new ValidacionDominioException("Los litros a transformar deben ser mayores que cero.");

            var cantidad = parametro.CalcularCantidad(litrosUsados);
            return new LoteProducto
            {
                Tipo = parametro.Tipo,
                FechaProduccion = fechaProduccion,
                FechaVencimiento = parametro.CalcularVencimiento(fechaProduccion),
                LitrosUsados = litrosUsados,
                CantidadInicial = cantidad,
                CantidadDisponible = cantidad
            };
        }

        /// <summary>RN-08: vencido cuando la fecha consultada es posterior al vencimiento.</summary>
        public bool EstaVencido(DateOnly fecha) => fecha > FechaVencimiento;

        /// <summary>Se puede vender si tiene existencias y no está vencido a esa fecha.</summary>
        public bool EsVendible(DateOnly fecha) => CantidadDisponible > 0 && !EstaVencido(fecha);

        public void Descontar(decimal cantidad)
        {
            if (cantidad <= 0 || cantidad > CantidadDisponible)
                throw new InvalidOperationException("Cantidad inválida para el lote.");
            CantidadDisponible -= cantidad;
        }

        public decimal MarcarComoDanado()
        {
            var perdida = CantidadDisponible;
            CantidadDanada += perdida;
            CantidadDisponible = 0;
            return perdida;
        }

        /// <summary>RN-08: si el lote está vencido a esa fecha, lo que quedaba pasa a dañado. Devuelve lo perdido.</summary>
        public decimal ProcesarVencimiento(DateOnly fecha)
            => EstaVencido(fecha) && CantidadDisponible > 0 ? MarcarComoDanado() : 0;
    }
}
