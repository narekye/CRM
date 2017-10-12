using System.Collections.Generic;

namespace CRM.DAL.Repository.Core
{
    public interface IBaseDAL
    {
        void Add<TEntity>(TEntity entity) where TEntity : class;
        void BulkInsert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        void SetValues(object entity, object dbEntity);
        void Save<TEntity>(TEntity entity) where TEntity : class;
        void Remove<TEntity>(TEntity entity) where TEntity : class;
        void Attach<TEntity>(TEntity entity) where TEntity : class;
        void Reload<TEntity>(TEntity entity) where TEntity : class;
        TEntity Find<TEntity>(params object[] keyValues) where TEntity : class;
        bool HasChanges();
        int SaveChanges();
        void EnsureTransaction();
        void BeginTransaction();
        void RollbackTransaction();
        void CommitChanges();
    }
}
