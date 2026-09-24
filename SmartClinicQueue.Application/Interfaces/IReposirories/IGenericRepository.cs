using SmartClinicQueue.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Application.Interfaces.IReposirories
{
    public interface IGenericRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        IQueryable<TEntity> GetAll(bool trackChanges = false);


        IQueryable<TEntity> FindByCondition(
            Expression<Func<TEntity, bool>> expression,
            bool trackChanges = false);

        Task<TEntity?> GetByIdAsync(
            TKey id,
            bool trackChanges = false);

        Task AddAsync(TEntity entity);
        void Update(TEntity entity);

        void Delete(TEntity entity);

        Task SaveChangesAsync();
        Task<bool> ExistsAsync(TKey id);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}
