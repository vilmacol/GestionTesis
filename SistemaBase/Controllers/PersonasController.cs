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
    public class PersonasController : Controller
    {
        private readonly UAADbContext _context;

        public PersonasController(UAADbContext context)
        {
            _context = context;
        }

        // GET: Personas
    public async Task<IActionResult> Index()
    {
            return _context.Personas != null ?
              View(await _context.Personas.AsNoTracking().ToListAsync()) :
              Problem("Entity set 'UAADbContext.Personas'  is null.");
    }









    public async Task<IActionResult>
    ResultTable()
    {
    ViewData["Show"] = true;
            return _context.Personas != null ?
              View("Index", await _context.Personas.AsNoTracking().ToListAsync()) :
              Problem("Entity set 'UAADbContext.Personas'  is null.");
    }












        // GET: Personas/Details/5
        public async Task<IActionResult> Details(string IdPersona)
            {
            var persona = await _context.Personas
            .FindAsync(IdPersona);
            if (persona == null)
            {
            return NotFound();
            }

            return View(persona);
            }

            // GET: Personas/Create
            public IActionResult Create()
            {
            return View();
            }

            // POST: Personas/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Persona persona)
                {
            _context.Add(persona);
            await _context.SaveChangesAsync();
                return RedirectToAction("ResultTable");

                //return RedirectToAction(nameof(Index));
                return RedirectToAction("ResultTable");

                // return View(persona);
                }

                // GET: Personas/Edit/5
        public async Task<IActionResult> Edit(string IdPersona)
                    {

                    var persona = await _context.Personas.FindAsync(IdPersona);
                    if (persona == null)
                    {
                    return NotFound();
                    }
                    return View(persona);
                    }

                    // POST: Personas/Edit/5
                    // To protect from overposting attacks, enable the specific properties you want to bind to.
                    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
                    [HttpPost]
                    [ValidateAntiForgeryToken]
                    public async Task<IActionResult>
                        Edit(string IdPersona,  Persona persona)
                        {

                        try
                        {
                        _context.Update(persona);
                        await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                        if (!PersonaExists(persona.IdPersona))
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

                        //return View(persona);
                        }

                        // GET: Personas/Delete/5
                        public async Task<IActionResult>
                            Delete(string IdPersona)
                            {

                            var persona = await _context.Personas
                            .FindAsync(IdPersona);
                            if (persona == null)
                            {
                            return NotFound();
                            }

                            return View(persona);
                            }

                            // POST: Personas/Delete/5
                            [HttpPost, ActionName("Delete")]
                            [ValidateAntiForgeryToken]
                            public async Task<IActionResult>
                                DeleteConfirmed(string IdPersona)
                                {
                                if (_context.Personas == null)
                                {
                                return Problem("Entity set 'UAADbContext.Personas'  is null.");
                                }
                                var persona = await _context.Personas.FindAsync(IdPersona);
                                if (persona != null)
                                {
                                _context.Personas.Remove(persona);
                                }

                                await _context.SaveChangesAsync();
                                return RedirectToAction("ResultTable");

                                //return RedirectToAction(nameof(Index));
                                //return RedirectToAction(nameof(Index));
                                }

                                private bool PersonaExists(string id)
                                {
                                return (_context.Personas?.Any(e => e.IdPersona == id)).GetValueOrDefault();
                                }
                                }
                                }
