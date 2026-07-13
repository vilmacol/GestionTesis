namespace SistemaBase.ModelsCustom
{
    public class SolicitudRevision
    {
        public int IdSolicitudTesis { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string Alumno { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public int IdCarrera { get; set; }
        public string Carrera { get; set; } = string.Empty;
        public string? Facultad { get; set; }
        public string RegistroAcademico { get; set; } = string.Empty;
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string? Tutor { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int IdEstadoSolicitud { get; set; }
        public List<SolicitudRevisionTransicion> Transiciones { get; set; } = new();
        public string? Tema { get; set; }
        public string? ObservacionRecepcion { get; set; }
        public DateTime? FechaAsignacionTutor { get; set; }
        public DateTime? FechaEnvioDecano { get; set; }
        public DateTime? FechaRespuestaDecano { get; set; }
        public DateTime? FechaPresentacion { get; set; }
        public List<SolicitudRevisionDocumento> Documentos { get; set; } = new();
        public List<SolicitudRevisionHistorial> Historial { get; set; } = new();
    }

    public class SolicitudRevisionDocumento
    {
        public int IdSolicitudDocumento { get; set; }
        public string TipoDocumento { get; set; } = string.Empty;
        public string NombreArchivo { get; set; } = string.Empty;
        public string RutaArchivo { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public bool TieneArchivoEnBase { get; set; }
        public int? TamanoKb { get; set; }
        public DateTime FechaCarga { get; set; }
        public string? Observacion { get; set; }
    }

    public class SolicitudRevisionHistorial
    {
        public DateTime FechaMovimiento { get; set; }
        public string Accion { get; set; } = string.Empty;
        public string? Observacion { get; set; }
        public string? EstadoAnterior { get; set; }
        public string? EstadoNuevo { get; set; }
        public string IdUsuario { get; set; } = string.Empty;
    }

    public class CambiarEstadoSolicitud
    {
        public int IdSolicitudTesis { get; set; }
        public int? IdCarreraFlujoTransicion { get; set; }
        public int IdEstadoSolicitudDestino { get; set; }
        public string? Comentario { get; set; }
        public DateTime? FechaPresentacion { get; set; }
    }

    public class SolicitudRevisionTransicion
    {
        public int IdCarreraFlujoTransicion { get; set; }
        public int IdEstadoDestino { get; set; }
        public string Accion { get; set; } = string.Empty;
        public string EstadoDestino { get; set; } = string.Empty;
        public bool RequiereComentario { get; set; }
        public bool RequiereFechaPresentacion { get; set; }
    }
}
