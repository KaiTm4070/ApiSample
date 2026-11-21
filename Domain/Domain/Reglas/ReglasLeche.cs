using Domain.Exceptions;

namespace Domain.Reglas
{
    /// <summary>
    /// Reglas puras sobre la leche del día (RN-02). No consultan datos: el servicio
    /// obtiene los litros y el dominio decide si la operación es válida.
    /// </summary>
    public static class ReglasLeche
    {
        public static void ValidarTransformacion(
            DateOnly fecha, decimal litrosProducidos, decimal litrosYaTransformados, decimal litrosSolicitados)
        {
            if (litrosSolicitados <= 0)
                throw new ValidacionDominioException("Los litros a transformar deben ser mayores que cero.");

            var disponibles = litrosProducidos - litrosYaTransformados;
            if (litrosSolicitados > disponibles)
                throw new LecheInsuficienteException(fecha, Math.Max(0, disponibles), litrosSolicitados);
        }

        /// <summary>Leche que no se transformó ese día: se reporta pero no se almacena.</summary>
        public static decimal LitrosNoAprovechados(decimal litrosProducidos, decimal litrosTransformados)
            => Math.Max(0, litrosProducidos - litrosTransformados);
    }
}
