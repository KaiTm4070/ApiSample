using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GranjaRepository : Repository<Granja>, IGranjaRepository
    {
        public GranjaRepository(AppDbContext context) : base(context) { }

        public Task<Granja?> ObtenerAsync(CancellationToken ct = default)
            => Context.Granjas
                .Include(g => g.Vacas)
                .Include(g => g.Parametros)
                .AsSplitQuery()
                .OrderBy(g => g.Id)
                .FirstOrDefaultAsync(ct);
    }
}
