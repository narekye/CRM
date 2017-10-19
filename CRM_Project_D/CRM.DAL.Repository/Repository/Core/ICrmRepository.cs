using System;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.DAL.Repository.Repository.Core
{
    public interface ICrmRepository<T> : IDisposable
    {
        void SetValues(object entity, object dbEntity);
        void Insert(T entity);
        void Delete(T entity);
        IQueryable<T> SearchFor(Func<T, bool> predicate);
        IQueryable<T> GetAll();
        T GetById(Guid guid);
        Task<int> SaveChanges();
        void BeginTransaction();
        void RollbackTransaction();
        void CommitTransaction();
    }
}
