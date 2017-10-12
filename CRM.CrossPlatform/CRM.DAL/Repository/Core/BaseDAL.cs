using System;
using System.Collections.Generic;
using CRM.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CRM.DAL.Repository.Core
{
    public class BaseDAL : IBaseDAL, IDisposable
    {
        public BaseDAL(CrmEntities db = null)
        {
            if (db == null)
            {
                db = new CrmEntities();
            }
            Database = db;
        }
        protected internal CrmEntities Database { get; }
        protected IDbContextTransaction Transaction { get; private set; }
        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            Database.Set<TEntity>().Add(entity);
        }

        public void BulkInsert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void SetValues(object entity, object dbEntity)
        {
            throw new NotImplementedException();
        }

        public void Save<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                return;
            }

            var dbEntry = Database.Entry(entity);
            if (dbEntry == null || dbEntry.State == EntityState.Detached)
            {
                Add(entity);
            }
            else if (dbEntry.Entity == entity)
            {
                SetValues(entity, dbEntry.Entity);
            }
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            Database.Set<TEntity>().Remove(entity);
        }

        public void Attach<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void Reload<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public TEntity Find<TEntity>(params object[] keyValues) where TEntity : class
        {
            try
            {
                return Database.Set<TEntity>().Find(keyValues);
            }
            catch
            {
                throw;
            }
        }

        public bool HasChanges()
        {
            return Database.ChangeTracker.HasChanges();
        }

        public int SaveChanges()
        {
            return Database.SaveChanges();
        }

        public void EnsureTransaction()
        {
            if (Transaction != null)
                return;
            BeginTransaction();
        }

        public void BeginTransaction()
        {
            if (Transaction != null) { throw new InvalidOperationException(); }
            Transaction = Database.Database.BeginTransaction();
        }

        public void RollbackTransaction()
        {
            if (Transaction == null)
            {
                return;
            }
            Transaction.Rollback();
            Transaction = null;
        }

        public void CommitChanges()
        {
            if (Transaction == null) return;
            Transaction.Commit();
            Transaction = null;
        }

        public void Dispose()
        {
            Database?.Dispose();
            Transaction?.Dispose();
        }
    }
}
