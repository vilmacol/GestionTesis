using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaBase.Filters;
using SistemaBase.Models;

namespace SistemaBase.Controllers
{
    [Authorize]
    [AccesoPantalla("GRUPO")]
    public class GruposUsuarioController : Controller
    {
        private readonly UAADbContext _context;

        public GruposUsuarioController(UAADbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.GruposUsuarios
                .AsNoTracking()
                .Include(g => g.IdFacultadNavigation)
                .OrderBy(g => g.IdGrupo)
                .ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            await CargarFacultades();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GruposUsuario gruposUsuario)
        {
            ModelState.Remove(nameof(GruposUsuario.Usuarios));
            ModelState.Remove(nameof(GruposUsuario.Formas));
            if (!ModelState.IsValid)
            {
                await CargarFacultades(gruposUsuario.IdFacultad);
                return View(gruposUsuario);
            }

            _context.GruposUsuarios.Add(gruposUsuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var grupo = await _context.GruposUsuarios.FindAsync(id);
            await CargarFacultades(grupo?.IdFacultad);
            return grupo == null ? NotFound() : View(grupo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, GruposUsuario gruposUsuario)
        {
            ModelState.Remove(nameof(GruposUsuario.Usuarios));
            ModelState.Remove(nameof(GruposUsuario.Formas));
            if (id != gruposUsuario.IdGrupo) return NotFound();
            if (!ModelState.IsValid)
            {
                await CargarFacultades(gruposUsuario.IdFacultad);
                return View(gruposUsuario);
            }

            _context.Update(gruposUsuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            var grupo = await _context.GruposUsuarios.AsNoTracking().FirstOrDefaultAsync(g => g.IdGrupo == id);
            return grupo == null ? NotFound() : View(grupo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var grupo = await _context.GruposUsuarios.FindAsync(id);
            if (grupo != null)
            {
                try
                {
                    _context.GruposUsuarios.Remove(grupo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    TempData["Error"] = "No se puede eliminar el grupo porque tiene usuarios o permisos relacionados.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarFacultades(int? idFacultad = null)
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

            ViewBag.IdFacultad = new SelectList(facultades, "IdFacultad", "Descripcion", idFacultad);
        }
    }
}
