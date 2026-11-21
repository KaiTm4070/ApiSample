using Microsoft.Extensions.DependencyInjection;

namespace Services
{
    public static class Dependencyinjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Scoped: dependen de repositorios y del DbContext, que viven una petición.
            services.AddScoped<IGranjaService, GranjaService>();
            services.AddScoped<IProduccionService, ProduccionService>();
            services.AddScoped<IInventarioService, InventarioService>();
            services.AddScoped<IVentaService, VentaService>();
            services.AddScoped<IReporteService, ReporteService>();
            return services;
        }
    }
}
