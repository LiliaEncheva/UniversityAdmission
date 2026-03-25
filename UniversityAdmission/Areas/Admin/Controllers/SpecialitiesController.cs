using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UniversityAdmission.Data;
using UniversityAdmission.Models.Entities;
using UniversityAdmission.Areas.Admin.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace UniversityAdmission.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SpecialitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SpecialitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Specialities
        public async Task<IActionResult> Index()
        {
            var list = await _context.Specialities
                                     .Select(s => new SpecialityViewModel
                                     {
                                         Id = s.Id,
                                         Name = s.Name,
                                         Seats = s.Seats
                                     }).ToListAsync();
            return View(list);
        }

        // GET: Admin/Specialities/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialityViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Specialities.AnyAsync(s => s.Name == model.Name))
                {
                    ModelState.AddModelError("Name", "Специалност с това име вече съществува.");
                    return View(model);
                }

                var entity = new Speciality
                {
                    Name = model.Name,
                    Seats = model.Seats
                };
                _context.Add(entity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Admin/Specialities/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _context.Specialities.FindAsync(id);
            if (entity == null) return NotFound();

            var model = new SpecialityViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Seats = entity.Seats
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SpecialityViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (await _context.Specialities.AnyAsync(s => s.Name == model.Name && s.Id != id))
                {
                    ModelState.AddModelError("Name", "Специалност с това име вече съществува.");
                    return View(model);
                }

                var entity = await _context.Specialities.FindAsync(id);
                if (entity == null) return NotFound();

                entity.Name = model.Name;
                entity.Seats = model.Seats;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Admin/Specialities/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Specialities.FindAsync(id);
            if (entity == null) return NotFound();

            var model = new SpecialityViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Seats = entity.Seats
            };
            return View(model);
        }

        // POST: Admin/Specialities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _context.Specialities.FindAsync(id);
            if (entity != null)
            {
                _context.Specialities.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
