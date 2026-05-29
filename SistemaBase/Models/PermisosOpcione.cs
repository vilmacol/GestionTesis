using System;
using System.Collections.Generic;

namespace SistemaBase.Models
{
    public partial class PermisosOpcione
    {
        public string CodEmpresa { get; set; } = null!;
        public string CodUsuario { get; set; } = null!;
        public string CodModulo { get; set; } = null!;
        public string Parametro { get; set; } = null!;
        public string NomForma { get; set; } = null!;
        public string? Permiso { get; set; }
        public Guid Rowid { get; set; }
    }
}
