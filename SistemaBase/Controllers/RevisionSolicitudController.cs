using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaBase.Filters;
using SistemaBase.Models;
using SistemaBase.ModelsCustom;
using System.Security.Claims;

namespace SistemaBase.Controllers
{
    [Authorize]
    [AccesoPantalla("REVSOL")]
    public class RevisionSolicitudController : Controller
    {
        private readonly UAADbContext _context;

        public RevisionSolicitudController(UAADbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? idCarrera)
        {
            var idFacultadUsuario = await ObtenerIdFacultadUsuario();
            var estadosPermitidos = await ObtenerEstadosPermitidosGrupo();
            await CargarFiltroCarreras(idFacultadUsuario, idCarrera);

            var query = _context.SolicitudTeses
                .AsNoTracking()
                .Include(s => s.IdAlumnoNavigation)
                    .ThenInclude(a => a.IdCarreraNavigation)
                        .ThenInclude(c => c.IdFacultadNavigation)
                .Include(s => s.IdTutorNavigation)
                .Include(s => s.IdEstadoSolicitudNavigation)
                .Where(s => s.Activo == null || s.Activo == "S");

            if (idFacultadUsuario.HasValue)
            {
                query = query.Where(s => s.IdAlumnoNavigation.IdCarreraNavigation.IdFacultad == idFacultadUsuario.Value);
            }

            if (idCarrera.HasValue)
            {
                query = query.Where(s => s.IdAlumnoNavigation.IdCarrera == idCarrera.Value);
            }

            if (estadosPermitidos.Any())
            {
                query = query.Where(s => estadosPermitidos.Contains(s.IdEstadoSolicitud));
            }

            var solicitudes = await query
                .OrderByDescending(s => s.FechaSolicitud)
                .Select(s => new SolicitudRevision
                {
                    IdSolicitudTesis = s.IdSolicitudTesis,
                    FechaSolicitud = s.FechaSolicitud,
                    Alumno = (s.IdAlumnoNavigation.Nombres + " " + s.IdAlumnoNavigation.Apellidos).Trim(),
                    Documento = s.IdAlumnoNavigation.NroDocumento,
                    IdCarrera = s.IdAlumnoNavigation.IdCarrera,
                    Carrera = s.IdAlumnoNavigation.IdCarreraNavigation.Descripcion,
                    Facultad = s.IdAlumnoNavigation.IdCarreraNavigation.IdFacultadNavigation != null
                        ? s.IdAlumnoNavigation.IdCarreraNavigation.IdFacultadNavigation.Codigo
                        : null,
                    RegistroAcademico = s.IdAlumnoNavigation.RegistroAcademico,
                    Tutor = s.IdTutorNavigation != null ? s.IdTutorNavigation.NombreCompleto : "Sin tutor",
                    Estado = s.IdEstadoSolicitudNavigation.Nombre,
                    IdEstadoSolicitud = s.IdEstadoSolicitud,
                    Tema = s.Tema
                })
                .ToListAsync();

            return View(solicitudes);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            if (!await PuedeAccederSolicitud(id))
            {
                return Forbid();
            }

            var solicitud = await ObtenerDetalle(id);
            return solicitud == null ? NotFound() : View(solicitud);
        }

        public async Task<IActionResult> Archivo(int id)
        {
            var documento = await _context.SolicitudDocumentos
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.IdSolicitudDocumento == id && (d.Activo == null || d.Activo == "S"));

            if (documento == null)
            {
                return NotFound();
            }

            if (!await PuedeAccederSolicitud(documento.IdSolicitudTesis))
            {
                return Forbid();
            }

            if (documento.ArchivoContenido != null && documento.ArchivoContenido.Length > 0)
            {
                var contentType = string.IsNullOrWhiteSpace(documento.ContentType)
                    ? "application/octet-stream"
                    : documento.ContentType;

                Response.Headers["Content-Disposition"] = $"inline; filename=\"{documento.NombreArchivo}\"";
                return File(documento.ArchivoContenido, contentType);
            }

