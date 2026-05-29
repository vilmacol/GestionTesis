using System;
using System.Collections.Generic;

namespace SistemaBase.Models
{
    public partial class CarreraDocumentoRequerido
    {
        public int IdCarreraDocumentoRequerido { get; set; }
        public int IdCarrera { get; set; }
        public int IdTipoDocumento { get; set; }
        public bool? Obligatorio { get; set; }
        public string? Observacion { get; set; }
        public string? Activo { get; set; }

        public virtual Carrera IdCarreraNavigation { get; set; } = null!;
        public virtual TipoDocumento IdTipoDocumentoNavigation { get; set; } = null!;
    }
}
