using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IClock, SystemClock>();

            services.AddScoped<IGranjaRepository, GranjaRepository>();
            services.AddScoped<IProduccionRepository, ProduccionRepository>();
            services.AddScoped<ILoteRepository, LoteRepository>();
            services.AddScoped<IVentaRepository, VentaRepository>();

            return services;
        }

        /// <summary>Aplica las migraciones pendientes y crea la granja inicial si la base está vacía.</summary>
        public static async Task InicializarBaseDeDatosAsync(this IServiceProvider services, CancellationToken ct = default)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Database.MigrateAsync(ct);
            await DbInitializer.SembrarAsync(context, ct);
        }
    }
}
