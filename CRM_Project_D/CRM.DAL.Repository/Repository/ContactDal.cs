using CRM.DAL.Repository.Interfaces;
using CRM.DAL.Repository.Repository.Core;
using CRM.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace CRM.DAL.Repository.Repository
{
    public class ContactDal : BaseDal<Contact>, IContactDal
    {


        public void Dispose()
        {

        }

        public async Task<IEnumerable<Contact>> GetAllContacts()
        {
            var result = await Repository.GetAll().ToArrayAsync();
            return result;
        }

        public bool UpdateContact(Contact original, Contact replace)
        {
            try
            {
                Repository.SetValues(original, replace);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public ContactDal(ICrmRepository<Contact> repository) : base(repository)
        {

        }
    }
}
