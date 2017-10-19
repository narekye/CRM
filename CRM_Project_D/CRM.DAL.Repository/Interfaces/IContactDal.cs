using CRM.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.DAL.Repository.Interfaces
{
    public interface IContactDal : IDisposable
    {
        Task<IEnumerable<Contact>> GetAllContacts();
    }
}
