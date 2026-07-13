using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaBase.Filters;
using SistemaBase.Models;

namespace SistemaBase.Controllers
{
    [Authorize]
    [AccesoPantalla("PERSONA")]
    public class PersonasController : Controller
    {
        private readonly UAADbContext _context;

        public PersonasController(UAADbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Personas.AsNoTracking().OrderBy(p => p.Nombre).ToListAsync());
        }

        public IActionResult Create()
        {
            return View(new Persona { FecAlta = DateTime.Now, FecActualizacion = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Persona persona)
        {
            ModelState.Remove(nameof(Persona.Usuarios));
            if (!ModelState.IsValid) return View(persona);

            persona.FecAlta ??= DateTime.Now;
            persona.FecActualizacion = DateTime.Now;

            _context.Personas.Add(persona);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var persona = await _context.Personas.FindAsync(id);
            return persona == null ? NotFound() : View(persona);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Persona persona)
        {
            ModelState.Remove(nameof(Persona.Usuarios));
            if (id != persona.IdPersona) return NotFound();
            if (!ModelState.IsValid) return View(persona);

            var existente = await _context.Personas.AsNoTracking().FirstOrDefaultAsync(p => p.IdPersona == id);
            if (existente == null) return NotFound();

            persona.FecAlta = existente.FecAlta;
            persona.FecActualizacion = DateTime.Now;

            _context.Update(persona);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            var persona = await _context.Personas.AsNoTracking().FirstOrDefaultAsync(p => p.IdPersona == id);
            return persona == null ? NotFound() : View(persona);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var persona = await _context.Personas.FindAsync(id);
            if (persona != null)
            {
                try
                {
                    _context.Personas.Remove(persona);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    TempData["Error"] = "No se puede eliminar la persona porque tiene usuarios relacionados.";
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
