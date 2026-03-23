using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversityAdmission.Data;
using UniversityAdmission.Models.Entities;
using UniversityAdmission.Models.Enums;
using UniversityAdmission.Models.Identity;

namespace UniversityAdmission.Data.Seed
{
    public static class SeedData
    {
        public static async Task Initialize(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            // Миграции
            await context.Database.MigrateAsync();

            // 1. Seed специалности
            if (!await context.Specialities.AnyAsync())
            {
                var specs = new[]
                {
                    new Speciality { Name = "Компютърни науки", Seats = 50 },
                    new Speciality { Name = "Математика", Seats = 30 },
                    new Speciality { Name = "Физика", Seats = 25 }
                };
                context.Specialities.AddRange(specs);
                await context.SaveChangesAsync();
            }

            // 2. Seed студент
            if (!await context.Users.AnyAsync(u => u.UserName == "student1@uni.bg"))
            {
                var user = new ApplicationUser
                {
                    UserName = "student1@uni.bg",
                    Email = "student1@uni.bg",
                    FirstName = "Ivan",
                    MiddleName = "Ivanov",
                    LastName = "Ivanov",
                    School = "SMG",
                    City = "Sofia",
                    Address = "Sofia 1000",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, "Password123!");
            }

            var student = await context.Users
                .FirstOrDefaultAsync(u => u.UserName == "student1@uni.bg");

            // 3. Seed кандидатура
            if (!await context.Applications.AnyAsync())
            {
                var app = new Application
                {
                    UserId = student.Id,
                    TotalScore = 5.5m,
                    CreatedOn = DateTime.Now,
                    Status = ApplicationStatus.Confirmed,
                    IsConfirmed = false
                };
                context.Applications.Add(app);
                await context.SaveChangesAsync();

                // Добавяне на първо желание
                var firstSpec = await context.Specialities.FirstAsync();
                context.ApplicationSpecialities.Add(new ApplicationSpeciality
                {
                    ApplicationId = app.Id,
                    SpecialityId = firstSpec.Id,
                    PreferenceOrder = 1
                });

                await context.SaveChangesAsync();
            }
        }
    }
}