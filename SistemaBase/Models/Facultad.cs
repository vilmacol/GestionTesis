using System;
using System.Collections.Generic;

namespace SistemaBase.Models
{
    public partial class Facultad
    {
        public Facultad()
        {
            Carreras = new HashSet<Carrera>();
            GruposUsuarios = new HashSet<GruposUsuario>();
        }

        public int IdFacultad { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? Activo { get; set; }

        public virtual ICollection<Carrera> Carreras { get; set; }
        public virtual ICollection<GruposUsuario> GruposUsuarios { get; set; }
    }
}
