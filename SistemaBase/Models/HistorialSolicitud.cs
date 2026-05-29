using System;
using System.Collections.Generic;

namespace SistemaBase.Models
{
    public partial class HistorialSolicitud
    {
        public int IdHistorialSolicitud { get; set; }
        public int IdSolicitudTesis { get; set; }
        public string IdUsuario { get; set; } = null!;
        public DateTime FechaMovimiento { get; set; }
        public string Accion { get; set; } = null!;
        public string? Observacion { get; set; }
        public string? EstadoAnterior { get; set; }
        public string? EstadoNuevo { get; set; }

        public virtual SolicitudTesi IdSolicitudTesisNavigation { get; set; } = null!;
        public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
    }
}
