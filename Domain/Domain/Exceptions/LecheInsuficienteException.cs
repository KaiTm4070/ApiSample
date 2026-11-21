namespace Domain.Exceptions
{
    /// <summary>RN-02: se intentó transformar más leche de la producida ese día (se mapea a 409).</summary>
    public class LecheInsuficienteException : DominioException
    {
        public LecheInsuficienteException(DateOnly fecha, decimal litrosDisponibles, decimal litrosSolicitados)
            : base($"El {fecha:yyyy-MM-dd} solo hay {litrosDisponibles} L sin transformar y se solicitaron {litrosSolicitados} L.")
        {
            Fecha = fecha;
            LitrosDisponibles = litrosDisponibles;
            LitrosSolicitados = litrosSolicitados;
        }

        public DateOnly Fecha { get; }
        public decimal LitrosDisponibles { get; }
        public decimal LitrosSolicitados { get; }
    }
}
