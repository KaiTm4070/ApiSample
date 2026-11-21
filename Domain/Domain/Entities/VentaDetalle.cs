namespace Domain.Entities
{
    /// <summary>Trazabilidad (RN-09): de qué lote salió cuánto producto en una venta.</summary>
    public class VentaDetalle
    {
        public int Id { get; private set; }
        public int VentaId { get; private set; }
        public int LoteId { get; private set; }
        public LoteProducto Lote { get; private set; } = null!;
        public DateOnly FechaProduccion { get; private set; }
        public decimal Cantidad { get; private set; }

        private VentaDetalle() { }

        internal VentaDetalle(LoteProducto lote, decimal cantidad)
        {
            Lote = lote;
            LoteId = lote.Id;
            FechaProduccion = lote.FechaProduccion;
            Cantidad = cantidad;
        }
    }
}
