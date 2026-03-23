using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversityAdmission.Areas.User.ViewModels;
using UniversityAdmission.Data;
using UniversityAdmission.Models.Entities;
using UniversityAdmission.Models.Enums;
using UniversityAdmission.Models.Identity;

namespace UniversityAdmission.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ===============================
        // ФОРМА ЗА КАНДИДАТСТВАНЕ
        // ===============================
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            // Ако вече има кандидатура
            if (await _context.Applications.AnyAsync(a => a.UserId == userId))
                return RedirectToAction(nameof(My));

            var grades = await _context.Grades.FirstOrDefaultAsync(g => g.UserId == userId);

            if (grades == null)
            {
                TempData["Error"] = "Първо въведете оценки.";
                return RedirectToAction("Create", "Grades");
            }

            // изчисляване на бал
            var totalScore = grades.Diploma * 0.3m +
                             grades.BEL * 0.3m +
                             grades.Math * 0.2m +
                             grades.English * 0.2m;

            var model = new ApplicationCreateViewModel
            {
                TotalScore = totalScore,
                Specialities = await _context.Specialities
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    }).ToListAsync()
            };

            return View(model);
        }

        // ===============================
        // ЗАПИС НА КАНДИДАТУРА
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ApplicationCreateViewModel model)
        {
            var userId = _userManager.GetUserId(User);

            if (!ModelState.IsValid)
            {
                model.Specialities = await _context.Specialities
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    }).ToListAsync();

                return View(model);
            }

            // Проверка за дублиране
            var choices = new List<int>();

            if (model.FirstChoiceId.HasValue)
                choices.Add(model.FirstChoiceId.Value);

            if (model.SecondChoiceId.HasValue)
                choices.Add(model.SecondChoiceId.Value);

            if (model.ThirdChoiceId.HasValue)
                choices.Add(model.ThirdChoiceId.Value);

            if (choices.Count != choices.Distinct().Count())
            {
                ModelState.AddModelError("", "Не можете да изберете една и съща специалност повече от веднъж.");

                model.Specialities = await _context.Specialities
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    }).ToListAsync();

                return View(model);
            }

            var grades = await _context.Grades.FirstAsync(g => g.UserId == userId);

            var totalScore = grades.Diploma * 0.3m +
                             grades.BEL * 0.3m +
                             grades.Math * 0.2m +
                             grades.English * 0.2m;

            // създаване на кандидатура
            var application = new Application
            {
                UserId = userId,
                TotalScore = totalScore,
                CreatedOn = DateTime.Now,
                Status = ApplicationStatus.NotAccepted,
                IsConfirmed = false
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            // запис на желанията
            int order = 1;

            if (model.FirstChoiceId.HasValue)
            {
                _context.ApplicationSpecialities.Add(new ApplicationSpeciality
                {
                    ApplicationId = application.Id,
                    SpecialityId = model.FirstChoiceId.Value,
                    PreferenceOrder = order++
                });
            }

            if (model.SecondChoiceId.HasValue)
            {
                _context.ApplicationSpecialities.Add(new ApplicationSpeciality
                {
                    ApplicationId = application.Id,
                    SpecialityId = model.SecondChoiceId.Value,
                    PreferenceOrder = order++
                });
            }

            if (model.ThirdChoiceId.HasValue)
            {
                _context.ApplicationSpecialities.Add(new ApplicationSpeciality
                {
                    ApplicationId = application.Id,
                    SpecialityId = model.ThirdChoiceId.Value,
                    PreferenceOrder = order++
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(My));
        }

        // ===============================
        // МОИТЕ КАНДИДАТУРИ
        // ===============================
        public async Task<IActionResult> My()
        {
            var userId = _userManager.GetUserId(User);

            var applications = await _context.Applications
                .Include(a => a.ApplicationSpecialities)
                .ThenInclude(s => s.Speciality)
                .Where(a => a.UserId == userId)
                .ToListAsync();

            return View(applications);
        }
    }
}