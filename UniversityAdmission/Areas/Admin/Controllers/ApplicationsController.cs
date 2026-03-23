using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityAdmission.Data;
using UniversityAdmission.Models.Enums;
using UniversityAdmission.Models.Entities;

namespace UniversityAdmission.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Фиксирана дата за край на кандидатурите
        private readonly DateTime ApplicationDeadline = new DateTime(2026, 06, 01); // <- смени според нуждата

        public ApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===============================
        // СПИСЪК С КАНДИДАТУРИ
        // ===============================
        public async Task<IActionResult> Index()
        {
            // Проверка за изтекъл срок
            if (DateTime.Now > ApplicationDeadline)
            {
                return RedirectToAction("Ranking");
            }

            var applications = await _context.Applications
                .Include(a => a.User)
                .Include(a => a.ApplicationSpecialities)
                    .ThenInclude(s => s.Speciality)
                .ToListAsync();

            return View(applications);
        }

        // ===============================
        // ПОТВЪРЖДАВАНЕ НА КАНДИДАТУРА
        // ===============================
        [HttpPost]
        public async Task<IActionResult> Confirm(int id)
        {
            var application = await _context.Applications
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application != null)
            {
                application.Status = ApplicationStatus.Confirmed; // Приет
                application.IsConfirmed = true; // за студента

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // КЛАСИРАНЕ И ПУБЛИКУВАНЕ НА РЕЗУЛТАТИТЕ
        // ===============================
        public async Task<IActionResult> Ranking()
        {
            // Вземаме всички кандидатури до крайния срок
            var applications = await _context.Applications
                .Include(a => a.ApplicationSpecialities)
                    .ThenInclude(s => s.Speciality)
                .Include(a => a.User)
                .Where(a => a.CreatedOn <= ApplicationDeadline)
                .ToListAsync();

            var specialities = await _context.Specialities
                .Include(s => s.ApplicationSpecialities)
                .ToListAsync();

            // Стартираме класирането
            RunAdmission(specialities, applications);

            // Запазваме резултатите
            await _context.SaveChangesAsync();

            return View(applications); // View, което показва резултатите
        }

        // ===============================
        // МЕТОД ЗА КЛАСИРАНЕ
        // ===============================
        private void RunAdmission(List<Speciality> specialities, List<Application> applications)
        {
            // Сортиране по успех (низходящо)
            var sortedApplications = applications
                .OrderByDescending(a => a.TotalScore)
                .ToList();

            // Държим текущ брой приети за всяка специалност
            var specialityAcceptedCount = specialities.ToDictionary(s => s.Id, s => 0);

            foreach (var app in sortedApplications)
            {
                // По подразбиране – не е приет
                app.Status = ApplicationStatus.NotAccepted;
                app.IsConfirmed = false;

                // Зануляваме старите резултати от предишни класирания,
                // за да гарантираме точни данни при повторно стартиране на алгоритъма.
                foreach (var c in app.ApplicationSpecialities)
                {
                    c.IsAdmitted = false;
                }

                // Минаваме по желанията (по ред на предпочитание)
                foreach (var choice in app.ApplicationSpecialities
                                           .OrderBy(c => c.PreferenceOrder))
                {
                    var speciality = specialities.First(s => s.Id == choice.SpecialityId);

                    // Проверка дали има свободни места
                    if (specialityAcceptedCount[speciality.Id] < speciality.Seats)
                    {
                        // Приемаме кандидата
                        app.Status = ApplicationStatus.Confirmed;
                        app.IsConfirmed = true;
                        choice.IsAdmitted = true;

                        // Увеличаваме броя приети в тази специалност
                        specialityAcceptedCount[speciality.Id]++;

                        break; // ВАЖНО: само в една специалност
                    }
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> PublishResults()
        {
            // Вземаме всички кандидатури до крайния срок
            var applications = await _context.Applications
                .Include(a => a.ApplicationSpecialities)
                    .ThenInclude(s => s.Speciality)
                .Include(a => a.User)
                .Where(a => a.CreatedOn <= ApplicationDeadline)
                .ToListAsync();

            var specialities = await _context.Specialities
                .Include(s => s.ApplicationSpecialities)
                .ToListAsync();

            // Стартираме класирането
            RunAdmission(specialities, applications);

            // Запазваме резултатите
            await _context.SaveChangesAsync();

            TempData["Success"] = "Резултатите бяха публикувани успешно!";

            return RedirectToAction("Ranking");
        }
    }
}