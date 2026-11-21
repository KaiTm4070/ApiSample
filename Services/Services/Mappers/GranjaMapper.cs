using Domain.Entities;
using Services.Dtos;

namespace Services.Mappers
{
    public static class GranjaMapper
    {
        public static GranjaResponse ToResponse(Granja granja) => new(
            granja.Nombre,
            granja.NumeroVacas,
            granja.LitrosPorVaca,
            granja.ProduccionDiariaEsperada,
            granja.Parametros
                .OrderBy(p => p.Tipo)
                .Select(p => new ParametroProductoResponse(
                    p.Tipo, UnidadProducto.De(p.Tipo), p.LitrosPorUnidad, p.VidaUtilDias, p.Precio))
                .ToList());

        public static VacaResponse ToResponse(Vaca vaca) => new(vaca.Id, vaca.Nombre, vaca.Raza, vaca.Activa);
    }
}
