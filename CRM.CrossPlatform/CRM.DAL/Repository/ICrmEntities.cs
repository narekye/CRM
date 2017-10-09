using System;
using CRM.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.DAL.Repository
{
    public interface ICrmEntities : IDisposable
    {
        DbSet<Contacts> Contacts { get; set; }
        DbSet<EmailListContacts> EmailListContacts { get; set; }
        DbSet<EmailLists> EmailLists { get; set; }
        DbSet<Roles> Roles { get; set; }
        DbSet<Templates> Templates { get; set; }
        DbSet<UserRoles> UserRoles { get; set; }
        DbSet<Users> Users { get; set; }
    }
}
