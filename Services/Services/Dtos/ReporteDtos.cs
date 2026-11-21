using Domain.Enums;

namespace Services.Dtos
{
    // ---- Ventas ----
    public sealed record VentaReporteItem(
        int VentaId, DateOnly Fecha, TipoProducto TipoProducto, string Unidad,
        decimal Cantidad, decimal PrecioUnitario, decimal Total);

    public sealed record ReporteVentasResponse(
        DateOnly? Desde, DateOnly? Hasta, TipoProducto? TipoProducto,
        IReadOnlyList<VentaReporteItem> Ventas, decimal TotalGeneral);

    // ---- Leche por día ----
    public sealed record LecheDiariaItem(
        DateOnly Fecha, decimal LitrosProducidos, decimal LitrosTransformados, decimal LitrosNoAprovechados);

    public sealed record ReporteLecheDiariaResponse(
        DateOnly? Desde, DateOnly? Hasta, IReadOnlyList<LecheDiariaItem> Dias,
        decimal TotalProducido, decimal TotalTransformado, decimal TotalNoAprovechado);

    // ---- Producto por día ----
    public sealed record ProductoDiarioItem(
        DateOnly Fecha, TipoProducto TipoProducto, string Unidad, decimal Cantidad, decimal LitrosUsados);

    public sealed record ReporteProductoDiarioResponse(
        DateOnly? Desde, DateOnly? Hasta, IReadOnlyList<ProductoDiarioItem> Items);

    // ---- Producto total ----
    public sealed record ProductoTotalItem(
        TipoProducto TipoProducto, string Unidad, decimal Producido, decimal Vendido, decimal Disponible, decimal Danado);

    public sealed record ReporteProductoTotalResponse(IReadOnlyList<ProductoTotalItem> Productos);

    // ---- Producto dañado ----
    public sealed record ProductoDanadoItem(
        int LoteId, TipoProducto TipoProducto, string Unidad,
        DateOnly FechaProduccion, DateOnly FechaVencimiento, decimal CantidadDanada);

    public sealed record ProductoDanadoTotal(TipoProducto TipoProducto, string Unidad, decimal CantidadDanada);

    public sealed record ReporteProductoDanadoResponse(
        DateOnly? Desde, DateOnly? Hasta,
        IReadOnlyList<ProductoDanadoItem> Lotes, IReadOnlyList<ProductoDanadoTotal> TotalesPorTipo);

    // ---- Vacas ----
    public sealed record VacaProduccionItem(
        int VacaId, string Nombre, decimal TotalLitros, int DiasRegistrados, decimal PromedioDiario);

    public sealed record ReporteVacasResponse(
        DateOnly? Desde, DateOnly? Hasta,
        decimal PromedioDiarioPorVaca,
        VacaProduccionItem? MasProductiva,
        VacaProduccionItem? MenosProductiva,
        IReadOnlyList<VacaProduccionItem> BajoElPromedio,
        IReadOnlyList<VacaProduccionItem> Vacas);
}
