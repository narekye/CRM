using System;
using CRM.DAL.Repository;

namespace CRM.DAL.Entities
{
    public partial class CrmEntities : ICrmEntities
    {
        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}