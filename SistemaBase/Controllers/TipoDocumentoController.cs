using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaBase.Filters;
using SistemaBase.Models;

namespace SistemaBase.Controllers
{
    [Authorize]
    [AccesoPantalla("TIPODOC")]
    public class TipoDocumentoController : Controller
    {
        private readonly UAADbContext _context;

        public TipoDocumentoController(UAADbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.TipoDocumentos.AsNoTracking().OrderBy(t => t.Nombre).ToListAsync());
        }

        public IActionResult Create()
        {
            return View(new TipoDocumento { Activo = "S" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoDocumento tipoDocumento)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoDocumento);
            }

            tipoDocumento.Activo ??= "S";
            _context.TipoDocumentos.Add(tipoDocumento);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var tipoDocumento = await _context.TipoDocumentos.FindAsync(id);
            return tipoDocumento == null ? NotFound() : View(tipoDocumento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoDocumento tipoDocumento)
        {
            if (id != tipoDocumento.IdTipoDocumento)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(tipoDocumento);
            }

            _context.Update(tipoDocumento);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var tipoDocumento = await _context.TipoDocumentos.AsNoTracking().FirstOrDefaultAsync(t => t.IdTipoDocumento == id);
            return tipoDocumento == null ? NotFound() : View(tipoDocumento);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoDocumento = await _context.TipoDocumentos.FindAsync(id);
            if (tipoDocumento != null)
            {
                tipoDocumento.Activo = "N";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
