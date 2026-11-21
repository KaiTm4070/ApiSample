namespace Domain.Enums
{
    public enum EstadoLote
    {
        /// <summary>Tiene existencias y se puede vender (mientras no venza).</summary>
        Disponible = 1,

        /// <summary>Se vendió completo.</summary>
        Agotado = 2,

        /// <summary>Venció con existencias: lo no vendido pasó a cantidad dañada.</summary>
        Vencido = 3
    }
}
