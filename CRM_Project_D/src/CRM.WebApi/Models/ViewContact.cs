namespace CRM.WebApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Entities;
    using AutoMapper;
    public class ViewContact
    {
        private static readonly CRMContext Datacontext = new CRMContext();
        static ViewContact()
        {
            Mapper.Initialize(f => f.CreateMap<Contact, ViewContact>()
               .ForMember("FullName", c => c.MapFrom(o => o.FullName))
               .ForMember("CompanyName", c => c.MapFrom(o => o.CompanyName))
               .ForMember("Position", c => c.MapFrom(o => o.Position))
               .ForMember("Country", c => c.MapFrom(o => o.Country))
               .ForMember("Email", c => c.MapFrom(o => o.Email))
               .ForMember("GuId", c => c.MapFrom(o => o.GuID))
               .ForMember("DateInserted", c => c.MapFrom(o => o.DateInserted))
               .ForMember("EmailLists", c => c.MapFrom(o => o.EmailLists.Select(p => p.EmailListName))));
        }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public Guid GuId { get; set; }
        public DateTime DateInserted { get; set; }
        public List<string> EmailLists { get; set; }

        public static ViewContact CreateViewModel(Contact contact)
        {
            var result = Mapper.Map<Contact, ViewContact>(contact);
            return result;
        }

        public static List<ViewContact> GetViewModelList(List<Contact> list)
        {
            var result = new List<ViewContact>();
            foreach (Contact contact in list)
                result.Add(CreateViewModel(contact));
            return result;
        }

        public static async Task<Contact> GetContactFromContactModel(ViewContact model, bool flag, List<EmailList> emailList = null)
        {
            Contact contact;
            if (flag) // true returns new object with new Guid from model without contactId
            {
                contact = new Contact()
                {
                    GuID = Guid.NewGuid(),
                    DateInserted = DateTime.UtcNow,
                    FullName = model.FullName,
                    CompanyName = model.CompanyName,
                    Country = model.Country,
                    Email = model.Email,
                    Position = model.Position,
                    EmailLists = emailList
                };
            }
            else // false returns object from database 
            {
                using (var database = new CRMContext())
                {
                    contact = await database.Contacts.FirstOrDefaultAsync(p => p.GuID == model.GuId);
                    contact.FullName = model.FullName;
                    contact.CompanyName = model.CompanyName;
                    contact.Country = model.Country;
                    contact.Email = model.Email;
                    contact.Position = model.Position;
                    contact.EmailLists = emailList;
                }
            }
            return contact;
        }
        #region testing remap
        // didn't use remap.
        [Obsolete("Didn't use this method", true)]
        public static Contact ReMap(ViewContact model)
        {
            // CRMContext datacontext = new CRMContext();
            Mapper.Initialize(f => f.CreateMap<ViewContact, Contact>()
            .ForMember("ContactId", c => c.MapFrom(o => Datacontext.Contacts.FirstOrDefault(z => z.GuID == o.GuId).ContactId))
            .ForMember("FullName", c => c.MapFrom(o => o.FullName))
               .ForMember("CompanyName", c => c.MapFrom(o => o.CompanyName))
               .ForMember("Position", c => c.MapFrom(o => o.Position))
               .ForMember("Country", c => c.MapFrom(o => o.Country))
               .ForMember("Email", c => c.MapFrom(o => o.Email))
               .ForMember("GuID", c => c.MapFrom(o => o.GuId))
               .ForMember("DateInserted", c => c.MapFrom(o => o.DateInserted))
            // .ForMember("EmailLists", c => c.MapFrom(p => datacontext.EmailLists.Where(i=> i.EmailListName == model.EmailLists.Any()))

            /*datacontext.Contacts
            .Select(k => k.EmailLists.Where(i => model.EmailLists.Contains(i.EmailListName)))))*/
            );
            var result = Mapper.Map<ViewContact, Contact>(model);
            return result;
        }

        #endregion
    }
}