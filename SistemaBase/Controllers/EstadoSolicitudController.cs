using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaBase.Filters;
using SistemaBase.Models;

namespace SistemaBase.Controllers
{
    [Authorize]
    [AccesoPantalla("ESTSOL")]
    public class EstadoSolicitudController : Controller
    {
        private readonly UAADbContext _context;

        public EstadoSolicitudController(UAADbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.EstadoSolicituds.AsNoTracking().OrderBy(e => e.Nombre).ToListAsync());
        }

        public IActionResult Create()
        {
            return View(new EstadoSolicitud { Activo = "S" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EstadoSolicitud estadoSolicitud)
        {
            if (!ModelState.IsValid)
            {
                return View(estadoSolicitud);
            }

            estadoSolicitud.Activo ??= "S";
            _context.EstadoSolicituds.Add(estadoSolicitud);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var estadoSolicitud = await _context.EstadoSolicituds.FindAsync(id);
            return estadoSolicitud == null ? NotFound() : View(estadoSolicitud);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EstadoSolicitud estadoSolicitud)
        {
            if (id != estadoSolicitud.IdEstadoSolicitud)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(estadoSolicitud);
            }

            _context.Update(estadoSolicitud);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var estadoSolicitud = await _context.EstadoSolicituds.AsNoTracking().FirstOrDefaultAsync(e => e.IdEstadoSolicitud == id);
            return estadoSolicitud == null ? NotFound() : View(estadoSolicitud);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estadoSolicitud = await _context.EstadoSolicituds.FindAsync(id);
            if (estadoSolicitud != null)
            {
                try
                {
                    _context.EstadoSolicituds.Remove(estadoSolicitud);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    TempData["Error"] = "No se puede eliminar el estado porque esta siendo utilizado en solicitudes, flujos o permisos por grupo.";
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
