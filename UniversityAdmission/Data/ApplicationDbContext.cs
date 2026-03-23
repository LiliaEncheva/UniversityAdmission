using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniversityAdmission.Models.Entities;
using UniversityAdmission.Models.Identity;
using UniversityAdmission.Models.Enums;

namespace UniversityAdmission.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Speciality> Specialities { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationSpeciality> ApplicationSpecialities { get; set; }
        public DbSet<AdmissionResult> AdmissionResults { get; set; }
        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ✅ Grades
            builder.Entity<Grade>(entity =>
            {
                entity.Property(g => g.Diploma).HasPrecision(4, 2);
                entity.Property(g => g.BEL).HasPrecision(4, 2);
                entity.Property(g => g.Math).HasPrecision(4, 2);
                entity.Property(g => g.English).HasPrecision(4, 2);
                entity.Property(g => g.TotalScore).HasPrecision(5, 2);
            });

            // Ако искаш TotalScore като компютед колонка, можеш да го оставиш, но за безопасност го изчислявай в C#:
            // builder.Entity<Grade>()
            //     .Property(g => g.TotalScore)
            //     .HasComputedColumnSql("[Diploma] * 0.3 + [BEL] * 0.3 + [Math] * 0.2 + [English] * 0.2");

            // ✅ Applications
            builder.Entity<Application>(entity =>
            {
                entity.Property(a => a.TotalScore).HasPrecision(5, 2);

                // Map enum към int
                entity.Property(a => a.Status)
                      .HasConversion<int>()
                      .IsRequired();

                entity.Property(a => a.IsConfirmed).HasDefaultValue(false);
            });

            // 🔹 Relations
            builder.Entity<ApplicationSpeciality>()
                .HasOne(a => a.Application)
                .WithMany(a => a.ApplicationSpecialities)
                .HasForeignKey(a => a.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationSpeciality>()
                 .HasOne(a => a.Speciality)
                 .WithMany(s => s.ApplicationSpecialities) 
                 .HasForeignKey(a => a.SpecialityId)
                 .OnDelete(DeleteBehavior.Restrict);
        }
    }
}