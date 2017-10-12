using System;

namespace CRM.DAL.Entities
{
    public partial class CrmEntities 
    {
        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}