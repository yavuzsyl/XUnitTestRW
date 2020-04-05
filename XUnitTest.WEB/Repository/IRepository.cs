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
        Task<bool> Create(TEntity entity);
        bool Update(TEntity entity);
        bool Delete(TEntity entity);
    }
}
