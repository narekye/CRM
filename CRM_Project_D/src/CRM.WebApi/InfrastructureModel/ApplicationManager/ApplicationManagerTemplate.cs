namespace CRM.WebApi.InfrastructureModel.ApplicationManager
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Entities;
    public partial class ApplicationManager
    {
        public async Task<List<Template>> GetAllTemplatesListAsync()
        {
            try
            {
                return await _database.Templates.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<Template> GetTemplateByIdAsync(int? id)
        {
            if (!id.HasValue) return null;
            try
            {
                return await _database.Templates.FirstOrDefaultAsync(p => p.TemplateId == id.Value);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}