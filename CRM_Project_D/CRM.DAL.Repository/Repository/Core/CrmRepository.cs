using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace CRM.DAL.Repository.Repository.Core
{
    public class CrmRepository<T> : ICrmRepository<T> where T : class
    {
        protected DbSet<T> DbSet;

        protected DbContext Db;

        protected DbContextTransaction transaction;

        public CrmRepository(DbContext context)
        {
            DbSet = context.Set<T>();
            context.Configuration.LazyLoadingEnabled = false;
            Db = context;
        }

        public void SetValues(object entity, object dbEntity)
        {
            Db.Entry(dbEntity).CurrentValues.SetValues(entity);
        }

        public void Insert(T entity)
        {
            DbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }

        public IQueryable<T> SearchFor(Func<T, bool> predicate)
        {
            return DbSet.Where(predicate).AsQueryable();
        }

        public IQueryable<T> GetAll()
        {
            return DbSet.AsQueryable();
        }

        public T GetById(Guid guid)
        {
            return DbSet.Find(guid);
        }

        public async Task<int> SaveChanges()
        {
            return await Db.SaveChangesAsync();
        }

        public void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }

        public void BeginTransaction()
        {
            if (transaction != null)
                transaction = Db.Database.BeginTransaction();
            else throw new TransactionException();
        }

        public void RollbackTransaction()
        {
            if (transaction == null) return;
            transaction.Rollback();
            transaction = null;
        }

        public void CommitTransaction()
        {
            if (transaction == null) return;
            transaction.Commit();
            transaction = null;
        }
    }
}
