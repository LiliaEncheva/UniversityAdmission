using Microsoft.EntityFrameworkCore;
using UniversityAdmission.Data;
using UniversityAdmission.Models.Entities;

namespace UniversityAdmission.Data.Seed
{
    public static class SeedSpecialities
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.Specialities.AnyAsync())
                return;

            var specialities = new List<Speciality>
            {
                new Speciality { Name = "Английски език и професионална комуникация", Seats = 50 },
                new Speciality { Name = "Анимационно кино", Seats = 25 },
                new Speciality { Name = "Антропология", Seats = 20 },
                new Speciality { Name = "Бизнес икономика - Маркетинг", Seats = 40 },
                new Speciality { Name = "Биология - Микробиология", Seats = 25 }
            };

            context.Specialities.AddRange(specialities);
            await context.SaveChangesAsync();
        }
    }
}