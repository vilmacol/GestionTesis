using System;
using System.Collections.Generic;

namespace SistemaBase.Models
{
    public partial class EstadoSolicitud
    {
        public EstadoSolicitud()
        {
            GruposUsuarios = new HashSet<GruposUsuario>();
            CarreraIdEstadoInicialTesisNavigations = new HashSet<Carrera>();
            CarreraFlujoTransicionIdEstadoDestinoNavigations = new HashSet<CarreraFlujoTransicion>();
            CarreraFlujoTransicionIdEstadoOrigenNavigations = new HashSet<CarreraFlujoTransicion>();
            SolicitudTesis = new HashSet<SolicitudTesi>();
        }

        public int IdEstadoSolicitud { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string? Activo { get; set; }

        public virtual ICollection<GruposUsuario> GruposUsuarios { get; set; }
        public virtual ICollection<Carrera> CarreraIdEstadoInicialTesisNavigations { get; set; }
        public virtual ICollection<CarreraFlujoTransicion> CarreraFlujoTransicionIdEstadoDestinoNavigations { get; set; }
        public virtual ICollection<CarreraFlujoTransicion> CarreraFlujoTransicionIdEstadoOrigenNavigations { get; set; }
        public virtual ICollection<SolicitudTesi> SolicitudTesis { get; set; }
    }
}
