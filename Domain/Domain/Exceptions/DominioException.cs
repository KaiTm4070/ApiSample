namespace Domain.Exceptions
{
    /// <summary>
    /// Base de las excepciones de negocio. El dominio no conoce HTTP: el middleware
    /// de la capa Web decide qué código devuelve para cada subtipo.
    /// </summary>
    public abstract class DominioException : Exception
    {
        protected DominioException(string mensaje) : base(mensaje) { }
    }
}
