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
    public class GruposUsuarioController : Controller
    {
        private readonly UAADbContext _context;

        public GruposUsuarioController(UAADbContext context)
        {
            _context = context;
        }

        // GET: GruposUsuario
    public async Task<IActionResult> Index()
    {
            return _context.GruposUsuarios != null ?
              View(await _context.GruposUsuarios.AsNoTracking().ToListAsync()) :
              Problem("Entity set 'UAADbContext.GruposUsuarios'  is null.");
    }









    public async Task<IActionResult>
    ResultTable()
    {
    ViewData["Show"] = true;
            return _context.GruposUsuarios != null ?
              View("Index", await _context.GruposUsuarios.AsNoTracking().ToListAsync()) :
              Problem("Entity set 'UAADbContext.GruposUsuarios'  is null.");
    }












        // GET: GruposUsuario/Details/5
        public async Task<IActionResult> Details(string IdGrupo)
            {
            var gruposUsuario = await _context.GruposUsuarios
            .FindAsync(IdGrupo);
            if (gruposUsuario == null)
            {
            return NotFound();
            }

            return View(gruposUsuario);
            }

            // GET: GruposUsuario/Create
            public IActionResult Create()
            {
            return View();
            }

            // POST: GruposUsuario/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( GruposUsuario gruposUsuario)
                {
            _context.Add(gruposUsuario);
            await _context.SaveChangesAsync();
                return RedirectToAction("ResultTable");

                //return RedirectToAction(nameof(Index));
                return RedirectToAction("ResultTable");

                // return View(gruposUsuario);
                }

                // GET: GruposUsuario/Edit/5
        public async Task<IActionResult> Edit(string IdGrupo)
                    {

                    var gruposUsuario = await _context.GruposUsuarios.FindAsync(IdGrupo);
                    if (gruposUsuario == null)
                    {
                    return NotFound();
                    }
                    return View(gruposUsuario);
                    }

                    // POST: GruposUsuario/Edit/5
                    // To protect from overposting attacks, enable the specific properties you want to bind to.
                    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
                    [HttpPost]
                    [ValidateAntiForgeryToken]
                    public async Task<IActionResult>
                        Edit(string IdGrupo,  GruposUsuario gruposUsuario)
                        {

                        try
                        {
                        _context.Update(gruposUsuario);
                        await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                        if (!GruposUsuarioExists(gruposUsuario.IdGrupo))
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
                        return RedirectToAction("ResultTable");

                        //return View(gruposUsuario);
                        }

                        // GET: GruposUsuario/Delete/5
                        public async Task<IActionResult>
                            Delete(string IdGrupo)
                            {

                            var gruposUsuario = await _context.GruposUsuarios
                            .FindAsync(IdGrupo);
                            if (gruposUsuario == null)
                            {
                            return NotFound();
                            }

                            return View(gruposUsuario);
                            }

                            // POST: GruposUsuario/Delete/5
                            [HttpPost, ActionName("Delete")]
                            [ValidateAntiForgeryToken]
                            public async Task<IActionResult>
                                DeleteConfirmed(string IdGrupo)
                                {
                                if (_context.GruposUsuarios == null)
                                {
                                return Problem("Entity set 'UAADbContext.GruposUsuarios'  is null.");
                                }
                                var gruposUsuario = await _context.GruposUsuarios.FindAsync(IdGrupo);
                                if (gruposUsuario != null)
                                {
                                _context.GruposUsuarios.Remove(gruposUsuario);
                                }

                                await _context.SaveChangesAsync();
                                return RedirectToAction("ResultTable");

                                //return RedirectToAction(nameof(Index));
                                //return RedirectToAction(nameof(Index));
                                }

                                private bool GruposUsuarioExists(string id)
                                {
                                return (_context.GruposUsuarios?.Any(e => e.IdGrupo == id)).GetValueOrDefault();
                                }
                                }
                                }
