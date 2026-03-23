using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UniversityAdmission.Data;
using UniversityAdmission.Models.Entities;

namespace UniversityAdmission.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class GradesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GradesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: User/Grades/Create
        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var grade = await _context.Grades
                .FirstOrDefaultAsync(g => g.UserId == userId);

            if (grade != null)
            {
                return View(grade); // Edit mode
            }

            return View(new Grade() { UserId = userId }); // Create mode
        }

        // POST: User/Grades/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Grade model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existing = await _context.Grades
                .FirstOrDefaultAsync(g => g.UserId == userId);

            if (existing == null)
            {
                model.UserId = userId;
                model.TotalScore = CalculateTotalScore(model);
                _context.Grades.Add(model);
            }
            else
            {
                // Update existing
                existing.Diploma = model.Diploma;
                existing.BEL = model.BEL;
                existing.Math = model.Math;
                existing.English = model.English;
                existing.TotalScore = CalculateTotalScore(model);
            }

            await _context.SaveChangesAsync();

            // Redirect директно към кандидатстване
            return RedirectToAction("Index", "Applications");
        }

        // Helper: изчисляване на TotalScore
        private decimal CalculateTotalScore(Grade grade)
        {
            return (grade.Diploma * 0.3m) +
                   (grade.BEL * 0.3m) +
                   (grade.Math * 0.2m) +
                   (grade.English * 0.2m);
        }
    }
}