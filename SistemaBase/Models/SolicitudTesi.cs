using System;
using System.Collections.Generic;

namespace SistemaBase.Models
{
    public partial class SolicitudTesi
    {
        public SolicitudTesi()
        {
            HistorialSolicituds = new HashSet<HistorialSolicitud>();
            RevisionDecanos = new HashSet<RevisionDecano>();
            SolicitudDocumentos = new HashSet<SolicitudDocumento>();
        }

        public int IdSolicitudTesis { get; set; }
        public int IdAlumno { get; set; }
        public int? IdTutor { get; set; }
        public int IdEstadoSolicitud { get; set; }
        public string? TituloTentativo { get; set; }
        public string? Tema { get; set; }
        public string? ObservacionRecepcion { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaAsignacionTutor { get; set; }
        public DateTime? FechaEnvioDecano { get; set; }
        public DateTime? FechaRespuestaDecano { get; set; }
        public DateTime? FechaPresentacion { get; set; }
        public string? Activo { get; set; }

        public virtual Alumno IdAlumnoNavigation { get; set; } = null!;
        public virtual EstadoSolicitud IdEstadoSolicitudNavigation { get; set; } = null!;
        public virtual Tutor? IdTutorNavigation { get; set; }
        public virtual ICollection<HistorialSolicitud> HistorialSolicituds { get; set; }
        public virtual ICollection<RevisionDecano> RevisionDecanos { get; set; }
        public virtual ICollection<SolicitudDocumento> SolicitudDocumentos { get; set; }
    }
}
