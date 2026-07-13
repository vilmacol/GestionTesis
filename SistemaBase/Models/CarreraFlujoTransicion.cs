using System;
using System.Collections.Generic;

namespace SistemaBase.Models
{
    public partial class CarreraFlujoTransicion
    {
        public int IdCarreraFlujoTransicion { get; set; }
        public int IdCarrera { get; set; }
        public int IdEstadoOrigen { get; set; }
        public int IdEstadoDestino { get; set; }
        public string Accion { get; set; } = null!;
        public bool RequiereComentario { get; set; }
        public bool RequiereFechaPresentacion { get; set; }
        public string? Activo { get; set; }

        public virtual Carrera IdCarreraNavigation { get; set; } = null!;
        public virtual EstadoSolicitud IdEstadoDestinoNavigation { get; set; } = null!;
        public virtual EstadoSolicitud IdEstadoOrigenNavigation { get; set; } = null!;
    }
}
