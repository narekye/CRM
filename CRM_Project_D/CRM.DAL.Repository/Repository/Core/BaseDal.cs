using CRM.Entities;

namespace CRM.DAL.Repository.Repository.Core
{
    public abstract class BaseDal<T> where T : class
    {
        protected ICrmRepository<T> Repository;

        protected BaseDal(ICrmRepository<T> repository)
        {
            if (repository == null)
                repository = new CrmRepository<T>(new CRMContext());

            Repository = repository;
        }
    }
}
