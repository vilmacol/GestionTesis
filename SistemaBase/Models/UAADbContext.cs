using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SistemaBase.Models
{
    public partial class UAADbContext : DbContext
    {
        public UAADbContext()
        {
        }

        public UAADbContext(DbContextOptions<UAADbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Alumno> Alumnos { get; set; } = null!;
        public virtual DbSet<Carrera> Carreras { get; set; } = null!;
        public virtual DbSet<CarreraDocumentoRequerido> CarreraDocumentoRequeridos { get; set; } = null!;
        public virtual DbSet<CarreraFlujoTransicion> CarreraFlujoTransicions { get; set; } = null!;
        public virtual DbSet<EstadoSolicitud> EstadoSolicituds { get; set; } = null!;
        public virtual DbSet<Facultad> Facultads { get; set; } = null!;
        public virtual DbSet<Forma> Formas { get; set; } = null!;
        public virtual DbSet<GruposUsuario> GruposUsuarios { get; set; } = null!;
        public virtual DbSet<HistorialSolicitud> HistorialSolicituds { get; set; } = null!;
        public virtual DbSet<Modulo> Modulos { get; set; } = null!;
        public virtual DbSet<Persona> Personas { get; set; } = null!;
        public virtual DbSet<RevisionDecano> RevisionDecanos { get; set; } = null!;
        public virtual DbSet<SolicitudDocumento> SolicitudDocumentos { get; set; } = null!;
        public virtual DbSet<SolicitudTesi> SolicitudTeses { get; set; } = null!;
        public virtual DbSet<TipoDocumento> TipoDocumentos { get; set; } = null!;
        public virtual DbSet<TipoExtension> TipoExtensions { get; set; } = null!;
        public virtual DbSet<Tutor> Tutors { get; set; } = null!;
        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-KP48E0B\\SQLEXPRESS;Database=GestionTesis;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=True;TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Alumno>(entity =>
            {
                entity.HasKey(e => e.IdAlumno)
                    .HasName("PK__ALUMNO__460B4740260C5C20");

                entity.ToTable("ALUMNO");

                entity.HasIndex(e => e.NroDocumento, "IX_ALUMNO_NRODOCUMENTO")
                    .IsUnique();

                entity.HasIndex(e => e.RegistroAcademico, "IX_ALUMNO_REGISTRO")
                    .IsUnique();

                entity.Property(e => e.Activo)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Apellidos)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Correo)
                    .HasMaxLength(120)
                    .IsUnicode(false);

                entity.Property(e => e.FechaRegistro)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Nombres)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.NroDocumento)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.RegistroAcademico)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Telefono)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCarreraNavigation)
                    .WithMany(p => p.Alumnos)
                    .HasForeignKey(d => d.IdCarrera)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ALUMNO_CARRERA");
            });

            modelBuilder.Entity<Carrera>(entity =>
            {
                entity.HasKey(e => e.IdCarrera)
                    .HasName("PK__CARRERA__884A8F1F55CC6E9C");

                entity.ToTable("CARRERA");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("DESCRIPCION");

                entity.HasOne(d => d.IdEstadoInicialTesisNavigation)
                    .WithMany(p => p.CarreraIdEstadoInicialTesisNavigations)
                    .HasForeignKey(d => d.IdEstadoInicialTesis)
                    .HasConstraintName("FK_CARRERA_ESTADO_INICIAL_TESIS");

                entity.HasOne(d => d.IdFacultadNavigation)
                    .WithMany(p => p.Carreras)
                    .HasForeignKey(d => d.IdFacultad)
                    .HasConstraintName("FK_CARRERA_FACULTAD");
            });

            modelBuilder.Entity<CarreraDocumentoRequerido>(entity =>
            {
                entity.HasKey(e => e.IdCarreraDocumentoRequerido)
                    .HasName("PK__CARRERA___221FC609BC6D93FE");

                entity.ToTable("CARRERA_DOCUMENTO_REQUERIDO");

                entity.HasIndex(e => new { e.IdCarrera, e.IdTipoDocumento }, "IX_CDR")
                    .IsUnique();

                entity.Property(e => e.Activo)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Obligatorio)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Observacion)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCarreraNavigation)
                    .WithMany(p => p.CarreraDocumentoRequeridos)
                    .HasForeignKey(d => d.IdCarrera)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CDR_CARRERA");

                entity.HasOne(d => d.IdTipoDocumentoNavigation)
                    .WithMany(p => p.CarreraDocumentoRequeridos)
                    .HasForeignKey(d => d.IdTipoDocumento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CDR_TIPO_DOCUMENTO");
            });

            modelBuilder.Entity<CarreraFlujoTransicion>(entity =>
            {
                entity.HasKey(e => e.IdCarreraFlujoTransicion)
                    .HasName("PK_CARRERA_FLUJO_TRANSICION");

                entity.ToTable("CARRERA_FLUJO_TRANSICION");

                entity.Property(e => e.Accion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Activo)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.RequiereComentario)
                    .IsRequired()
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.RequiereFechaPresentacion)
                    .IsRequired()
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.IdCarreraNavigation)
                    .WithMany(p => p.CarreraFlujoTransicions)
                    .HasForeignKey(d => d.IdCarrera)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CFT_CARRERA");

                entity.HasOne(d => d.IdEstadoDestinoNavigation)
                    .WithMany(p => p.CarreraFlujoTransicionIdEstadoDestinoNavigations)
                    .HasForeignKey(d => d.IdEstadoDestino)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CFT_ESTADO_DESTINO");

                entity.HasOne(d => d.IdEstadoOrigenNavigation)
                    .WithMany(p => p.CarreraFlujoTransicionIdEstadoOrigenNavigations)
                    .HasForeignKey(d => d.IdEstadoOrigen)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CFT_ESTADO_ORIGEN");
            });

            modelBuilder.Entity<EstadoSolicitud>(entity =>
            {
                entity.HasKey(e => e.IdEstadoSolicitud)
                    .HasName("PK__ESTADO_S__4148EDBC90A0BB2D");

                entity.ToTable("ESTADO_SOLICITUD");

                entity.HasIndex(e => e.Nombre, "UQ__ESTADO_S__75E3EFCF9C9F8A01")
                    .IsUnique();

                entity.Property(e => e.Activo)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasMany(d => d.GruposUsuarios)
                    .WithMany(p => p.EstadoSolicituds)
                    .UsingEntity<Dictionary<string, object>>(
                        "GruposEstadosSolicitud",
                        l => l.HasOne<GruposUsuario>().WithMany().HasForeignKey("IdGrupo").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_GES_GRUPO"),
                        r => r.HasOne<EstadoSolicitud>().WithMany().HasForeignKey("IdEstadoSolicitud").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_GES_ESTADO_SOLICITUD"),
                        j =>
                        {
                            j.HasKey("IdGrupo", "IdEstadoSolicitud");

                            j.ToTable("GRUPOS_ESTADOS_SOLICITUD");

                            j.IndexerProperty<string>("IdGrupo").HasMaxLength(6).IsUnicode(false);

                            j.IndexerProperty<int>("IdEstadoSolicitud");
                        });
            });

            modelBuilder.Entity<Forma>(entity =>
            {
                entity.HasKey(e => new { e.IdModulo, e.NomForma });

                entity.ToTable("FORMAS");

                entity.Property(e => e.IdModulo)
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.NomForma)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("NOM_FORMA");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .HasColumnName("DESCRIPCION");

                entity.HasOne(d => d.IdModuloNavigation)
                    .WithMany(p => p.Formas)
                    .HasForeignKey(d => d.IdModulo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FORMAS_MODULOS");
            });

            modelBuilder.Entity<Facultad>(entity =>
            {
                entity.HasKey(e => e.IdFacultad)
                    .HasName("PK_FACULTAD");

                entity.ToTable("FACULTAD");

                entity.HasIndex(e => e.Codigo, "UQ_FACULTAD_CODIGO")
                    .IsUnique();

                entity.Property(e => e.Activo)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Codigo)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(150)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<GruposUsuario>(entity =>
            {
                entity.HasKey(e => e.IdGrupo)
                    .HasName("PK__GRUPOS_U__303F6FD910E6B45D");

                entity.ToTable("GRUPOS_USUARIOS");

                entity.Property(e => e.IdGrupo)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("DESCRIPCION");

                entity.HasOne(d => d.IdFacultadNavigation)
                    .WithMany(p => p.GruposUsuarios)
                    .HasForeignKey(d => d.IdFacultad)
                    .HasConstraintName("FK_GRUPOS_USUARIOS_FACULTAD");

                entity.HasMany(d => d.Formas)
                    .WithMany(p => p.IdGrupos)
                    .UsingEntity<Dictionary<string, object>>(
                        "AccesosGrupo",
                        l => l.HasOne<Forma>().WithMany().HasForeignKey("IdModulo", "NomForma").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ACCESOS_GRUPOS_FORMA"),
                        r => r.HasOne<GruposUsuario>().WithMany().HasForeignKey("IdGrupo").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ACCESOS_GRUPOS_GRUPO"),
                        j =>
                        {
                            j.HasKey("IdGrupo", "IdModulo", "NomForma");

                            j.ToTable("ACCESOS_GRUPOS");

                            j.IndexerProperty<string>("IdGrupo").HasMaxLength(6).IsUnicode(false);

                            j.IndexerProperty<string>("IdModulo").HasMaxLength(2).IsUnicode(false);

                            j.IndexerProperty<string>("NomForma").HasMaxLength(10).IsUnicode(false).HasColumnName("NOM_FORMA");
                        });
            });

            modelBuilder.Entity<HistorialSolicitud>(entity =>
            {
                entity.HasKey(e => e.IdHistorialSolicitud)
                    .HasName("PK__HISTORIA__75162DB04C40BB34");

                entity.ToTable("HISTORIAL_SOLICITUD");

                entity.Property(e => e.Accion)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.EstadoAnterior)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EstadoNuevo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FechaMovimiento)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IdUsuario)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Observacion)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdSolicitudTesisNavigation)
                    .WithMany(p => p.HistorialSolicituds)
                    .HasForeignKey(d => d.IdSolicitudTesis)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HS_SOLICITUD");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.HistorialSolicituds)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HS_USUARIO");
            });

            modelBuilder.Entity<Modulo>(entity =>
            {
                entity.HasKey(e => e.IdModulo)
                    .HasName("PK__MODULOS__D9F153156F566A7B");

                entity.ToTable("MODULOS");

                entity.Property(e => e.IdModulo)
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .HasColumnName("DESCRIPCION");
            });

            modelBuilder.Entity<Persona>(entity =>
            {
                entity.HasKey(e => e.IdPersona)
                    .HasName("PK__PERSONAS__2EC8D2AC3CEFE30C");

                entity.ToTable("PERSONAS");

                entity.Property(e => e.IdPersona)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DireccionParticular)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("DIRECCION_PARTICULAR");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.EstadoCivil)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ESTADO_CIVIL");

                entity.Property(e => e.FecActualizacion)
                    .HasColumnType("datetime")
                    .HasColumnName("FEC_ACTUALIZACION");

                entity.Property(e => e.FecAlta)
                    .HasColumnType("datetime")
                    .HasColumnName("FEC_ALTA")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FecNacimiento)
                    .HasPrecision(0)
                    .HasColumnName("FEC_NACIMIENTO");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE");

                entity.Property(e => e.Sexo)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("SEXO");

                entity.Property(e => e.SitioWeb)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("SITIO_WEB");
            });

            modelBuilder.Entity<RevisionDecano>(entity =>
            {
                entity.HasKey(e => e.IdRevisionDecano)
                    .HasName("PK__REVISION__B20DAAF54CF8A968");

                entity.ToTable("REVISION_DECANO");

                entity.Property(e => e.Comentario)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.EstadoRevision)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.FechaRevision)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IdUsuarioDecano)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdSolicitudTesisNavigation)
                    .WithMany(p => p.RevisionDecanos)
                    .HasForeignKey(d => d.IdSolicitudTesis)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RD_SOLICITUD");

                entity.HasOne(d => d.IdUsuarioDecanoNavigation)
                    .WithMany(p => p.RevisionDecanos)
                    .HasForeignKey(d => d.IdUsuarioDecano)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RD_USUARIO");
            });

            modelBuilder.Entity<SolicitudDocumento>(entity =>
            {
                entity.HasKey(e => e.IdSolicitudDocumento)
                    .HasName("PK__SOLICITU__10514865142F002B");

                entity.ToTable("SOLICITUD_DOCUMENTO");

                entity.Property(e => e.Activo)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ArchivoContenido).HasColumnName("ArchivoContenido");

                entity.Property(e => e.ContentType)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Extension)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.FechaCarga)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IdUsuarioCarga)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.NombreArchivo)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Observacion)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.RutaArchivo)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TamanoKb).HasColumnName("TamanoKB");

                entity.HasOne(d => d.IdSolicitudTesisNavigation)
                    .WithMany(p => p.SolicitudDocumentos)
                    .HasForeignKey(d => d.IdSolicitudTesis)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SD_SOLICITUD");

                entity.HasOne(d => d.IdTipoDocumentoNavigation)
                    .WithMany(p => p.SolicitudDocumentos)
                    .HasForeignKey(d => d.IdTipoDocumento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SD_TIPO_DOCUMENTO");

                entity.HasOne(d => d.IdUsuarioCargaNavigation)
                    .WithMany(p => p.SolicitudDocumentos)
                    .HasForeignKey(d => d.IdUsuarioCarga)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SD_USUARIO");
            });

            modelBuilder.Entity<SolicitudTesi>(entity =>
            {
                entity.HasKey(e => e.IdSolicitudTesis)
                    .HasName("PK__SOLICITU__C8B51A43A751D7B2");

                entity.ToTable("SOLICITUD_TESIS");

                entity.Property(e => e.Activo)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.FechaAsignacionTutor).HasColumnType("datetime");

                entity.Property(e => e.FechaEnvioDecano).HasColumnType("datetime");

                entity.Property(e => e.FechaPresentacion).HasColumnType("datetime");

                entity.Property(e => e.FechaRespuestaDecano).HasColumnType("datetime");   

                entity.Property(e => e.FechaSolicitud)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ObservacionRecepcion)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Tema)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TituloTentativo)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdAlumnoNavigation)
                    .WithMany(p => p.SolicitudTesis)
                    .HasForeignKey(d => d.IdAlumno)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ST_ALUMNO");

                entity.HasOne(d => d.IdEstadoSolicitudNavigation)
                    .WithMany(p => p.SolicitudTesis)
                    .HasForeignKey(d => d.IdEstadoSolicitud)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ST_ESTADO");

                entity.HasOne(d => d.IdTutorNavigation)
                    .WithMany(p => p.SolicitudTesis)
                    .HasForeignKey(d => d.IdTutor)
                    .HasConstraintName("FK_ST_TUTOR");
            });

            modelBuilder.Entity<TipoDocumento>(entity =>
            {
                entity.HasKey(e => e.IdTipoDocumento)
                    .HasName("PK__TIPO_DOC__3AB3332FF98F16B4");

                entity.ToTable("TIPO_DOCUMENTO");

                entity.Property(e => e.Activo)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoExtension>(entity =>
            {
                entity.HasKey(e => e.IdTipoExtension)
                    .HasName("PK_TIPO_EXTENSION");

                entity.ToTable("TIPO_EXTENSION");

                entity.HasIndex(e => e.Extension, "UQ_TIPO_EXTENSION_EXTENSION")
                    .IsUnique();

                entity.Property(e => e.Activo)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Extension)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MimeType)
                    .HasMaxLength(150)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Tutor>(entity =>
            {
                entity.HasKey(e => e.IdTutor)
                    .HasName("PK__TUTOR__C168D388AE67D2F2");

                entity.ToTable("TUTOR");

                entity.Property(e => e.Activo)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Correo)
                    .HasMaxLength(120)
                    .IsUnicode(false);

                entity.Property(e => e.Especialidad)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.FechaRegistro)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.NombreCompleto)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Telefono)
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario)
                    .HasName("PK__USUARIOS__5B65BF97BF6D5D50");

                entity.ToTable("USUARIOS");

                entity.Property(e => e.IdUsuario)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Activo)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("ACTIVO")
                    .IsFixedLength();

                entity.Property(e => e.CodGrupo)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasColumnName("COD_GRUPO");

                entity.Property(e => e.FecCreacion)
                    .HasColumnType("datetime")
                    .HasColumnName("FEC_CREACION")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IdPersona)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("PASSWORD_HASH");

                entity.HasOne(d => d.CodGrupoNavigation)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.CodGrupo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_USUARIOS_GRUPO");

                entity.HasOne(d => d.IdPersonaNavigation)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.IdPersona)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_USUARIOS_PERSONA"); 
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
