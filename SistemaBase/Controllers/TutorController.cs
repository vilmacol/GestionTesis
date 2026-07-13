using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaBase.Filters;
using SistemaBase.Models;

namespace SistemaBase.Controllers
{
    [Authorize]
    [AccesoPantalla("TUTOR")]
    public class TutorController : Controller
    {
        private readonly UAADbContext _context;

        public TutorController(UAADbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Tutors.AsNoTracking().OrderBy(t => t.NombreCompleto).ToListAsync());
        }

        public IActionResult Create()
        {
            return View(new Tutor { Activo = "S", FechaRegistro = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tutor tutor)
        {
            if (!ModelState.IsValid)
            {
                return View(tutor);
            }

            tutor.FechaRegistro = DateTime.Now;
            tutor.Activo ??= "S";
            _context.Tutors.Add(tutor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var tutor = await _context.Tutors.FindAsync(id);
            return tutor == null ? NotFound() : View(tutor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tutor tutor)
        {
            if (id != tutor.IdTutor)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(tutor);
            }

            _context.Update(tutor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var tutor = await _context.Tutors.AsNoTracking().FirstOrDefaultAsync(t => t.IdTutor == id);
            return tutor == null ? NotFound() : View(tutor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tutor = await _context.Tutors.FindAsync(id);
            if (tutor != null)
            {
                tutor.Activo = "N";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
