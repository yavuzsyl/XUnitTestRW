using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XUnitTest.WEB.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetEntities();
        Task<TEntity> GetEntity(int id);
        Task Create(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
