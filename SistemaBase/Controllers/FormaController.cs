using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaBase.Filters;
using SistemaBase.Models;

namespace SistemaBase.Controllers
{
    [Authorize]
    [AccesoPantalla("FORMA")]
    public class FormaController : Controller
    {
        private readonly UAADbContext _context;

        public FormaController(UAADbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var formas = _context.Formas.AsNoTracking().Include(f => f.IdModuloNavigation).OrderBy(f => f.IdModulo).ThenBy(f => f.NomForma);
            return View(await formas.ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            await CargarCombos();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Forma forma)
        {
            LimpiarValidaciones();
            if (!ModelState.IsValid)
            {
                await CargarCombos(forma.IdModulo);
                return View(forma);
            }

            _context.Formas.Add(forma);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string idModulo, string nomForma)
        {
            var forma = await _context.Formas.FindAsync(idModulo, nomForma);
            if (forma == null) return NotFound();
            await CargarCombos(forma.IdModulo);
            return View(forma);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string idModulo, string nomForma, Forma forma)
        {
            LimpiarValidaciones();
            if (idModulo != forma.IdModulo || nomForma != forma.NomForma) return NotFound();
            if (!ModelState.IsValid)
            {
                await CargarCombos(forma.IdModulo);
                return View(forma);
            }

            _context.Update(forma);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string idModulo, string nomForma)
        {
            var forma = await _context.Formas.AsNoTracking().FirstOrDefaultAsync(f => f.IdModulo == idModulo && f.NomForma == nomForma);
            return forma == null ? NotFound() : View(forma);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string idModulo, string nomForma)
        {
            var forma = await _context.Formas.FindAsync(idModulo, nomForma);
            if (forma != null)
            {
                try
                {
                    _context.Formas.Remove(forma);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    TempData["Error"] = "No se puede eliminar el formulario porque tiene accesos relacionados.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarCombos(string? idModulo = null)
        {
            ViewData["IdModulo"] = new SelectList(await _context.Modulos.AsNoTracking().OrderBy(m => m.IdModulo).ToListAsync(), "IdModulo", "IdModulo", idModulo);
        }

        private void LimpiarValidaciones()
        {
            ModelState.Remove(nameof(Forma.IdModuloNavigation));
            ModelState.Remove(nameof(Forma.IdGrupos));
        }
    }
}
