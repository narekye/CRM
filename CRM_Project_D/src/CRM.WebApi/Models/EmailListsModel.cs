using System.Collections.Generic;
using System.Linq;
using CRM.Entities;

namespace CRM.WebApi.Models
{
    using AutoMapper;
    public class EmailListsModel
    {
        static EmailListsModel()
        {
            Mapper.Initialize(f => f.CreateMap<EmailList, EmailListsModel>()
            .ForMember("EmailListId", p => p.MapFrom(z => z.EmailListID))
            .ForMember("EmailListName", p => p.MapFrom(z => z.EmailListName))
            .ForMember("Contacts", p => p.MapFrom(z => z.Contacts.Select(o => new ContactModel()
            {
                FullName = o.FullName,
                Email = o.Email,
                CompanyName = o.CompanyName,
                Country = o.Country,
                DateInserted = o.DateInserted,
                GuId = o.GuID,
                Position = o.Position
            }))
            ));
        }
        public int EmailListId { get; set; }
        public string EmailListName { get; set; }

        public List<ContactModel> Contacts { get; set; }

        public static EmailListsModel GetEmailListsModel(EmailList emailList)
        {
            var result = Mapper.Map<EmailList, EmailListsModel>(emailList);
            return result;
        }

        public static List<EmailListsModel> GetEmailListsModels(List<EmailList> list)
        {
            var result = new List<EmailListsModel>();
            foreach (EmailList emailList in list)
            {
                result.Add(GetEmailListsModel(emailList));
            }
            return result;
        }
    }
}