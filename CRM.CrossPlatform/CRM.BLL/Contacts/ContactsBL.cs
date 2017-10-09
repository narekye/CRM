using System.Collections.Generic;
using System.Threading.Tasks;
using CRM.BLL.Core;
using CRM.DAL.Repository;
using Microsoft.EntityFrameworkCore;

namespace CRM.BLL.Contacts
{
    public class ContactsBL : CoreBL
    {
        public ContactsBL(ICrmEntities repository) : base(repository) { }

        public Task<List<DAL.Entities.Contacts>> GetAllContactsAsync()
        {
            var list = Repository.Contacts.ToListAsync();
            return list;
        }

        public Task UpdateContact(DAL.Entities.Contacts contact)
        {
            return null;
        }
    }
}
