using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaBase.Filters;
using SistemaBase.Models;

namespace SistemaBase.Controllers
{
    [Authorize]
    [AccesoPantalla("TIPEXT")]
    public class TipoExtensionController : Controller
    {
        private readonly UAADbContext _context;

        public TipoExtensionController(UAADbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.TipoExtensions.AsNoTracking().OrderBy(t => t.Extension).ToListAsync());
        }

        public IActionResult Create()
        {
            return View(new TipoExtension { Activo = "S" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoExtension tipoExtension)
        {
            Normalizar(tipoExtension);
            if (!ModelState.IsValid) return View(tipoExtension);

            _context.TipoExtensions.Add(tipoExtension);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var tipoExtension = await _context.TipoExtensions.FindAsync(id);
            return tipoExtension == null ? NotFound() : View(tipoExtension);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoExtension tipoExtension)
        {
            if (id != tipoExtension.IdTipoExtension) return NotFound();

            Normalizar(tipoExtension);
            if (!ModelState.IsValid) return View(tipoExtension);

            _context.Update(tipoExtension);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var tipoExtension = await _context.TipoExtensions.AsNoTracking().FirstOrDefaultAsync(t => t.IdTipoExtension == id);
            return tipoExtension == null ? NotFound() : View(tipoExtension);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoExtension = await _context.TipoExtensions.FindAsync(id);
            if (tipoExtension != null)
            {
                tipoExtension.Activo = "N";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private static void Normalizar(TipoExtension tipoExtension)
        {
            var extension = tipoExtension.Extension?.Trim().ToLowerInvariant() ?? string.Empty;
            tipoExtension.Extension = extension.StartsWith('.') ? extension : $".{extension}";
            tipoExtension.Activo = string.IsNullOrWhiteSpace(tipoExtension.Activo) ? "S" : tipoExtension.Activo;
        }
    }
}
