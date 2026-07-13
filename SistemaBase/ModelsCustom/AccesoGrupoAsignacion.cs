namespace SistemaBase.ModelsCustom
{
    public class AccesoGrupoAsignacion
    {
        public string IdGrupo { get; set; } = string.Empty;
        public string? DescripcionGrupo { get; set; }
        public List<AccesoGrupoForma> Formas { get; set; } = new();
        public List<AccesoGrupoEstadoSolicitud> EstadosSolicitud { get; set; } = new();
        public List<string> FormasSeleccionadas { get; set; } = new();
        public List<int> EstadosSolicitudSeleccionados { get; set; } = new();
    }

    public class AccesoGrupoForma
    {
        public string IdModulo { get; set; } = string.Empty;
        public string NomForma { get; set; } = string.Empty;
        public string? Modulo { get; set; }
        public string? Descripcion { get; set; }
        public bool Seleccionada { get; set; }
        public string Key => $"{IdModulo}|{NomForma}";
    }

    public class AccesoGrupoEstadoSolicitud
    {
        public int IdEstadoSolicitud { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Seleccionado { get; set; }
    }
}
