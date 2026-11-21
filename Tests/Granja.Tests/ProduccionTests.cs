using Domain.Enums;
using Domain.Exceptions;
using Services.Dtos;

namespace Granja.Tests;

public class ProduccionTests
{
    // Prueba mínima 1: la producción diaria es n × l.
    [Fact]
    public async Task Produccion_diaria_es_n_por_l()
    {
        using var env = await TestEnv.CrearAsync();

        var resultado = await env.ProducirAsync(1);

        Assert.Equal(10 * 15, resultado.TotalLitros);
        Assert.Equal(10, resultado.Vacas.Count);
    }

    // Prueba mínima 1 (segunda parte): no puede registrarse dos veces para la misma fecha.
    [Fact]
    public async Task Produccion_no_puede_registrarse_dos_veces_la_misma_fecha()
    {
        using var env = await TestEnv.CrearAsync();
        await env.ProducirAsync(1);

        await Assert.ThrowsAsync<ProduccionDuplicadaException>(() => env.ProducirAsync(1));
    }

    [Fact]
    public async Task Cambiar_n_y_l_cambia_la_produccion_diaria()
    {
        using var env = await TestEnv.CrearAsync();
        await env.Granja.ActualizarAsync(new GranjaUpdateRequest { NumeroVacas = 4, LitrosPorVaca = 20 });

        var resultado = await env.ProducirAsync(1);

        Assert.Equal(80, resultado.TotalLitros);
        Assert.Equal(4, resultado.Vacas.Count);
    }

    [Fact]
    public async Task Actualizar_granja_con_datos_invalidos_lanza_validacion()
    {
        using var env = await TestEnv.CrearAsync();

        await Assert.ThrowsAsync<ValidacionDominioException>(() =>
            env.Granja.ActualizarAsync(new GranjaUpdateRequest { NumeroVacas = 0, LitrosPorVaca = 15 }));
    }

    // Prueba mínima 2: no se pueden transformar más litros que los disponibles de ese día.
    [Fact]
    public async Task No_se_pueden_transformar_mas_litros_que_los_producidos()
    {
        using var env = await TestEnv.CrearAsync();
        await env.ProducirAsync(1);
        await env.TransformarAsync(1, TipoProducto.Queso, 100);

        // Quedan 50 L: pedir 60 falla y pedir 50 funciona.
        await Assert.ThrowsAsync<LecheInsuficienteException>(() => env.TransformarAsync(1, TipoProducto.Yogurt, 60));

        var ok = await env.TransformarAsync(1, TipoProducto.Yogurt, 50);
        Assert.Equal(50, ok.Cantidad);
    }

    [Fact]
    public async Task No_se_puede_transformar_un_dia_sin_produccion()
    {
        using var env = await TestEnv.CrearAsync();

        await Assert.ThrowsAsync<LecheInsuficienteException>(() => env.TransformarAsync(1, TipoProducto.Queso, 10));
    }

    [Fact]
    public async Task Transformacion_aplica_rendimiento_y_vida_util()
    {
        using var env = await TestEnv.CrearAsync();
        await env.ProducirAsync(1);

        var queso = await env.TransformarAsync(1, TipoProducto.Queso, 100);
        var yogurt = await env.TransformarAsync(1, TipoProducto.Yogurt, 50);

        Assert.Equal(10, queso.Cantidad);                         // 100 L ÷ 10 L/kg
        Assert.Equal(TestEnv.Dia(1).AddDays(30), queso.FechaVencimiento);
        Assert.Equal(50, yogurt.Cantidad);                        // 50 L ÷ 1 L/L
        Assert.Equal(TestEnv.Dia(16), yogurt.FechaVencimiento);   // vence el día 16
    }
}
