using System;
using CRM.DAL.Repository;

namespace CRM.BLL.Core
{
    public class CoreBL : IDisposable
    {
        public CoreBL(ICrmEntities repository)
        {
            Repository = repository;
        }

        protected virtual ICrmEntities Repository { get; }

        public void Dispose()
        {
            Repository?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}