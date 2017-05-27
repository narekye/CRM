using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using AutoMapper;
using CRM.Entities;

namespace CRM.WebApi.Models
{
    public class ViewContactLess
    {
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public Guid GuId { get; set; }
        public DateTime DateInserted { get; set; }

        public static List<ViewContactLess> CreateViewModelLess(List<Contact> contacts)
        {
            var data = new List<ViewContactLess>();
            contacts.ForEach(p =>
            {
                data.Add(new ViewContactLess()
                {
                    CompanyName = p.CompanyName,
                    FullName = p.FullName,
                    DateInserted = p.DateInserted,
                    Email = p.Email,
                    Position = p.Position,
                    GuId = p.GuID,
                    Country = p.Country
                });
            });
            return data;
        }
    }
}