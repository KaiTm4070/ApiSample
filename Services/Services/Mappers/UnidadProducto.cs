using Domain.Enums;

namespace Services.Mappers
{
    public static class UnidadProducto
    {
        public static string De(TipoProducto tipo) => tipo switch
        {
            TipoProducto.Queso => "kg",
            TipoProducto.Yogurt => "L",
            _ => "u"
        };
    }
}
