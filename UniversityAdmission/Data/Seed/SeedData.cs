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

            // 1. Cleanup "Testov" middle names and seed roles/admin if needed
            var usersToFix = await context.Users.Where(u => u.MiddleName == "Testov").ToListAsync();
            foreach (var user in usersToFix)
            {
                user.MiddleName = "Георгиев"; // Заменяме с често срещано име
            }
            if (usersToFix.Any()) await context.SaveChangesAsync();

            // 2. Seed специалности (ако няма)
            if (!await context.Specialities.AnyAsync())
            {
                var specs = new[]
                {
                    new Speciality { Name = "Компютърни науки", Seats = 2 },
                    new Speciality { Name = "Софтуерно инженерство", Seats = 3 },
                    new Speciality { Name = "Информационни системи", Seats = 2 }
                };
                context.Specialities.AddRange(specs);
                await context.SaveChangesAsync();
            }

            var specialities = await context.Specialities.ToListAsync();

            // 3. Seed разнообразни студенти и кандидатури
            var testStudents = new[]
            {
                new { Email = "ivan@uni.bg", First = "Иван", Middle = "Георгиев", Last = "Иванов", Score = 5.80m, Prefs = new[] {0, 1, 2} },
                new { Email = "maria@uni.bg", First = "Мария", Middle = "Ангелова", Last = "Петрова", Score = 5.95m, Prefs = new[] {0, 2} },
                new { Email = "georgi@uni.bg", First = "Георги", Middle = "Борисов", Last = "Димитров", Score = 5.50m, Prefs = new[] {1, 0, 2} },
                new { Email = "elena@uni.bg", First = "Елена", Middle = "Тодорова", Last = "Колева", Score = 5.20m, Prefs = new[] {0, 1} },
                new { Email = "stefan@uni.bg", First = "Стефан", Middle = "Василев", Last = "Стойнов", Score = 4.80m, Prefs = new[] {2, 0} },
                new { Email = "anna@uni.bg", First = "Анна", Middle = "Маринова", Last = "Павлова", Score = 3.50m, Prefs = new[] {0} },
                new { Email = "dimitar@uni.bg", First = "Димитър", Middle = "Николов", Last = "Колев", Score = 5.75m, Prefs = new[] {0, 1} },
            };

            foreach (var s in testStudents)
            {
                var user = await userManager.FindByEmailAsync(s.Email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = s.Email,
                        Email = s.Email,
                        FirstName = s.First,
                        MiddleName = s.Middle,
                        LastName = s.Last,
                        School = "ПМГ",
                        City = "София",
                        Address = "ул. Примерна 123",
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(user, "Password123!");
                }

                // Кандидатура
                if (!await context.Applications.AnyAsync(a => a.UserId == user.Id))
                {
                    var app = new Application
                    {
                        UserId = user.Id,
                        TotalScore = s.Score,
                        CreatedOn = DateTime.Now,
                        Status = ApplicationStatus.NotAccepted,
                        IsConfirmed = false
                    };
                    context.Applications.Add(app);
                    await context.SaveChangesAsync();

                    for (int i = 0; i < s.Prefs.Length; i++)
                    {
                        context.ApplicationSpecialities.Add(new ApplicationSpeciality
                        {
                            ApplicationId = app.Id,
                            SpecialityId = specialities[s.Prefs[i]].Id,
                            PreferenceOrder = i + 1
                        });
                    }
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}