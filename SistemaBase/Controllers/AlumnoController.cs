using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaBase.Filters;
using SistemaBase.Models;

namespace SistemaBase.Controllers
{
    [Authorize]
    [AccesoPantalla("ALUMNO")]
    public class AlumnoController : Controller
    {
        private readonly UAADbContext _context;

        public AlumnoController(UAADbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var alumnos = await _context.Alumnos
                .AsNoTracking()
                .Include(a => a.IdCarreraNavigation)
                .OrderBy(a => a.Nombres)
                .ThenBy(a => a.Apellidos)
                .ToListAsync();

            return View(alumnos);
        }

        public async Task<IActionResult> Create()
        {
            await CargarCombos();
            return View(new Alumno { Activo = "S", FechaRegistro = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Alumno alumno)
        {
            LimpiarValidaciones();
            if (!ModelState.IsValid)
            {
                await CargarCombos(alumno.IdCarrera);
                return View(alumno);
            }

            alumno.FechaRegistro = alumno.FechaRegistro == default ? DateTime.Now : alumno.FechaRegistro;
            alumno.Activo = string.IsNullOrWhiteSpace(alumno.Activo) ? "S" : alumno.Activo;

            _context.Alumnos.Add(alumno);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var alumno = await _context.Alumnos.FindAsync(id);
            if (alumno == null) return NotFound();

            await CargarCombos(alumno.IdCarrera);
            return View(alumno);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Alumno alumno)
        {
            LimpiarValidaciones();
            if (id != alumno.IdAlumno) return NotFound();

            if (!ModelState.IsValid)
            {
                await CargarCombos(alumno.IdCarrera);
                return View(alumno);
            }

            var existente = await _context.Alumnos.AsNoTracking().FirstOrDefaultAsync(a => a.IdAlumno == id);
            if (existente == null) return NotFound();

            alumno.FechaRegistro = existente.FechaRegistro;
            alumno.Activo = string.IsNullOrWhiteSpace(alumno.Activo) ? "S" : alumno.Activo;

            _context.Update(alumno);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var alumno = await _context.Alumnos
                .AsNoTracking()
                .Include(a => a.IdCarreraNavigation)
                .FirstOrDefaultAsync(a => a.IdAlumno == id);

            return alumno == null ? NotFound() : View(alumno);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alumno = await _context.Alumnos.FindAsync(id);
            if (alumno != null)
            {
                try
                {
                    _context.Alumnos.Remove(alumno);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    TempData["Error"] = "No se puede eliminar el alumno porque tiene solicitudes relacionadas.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarCombos(int? idCarrera = null)
        {
            ViewData["IdCarrera"] = new SelectList(
                await _context.Carreras.AsNoTracking().OrderBy(c => c.Descripcion).ToListAsync(),
                "IdCarrera",
                "Descripcion",
                idCarrera);
        }

        private void LimpiarValidaciones()
        {
            ModelState.Remove(nameof(Alumno.IdCarreraNavigation));
            ModelState.Remove(nameof(Alumno.SolicitudTesis));
        }
    }
}
