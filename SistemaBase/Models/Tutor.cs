using System;
using System.Collections.Generic;

namespace SistemaBase.Models
{
    public partial class Tutor
    {
        public Tutor()
        {
            SolicitudTesis = new HashSet<SolicitudTesi>();
        }

        public int IdTutor { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string? Especialidad { get; set; }
        public string? Activo { get; set; }
        public DateTime FechaRegistro { get; set; }

        public virtual ICollection<SolicitudTesi> SolicitudTesis { get; set; }
    }
}
