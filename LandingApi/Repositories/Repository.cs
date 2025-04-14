using LandingApi.Data;
using LandingApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LandingApi.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();
        public async Task<T> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);
        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);
        public async Task UpdateAsync(T entity) => _context.Set<T>().Update(entity);
        public async Task DeleteAsync(int id) => _context.Set<T>().Remove(await GetByIdAsync(id));
        public IQueryable<T> AsQueryable() => _context.Set<T>().AsQueryable();
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
