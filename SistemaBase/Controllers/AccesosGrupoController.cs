using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaBase.Filters;
using SistemaBase.Models;
using SistemaBase.ModelsCustom;

namespace SistemaBase.Controllers
{
    [Authorize]
    [AccesoPantalla("ACCESO")]
    public class AccesosGrupoController : Controller
    {
        private readonly UAADbContext _context;

        public AccesosGrupoController(UAADbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var grupos = await _context.GruposUsuarios
                .AsNoTracking()
                .OrderBy(g => g.IdGrupo)
                .ToListAsync();

            return View(grupos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SincronizarPantallas()
        {
            var modulo = await _context.Modulos.FindAsync("01");
            if (modulo == null)
            {
                modulo = new Modulo
                {
                    IdModulo = "01",
                    Descripcion = "Sistema"
                };
                _context.Modulos.Add(modulo);
                await _context.SaveChangesAsync();
            }

            var pantallas = new List<Forma>
            {
                new() { IdModulo = "01", NomForma = "ACCESO", Descripcion = "Accesos por Grupo" },
                new() { IdModulo = "01", NomForma = "FORMA", Descripcion = "Formularios" },
                new() { IdModulo = "01", NomForma = "MODULO", Descripcion = "Modulos" },
                new() { IdModulo = "01", NomForma = "GRUPO", Descripcion = "Grupos de Usuario" },
                new() { IdModulo = "01", NomForma = "USUARIO", Descripcion = "Usuarios" },
                new() { IdModulo = "01", NomForma = "PERSONA", Descripcion = "Personas" },
                new() { IdModulo = "01", NomForma = "ALUMNO", Descripcion = "Alumnos" },
                new() { IdModulo = "01", NomForma = "FACULTAD", Descripcion = "Facultades" },
                new() { IdModulo = "01", NomForma = "CARRERA", Descripcion = "Carreras" },
                new() { IdModulo = "01", NomForma = "TUTOR", Descripcion = "Tutores" },
                new() { IdModulo = "01", NomForma = "TIPODOC", Descripcion = "Tipos de Documento" },
                new() { IdModulo = "01", NomForma = "TIPEXT", Descripcion = "Tipos de Extension" },
                new() { IdModulo = "01", NomForma = "DOCCARR", Descripcion = "Documentos por Carrera" },
                new() { IdModulo = "01", NomForma = "FLUJOTES", Descripcion = "Flujo de Tesis" },
                new() { IdModulo = "01", NomForma = "ESTSOL", Descripcion = "Estados de Solicitud" },
                new() { IdModulo = "01", NomForma = "ENTDOC", Descripcion = "Entrada de Documentos" },
                new() { IdModulo = "01", NomForma = "REVSOL", Descripcion = "Revision de Solicitudes" }
            };

            foreach (var pantalla in pantallas)
            {
                var existe = await _context.Formas.AnyAsync(f => f.IdModulo == pantalla.IdModulo && f.NomForma == pantalla.NomForma);
                if (!existe)
                {
                    _context.Formas.Add(pantalla);
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Pantallas sincronizadas correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Asignar(string id)
        {
            var model = await CrearModelo(id);
            return model == null ? NotFound() : View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Asignar(AccesoGrupoAsignacion model)
        {
            var grupo = await _context.GruposUsuarios
                .Include(g => g.Formas)
                .Include(g => g.EstadoSolicituds)
                .FirstOrDefaultAsync(g => g.IdGrupo == model.IdGrupo);

            if (grupo == null)
            {
                return NotFound();
            }

            grupo.Formas.Clear();
            grupo.EstadoSolicituds.Clear();

            if (model.FormasSeleccionadas.Any())
            {
                foreach (var key in model.FormasSeleccionadas)
                {
                    var partes = key.Split('|');
                    if (partes.Length != 2)
                    {
                        continue;
                    }

                    var forma = await _context.Formas.FindAsync(partes[0], partes[1]);
                    if (forma != null)
                    {
                        grupo.Formas.Add(forma);
                    }
                }
            }

            if (model.EstadosSolicitudSeleccionados.Any())
            {
                foreach (var idEstadoSolicitud in model.EstadosSolicitudSeleccionados.Distinct())
                {
                    var estado = await _context.EstadoSolicituds.FindAsync(idEstadoSolicitud);
                    if (estado != null)
                    {
                        grupo.EstadoSolicituds.Add(estado);
                    }
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Permisos actualizados para el grupo {grupo.IdGrupo}.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<AccesoGrupoAsignacion?> CrearModelo(string idGrupo)
        {
            var grupo = await _context.GruposUsuarios
                .AsNoTracking()
                .Include(g => g.Formas)
                .Include(g => g.EstadoSolicituds)
                .FirstOrDefaultAsync(g => g.IdGrupo == idGrupo);

            if (grupo == null)
            {
                return null;
            }

            var seleccionadas = grupo.Formas
                .Select(f => $"{f.IdModulo}|{f.NomForma}")
                .ToHashSet();

            var estadosSeleccionados = grupo.EstadoSolicituds
                .Select(e => e.IdEstadoSolicitud)
                .ToHashSet();

            var formas = await _context.Formas
                .AsNoTracking()
                .Include(f => f.IdModuloNavigation)
                .OrderBy(f => f.IdModulo)
                .ThenBy(f => f.Descripcion)
                .Select(f => new AccesoGrupoForma
                {
                    IdModulo = f.IdModulo,
                    NomForma = f.NomForma,
                    Modulo = f.IdModuloNavigation.Descripcion,
                    Descripcion = f.Descripcion
                })
                .ToListAsync();

            foreach (var forma in formas)
            {
                forma.Seleccionada = seleccionadas.Contains(forma.Key);
            }

            var estados = await _context.EstadoSolicituds
                .AsNoTracking()
                .Where(e => e.Activo == null || e.Activo == "S")
                .OrderBy(e => e.Nombre)
                .Select(e => new AccesoGrupoEstadoSolicitud
                {
                    IdEstadoSolicitud = e.IdEstadoSolicitud,
                    Nombre = e.Nombre,
                    Descripcion = e.Descripcion
                })
                .ToListAsync();

            foreach (var estado in estados)
            {
                estado.Seleccionado = estadosSeleccionados.Contains(estado.IdEstadoSolicitud);
            }

            return new AccesoGrupoAsignacion
            {
                IdGrupo = grupo.IdGrupo,
                DescripcionGrupo = grupo.Descripcion,
                Formas = formas,
                EstadosSolicitud = estados,
                FormasSeleccionadas = seleccionadas.ToList(),
                EstadosSolicitudSeleccionados = estadosSeleccionados.ToList()
            };
        }
    }
}
