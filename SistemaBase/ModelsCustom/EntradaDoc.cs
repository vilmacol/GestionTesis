using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaBase.ModelsCustom
{
    public class EntradaDoc
    {
        public string NombreAlumno { get; set; } = string.Empty;
        public string NroDocumento { get; set; } = string.Empty;
        public int IdCarrera { get; set; }
        public string? RegistroAcademico { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string? Tema { get; set; }
        public string? ObservacionRecepcion { get; set; }
        public int? IdTutor { get; set; }
        public List<SelectListItem> Carreras { get; set; } = new();
        public List<SelectListItem> Tutores { get; set; } = new();
        public List<EntradaDocDocumentoRequerido> DocumentosRequeridos { get; set; } = new();
        public string ExtensionesPermitidas { get; set; } = string.Empty;
    }

    public class EntradaDocDocumentoRequerido
    {
        public int IdTipoDocumento { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Observacion { get; set; }
        public bool Obligatorio { get; set; }
    }
}
