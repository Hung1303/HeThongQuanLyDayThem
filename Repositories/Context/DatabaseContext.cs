using BusinessObjects;
using Core.Base;
using Microsoft.EntityFrameworkCore;

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
                b.Property(x => x.TuitionFee).HasPrecision(18, 2);
            });

            var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    Email = "tuanhung01032003@gmail.com",
                    Password = "123456",
                    Fullname = "Đỗ Tuấn Hùng",
                    PhoneNumber = "0932760162",
                    DateOfBirth = new DateTime(2003, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                    Gender = Gender.Male,
                    UserRole = Role.Admin,
                    AccountStatus = AccountStatus.Active,
                    Username = "tuanhung0103"
                }
            );
        }
    }
}
