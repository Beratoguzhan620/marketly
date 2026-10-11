using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace App.Data.Infrastructure
{
    public class DataRepository<T>(DbContext dbContext) where T : EntityBase
    {
        /// <summary>
        /// Birden fazla entity/DataRepository üzerinde birlikte başarılı olması gereken işlemler
        /// (ör. sipariş oluşturma) için transaction başlatır. Application katmanındaki servisler
        /// DbContext'e doğrudan erişmeden transaction yönetimini bu metot üzerinden yapabilir.
        /// </summary>
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await dbContext.Database.BeginTransactionAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await dbContext.Set<T>().FindAsync(id);
        }

        public IQueryable<T> GetAll()
        {
            return dbContext.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            entity.Id = default;
            entity.CreatedAt = DateTime.UtcNow;

            await dbContext.Set<T>().AddAsync(entity);
            await dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task<T?> UpdateAsync(T entity)
        {
            if (entity.Id == default)
            {
                return null;
            }

            var dbEntity = await GetByIdAsync(entity.Id);
            if (dbEntity == null)
            {
                return null;
            }

            entity.CreatedAt = dbEntity.CreatedAt;

            dbContext.Set<T>().Update(entity);
            await dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);

            if (entity == null)
            {
                return;
            }

            dbContext.Set<T>().Remove(entity);
            await dbContext.SaveChangesAsync();
        }
    }
}