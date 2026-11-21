using Domain.Enums;

namespace Services.Dtos
{
    public sealed record LoteResponse(
        int Id,
        TipoProducto TipoProducto,
        DateOnly FechaProduccion,
        DateOnly FechaVencimiento,
        decimal CantidadInicial,
        decimal CantidadDisponible,
        decimal CantidadDanada,
        string Unidad,
        EstadoLote Estado);

    public sealed record ExistenciaResponse(TipoProducto TipoProducto, string Unidad, decimal Disponible);

    public sealed record InventarioResponse(
        DateOnly Fecha,
        IReadOnlyList<ExistenciaResponse> Existencias,
        IReadOnlyList<LoteResponse> Lotes);

    public sealed record VencimientoLoteResponse(
        int LoteId,
        TipoProducto TipoProducto,
        string Unidad,
        DateOnly FechaProduccion,
        DateOnly FechaVencimiento,
        decimal CantidadDanada);

    public sealed record VencimientosResponse(
        DateOnly Fecha, int LotesProcesados, IReadOnlyList<VencimientoLoteResponse> Lotes);
}
