namespace Domain.Exceptions
{
    /// <summary>Dato inválido para una regla del dominio (se mapea a 400).</summary>
    public class ValidacionDominioException : DominioException
    {
        public ValidacionDominioException(string mensaje) : base(mensaje) { }
    }
}
