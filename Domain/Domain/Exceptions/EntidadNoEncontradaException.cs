namespace Domain.Exceptions
{
    /// <summary>No existe la entidad solicitada (se mapea a 404).</summary>
    public class EntidadNoEncontradaException : DominioException
    {
        public EntidadNoEncontradaException(string entidad, object? clave = null)
            : base(clave is null
                ? $"No se encontró {entidad}."
                : $"No se encontró {entidad} con identificador '{clave}'.")
        { }
    }
}
