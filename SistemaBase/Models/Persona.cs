using System;
using System.Collections.Generic;

namespace SistemaBase.Models
{
    public partial class Persona
    {
        public Persona()
        {
            Usuarios = new HashSet<Usuario>();
        }

        public string IdPersona { get; set; } = null!;
        public string? Nombre { get; set; }
        public string? Sexo { get; set; }
        public DateTime? FecNacimiento { get; set; }
        public DateTime? FecAlta { get; set; }
        public DateTime? FecActualizacion { get; set; }
        public string? EstadoCivil { get; set; }
        public string? Email { get; set; }
        public string? DireccionParticular { get; set; }
        public string? SitioWeb { get; set; }

        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
