using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityAdmission.Data;
using UniversityAdmission.Models;
using UniversityAdmission.Models.Enums;
using UniversityAdmission.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        // КЛАСИРАНЕ И ПУБЛИКУВАНЕ НА РЕЗУЛТАТИТЕ
        // ===============================
        public async Task<IActionResult> Ranking(int? specialityId, int? preferenceOrder, ApplicationStatus? status, int page = 1)
        {
            const int pageSize = 10;

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

            // Стартираме класирането (за да са актуални данните)
            RunAdmission(specialities, applications);
            await _context.SaveChangesAsync();

            // Филтриране
            var query = applications.AsEnumerable(); // Сменяме на AsEnumerable, защото RunAdmission вече е заредил всичко в паметта

            if (status.HasValue)
            {
                query = query.Where(a => a.Status == status.Value);
            }

            if (specialityId.HasValue)
            {
                // Показваме тези, които са приети в тази специалност
                query = query.Where(a => a.ApplicationSpecialities.Any(s => s.IsAdmitted && s.SpecialityId == specialityId.Value));
            }

            if (preferenceOrder.HasValue)
            {
                // Показваме тези, които са приети по това желание
                query = query.Where(a => a.ApplicationSpecialities.Any(s => s.IsAdmitted && s.PreferenceOrder == preferenceOrder.Value));
            }

            var sorted = query.OrderByDescending(a => a.TotalScore).ToList();

            var totalItems = sorted.Count;
            var pagedApplications = sorted.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new RankingViewModel
            {
                Applications = pagedApplications,
                SpecialityId = specialityId,
                PreferenceOrder = preferenceOrder,
                Status = status,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                TotalItems = totalItems,
                Specialities = specialities.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name,
                    Selected = s.Id == specialityId
                }).ToList(),
                Statuses = Enum.GetValues(typeof(ApplicationStatus))
                    .Cast<ApplicationStatus>()
                    .Select(s => new SelectListItem
                    {
                        Value = s.ToString(),
                        Text = s switch
                        {
                            ApplicationStatus.Pending => "Изчакваща",
                            ApplicationStatus.NotAccepted => "Не е приет",
                            ApplicationStatus.Confirmed => "Приет",
                            ApplicationStatus.Rejected => "Отхвърлен",
                            _ => s.ToString()
                        },
                        Selected = s == status
                    }).ToList()
            };

            return View(viewModel);
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