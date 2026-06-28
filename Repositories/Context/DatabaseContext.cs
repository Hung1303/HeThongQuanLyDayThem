using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Context
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<CenterProfile> CenterProfiles { get; set; }
        public DbSet<TeacherProfile> TeacherProfiles { get; set; }
        public DbSet<Course> Courses { get; set; }
        protected DatabaseContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(b =>
            {
                b.HasIndex(x => x.Username).IsUnique();
                b.HasIndex(x => x.Email).IsUnique();
                b.Property(x => x.Username).HasMaxLength(256).IsRequired();
                b.Property(x => x.Email).HasMaxLength(256).IsRequired();
                b.Property(x => x.Fullname).HasMaxLength(256).IsRequired();
                
            });

            modelBuilder.Entity<CenterProfile>(b =>
            {
                b.HasIndex(x => x.UserId).IsUnique();
                b.Property(x => x.CenterName).HasMaxLength(256).IsRequired();
                b.Property(x => x.Address).HasMaxLength(256);
                b.Property(x => x.ContactPhoneNumber).HasMaxLength(256).IsRequired();
                b.Property(x => x.ContactEmail).HasMaxLength(256).IsRequired();
            });

            modelBuilder.Entity<TeacherProfile>(b =>
            {
               
            });

            modelBuilder.Entity<Course>(b =>
            {
                b.Property(x => x.ClassName).HasMaxLength(256).IsRequired();
                b.Property(x => x.Subject).HasMaxLength(256).IsRequired();
                b.Property(x => x.TuitionFee).HasPrecision(18,2);
            });
        }
    }
}
