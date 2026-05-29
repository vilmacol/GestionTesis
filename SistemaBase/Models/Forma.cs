using System;
using System.Collections.Generic;

namespace SistemaBase.Models
{
    public partial class Forma
    {
        public Forma()
        {
            IdGrupos = new HashSet<GruposUsuario>();
        }

        public string IdModulo { get; set; } = null!;
        public string NomForma { get; set; } = null!;
        public string? Descripcion { get; set; }

        public virtual Modulo IdModuloNavigation { get; set; } = null!;

        public virtual ICollection<GruposUsuario> IdGrupos { get; set; }
    }
}
