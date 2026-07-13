IF COL_LENGTH('dbo.CARRERA', 'IdEstadoInicialTesis') IS NULL
BEGIN
    ALTER TABLE dbo.CARRERA
    ADD IdEstadoInicialTesis INT NULL;
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_CARRERA_ESTADO_INICIAL_TESIS'
)
BEGIN
    ALTER TABLE dbo.CARRERA
    ADD CONSTRAINT FK_CARRERA_ESTADO_INICIAL_TESIS
        FOREIGN KEY (IdEstadoInicialTesis)
        REFERENCES dbo.ESTADO_SOLICITUD(IdEstadoSolicitud);
END;

IF OBJECT_ID('dbo.CARRERA_ESTADO_SOLICITUD', 'U') IS NOT NULL
BEGIN
    ;WITH PrimerPaso AS
    (
        SELECT
            IdCarrera,
            IdEstadoSolicitud,
            ROW_NUMBER() OVER (PARTITION BY IdCarrera ORDER BY Orden) AS Nro
        FROM dbo.CARRERA_ESTADO_SOLICITUD
        WHERE Activo IS NULL OR Activo = 'S'
    )
    UPDATE c
    SET IdEstadoInicialTesis = p.IdEstadoSolicitud
    FROM dbo.CARRERA c
    INNER JOIN PrimerPaso p ON p.IdCarrera = c.IdCarrera AND p.Nro = 1
    WHERE c.IdEstadoInicialTesis IS NULL;
END;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.MODULOS
    WHERE IdModulo = '01'
)
BEGIN
    INSERT INTO dbo.MODULOS (IdModulo, DESCRIPCION)
    VALUES ('01', 'Sistema');
END;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.FORMAS
    WHERE IdModulo = '01' AND NOM_FORMA = 'FLUJOTES'
)
BEGIN
    INSERT INTO dbo.FORMAS (IdModulo, NOM_FORMA, DESCRIPCION)
    VALUES ('01', 'FLUJOTES', 'Flujo de Tesis');
END;
