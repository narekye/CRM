namespace CRM.WebApi.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Entities;

    using AutoMapper;
    public class EmailListsModel
    {
        static EmailListsModel()
        {
            Mapper.Initialize(f => f.CreateMap<EmailList, EmailListsModel>()
            .ForMember("EmailListId", p => p.MapFrom(z => z.EmailListID))
            .ForMember("EmailListName", p => p.MapFrom(z => z.EmailListName))
            .ForMember("Contacts", p => p.MapFrom(z => z.Contacts.Select(t => t.FullName))));
        }
        public int EmailListId { get; set; }
        public string EmailListName { get; set; }

        public List<string> Contacts { get; set; }

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