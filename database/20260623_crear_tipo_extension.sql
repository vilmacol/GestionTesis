IF OBJECT_ID('dbo.TIPO_EXTENSION', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TIPO_EXTENSION
    (
        IdTipoExtension INT IDENTITY(1,1) NOT NULL,
        Extension VARCHAR(10) NOT NULL,
        Descripcion VARCHAR(100) NULL,
        MimeType VARCHAR(150) NULL,
        Activo CHAR(1) NULL CONSTRAINT DF_TIPO_EXTENSION_ACTIVO DEFAULT ('S'),
        CONSTRAINT PK_TIPO_EXTENSION PRIMARY KEY (IdTipoExtension),
        CONSTRAINT UQ_TIPO_EXTENSION_EXTENSION UNIQUE (Extension)
    );
END;

INSERT INTO dbo.TIPO_EXTENSION (Extension, Descripcion, MimeType, Activo)
SELECT v.Extension, v.Descripcion, v.MimeType, 'S'
FROM (VALUES
    ('.pdf',  'Documento PDF',           'application/pdf'),
    ('.doc',  'Documento Word 97-2003',  'application/msword'),
    ('.docx', 'Documento Word',          'application/vnd.openxmlformats-officedocument.wordprocessingml.document'),
    ('.xls',  'Planilla Excel 97-2003',  'application/vnd.ms-excel'),
    ('.xlsx', 'Planilla Excel',          'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet')
) AS v (Extension, Descripcion, MimeType)
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.TIPO_EXTENSION te
    WHERE te.Extension = v.Extension
);
