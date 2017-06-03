namespace CRM.WebApi.InfrastructureModel.ApplicationManager
{
    using System.Data.Entity;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Models.Response;
    using Entities;

    public partial class ApplicationManager
    {
        public async Task<List<ViewTemplate>> GetAllTemplatesListAsync()
        {
            var data = await _database.Templates.ToListAsync();
            var list = new List<ViewTemplate>();
            foreach (Template template in data)
                list.Add(await GetTemplateByIdAsync(template.TemplateId));
            return list;
        }
        public async Task<ViewTemplate> GetTemplateByIdAsync(int? id)
        {
            if (!id.HasValue) return null;
            var data = await _database.Templates.FirstOrDefaultAsync(p => p.TemplateId == id.Value);
            if (ReferenceEquals(data, null)) return null;
            var viewtemplate = new ViewTemplate();
            AutoMapper.Mapper.Map(data, viewtemplate);
            return viewtemplate;
        }
    }
}