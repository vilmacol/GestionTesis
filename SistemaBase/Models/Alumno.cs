using System;
using System.Collections.Generic;

namespace SistemaBase.Models
{
    public partial class Alumno
    {
        public Alumno()
        {
            SolicitudTesis = new HashSet<SolicitudTesi>();
        }

        public int IdAlumno { get; set; }
        public int IdCarrera { get; set; }
        public string NroDocumento { get; set; } = null!;
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string RegistroAcademico { get; set; } = null!;
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string? Activo { get; set; }
        public DateTime FechaRegistro { get; set; }

        public virtual Carrera IdCarreraNavigation { get; set; } = null!;
        public virtual ICollection<SolicitudTesi> SolicitudTesis { get; set; }
    }
}
