using System;
using System.Collections.Generic;

namespace SistemaBase.Models
{
    public partial class SolicitudDocumento
    {
        public int IdSolicitudDocumento { get; set; }
        public int IdSolicitudTesis { get; set; }
        public int IdTipoDocumento { get; set; }
        public string NombreArchivo { get; set; } = null!;
        public string RutaArchivo { get; set; } = null!;
        public string Extension { get; set; } = null!;
        public int? TamanoKb { get; set; }
        public DateTime FechaCarga { get; set; }
        public string IdUsuarioCarga { get; set; } = null!;
        public string? Observacion { get; set; }
        public string? Activo { get; set; }

        public virtual SolicitudTesi IdSolicitudTesisNavigation { get; set; } = null!;
        public virtual TipoDocumento IdTipoDocumentoNavigation { get; set; } = null!;
        public virtual Usuario IdUsuarioCargaNavigation { get; set; } = null!;
    }
}