            if (!string.IsNullOrWhiteSpace(documento.RutaArchivo) && documento.RutaArchivo != "BD")
            {
                return Redirect(Url.Content(documento.RutaArchivo));
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(CambiarEstadoSolicitud model)
        {
            if (model.IdEstadoSolicitudDestino <= 0)
            {
                TempData["Error"] = "No existe un siguiente paso configurado para esta solicitud.";
                return RedirectToAction(nameof(Detalle), new { id = model.IdSolicitudTesis });
            }

            var solicitud = await _context.SolicitudTeses
                .Include(s => s.IdEstadoSolicitudNavigation)
                .Include(s => s.IdAlumnoNavigation)
                .FirstOrDefaultAsync(s => s.IdSolicitudTesis == model.IdSolicitudTesis);

            if (solicitud == null)
            {
                return NotFound();
            }

            if (!await PuedeAccederSolicitud(solicitud.IdSolicitudTesis))
            {
                return Forbid();
            }

            var transicion = model.IdCarreraFlujoTransicion.HasValue
                ? await ObtenerTransicion(model.IdCarreraFlujoTransicion.Value, solicitud.IdAlumnoNavigation.IdCarrera, solicitud.IdEstadoSolicitud)
                : null;

            if (transicion == null || transicion.IdEstadoDestino != model.IdEstadoSolicitudDestino)
            {
                TempData["Error"] = "La solicitud solo puede moverse por una transicion configurada para su carrera.";
                return RedirectToAction(nameof(Detalle), new { id = model.IdSolicitudTesis });
            }

            var requiereComentario = transicion?.RequiereComentario ?? false;
            var requiereFechaPresentacion = transicion?.RequiereFechaPresentacion ?? false;

            if (requiereComentario && string.IsNullOrWhiteSpace(model.Comentario))
            {
                TempData["Error"] = "Ingrese un comentario para realizar esta accion.";
                return RedirectToAction(nameof(Detalle), new { id = model.IdSolicitudTesis });
            }

            if (requiereFechaPresentacion && !model.FechaPresentacion.HasValue)
            {
                TempData["Error"] = "Ingrese la fecha de presentacion para avanzar a este paso.";
                return RedirectToAction(nameof(Detalle), new { id = model.IdSolicitudTesis });
            }

            var estadoNuevo = transicion.IdEstadoDestinoNavigation;

            var estadoAnterior = solicitud.IdEstadoSolicitudNavigation?.Nombre;
            solicitud.IdEstadoSolicitud = estadoNuevo.IdEstadoSolicitud;
            if (requiereFechaPresentacion)
            {
                solicitud.FechaPresentacion = model.FechaPresentacion;
            }

            _context.HistorialSolicituds.Add(new HistorialSolicitud
            {
                IdSolicitudTesis = solicitud.IdSolicitudTesis,
                IdUsuario = ObtenerIdUsuario(),
                FechaMovimiento = DateTime.Now,
                Accion = transicion.Accion,
                Observacion = model.Comentario,
                EstadoAnterior = estadoAnterior,
                EstadoNuevo = estadoNuevo.Nombre
            });

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Solicitud Nro. {solicitud.IdSolicitudTesis} actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<SolicitudRevision?> ObtenerDetalle(int id)
        {
            var solicitud = await _context.SolicitudTeses
                .AsNoTracking()
                .Include(s => s.IdAlumnoNavigation)
                    .ThenInclude(a => a.IdCarreraNavigation)
                        .ThenInclude(c => c.IdFacultadNavigation)
                .Include(s => s.IdTutorNavigation)
                .Include(s => s.IdEstadoSolicitudNavigation)
                .Include(s => s.SolicitudDocumentos)
                    .ThenInclude(d => d.IdTipoDocumentoNavigation)
                .Include(s => s.HistorialSolicituds)
                .FirstOrDefaultAsync(s => s.IdSolicitudTesis == id);

            if (solicitud == null)
            {
                return null;
            }

            var revision = new SolicitudRevision
            {
                IdSolicitudTesis = solicitud.IdSolicitudTesis,
                FechaSolicitud = solicitud.FechaSolicitud,
                Alumno = $"{solicitud.IdAlumnoNavigation.Nombres} {solicitud.IdAlumnoNavigation.Apellidos}".Trim(),
                Documento = solicitud.IdAlumnoNavigation.NroDocumento,
                IdCarrera = solicitud.IdAlumnoNavigation.IdCarrera,
                Carrera = solicitud.IdAlumnoNavigation.IdCarreraNavigation.Descripcion,
                Facultad = solicitud.IdAlumnoNavigation.IdCarreraNavigation.IdFacultadNavigation?.Codigo,
                RegistroAcademico = solicitud.IdAlumnoNavigation.RegistroAcademico,
                Correo = solicitud.IdAlumnoNavigation.Correo,
                Telefono = solicitud.IdAlumnoNavigation.Telefono,
                Tutor = solicitud.IdTutorNavigation?.NombreCompleto ?? "Sin tutor",
                Estado = solicitud.IdEstadoSolicitudNavigation.Nombre,
                IdEstadoSolicitud = solicitud.IdEstadoSolicitud,
                Tema = solicitud.Tema,
                ObservacionRecepcion = solicitud.ObservacionRecepcion,
                FechaAsignacionTutor = solicitud.FechaAsignacionTutor,
                FechaEnvioDecano = solicitud.FechaEnvioDecano,
                FechaRespuestaDecano = solicitud.FechaRespuestaDecano,
                FechaPresentacion = solicitud.FechaPresentacion,
                Documentos = solicitud.SolicitudDocumentos
                    .Where(d => d.Activo == null || d.Activo == "S")
                    .OrderBy(d => d.IdTipoDocumentoNavigation.Nombre)
                    .Select(d => new SolicitudRevisionDocumento
                    {
                        IdSolicitudDocumento = d.IdSolicitudDocumento,
                        TipoDocumento = d.IdTipoDocumentoNavigation.Nombre,
                        NombreArchivo = d.NombreArchivo,
                        RutaArchivo = d.RutaArchivo,
                        Extension = d.Extension,
                        TieneArchivoEnBase = d.ArchivoContenido != null && d.ArchivoContenido.Length > 0,
                        TamanoKb = d.TamanoKb,
                        FechaCarga = d.FechaCarga,
                        Observacion = d.Observacion
                    })
                    .ToList(),
                Historial = solicitud.HistorialSolicituds
                    .OrderByDescending(h => h.FechaMovimiento)
                    .Select(h => new SolicitudRevisionHistorial
                    {
                        FechaMovimiento = h.FechaMovimiento,
                        Accion = h.Accion,
                        Observacion = h.Observacion,
                        EstadoAnterior = h.EstadoAnterior,
                        EstadoNuevo = h.EstadoNuevo,
                        IdUsuario = h.IdUsuario
                    })
                    .ToList()
            };

            revision.Transiciones = await ObtenerTransicionesDisponibles(
                solicitud.IdAlumnoNavigation.IdCarrera,
                solicitud.IdEstadoSolicitud);

            return revision;
        }

        private async Task<CarreraFlujoTransicion?> ObtenerTransicion(int idTransicion, int idCarrera, int idEstadoOrigen)
        {
            return await _context.CarreraFlujoTransicions
                .Include(t => t.IdEstadoDestinoNavigation)
                .FirstOrDefaultAsync(t => t.IdCarreraFlujoTransicion == idTransicion
                    && t.IdCarrera == idCarrera
                    && t.IdEstadoOrigen == idEstadoOrigen
                    && (t.Activo == null || t.Activo == "S"));
        }

        private async Task<List<SolicitudRevisionTransicion>> ObtenerTransicionesDisponibles(int idCarrera, int idEstadoOrigen)
        {
            return await _context.CarreraFlujoTransicions
                .AsNoTracking()
                .Include(t => t.IdEstadoDestinoNavigation)
                .Where(t => t.IdCarrera == idCarrera
                    && t.IdEstadoOrigen == idEstadoOrigen
                    && (t.Activo == null || t.Activo == "S"))
                .OrderBy(t => t.Accion)
                .Select(t => new SolicitudRevisionTransicion
                {
                    IdCarreraFlujoTransicion = t.IdCarreraFlujoTransicion,
                    IdEstadoDestino = t.IdEstadoDestino,
                    Accion = t.Accion,
                    EstadoDestino = t.IdEstadoDestinoNavigation.Nombre,
                    RequiereComentario = t.RequiereComentario,
                    RequiereFechaPresentacion = t.RequiereFechaPresentacion
                })
                .ToListAsync();
        }

        private string ObtenerIdUsuario()
        {
            return User.FindFirstValue("IdUsuario") ?? User.Identity?.Name ?? "admin";
        }

        private async Task<int?> ObtenerIdFacultadUsuario()
        {
            var idUsuario = ObtenerIdUsuario();
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Include(u => u.CodGrupoNavigation)
                .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

            return usuario?.CodGrupoNavigation?.IdFacultad;
        }

        private async Task<bool> PuedeAccederSolicitud(int idSolicitudTesis)
        {
            var idFacultadUsuario = await ObtenerIdFacultadUsuario();
            var estadosPermitidos = await ObtenerEstadosPermitidosGrupo();
            
            var query = _context.SolicitudTeses
                .AsNoTracking()
                .Where(s => s.IdSolicitudTesis == idSolicitudTesis);

            if (!idFacultadUsuario.HasValue)
            {
                if (estadosPermitidos.Any())
                {
                    query = query.Where(s => estadosPermitidos.Contains(s.IdEstadoSolicitud));
                }

                return await query.AnyAsync();
            }

            query = query.Where(s => s.IdAlumnoNavigation.IdCarreraNavigation.IdFacultad == idFacultadUsuario.Value);

            if (estadosPermitidos.Any())
            {
                query = query.Where(s => estadosPermitidos.Contains(s.IdEstadoSolicitud));
            }

            return await query.AnyAsync();
        }

        private async Task CargarFiltroCarreras(int? idFacultadUsuario, int? idCarrera)
        {
            var query = _context.Carreras
                .AsNoTracking()
                .Include(c => c.IdFacultadNavigation)
                .AsQueryable();

            if (idFacultadUsuario.HasValue)
            {
                query = query.Where(c => c.IdFacultad == idFacultadUsuario.Value);
            }

            var carreras = await query
                .OrderBy(c => c.Descripcion)
                .Select(c => new
                {
                    c.IdCarrera,
                    c.Descripcion
                })
                .ToListAsync();

            var facultad = idFacultadUsuario.HasValue
                ? await _context.Facultads.AsNoTracking().FirstOrDefaultAsync(f => f.IdFacultad == idFacultadUsuario.Value)
                : null;

            ViewBag.IdCarrera = new SelectList(carreras, "IdCarrera", "Descripcion", idCarrera);
            ViewBag.FacultadUsuario = facultad == null ? "Todas las facultades" : $"{facultad.Codigo} - {facultad.Nombre}";
            ViewBag.IdCarreraSeleccionada = idCarrera;
        }

        private async Task<List<int>> ObtenerEstadosPermitidosGrupo()
        {
            var codGrupo = ObtenerCodGrupoUsuario();
            if (string.IsNullOrWhiteSpace(codGrupo))
            {
                return new List<int>();
            }

            return await _context.GruposUsuarios
                .AsNoTracking()
                .Where(g => g.IdGrupo == codGrupo)
                .SelectMany(g => g.EstadoSolicituds.Select(e => e.IdEstadoSolicitud))
                .ToListAsync();
        }

        private string? ObtenerCodGrupoUsuario()
        {
            return User.FindFirstValue(ClaimTypes.Role);
        }
    }
}
