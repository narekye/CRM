namespace CRM.WebApi.InfrastructureModel
{
    using System;
    using System.Collections.Generic;
    using Entities;
    using Models.Response;
    using Converter;
    public class ModelFactory : IDisposable
    {
        private readonly CRMContext _database;
        public ModelFactory()
        {
            _database = new CRMContext();
        }
        public List<ViewContactLess> CreateViewContactLessList(List<Contact> contacts)
        {
            var data = new List<ViewContactLess>();
            contacts.ForEach(p =>
            {
                data.Add(new ViewContactLess
                {
                    CompanyName = p.CompanyName,
                    FullName = p.FullName,
                    Email = p.Email,
                    Position = p.Position,
                    GuID = p.GuID,
                    Country = p.Country
                });
            });
            return data;
        }
        public ViewTemplate GetViewTemplate(Template template)
        {
            ViewTemplate up = new ViewTemplate();
            template.ConvertTo(up);
            return up;
        }
        public List<ViewTemplate> GetViewTemplates(List<Template> templates)
        {
            if (ReferenceEquals(templates, null)) return null;
            var list = new List<ViewTemplate>();
            templates.ForEach(i => list.Add(GetViewTemplate(i)));
            return list;
        }
        public void Dispose()
        {
            _database.Dispose();
        }

    }
}