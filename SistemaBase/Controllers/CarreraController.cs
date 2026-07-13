using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaBase.Filters;
using SistemaBase.Models;

namespace SistemaBase.Controllers
{
    [Authorize]
    [AccesoPantalla("CARRERA")]
    public class CarreraController : Controller
    {
        private readonly UAADbContext _context;

        public CarreraController(UAADbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Carreras
                .AsNoTracking()
                .Include(c => c.IdFacultadNavigation)
                .Include(c => c.IdEstadoInicialTesisNavigation)
                .OrderBy(c => c.Descripcion)
                .ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            await CargarCombos();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Carrera carrera)
        {
            if (!ModelState.IsValid)
            {
                await CargarCombos(carrera.IdFacultad, carrera.IdEstadoInicialTesis);
                return View(carrera);
            }

            _context.Carreras.Add(carrera);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var carrera = await _context.Carreras.FindAsync(id);
            await CargarCombos(carrera?.IdFacultad, carrera?.IdEstadoInicialTesis);
            return carrera == null ? NotFound() : View(carrera);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Carrera carrera)
        {
            if (id != carrera.IdCarrera)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await CargarCombos(carrera.IdFacultad, carrera.IdEstadoInicialTesis);
                return View(carrera);
            }

            _context.Update(carrera);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var carrera = await _context.Carreras.AsNoTracking().FirstOrDefaultAsync(c => c.IdCarrera == id);
            return carrera == null ? NotFound() : View(carrera);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carrera = await _context.Carreras.FindAsync(id);
            if (carrera == null)
            {
                return NotFound();
            }

            try
            {
                _context.Carreras.Remove(carrera);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "No se puede eliminar la carrera porque tiene registros relacionados.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarCombos(int? idFacultad = null, int? idEstadoInicialTesis = null)
        {
            var facultades = await _context.Facultads
                .AsNoTracking()
                .Where(f => f.Activo == null || f.Activo == "S")
                .OrderBy(f => f.Codigo)
                .Select(f => new
                {
                    f.IdFacultad,
                    Descripcion = f.Codigo + " - " + f.Nombre
                })
                .ToListAsync();

            var estados = await _context.EstadoSolicituds
                .AsNoTracking()
                .Where(e => e.Activo == null || e.Activo == "S")
                .OrderBy(e => e.Nombre)
                .ToListAsync();

            ViewBag.IdFacultad = new SelectList(facultades, "IdFacultad", "Descripcion", idFacultad);
            ViewBag.IdEstadoInicialTesis = new SelectList(estados, "IdEstadoSolicitud", "Nombre", idEstadoInicialTesis);
        }
    }
}
