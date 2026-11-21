using Domain.Exceptions;

namespace Domain.Entities
{
    public class Vaca
    {
        public int Id { get; private set; }
        public int GranjaId { get; private set; }
        public string Nombre { get; private set; }
        public string Raza { get; private set; }

        /// <summary>Solo las vacas activas producen leche. Al bajar n se desactivan, no se borran.</summary>
        public bool Activa { get; private set; } = true;

        private Vaca()
        {
            Nombre = string.Empty;
            Raza = string.Empty;
        }

        public Vaca(string nombre, string raza = "")
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ValidacionDominioException("La vaca debe tener nombre.");
            Nombre = nombre.Trim();
            Raza = raza?.Trim() ?? string.Empty;
        }

        internal void Activar() => Activa = true;
        internal void Desactivar() => Activa = false;
    }
}
