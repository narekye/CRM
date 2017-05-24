using System.Collections.Generic;

namespace CRM.WebApi.Models
{
    using AutoMapper;
    public class EmailListsModel
    {
        public int EmailListId { get; set; }
        public string EmailListName { get; set; }

        public List<ContactModel> Contacts { get; set; }


    }
}