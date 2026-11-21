using Domain.Exceptions;

namespace Domain.Entities
{
    /// <summary>Leche producida por una vaca en una fecha (RN-01: única por vaca y fecha).</summary>
    public class ProduccionLeche
    {
        public int Id { get; private set; }
        public int VacaId { get; private set; }
        public Vaca Vaca { get; private set; } = null!;
        public DateOnly Fecha { get; private set; }
        public decimal Litros { get; private set; }

        private ProduccionLeche() { }

        public ProduccionLeche(int vacaId, DateOnly fecha, decimal litros)
        {
            if (litros < 0)
                throw new ValidacionDominioException("Los litros producidos no pueden ser negativos.");
            VacaId = vacaId;
            Fecha = fecha;
            Litros = litros;
        }
    }
}
