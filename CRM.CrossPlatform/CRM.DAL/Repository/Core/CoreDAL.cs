using System;
using CRM.DAL.Entities;

namespace CRM.DAL.Repository.Core
{
    public abstract class CoreDAL
    {
        private readonly BaseDAL _Dal;
        protected BaseDAL DAL => _Dal;
        protected CrmEntities db => _Dal.Database;
        protected CoreDAL(BaseDAL dal)
        {
            _Dal = dal ?? throw new ArgumentException(nameof(dal));
        }
    }
}
