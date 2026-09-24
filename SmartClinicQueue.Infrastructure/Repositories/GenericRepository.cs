using Microsoft.EntityFrameworkCore;
using SmartClinicQueue.Application.Interfaces.IReposirories;
using SmartClinicQueue.Domain.Entities;
using SmartClinicQueue.Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Infrastructure.Repositories
{
    public class GenericRepository<TEntity, TKey> :
        IGenericRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }
        public IQueryable<TEntity> GetAll(bool trackChanges = false)
        {
            return trackChanges
                ? _dbSet
                : _dbSet.AsNoTracking();
        }
        public IQueryable<TEntity> FindByCondition(
         Expression<Func<TEntity, bool>> expression,
         bool trackChanges = false)
        {
            return trackChanges
                ? _dbSet.Where(expression)
                : _dbSet.AsNoTracking().Where(expression);
        }
        public async Task<TEntity?> GetByIdAsync(
            TKey id,
            bool trackChanges = false)
        {
            if (trackChanges)
            {
                return await _dbSet.FindAsync(id);
            }

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id!.Equals(id));
        }
        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }
        public async Task<bool> ExistsAsync(TKey id)
        {
            return await _dbSet.AsNoTracking().AnyAsync(e => e.Id!.Equals(id));
        }
    }
}
