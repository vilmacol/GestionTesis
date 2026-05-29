using System;
using System.Collections.Generic;

namespace SistemaBase.Models
{
    public partial class TipoDocumento
    {
        public TipoDocumento()
        {
            CarreraDocumentoRequeridos = new HashSet<CarreraDocumentoRequerido>();
            SolicitudDocumentos = new HashSet<SolicitudDocumento>();
        }

        public int IdTipoDocumento { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string? Activo { get; set; }

        public virtual ICollection<CarreraDocumentoRequerido> CarreraDocumentoRequeridos { get; set; }
        public virtual ICollection<SolicitudDocumento> SolicitudDocumentos { get; set; }
    }
}
