using Domain.Enums;

namespace Domain.Exceptions
{
    /// <summary>RN-07: no hay existencias no vencidas suficientes para la venta (se mapea a 409).</summary>
    public class InventarioInsuficienteException : DominioException
    {
        public InventarioInsuficienteException(TipoProducto tipo, decimal cantidadSolicitada, decimal disponible = 0)
            : base($"No hay existencias suficientes de {tipo} sin vencer: se pidieron {cantidadSolicitada} y hay {disponible}.")
        {
            Tipo = tipo;
            CantidadSolicitada = cantidadSolicitada;
            Disponible = disponible;
        }

        public TipoProducto Tipo { get; }
        public decimal CantidadSolicitada { get; }
        public decimal Disponible { get; }
    }
}
