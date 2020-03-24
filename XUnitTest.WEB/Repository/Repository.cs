using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XUnitTest.WEB.Data;

namespace XUnitTest.WEB.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext context;
        private readonly DbSet<TEntity> dbSet;
        public Repository(ApplicationDbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();

        }

        public async Task Create(TEntity entity)
        {
            await dbSet.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public void Delete(TEntity entity)
        {
            dbSet.Remove(entity);
            context.SaveChanges();
        }

        public async Task<IEnumerable<TEntity>> GetEntities() => await dbSet.ToListAsync();
       
        public async Task<TEntity> GetEntity(int id) => await dbSet.FindAsync(id);


        public void Update(TEntity entity)
        {
            context.Entry<TEntity>(entity).State = EntityState.Modified;
            //modified ile entity komple güncellenir
            //update ile sadece değişen alan güncellenir
            //dbSet.Update(entity);
            context.SaveChanges();
        }
    }
}
