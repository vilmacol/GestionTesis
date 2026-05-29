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
        }

        public int IdCarrera { get; set; }
        public string Descripcion { get; set; } = null!;

        public virtual ICollection<Alumno> Alumnos { get; set; }
        public virtual ICollection<CarreraDocumentoRequerido> CarreraDocumentoRequeridos { get; set; }
    }
}
