using Domain.Enums;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Services.Dtos;

namespace Granja.Tests;

public class VentaTests
{
    // Prueba mínima 3: una venta toma producto del lote más antiguo antes que de uno más nuevo.
    [Fact]
    public async Task Venta_toma_del_lote_mas_antiguo()
    {
        using var env = await TestEnv.CrearAsync();
        await env.PrepararAsync();

        var venta = await env.VenderAsync(3, TipoProducto.Queso, 5);

        var detalle = Assert.Single(venta.Detalle);
        Assert.Equal(TestEnv.Dia(1), detalle.FechaProduccion);
        Assert.Equal(5, detalle.Cantidad);
    }

    // Prueba mínima 4: una venta que excede un lote consume el siguiente y genera varios detalles.
    [Fact]
    public async Task Venta_que_excede_un_lote_consume_el_siguiente()
    {
        using var env = await TestEnv.CrearAsync();
        await env.PrepararAsync();

        var venta = await env.VenderAsync(3, TipoProducto.Queso, 12);

        Assert.Equal(2, venta.Detalle.Count);
        Assert.Equal(10, venta.Detalle[0].Cantidad);
        Assert.Equal(TestEnv.Dia(1), venta.Detalle[0].FechaProduccion);
        Assert.Equal(2, venta.Detalle[1].Cantidad);
        Assert.Equal(TestEnv.Dia(2), venta.Detalle[1].FechaProduccion);
        Assert.Equal(28000, venta.PrecioUnitario);
        Assert.Equal(336000, venta.Total);   // 12 kg × 28.000 (ejemplo del punto 6.3)
    }

    // Prueba mínima 5: un lote vencido nunca se vende, aunque sea el más antiguo.
    [Fact]
    public async Task Lote_vencido_nunca_se_vende_aunque_sea_el_mas_antiguo()
    {
        using var env = await TestEnv.CrearAsync();

        // Y1 (día 1, vence el 16) y Y2 (día 10, vence el 25), 50 L cada uno.
        await env.ProducirAsync(1);
        await env.TransformarAsync(1, TipoProducto.Yogurt, 50);
        await env.ProducirAsync(10);
        var y2 = await env.TransformarAsync(10, TipoProducto.Yogurt, 50);

        // Día 17: Y1 ya venció (17 > 16), así que la venta sale solo de Y2.
        var venta = await env.VenderAsync(17, TipoProducto.Yogurt, 5);

        var detalle = Assert.Single(venta.Detalle);
        Assert.Equal(y2.LoteId, detalle.LoteId);
    }

    [Fact]
    public async Task Lote_vencido_no_cuenta_como_existencia_aunque_no_se_hayan_procesado_vencimientos()
    {
        using var env = await TestEnv.CrearAsync();
        await env.PrepararAsync();

        await Assert.ThrowsAsync<InventarioInsuficienteException>(() => env.VenderAsync(17, TipoProducto.Yogurt, 5));
    }

    // Prueba mínima 6: una venta con existencias insuficientes no modifica el inventario.
    [Fact]
    public async Task Venta_con_existencias_insuficientes_no_modifica_el_inventario()
    {
        using var env = await TestEnv.CrearAsync();
        await env.PrepararAsync();

        // Hay 25 kg de queso: pedir 30 se rechaza completa.
        await Assert.ThrowsAsync<InventarioInsuficienteException>(() => env.VenderAsync(3, TipoProducto.Queso, 30));

        env.Db.ChangeTracker.Clear();
        var lotes = await env.Db.Lotes.AsNoTracking().Where(l => l.Tipo == TipoProducto.Queso).ToListAsync();
        Assert.Equal(25, lotes.Sum(l => l.CantidadDisponible));
        Assert.Empty(await env.Db.Ventas.AsNoTracking().ToListAsync());
    }

    [Fact]
    public async Task Venta_no_toma_lotes_producidos_despues_de_la_fecha_de_venta()
    {
        using var env = await TestEnv.CrearAsync();
        await env.PrepararAsync();

        // El día 1 solo existe Q1 (10 kg): Q2 se produce el día 2.
        await Assert.ThrowsAsync<InventarioInsuficienteException>(() => env.VenderAsync(1, TipoProducto.Queso, 12));
    }

    [Fact]
    public async Task Venta_con_cantidad_invalida_lanza_validacion()
    {
        using var env = await TestEnv.CrearAsync();

        await Assert.ThrowsAsync<ValidacionDominioException>(() =>
            env.Ventas.RegistrarVentaAsync(new VentaRequest { TipoProducto = TipoProducto.Queso, Cantidad = 0 }));
    }

    [Fact]
    public async Task Consultar_una_venta_inexistente_lanza_no_encontrado()
    {
        using var env = await TestEnv.CrearAsync();

        await Assert.ThrowsAsync<EntidadNoEncontradaException>(() => env.Ventas.ObtenerVentaAsync(999));
    }
}
