using System;
using System.Collections.Generic;

namespace SistemaBase.Models
{
    public partial class EstadoSolicitud
    {
        public EstadoSolicitud()
        {
            SolicitudTesis = new HashSet<SolicitudTesi>();
        }

        public int IdEstadoSolicitud { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string? Activo { get; set; }

        public virtual ICollection<SolicitudTesi> SolicitudTesis { get; set; }
    }
}
