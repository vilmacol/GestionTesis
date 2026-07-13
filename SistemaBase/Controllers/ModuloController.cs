using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaBase.Filters;
using SistemaBase.Models;

namespace SistemaBase.Controllers
{
    [Authorize]
    [AccesoPantalla("MODULO")]
    public class ModuloController : Controller
    {
        private readonly UAADbContext _context;

        public ModuloController(UAADbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Modulos.AsNoTracking().OrderBy(m => m.IdModulo).ToListAsync());
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Modulo modulo)
        {
            if (!ModelState.IsValid) return View(modulo);

            _context.Modulos.Add(modulo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var modulo = await _context.Modulos.FindAsync(id);
            return modulo == null ? NotFound() : View(modulo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Modulo modulo)
        {
            if (id != modulo.IdModulo) return NotFound();
            if (!ModelState.IsValid) return View(modulo);

            _context.Update(modulo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            var modulo = await _context.Modulos.AsNoTracking().FirstOrDefaultAsync(m => m.IdModulo == id);
            return modulo == null ? NotFound() : View(modulo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var modulo = await _context.Modulos.FindAsync(id);
            if (modulo != null)
            {
                try
                {
                    _context.Modulos.Remove(modulo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    TempData["Error"] = "No se puede eliminar el modulo porque tiene formularios relacionados.";
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
