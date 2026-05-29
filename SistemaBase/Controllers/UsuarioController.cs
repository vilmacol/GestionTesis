using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
        using SistemaBase.Models;

namespace SistemaBase.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UAADbContext _context;

        public UsuarioController(UAADbContext context)
        {
            _context = context;
        }

        // GET: Usuario
    public async Task<IActionResult> Index()
    {
        ViewData["CodGrupo"] = new SelectList(_context.GruposUsuarios, "IdGrupo", "IdGrupo");
        ViewData["IdPersona"] = new SelectList(_context.Personas, "IdPersona", "IdPersona");
        var uAADbContext = _context.Usuarios.Include(u => u.CodGrupoNavigation).Include(u => u.IdPersonaNavigation);
        return View(await uAADbContext.AsNoTracking().ToListAsync());
    }









    public async Task<IActionResult>
    ResultTable()
    {
        ViewData["CodGrupo"] = new SelectList(_context.GruposUsuarios, "IdGrupo", "IdGrupo");
        ViewData["IdPersona"] = new SelectList(_context.Personas, "IdPersona", "IdPersona");
    ViewData["Show"] = true;
        var uAADbContext = _context.Usuarios.Include(u => u.CodGrupoNavigation).Include(u => u.IdPersonaNavigation);
        return View("Index",await uAADbContext.AsNoTracking().ToListAsync());
    }












        // GET: Usuario/Details/5
        public async Task<IActionResult> Details(string IdUsuario)
            {
            var usuario = await _context.Usuarios
            .FindAsync(IdUsuario);
            if (usuario == null)
            {
            return NotFound();
            }

            return View(usuario);
            }

            // GET: Usuario/Create
            public IActionResult Create()
            {
            ViewData["CodGrupo"] = new SelectList(_context.GruposUsuarios, "IdGrupo", "IdGrupo");
            ViewData["IdPersona"] = new SelectList(_context.Personas, "IdPersona", "IdPersona");
            return View();
            }

            // POST: Usuario/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Usuario usuario)
                {
            _context.Add(usuario);
            await _context.SaveChangesAsync();
                return RedirectToAction("ResultTable");

                //return RedirectToAction(nameof(Index));
                ViewData["CodGrupo"] = new SelectList(_context.GruposUsuarios, "IdGrupo", "IdGrupo", usuario.CodGrupo);
                ViewData["IdPersona"] = new SelectList(_context.Personas, "IdPersona", "IdPersona", usuario.IdPersona);
                return RedirectToAction("ResultTable");

                // return View(usuario);
                }

                // GET: Usuario/Edit/5
        public async Task<IActionResult> Edit(string IdUsuario)
                    {

                    var usuario = await _context.Usuarios.FindAsync(IdUsuario);
                    if (usuario == null)
                    {
                    return NotFound();
                    }
                    ViewData["CodGrupo"] = new SelectList(_context.GruposUsuarios, "IdGrupo", "IdGrupo", usuario.CodGrupo);
                    ViewData["IdPersona"] = new SelectList(_context.Personas, "IdPersona", "IdPersona", usuario.IdPersona);
                    return View(usuario);
                    }

                    // POST: Usuario/Edit/5
                    // To protect from overposting attacks, enable the specific properties you want to bind to.
                    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
                    [HttpPost]
                    [ValidateAntiForgeryToken]
                    public async Task<IActionResult>
                        Edit(string IdUsuario,  Usuario usuario)
                        {

                        try
                        {
                        _context.Update(usuario);
                        await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                        if (!UsuarioExists(usuario.IdUsuario))
                        {
                        return NotFound();
                        }
                        else
                        {
                        throw;
                        }
                        }
                        return RedirectToAction("ResultTable");

                        // return RedirectToAction(nameof(Index));
                        ViewData["CodGrupo"] = new SelectList(_context.GruposUsuarios, "IdGrupo", "IdGrupo", usuario.CodGrupo);
                        ViewData["IdPersona"] = new SelectList(_context.Personas, "IdPersona", "IdPersona", usuario.IdPersona);
                        return RedirectToAction("ResultTable");

                        //return View(usuario);
                        }

                        // GET: Usuario/Delete/5
                        public async Task<IActionResult>
                            Delete(string IdUsuario)
                            {

                            var usuario = await _context.Usuarios
                            .FindAsync(IdUsuario);
                            if (usuario == null)
                            {
                            return NotFound();
                            }

                            return View(usuario);
                            }

                            // POST: Usuario/Delete/5
                            [HttpPost, ActionName("Delete")]
                            [ValidateAntiForgeryToken]
                            public async Task<IActionResult>
                                DeleteConfirmed(string IdUsuario)
                                {
                                if (_context.Usuarios == null)
                                {
                                return Problem("Entity set 'UAADbContext.Usuarios'  is null.");
                                }
                                var usuario = await _context.Usuarios.FindAsync(IdUsuario);
                                if (usuario != null)
                                {
                                _context.Usuarios.Remove(usuario);
                                }

                                await _context.SaveChangesAsync();
                                return RedirectToAction("ResultTable");

                                //return RedirectToAction(nameof(Index));
                                //return RedirectToAction(nameof(Index));
                                }

                                private bool UsuarioExists(string id)
                                {
                                return (_context.Usuarios?.Any(e => e.IdUsuario == id)).GetValueOrDefault();
                                }
                                }
                                }
