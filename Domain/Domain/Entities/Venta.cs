using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities
{
    /// <summary>Venta de un producto. Guarda precio unitario, cantidad, total y el detalle por lote (RN-09).</summary>
    public class Venta
    {
        private readonly List<VentaDetalle> _detalles = new();

        public int Id { get; private set; }
        public DateOnly Fecha { get; private set; }
        public TipoProducto Tipo { get; private set; }
        public decimal PrecioUnitario { get; private set; }
        public decimal Cantidad { get; private set; }
        public decimal Total { get; private set; }

        public IReadOnlyCollection<VentaDetalle> Detalles => _detalles;

        private Venta() { }

        public Venta(DateOnly fecha, TipoProducto tipo, decimal precioUnitario)
        {
            if (precioUnitario < 0)
                throw new ValidacionDominioException("El precio unitario no puede ser negativo.");
            Fecha = fecha;
            Tipo = tipo;
            PrecioUnitario = precioUnitario;
        }

        /// <summary>
        /// Registra de qué lote salió producto y actualiza cantidad y total.
        /// No descuenta del lote: eso lo hace el servicio con <see cref="LoteProducto.Descontar"/>.
        /// </summary>
        public void AgregarDetalle(LoteProducto lote, decimal cantidad)
        {
            if (cantidad <= 0)
                throw new ValidacionDominioException("La cantidad del detalle debe ser mayor que cero.");
            if (lote.Tipo != Tipo)
                throw new ValidacionDominioException($"El lote es de {lote.Tipo} y la venta es de {Tipo}.");

            _detalles.Add(new VentaDetalle(lote, cantidad));
            Cantidad += cantidad;
            Total = Cantidad * PrecioUnitario;
        }
    }
}
