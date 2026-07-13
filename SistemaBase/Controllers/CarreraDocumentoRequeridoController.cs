using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaBase.Filters;
using SistemaBase.Models;

namespace SistemaBase.Controllers
{
    [Authorize]
    [AccesoPantalla("DOCCARR")]
    public class CarreraDocumentoRequeridoController : Controller
    {
        private readonly UAADbContext _context;

        public CarreraDocumentoRequeridoController(UAADbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var documentos = _context.CarreraDocumentoRequeridos
                .AsNoTracking()
                .Include(c => c.IdCarreraNavigation)
                .Include(c => c.IdTipoDocumentoNavigation)
                .OrderBy(c => c.IdCarreraNavigation.Descripcion)
                .ThenBy(c => c.IdTipoDocumentoNavigation.Nombre);

            return View(await documentos.ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            await CargarCombos();
            return View(new CarreraDocumentoRequerido { Activo = "S", Obligatorio = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarreraDocumentoRequerido carreraDocumentoRequerido)
        {
            LimpiarValidacionesDeNavegacion();

            if (!ModelState.IsValid)
            {
                await CargarCombos(carreraDocumentoRequerido.IdCarrera, carreraDocumentoRequerido.IdTipoDocumento);
                return View(carreraDocumentoRequerido);
            }

            carreraDocumentoRequerido.Activo ??= "S";
            carreraDocumentoRequerido.Obligatorio ??= false;

            try
            {
                _context.CarreraDocumentoRequeridos.Add(carreraDocumentoRequerido);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Ya existe este tipo de documento para la carrera seleccionada.");
                await CargarCombos(carreraDocumentoRequerido.IdCarrera, carreraDocumentoRequerido.IdTipoDocumento);
                return View(carreraDocumentoRequerido);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.CarreraDocumentoRequeridos.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await CargarCombos(item.IdCarrera, item.IdTipoDocumento);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarreraDocumentoRequerido carreraDocumentoRequerido)
        {
            LimpiarValidacionesDeNavegacion();

            if (id != carreraDocumentoRequerido.IdCarreraDocumentoRequerido)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await CargarCombos(carreraDocumentoRequerido.IdCarrera, carreraDocumentoRequerido.IdTipoDocumento);
                return View(carreraDocumentoRequerido);
            }

            carreraDocumentoRequerido.Obligatorio ??= false;
            _context.Update(carreraDocumentoRequerido);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.CarreraDocumentoRequeridos
                .AsNoTracking()
                .Include(c => c.IdCarreraNavigation)
                .Include(c => c.IdTipoDocumentoNavigation)
                .FirstOrDefaultAsync(c => c.IdCarreraDocumentoRequerido == id);

            return item == null ? NotFound() : View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.CarreraDocumentoRequeridos.FindAsync(id);
            if (item != null)
            {
                item.Activo = "N";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarCombos(int? idCarrera = null, int? idTipoDocumento = null)
        {
            ViewData["IdCarrera"] = new SelectList(await _context.Carreras.AsNoTracking().OrderBy(c => c.Descripcion).ToListAsync(), "IdCarrera", "Descripcion", idCarrera);
            ViewData["IdTipoDocumento"] = new SelectList(await _context.TipoDocumentos.AsNoTracking().Where(t => t.Activo == null || t.Activo == "S").OrderBy(t => t.Nombre).ToListAsync(), "IdTipoDocumento", "Nombre", idTipoDocumento);
        }

        private void LimpiarValidacionesDeNavegacion()
        {
            ModelState.Remove(nameof(CarreraDocumentoRequerido.IdCarreraNavigation));
            ModelState.Remove(nameof(CarreraDocumentoRequerido.IdTipoDocumentoNavigation));
        }
    }
}
