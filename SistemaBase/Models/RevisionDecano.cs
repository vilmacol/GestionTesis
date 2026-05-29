using System;
using System.Collections.Generic;

namespace SistemaBase.Models
{
    public partial class RevisionDecano
    {
        public int IdRevisionDecano { get; set; }
        public int IdSolicitudTesis { get; set; }
        public string IdUsuarioDecano { get; set; } = null!;
        public string EstadoRevision { get; set; } = null!;
        public string? Comentario { get; set; }
        public DateTime FechaRevision { get; set; }

        public virtual SolicitudTesi IdSolicitudTesisNavigation { get; set; } = null!;
        public virtual Usuario IdUsuarioDecanoNavigation { get; set; } = null!;
    }
}
