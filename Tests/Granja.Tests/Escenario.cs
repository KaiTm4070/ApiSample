using Domain.Enums;
using Services.Dtos;

namespace Granja.Tests;

/// <summary>Pasos reutilizables del escenario de referencia del taller (punto 8).</summary>
public static class Escenario
{
    public static Task<ProduccionResponse> ProducirAsync(this TestEnv env, int dia) =>
        env.Produccion.RegistrarProduccionAsync(new ProduccionRequest { Fecha = TestEnv.Dia(dia) });

    public static Task<TransformacionResponse> TransformarAsync(this TestEnv env, int dia, TipoProducto tipo, decimal litros) =>
        env.Produccion.TransformarAsync(new TransformacionRequest { Fecha = TestEnv.Dia(dia), TipoProducto = tipo, Litros = litros });

    public static Task<VentaResponse> VenderAsync(this TestEnv env, int dia, TipoProducto tipo, decimal cantidad) =>
        env.Ventas.RegistrarVentaAsync(new VentaRequest { Fecha = TestEnv.Dia(dia), TipoProducto = tipo, Cantidad = cantidad });

    /// <summary>Día 1: Q1 = 10 kg y Y1 = 50 L. Día 2: Q2 = 15 kg.</summary>
    public static async Task PrepararAsync(this TestEnv env)
    {
        await env.ProducirAsync(1);
        await env.TransformarAsync(1, TipoProducto.Queso, 100);
        await env.TransformarAsync(1, TipoProducto.Yogurt, 50);
        await env.ProducirAsync(2);
        await env.TransformarAsync(2, TipoProducto.Queso, 150);
    }
}
