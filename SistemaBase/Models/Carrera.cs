using System;
using System.Collections.Generic;

namespace SistemaBase.Models
{
    public partial class Carrera
    {
        public Carrera()
        {
            Alumnos = new HashSet<Alumno>();
            CarreraDocumentoRequeridos = new HashSet<CarreraDocumentoRequerido>();
            CarreraFlujoTransicions = new HashSet<CarreraFlujoTransicion>();
        }

        public int IdCarrera { get; set; }
        public string Descripcion { get; set; } = null!;
        public int? IdFacultad { get; set; }
        public int? IdEstadoInicialTesis { get; set; }

        public virtual EstadoSolicitud? IdEstadoInicialTesisNavigation { get; set; }
        public virtual Facultad? IdFacultadNavigation { get; set; }
        public virtual ICollection<Alumno> Alumnos { get; set; }
        public virtual ICollection<CarreraDocumentoRequerido> CarreraDocumentoRequeridos { get; set; }
        public virtual ICollection<CarreraFlujoTransicion> CarreraFlujoTransicions { get; set; }
    }
}
