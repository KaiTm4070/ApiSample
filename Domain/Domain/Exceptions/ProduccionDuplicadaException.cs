namespace Domain.Exceptions
{
    /// <summary>RN-01: la producción se registra una sola vez por fecha y por vaca (se mapea a 409).</summary>
    public class ProduccionDuplicadaException : DominioException
    {
        public ProduccionDuplicadaException(int vacaId, DateOnly fecha)
            : base($"La vaca {vacaId} ya tiene producción registrada el {fecha:yyyy-MM-dd}.")
        {
            VacaId = vacaId;
            Fecha = fecha;
        }

        public int VacaId { get; }
        public DateOnly Fecha { get; }
    }
}
