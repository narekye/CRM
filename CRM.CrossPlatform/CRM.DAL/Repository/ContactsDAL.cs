using System;
using System.Collections.Generic;
using System.Text;
using CRM.DAL.Entities;
using CRM.DAL.Repository.Core;

namespace CRM.DAL.Repository
{
    public class ContactsDAL : CoreDAL
    {
        public ContactsDAL(BaseDAL dal) : base(dal) { }

        public IEnumerable<Contacts> GetAllContacts()
        {
            return null;
        }
    }
}
