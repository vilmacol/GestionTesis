using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaBase.Filters;
using SistemaBase.Models;

namespace SistemaBase.Controllers
{
    [Authorize]
    [AccesoPantalla("FLUJOTES")]
    public class CarreraFlujoTransicionController : Controller
    {
        private readonly UAADbContext _context;

        public CarreraFlujoTransicionController(UAADbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var transiciones = await _context.CarreraFlujoTransicions
                .AsNoTracking()
                .Include(t => t.IdCarreraNavigation)
                .Include(t => t.IdEstadoOrigenNavigation)
                .Include(t => t.IdEstadoDestinoNavigation)
                .OrderBy(t => t.IdCarreraNavigation.Descripcion)
                .ThenBy(t => t.IdEstadoOrigenNavigation.Nombre)
                .ThenBy(t => t.Accion)
                .ToListAsync();

            return View(transiciones);
        }

        public async Task<IActionResult> Create()
        {
            await CargarCombos();
            return View(new CarreraFlujoTransicion { Activo = "S" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarreraFlujoTransicion transicion)
        {
            LimpiarModelo();
            Normalizar(transicion);
            if (!ModelState.IsValid)
            {
                await CargarCombos(transicion.IdCarrera, transicion.IdEstadoOrigen, transicion.IdEstadoDestino);
                return View(transicion);
            }

            _context.CarreraFlujoTransicions.Add(transicion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var transicion = await _context.CarreraFlujoTransicions.FindAsync(id);
            if (transicion == null) return NotFound();

            await CargarCombos(transicion.IdCarrera, transicion.IdEstadoOrigen, transicion.IdEstadoDestino);
            return View(transicion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarreraFlujoTransicion transicion)
        {
            LimpiarModelo();
            if (id != transicion.IdCarreraFlujoTransicion) return NotFound();
            Normalizar(transicion);
            if (!ModelState.IsValid)
            {
                await CargarCombos(transicion.IdCarrera, transicion.IdEstadoOrigen, transicion.IdEstadoDestino);
                return View(transicion);
            }

            _context.Update(transicion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var transicion = await _context.CarreraFlujoTransicions
                .AsNoTracking()
                .Include(t => t.IdCarreraNavigation)
                .Include(t => t.IdEstadoOrigenNavigation)
                .Include(t => t.IdEstadoDestinoNavigation)
                .FirstOrDefaultAsync(t => t.IdCarreraFlujoTransicion == id);

            return transicion == null ? NotFound() : View(transicion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transicion = await _context.CarreraFlujoTransicions.FindAsync(id);
            if (transicion != null)
            {
                transicion.Activo = "N";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private void LimpiarModelo()
        {
            ModelState.Remove(nameof(CarreraFlujoTransicion.IdCarreraNavigation));
            ModelState.Remove(nameof(CarreraFlujoTransicion.IdEstadoOrigenNavigation));
            ModelState.Remove(nameof(CarreraFlujoTransicion.IdEstadoDestinoNavigation));
        }

        private static void Normalizar(CarreraFlujoTransicion transicion)
        {
            transicion.Accion = (transicion.Accion ?? string.Empty).Trim();
            transicion.Activo = string.IsNullOrWhiteSpace(transicion.Activo) ? "S" : transicion.Activo.Trim().ToUpperInvariant();
        }

        private async Task CargarCombos(int? idCarrera = null, int? idEstadoOrigen = null, int? idEstadoDestino = null)
        {
            var carreras = await _context.Carreras.AsNoTracking().OrderBy(c => c.Descripcion).ToListAsync();
            var estados = await _context.EstadoSolicituds
                .AsNoTracking()
                .Where(e => e.Activo == null || e.Activo == "S")
                .OrderBy(e => e.Nombre)
                .ToListAsync();

            ViewBag.IdCarrera = new SelectList(carreras, "IdCarrera", "Descripcion", idCarrera);
            ViewBag.IdEstadoOrigen = new SelectList(estados, "IdEstadoSolicitud", "Nombre", idEstadoOrigen);
            ViewBag.IdEstadoDestino = new SelectList(estados, "IdEstadoSolicitud", "Nombre", idEstadoDestino);
        }
    }
}
