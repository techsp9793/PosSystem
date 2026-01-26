using Microsoft.EntityFrameworkCore;
using PosSystem.Data.Repositories.Interfaces;
using System.Linq.Expressions;

namespace PosSystem.Data.Repositories.Implementations
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            this.dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(string id) => await dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync() => await dbSet.ToListAsync();

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await dbSet.Where(predicate).ToListAsync();

        public async Task AddAsync(T entity) => await dbSet.AddAsync(entity);

        public void Remove(T entity) => dbSet.Remove(entity);

        public void Update(T entity) => dbSet.Update(entity);
    }
}
