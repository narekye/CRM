namespace CRM.WebApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using AutoMapper;
    public class ContactModel
    {
        private static readonly CRMContext Datacontext = new CRMContext();
        static ContactModel()
        {
            Mapper.Initialize(f => f.CreateMap<Contact, ContactModel>()
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
        public static ContactModel GetContactModel(Contact contact)
        {
            var result = Mapper.Map<Contact, ContactModel>(contact);
            return result;
        }

        public static List<ContactModel> GetContactModelList(List<Contact> list)
        {
            var result = new List<ContactModel>();
            foreach (Contact contact in list)
                result.Add(GetContactModel(contact));
            return result;
        }

        #region testing remap

        public static Contact ReMap(ContactModel model)
        {
            // CRMContext datacontext = new CRMContext();
            Mapper.Initialize(f => f.CreateMap<ContactModel, Contact>()
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
            var result = Mapper.Map<ContactModel, Contact>(model);
            return result;
        }

        #endregion
    }
}