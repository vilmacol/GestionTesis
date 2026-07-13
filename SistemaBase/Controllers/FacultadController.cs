using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaBase.Filters;
using SistemaBase.Models;

namespace SistemaBase.Controllers
{
    [Authorize]
    [AccesoPantalla("FACULTAD")]
    public class FacultadController : Controller
    {
        private readonly UAADbContext _context;

        public FacultadController(UAADbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Facultads.AsNoTracking().OrderBy(f => f.Codigo).ToListAsync());
        }

        public IActionResult Create()
        {
            return View(new Facultad { Activo = "S" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Facultad facultad)
        {
            ModelState.Remove(nameof(Facultad.Carreras));
            ModelState.Remove(nameof(Facultad.GruposUsuarios));
            Normalizar(facultad);
            if (!ModelState.IsValid) return View(facultad);

            _context.Facultads.Add(facultad);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var facultad = await _context.Facultads.FindAsync(id);
            return facultad == null ? NotFound() : View(facultad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Facultad facultad)
        {
            ModelState.Remove(nameof(Facultad.Carreras));
            ModelState.Remove(nameof(Facultad.GruposUsuarios));
            if (id != facultad.IdFacultad) return NotFound();
            Normalizar(facultad);
            if (!ModelState.IsValid) return View(facultad);

            _context.Update(facultad);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var facultad = await _context.Facultads.AsNoTracking().FirstOrDefaultAsync(f => f.IdFacultad == id);
            return facultad == null ? NotFound() : View(facultad);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facultad = await _context.Facultads.FindAsync(id);
            if (facultad != null)
            {
                facultad.Activo = "N";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private static void Normalizar(Facultad facultad)
        {
            facultad.Codigo = (facultad.Codigo ?? string.Empty).Trim().ToUpperInvariant();
            facultad.Nombre = (facultad.Nombre ?? string.Empty).Trim();
            facultad.Activo = string.IsNullOrWhiteSpace(facultad.Activo) ? "S" : facultad.Activo.Trim().ToUpperInvariant();
        }
    }
}
