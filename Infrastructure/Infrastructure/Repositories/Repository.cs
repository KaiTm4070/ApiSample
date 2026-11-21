using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext Context;

        public Repository(AppDbContext context) => Context = context;

        protected DbSet<T> Set => Context.Set<T>();

        public virtual async Task<T?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
            => await Set.FindAsync(new object[] { id }, ct);

        public virtual async Task<IReadOnlyList<T>> ListarAsync(CancellationToken ct = default)
            => await Set.ToListAsync(ct);

        public virtual async Task AgregarAsync(T entidad, CancellationToken ct = default)
            => await Set.AddAsync(entidad, ct);
    }
}
