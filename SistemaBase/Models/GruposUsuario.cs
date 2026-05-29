using System;
using System.Collections.Generic;

namespace SistemaBase.Models
{
    public partial class GruposUsuario
    {
        public GruposUsuario()
        {
            Usuarios = new HashSet<Usuario>();
            Formas = new HashSet<Forma>();
        }

        public string IdGrupo { get; set; } = null!;
        public string? Descripcion { get; set; }

        public virtual ICollection<Usuario> Usuarios { get; set; }

        public virtual ICollection<Forma> Formas { get; set; }
    }
}
