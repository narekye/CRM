using Microsoft.EntityFrameworkCore;

namespace CRM.DAL.Entities
{
    public partial class CrmEntities : DbContext
    {
        public virtual DbSet<Contacts> Contacts { get; set; }
        public virtual DbSet<EmailListContacts> EmailListContacts { get; set; }
        public virtual DbSet<EmailLists> EmailLists { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Templates> Templates { get; set; }
        public virtual DbSet<UserRoles> UserRoles { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(@"Server=tcp:bet.database.windows.net\;Database=crmD;User Id=betadmin;Password=betConstract$$;Trusted_Connection=False;Encrypt=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contacts>(entity =>
            {
                entity.HasKey(e => e.ContactId);

                entity.HasIndex(e => e.Email)
                    .HasName("UQ__Contacts__A9D10534410F33D7")
                    .IsUnique();

                entity.Property(e => e.CompanyName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.DateInserted).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.GuId)
                    .HasColumnName("GuID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Position)
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EmailListContacts>(entity =>
            {
                entity.HasKey(e => new { e.EmailListId, e.ContactId });

                entity.HasIndex(e => e.ContactId)
                    .HasName("IX_ContactID");

                entity.HasIndex(e => e.EmailListId)
                    .HasName("IX_EmailListID");

                entity.Property(e => e.EmailListId).HasColumnName("EmailListID");

                entity.Property(e => e.ContactId).HasColumnName("ContactID");

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.EmailListContacts)
                    .HasForeignKey(d => d.ContactId)
                    .HasConstraintName("FK_dbo.EmailListContacts_dbo.Contacts_Contact_Id");

                entity.HasOne(d => d.EmailList)
                    .WithMany(p => p.EmailListContacts)
                    .HasForeignKey(d => d.EmailListId)
                    .HasConstraintName("FK_dbo.EmailListContacts_dbo.EmailLists_EmailList_ID");
            });

            modelBuilder.Entity<EmailLists>(entity =>
            {
                entity.HasKey(e => e.EmailListId);

                entity.Property(e => e.EmailListId).HasColumnName("EmailListID");

                entity.Property(e => e.EmailListName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("RoleNameIndex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(128)
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<Templates>(entity =>
            {
                entity.HasKey(e => e.TemplateId);

                entity.Property(e => e.TemplateName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.TemplatePath).IsRequired();
            });

            modelBuilder.Entity<UserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId)
                    .HasName("IX_RoleId");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_UserId");

                entity.Property(e => e.UserId).HasMaxLength(128);

                entity.Property(e => e.RoleId).HasMaxLength(128);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasIndex(e => e.UserName)
                    .HasName("UserNameIndex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(128)
                    .ValueGeneratedNever();

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(256);
            });
        }
    }
}
