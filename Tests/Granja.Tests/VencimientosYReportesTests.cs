using Domain.Enums;
using Domain.Exceptions;
using Services.Dtos;

namespace Granja.Tests;

public class VencimientosYReportesTests
{
    // Prueba mínima 7: al procesar vencimientos, lo disponible pasa a dañado y el reporte lo refleja.
    [Fact]
    public async Task Procesar_vencimientos_pasa_disponible_a_danado_y_el_reporte_lo_refleja()
    {
        using var env = await TestEnv.CrearAsync();
        await env.ProducirAsync(1);
        await env.TransformarAsync(1, TipoProducto.Yogurt, 50);
        await env.VenderAsync(3, TipoProducto.Yogurt, 20);

        var resultado = await env.Inventario.ProcesarVencimientosAsync(TestEnv.Dia(17));

        Assert.Equal(1, resultado.LotesProcesados);
        Assert.Equal(30, resultado.Lotes.Single().CantidadDanada);

        var reporte = await env.Reportes.ProductoDanadoAsync(null, null);
        var lote = Assert.Single(reporte.Lotes);
        Assert.Equal(30, lote.CantidadDanada);
        Assert.Equal(TestEnv.Dia(16), lote.FechaVencimiento);
        Assert.Equal(30, reporte.TotalesPorTipo.Single(t => t.TipoProducto == TipoProducto.Yogurt).CantidadDanada);
    }

    [Fact]
    public async Task Procesar_vencimientos_no_toca_lotes_vigentes()
    {
        using var env = await TestEnv.CrearAsync();
        await env.ProducirAsync(1);
        await env.TransformarAsync(1, TipoProducto.Yogurt, 50);

        // El día 16 todavía no está vencido: solo vence si fecha > vencimiento.
        var resultado = await env.Inventario.ProcesarVencimientosAsync(TestEnv.Dia(16));

        Assert.Equal(0, resultado.LotesProcesados);
    }

    /// <summary>Escenario completo del punto 8 del taller.</summary>
    [Fact]
    public async Task Escenario_de_referencia_produce_los_reportes_esperados()
    {
        using var env = await TestEnv.CrearAsync();
        await env.PrepararAsync();
        await env.VenderAsync(3, TipoProducto.Queso, 12);
        await env.VenderAsync(3, TipoProducto.Yogurt, 20);
        await env.Inventario.ProcesarVencimientosAsync(TestEnv.Dia(17));
        await Assert.ThrowsAsync<InventarioInsuficienteException>(() => env.VenderAsync(17, TipoProducto.Yogurt, 5));

        // Producto total
        var total = await env.Reportes.ProductoTotalAsync();
        var queso = total.Productos.Single(p => p.TipoProducto == TipoProducto.Queso);
        var yogurt = total.Productos.Single(p => p.TipoProducto == TipoProducto.Yogurt);
        Assert.Equal((25m, 12m, 13m, 0m), (queso.Producido, queso.Vendido, queso.Disponible, queso.Danado));
        Assert.Equal((50m, 20m, 0m, 30m), (yogurt.Producido, yogurt.Vendido, yogurt.Disponible, yogurt.Danado));

        // Leche por día: 150 L el día 1 y el día 2, sin leche desperdiciada
        var leche = await env.Reportes.LecheDiariaAsync(null, null);
        Assert.Equal(2, leche.Dias.Count);
        Assert.All(leche.Dias, d => Assert.Equal(150, d.LitrosProducidos));
        Assert.All(leche.Dias, d => Assert.Equal(0, d.LitrosNoAprovechados));

        // Producto por día: queso (día 1), yogurt (día 1) y queso (día 2)
        var diario = await env.Reportes.ProductoDiarioAsync(null, null);
        Assert.Equal(3, diario.Items.Count);

        // Ventas: 12 × 28.000 + 20 × 5.000
        var ventas = await env.Reportes.VentasAsync(null, null, null);
        Assert.Equal(336000m + 100000m, ventas.TotalGeneral);
    }

    [Fact]
    public async Task Reporte_con_rango_invertido_lanza_validacion()
    {
        using var env = await TestEnv.CrearAsync();

        await Assert.ThrowsAsync<ValidacionDominioException>(() =>
            env.Reportes.VentasAsync(TestEnv.Dia(10), TestEnv.Dia(1), null));
    }

    [Fact]
    public async Task Reporte_de_vacas_calcula_promedio_mas_y_menos_productiva_y_bajo_el_promedio()
    {
        using var env = await TestEnv.CrearAsync();
        var vacas = await env.Granja.ListarVacasAsync();

        await env.Produccion.RegistrarProduccionAsync(new ProduccionRequest
        {
            Fecha = TestEnv.Dia(1),
            Vacas = new List<ProduccionVacaRequest>
            {
                new() { VacaId = vacas[0].Id, Litros = 20 },
                new() { VacaId = vacas[1].Id, Litros = 10 },
                new() { VacaId = vacas[2].Id, Litros = 6 }
            }
        });

        var reporte = await env.Reportes.VacasAsync(null, null);

        Assert.Equal(12m, reporte.PromedioDiarioPorVaca);                  // (20 + 10 + 6) / 3
        Assert.Equal(vacas[0].Id, reporte.MasProductiva!.VacaId);
        Assert.Equal(vacas[2].Id, reporte.MenosProductiva!.VacaId);
        Assert.Equal(2, reporte.BajoElPromedio.Count);                     // 10 y 6 están bajo 12
    }
}
