using System;
using System.Collections.Generic;

namespace SistemaBase.Models
{
    public partial class TipoExtension
    {
        public int IdTipoExtension { get; set; }
        public string Extension { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string? MimeType { get; set; }
        public string? Activo { get; set; }
    }
}
