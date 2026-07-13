using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaBase.Filters;
using SistemaBase.Models;

namespace SistemaBase.Controllers
{
    [Authorize]
    [AccesoPantalla("USUARIO")]
    public class UsuarioController : Controller
    {
        private readonly UAADbContext _context;

        public UsuarioController(UAADbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = _context.Usuarios
                .AsNoTracking()
                .Include(u => u.CodGrupoNavigation)
                .Include(u => u.IdPersonaNavigation)
                .OrderBy(u => u.IdUsuario);

            return View(await usuarios.ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            await CargarCombos();
            return View(new Usuario { Activo = "S", FecCreacion = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            LimpiarValidaciones();

            if (string.IsNullOrWhiteSpace(usuario.PasswordHash))
            {
                ModelState.AddModelError(nameof(Usuario.PasswordHash), "Ingrese una contrasena.");
            }

            if (!ModelState.IsValid)
            {
                await CargarCombos(usuario.CodGrupo, usuario.IdPersona);
                return View(usuario);
            }

            usuario.PasswordHash = HashPassword(usuario.PasswordHash);
            usuario.FecCreacion = usuario.FecCreacion == default ? DateTime.Now : usuario.FecCreacion;
            usuario.Activo = string.IsNullOrWhiteSpace(usuario.Activo) ? "S" : usuario.Activo;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            usuario.PasswordHash = string.Empty;
            await CargarCombos(usuario.CodGrupo, usuario.IdPersona);
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Usuario usuario)
        {
            LimpiarValidaciones();
            if (id != usuario.IdUsuario) return NotFound();

            var existente = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.IdUsuario == id);
            if (existente == null) return NotFound();

            if (string.IsNullOrWhiteSpace(usuario.PasswordHash))
            {
                ModelState.Remove(nameof(Usuario.PasswordHash));
            }

            if (!ModelState.IsValid)
            {
                usuario.PasswordHash = string.Empty;
                await CargarCombos(usuario.CodGrupo, usuario.IdPersona);
                return View(usuario);
            }

            usuario.PasswordHash = string.IsNullOrWhiteSpace(usuario.PasswordHash)
                ? existente.PasswordHash
                : HashPassword(usuario.PasswordHash);
            usuario.FecCreacion = existente.FecCreacion;
            usuario.Activo = string.IsNullOrWhiteSpace(usuario.Activo) ? "S" : usuario.Activo;

            _context.Update(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Include(u => u.IdPersonaNavigation)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            return usuario == null ? NotFound() : View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                try
                {
                    _context.Usuarios.Remove(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    TempData["Error"] = "No se puede eliminar el usuario porque tiene registros relacionados.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarCombos(string? codGrupo = null, string? idPersona = null)
        {
            ViewData["CodGrupo"] = new SelectList(
                await _context.GruposUsuarios.AsNoTracking().OrderBy(g => g.IdGrupo).ToListAsync(),
                "IdGrupo",
                "Descripcion",
                codGrupo);

            ViewData["IdPersona"] = new SelectList(
                await _context.Personas.AsNoTracking().OrderBy(p => p.Nombre).ToListAsync(),
                "IdPersona",
                "Nombre",
                idPersona);
        }

        private void LimpiarValidaciones()
        {
            ModelState.Remove(nameof(Usuario.CodGrupoNavigation));
            ModelState.Remove(nameof(Usuario.IdPersonaNavigation));
            ModelState.Remove(nameof(Usuario.HistorialSolicituds));
            ModelState.Remove(nameof(Usuario.RevisionDecanos));
            ModelState.Remove(nameof(Usuario.SolicitudDocumentos));
        }

        private static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
