using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Services.Extensions
{
    internal static class GranjaRepositoryExtensions
    {
        /// <summary>La granja es única: si no existe es un error (404), normalmente porque falta sembrarla.</summary>
        public static async Task<Granja> ObtenerGranjaAsync(this IGranjaRepository repo, CancellationToken ct = default)
            => await repo.ObtenerAsync(ct) ?? throw new EntidadNoEncontradaException("la granja");
    }
}
