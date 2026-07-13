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
    [AccesoPantalla("ENTDOC")]
    public class EntradaDocController : Controller
    {
        private const long MaxFileSize = 20 * 1024 * 1024;
        private readonly UAADbContext _context;
        private readonly IWebHostEnvironment _environment;

        public EntradaDocController(UAADbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            return View(await CrearViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> DocumentosPorCarrera(int idCarrera)
        {
            var documentos = await ObtenerDocumentosRequeridos(idCarrera);
            return Json(documentos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(EntradaDoc model)
        {
            model.DocumentosRequeridos = await ObtenerDocumentosRequeridos(model.IdCarrera);

            ValidarDatos(model);
            await ValidarDocumentosRequeridos(model.DocumentosRequeridos);

            if (!ModelState.IsValid)
            {
                await CargarCombos(model);
                return View("Index", model);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var alumno = await ObtenerOCrearAlumno(model);
            var estadoInicial = await ObtenerEstadoInicial(model.IdCarrera);

            var solicitud = new SolicitudTesi
            {
                IdAlumno = alumno.IdAlumno,
                IdTutor = model.IdTutor,
                IdEstadoSolicitud = estadoInicial.IdEstadoSolicitud,
                Tema = model.Tema,
                ObservacionRecepcion = model.ObservacionRecepcion,
                FechaSolicitud = DateTime.Now,
                Activo = "S"
            };

            _context.SolicitudTeses.Add(solicitud);
            await _context.SaveChangesAsync();

            await GuardarDocumentos(solicitud.IdSolicitudTesis, model.DocumentosRequeridos);
            await RegistrarHistorial(solicitud.IdSolicitudTesis, estadoInicial.Nombre);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["Success"] = $"Solicitud de tesis registrada correctamente. Nro. {solicitud.IdSolicitudTesis}.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<EntradaDoc> CrearViewModel()
        {
            var model = new EntradaDoc();
            await CargarCombos(model);
            return model;
        }

        private async Task CargarCombos(EntradaDoc model)
        {
            model.Carreras = await _context.Carreras
                .AsNoTracking()
                .OrderBy(c => c.Descripcion)
                .Select(c => new SelectListItem
                {
                    Value = c.IdCarrera.ToString(),
                    Text = c.Descripcion
                })
                .ToListAsync();

            model.Tutores = await _context.Tutors
                .AsNoTracking()
                .Where(t => t.Activo == null || t.Activo == "S")
                .OrderBy(t => t.NombreCompleto)
                .Select(t => new SelectListItem
                {
                    Value = t.IdTutor.ToString(),
                    Text = t.NombreCompleto
                })
                .ToListAsync();

            model.ExtensionesPermitidas = await ObtenerAcceptExtensiones();

            if (model.IdCarrera > 0)
            {
                model.DocumentosRequeridos = await ObtenerDocumentosRequeridos(model.IdCarrera);
            }
        }

        private async Task<List<EntradaDocDocumentoRequerido>> ObtenerDocumentosRequeridos(int idCarrera)
        {
            if (idCarrera <= 0)
            {
                return new List<EntradaDocDocumentoRequerido>();
            }

            return await _context.CarreraDocumentoRequeridos
                .AsNoTracking()
                .Include(c => c.IdTipoDocumentoNavigation)
                .Where(c => c.IdCarrera == idCarrera && (c.Activo == null || c.Activo == "S"))
                .OrderBy(c => c.IdTipoDocumentoNavigation.Nombre)
                .Select(c => new EntradaDocDocumentoRequerido
                {
                    IdTipoDocumento = c.IdTipoDocumento,
                    Nombre = c.IdTipoDocumentoNavigation.Nombre,
                    Observacion = c.Observacion,
                    Obligatorio = c.Obligatorio ?? true
                })
                .ToListAsync();
        }

        private void ValidarDatos(EntradaDoc model)
        {
            if (string.IsNullOrWhiteSpace(model.NombreAlumno))
            {
                ModelState.AddModelError(nameof(model.NombreAlumno), "Ingrese el nombre del alumno.");
            }

            if (string.IsNullOrWhiteSpace(model.NroDocumento))
            {
                ModelState.AddModelError(nameof(model.NroDocumento), "Ingrese el documento o CI.");
            }

            if (model.IdCarrera <= 0)
            {
                ModelState.AddModelError(nameof(model.IdCarrera), "Seleccione una carrera.");
            }
        }

        private async Task ValidarDocumentosRequeridos(List<EntradaDocDocumentoRequerido> documentos)
        {
            var extensionesPermitidas = await ObtenerExtensionesPermitidas();

            if (!extensionesPermitidas.Any())
            {
                ModelState.AddModelError(string.Empty, "No existen extensiones activas configuradas para cargar documentos.");
                return;
            }

            foreach (var documento in documentos.Where(d => d.Obligatorio))
            {
                var archivo = Request.Form.Files.FirstOrDefault(f => f.Name == NombreInputDocumento(documento.IdTipoDocumento));
                if (archivo == null || archivo.Length == 0)
                {
                    ModelState.AddModelError(string.Empty, $"Debe cargar el documento: {documento.Nombre}.");
                }
            }

            foreach (var archivo in Request.Form.Files)
            {
                if (archivo.Length > MaxFileSize)
                {
                    ModelState.AddModelError(string.Empty, $"El archivo {archivo.FileName} supera los 20 MB.");
                }

                var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
                if (!extensionesPermitidas.Contains(extension))
                {
                    ModelState.AddModelError(string.Empty, $"El archivo {archivo.FileName} no tiene un formato permitido. Extensiones permitidas: {string.Join(", ", extensionesPermitidas)}.");
                }
            }
        }

        private async Task<Alumno> ObtenerOCrearAlumno(EntradaDoc model)
        {
            var nroDocumento = model.NroDocumento.Trim();
            var registroAcademico = string.IsNullOrWhiteSpace(model.RegistroAcademico)
                ? $"SIN-{nroDocumento}"
                : model.RegistroAcademico.Trim();

            var alumno = await _context.Alumnos.FirstOrDefaultAsync(a => a.NroDocumento == nroDocumento);
            if (alumno == null)
            {
                alumno = new Alumno
                {
                    IdCarrera = model.IdCarrera,
                    NroDocumento = nroDocumento,
                    Nombres = model.NombreAlumno.Trim(),
                    Apellidos = string.Empty,
                    RegistroAcademico = registroAcademico,
                    Correo = model.Correo,
                    Telefono = model.Telefono,
                    Activo = "S",
                    FechaRegistro = DateTime.Now
                };

                _context.Alumnos.Add(alumno);
                await _context.SaveChangesAsync();
                return alumno;
            }

            alumno.IdCarrera = model.IdCarrera;
            alumno.Nombres = model.NombreAlumno.Trim();
            alumno.RegistroAcademico = registroAcademico;
            alumno.Correo = model.Correo;
            alumno.Telefono = model.Telefono;
            alumno.Activo = "S";
            await _context.SaveChangesAsync();

            return alumno;
        }

        private async Task<EstadoSolicitud> ObtenerEstadoInicial(int idCarrera)
        {
            var carrera = await _context.Carreras
                .AsNoTracking()
                .Include(c => c.IdEstadoInicialTesisNavigation)
                .FirstOrDefaultAsync(c => c.IdCarrera == idCarrera);

            if (carrera?.IdEstadoInicialTesisNavigation != null)
            {
                return carrera.IdEstadoInicialTesisNavigation;
            }

            var estado = await _context.EstadoSolicituds
                .FirstOrDefaultAsync(e => (e.Activo == null || e.Activo == "S") && e.Nombre.Contains("PENDIENTE"));

            if (estado != null)
            {
                return estado;
            }

            estado = await _context.EstadoSolicituds
                .FirstOrDefaultAsync(e => e.Activo == null || e.Activo == "S");

            if (estado == null)
            {
                throw new InvalidOperationException("No existe un estado de solicitud activo configurado.");
            }

            return estado;
        }

        private async Task GuardarDocumentos(int idSolicitudTesis, List<EntradaDocDocumentoRequerido> documentos)
        {
            foreach (var documento in documentos)
            {
                var archivo = Request.Form.Files.FirstOrDefault(f => f.Name == NombreInputDocumento(documento.IdTipoDocumento));
                if (archivo == null || archivo.Length == 0)
                {
                    continue;
                }

                var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
                byte[] contenido;

                await using (var memoryStream = new MemoryStream())
                {
                    await archivo.CopyToAsync(memoryStream);
                    contenido = memoryStream.ToArray();
                }

                _context.SolicitudDocumentos.Add(new SolicitudDocumento
                {
                    IdSolicitudTesis = idSolicitudTesis,
                    IdTipoDocumento = documento.IdTipoDocumento,
                    NombreArchivo = archivo.FileName,
                    RutaArchivo = "BD",
                    Extension = extension.TrimStart('.').ToUpperInvariant(),
                    ArchivoContenido = contenido,
                    ContentType = string.IsNullOrWhiteSpace(archivo.ContentType) ? ObtenerContentType(extension) : archivo.ContentType,
                    TamanoKb = (int)Math.Ceiling(archivo.Length / 1024.0),
                    FechaCarga = DateTime.Now,
                    IdUsuarioCarga = ObtenerIdUsuario(),
                    Activo = "S"
                });
            }
        }

        private async Task RegistrarHistorial(int idSolicitudTesis, string estadoNuevo)
        {
            _context.HistorialSolicituds.Add(new HistorialSolicitud
            {
                IdSolicitudTesis = idSolicitudTesis,
                IdUsuario = ObtenerIdUsuario(),
                FechaMovimiento = DateTime.Now,
                Accion = "REGISTRO_RECEPCION",
                Observacion = "Solicitud registrada por recepción.",
                EstadoNuevo = estadoNuevo
            });

            await Task.CompletedTask;
        }

        private string ObtenerIdUsuario()
        {
            return User.FindFirstValue("IdUsuario") ?? User.Identity?.Name ?? "admin";
        }

        private static string NombreInputDocumento(int idTipoDocumento)
        {
            return $"documento_{idTipoDocumento}";
        }

        private async Task<List<string>> ObtenerExtensionesPermitidas()
        {
            return await _context.TipoExtensions
                .AsNoTracking()
                .Where(e => e.Activo == null || e.Activo == "S")
                .OrderBy(e => e.Extension)
                .Select(e => e.Extension.ToLower())
                .ToListAsync();
        }

        private async Task<string> ObtenerAcceptExtensiones()
        {
            var extensiones = await ObtenerExtensionesPermitidas();
            return string.Join(",", extensiones);
        }

        private static string ObtenerContentType(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };
        }

        private static string LimpiarNombreArchivo(string nombre)
        {
            var caracteresInvalidos = Path.GetInvalidFileNameChars();
            var limpio = new string(nombre.Select(c => caracteresInvalidos.Contains(c) ? '_' : c).ToArray());
            limpio = limpio.Trim();

            return string.IsNullOrWhiteSpace(limpio) ? "archivo" : limpio;
        }
    }
}
