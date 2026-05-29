using System;
using System.Collections.Generic;

namespace SistemaBase.Models
{
    public partial class Usuario
    {
        public Usuario()
        {
            HistorialSolicituds = new HashSet<HistorialSolicitud>();
            RevisionDecanos = new HashSet<RevisionDecano>();
            SolicitudDocumentos = new HashSet<SolicitudDocumento>();
        }

        public string IdUsuario { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public DateTime FecCreacion { get; set; }
        public string CodGrupo { get; set; } = null!;
        public string IdPersona { get; set; } = null!;
        public string? Activo { get; set; }

        public virtual GruposUsuario CodGrupoNavigation { get; set; } = null!;
        public virtual Persona IdPersonaNavigation { get; set; } = null!;
        public virtual ICollection<HistorialSolicitud> HistorialSolicituds { get; set; }
        public virtual ICollection<RevisionDecano> RevisionDecanos { get; set; }
        public virtual ICollection<SolicitudDocumento> SolicitudDocumentos { get; set; }
    }
}
