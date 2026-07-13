# DER - Gestion de Tesis

Este DER esta basado en las entidades y relaciones configuradas en `SistemaBase/Models/UAADbContext.cs`.

```mermaid
erDiagram
    CARRERA ||--o{ ALUMNO : "tiene"
    CARRERA ||--o{ CARRERA_DOCUMENTO_REQUERIDO : "requiere"
    TIPO_DOCUMENTO ||--o{ CARRERA_DOCUMENTO_REQUERIDO : "configura"

    ALUMNO ||--o{ SOLICITUD_TESIS : "registra"
    TUTOR ||--o{ SOLICITUD_TESIS : "tutor tentativo"
    ESTADO_SOLICITUD ||--o{ SOLICITUD_TESIS : "estado actual"

    SOLICITUD_TESIS ||--o{ SOLICITUD_DOCUMENTO : "adjunta"
    TIPO_DOCUMENTO ||--o{ SOLICITUD_DOCUMENTO : "clasifica"
    USUARIOS ||--o{ SOLICITUD_DOCUMENTO : "carga"

    SOLICITUD_TESIS ||--o{ HISTORIAL_SOLICITUD : "movimientos"
    USUARIOS ||--o{ HISTORIAL_SOLICITUD : "registra"

    SOLICITUD_TESIS ||--o{ REVISION_DECANO : "revision"
    USUARIOS ||--o{ REVISION_DECANO : "decano"

    PERSONAS ||--o{ USUARIOS : "credenciales"
    GRUPOS_USUARIOS ||--o{ USUARIOS : "agrupa"

    MODULOS ||--o{ FORMAS : "contiene"
    GRUPOS_USUARIOS ||--o{ ACCESOS_GRUPOS : "permisos"
    FORMAS ||--o{ ACCESOS_GRUPOS : "pantallas"

    CARRERA {
        int IdCarrera PK
        varchar DESCRIPCION
    }

    ALUMNO {
        int IdAlumno PK
        int IdCarrera FK
        varchar NroDocumento UK
        varchar Nombres
        varchar Apellidos
        varchar RegistroAcademico UK
        varchar Correo
        varchar Telefono
        char Activo
        datetime FechaRegistro
    }

    TUTOR {
        int IdTutor PK
        varchar NombreCompleto
        varchar Correo
        varchar Telefono
        varchar Especialidad
        char Activo
        datetime FechaRegistro
    }

    ESTADO_SOLICITUD {
        int IdEstadoSolicitud PK
        varchar Nombre UK
        varchar Descripcion
        char Activo
    }

    SOLICITUD_TESIS {
        int IdSolicitudTesis PK
        int IdAlumno FK
        int IdTutor FK
        int IdEstadoSolicitud FK
        varchar TituloTentativo
        varchar Tema
        varchar ObservacionRecepcion
        datetime FechaSolicitud
        datetime FechaAsignacionTutor
        datetime FechaEnvioDecano
        datetime FechaRespuestaDecano
        char Activo
    }

    TIPO_DOCUMENTO {
        int IdTipoDocumento PK
        varchar Nombre
        varchar Descripcion
        char Activo
    }

    CARRERA_DOCUMENTO_REQUERIDO {
        int IdCarreraDocumentoRequerido PK
        int IdCarrera FK
        int IdTipoDocumento FK
        bit Obligatorio
        varchar Observacion
        char Activo
    }

    SOLICITUD_DOCUMENTO {
        int IdSolicitudDocumento PK
        int IdSolicitudTesis FK
        int IdTipoDocumento FK
        varchar NombreArchivo
        varchar RutaArchivo
        varchar Extension
        int TamanoKB
        datetime FechaCarga
        varchar IdUsuarioCarga FK
        varchar Observacion
        char Activo
    }

    HISTORIAL_SOLICITUD {
        int IdHistorialSolicitud PK
        int IdSolicitudTesis FK
        varchar IdUsuario FK
        datetime FechaMovimiento
        varchar Accion
        varchar Observacion
        varchar EstadoAnterior
        varchar EstadoNuevo
    }

    REVISION_DECANO {
        int IdRevisionDecano PK
        int IdSolicitudTesis FK
        varchar IdUsuarioDecano FK
        datetime FechaRevision
        varchar EstadoRevision
        varchar Comentario
    }

    PERSONAS {
        varchar IdPersona PK
        varchar NOMBRE
        varchar SEXO
        datetime FEC_NACIMIENTO
        datetime FEC_ALTA
        datetime FEC_ACTUALIZACION
        varchar ESTADO_CIVIL
        varchar EMAIL
        varchar DIRECCION_PARTICULAR
        varchar SITIO_WEB
    }

    USUARIOS {
        varchar IdUsuario PK
        varchar PASSWORD_HASH
        datetime FEC_CREACION
        varchar COD_GRUPO FK
        varchar IdPersona FK
        char ACTIVO
    }

    GRUPOS_USUARIOS {
        varchar IdGrupo PK
        varchar DESCRIPCION
    }

    MODULOS {
        varchar IdModulo PK
        varchar DESCRIPCION
    }

    FORMAS {
        varchar IdModulo PK,FK
        varchar NOM_FORMA PK
        varchar DESCRIPCION
    }

    ACCESOS_GRUPOS {
        varchar IdGrupo PK,FK
        varchar IdModulo PK,FK
        varchar NOM_FORMA PK,FK
    }
```

## Relaciones principales

| Tabla origen | Tabla destino | Relacion |
|---|---|---|
| `CARRERA` | `ALUMNO` | Una carrera tiene muchos alumnos |
| `CARRERA` | `CARRERA_DOCUMENTO_REQUERIDO` | Una carrera configura muchos documentos requeridos |
| `TIPO_DOCUMENTO` | `CARRERA_DOCUMENTO_REQUERIDO` | Un tipo de documento puede requerirse en muchas carreras |
| `ALUMNO` | `SOLICITUD_TESIS` | Un alumno puede registrar muchas solicitudes |
| `TUTOR` | `SOLICITUD_TESIS` | Un tutor puede estar asociado a muchas solicitudes |
| `ESTADO_SOLICITUD` | `SOLICITUD_TESIS` | Un estado puede estar asignado a muchas solicitudes |
| `SOLICITUD_TESIS` | `SOLICITUD_DOCUMENTO` | Una solicitud tiene muchos documentos cargados |
| `TIPO_DOCUMENTO` | `SOLICITUD_DOCUMENTO` | Un documento cargado corresponde a un tipo |
| `USUARIOS` | `SOLICITUD_DOCUMENTO` | Un usuario carga muchos documentos |
| `SOLICITUD_TESIS` | `HISTORIAL_SOLICITUD` | Una solicitud tiene muchos movimientos de historial |
| `USUARIOS` | `HISTORIAL_SOLICITUD` | Un usuario registra movimientos |
| `SOLICITUD_TESIS` | `REVISION_DECANO` | Una solicitud puede tener revisiones |
| `USUARIOS` | `REVISION_DECANO` | Un usuario decano registra revisiones |
| `PERSONAS` | `USUARIOS` | Una persona puede tener usuario |
| `GRUPOS_USUARIOS` | `USUARIOS` | Un grupo tiene muchos usuarios |
| `MODULOS` | `FORMAS` | Un modulo contiene muchas pantallas |
| `GRUPOS_USUARIOS` | `FORMAS` | Relacion muchos a muchos mediante `ACCESOS_GRUPOS` |

