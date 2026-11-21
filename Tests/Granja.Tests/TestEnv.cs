using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Granja.Tests;

public class FakeClock : IClock
{
    public DateOnly Today { get; set; } = new(2026, 12, 1);
}

/// <summary>
/// Entorno de pruebas: SQLite en memoria (la misma tecnología de la API), repositorios y servicios reales
/// y un reloj simulado (RN-10). Cada prueba crea el suyo, así no comparten datos.
/// </summary>
public sealed class TestEnv : IDisposable
{
    private readonly SqliteConnection _connection;

    public AppDbContext Db { get; }
    public FakeClock Clock { get; } = new();
    public GranjaService Granja { get; }
    public ProduccionService Produccion { get; }
    public InventarioService Inventario { get; }
    public VentaService Ventas { get; }
    public ReporteService Reportes { get; }

    private TestEnv(SqliteConnection connection, AppDbContext db)
    {
        _connection = connection;
        Db = db;

        IUnitOfWork uow = new UnitOfWork(db);
        IGranjaRepository granjas = new GranjaRepository(db);
        IProduccionRepository producciones = new ProduccionRepository(db);
        ILoteRepository lotes = new LoteRepository(db);
        IVentaRepository ventas = new VentaRepository(db);

        Granja = new GranjaService(granjas, uow);
        Produccion = new ProduccionService(granjas, producciones, lotes, uow, Clock);
        Inventario = new InventarioService(lotes, uow, Clock);
        Ventas = new VentaService(granjas, lotes, ventas, uow, Clock);
        Reportes = new ReporteService(ventas, lotes, producciones);
    }

    public static async Task<TestEnv> CrearAsync()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
        var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();
        await DbInitializer.SembrarAsync(db);   // n = 10, l = 15; queso 10 L/kg 30 días; yogurt 1 L/L 15 días

        return new TestEnv(connection, db);
    }

    /// <summary>Fecha del "día N" del escenario de referencia (día 1 = 2026-12-01).</summary>
    public static DateOnly Dia(int n) => new DateOnly(2026, 12, 1).AddDays(n - 1);

    public void Dispose()
    {
        Db.Dispose();
        _connection.Dispose();
    }
}
