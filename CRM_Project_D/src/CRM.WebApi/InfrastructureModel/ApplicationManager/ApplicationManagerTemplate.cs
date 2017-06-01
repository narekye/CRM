namespace CRM.WebApi.InfrastructureModel.ApplicationManager
{
    using System.Data.Entity;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Entities;
    public partial class ApplicationManager
    {
        public async Task<List<Template>> GetAllTemplatesListAsync()
        {
            return await _database.Templates.ToListAsync();
        }
        public async Task<Template> GetTemplateByIdAsync(int? id)
        {
            if (!id.HasValue) return null;
            return await _database.Templates.FirstOrDefaultAsync(p => p.TemplateId == id.Value);
        }
    }
}