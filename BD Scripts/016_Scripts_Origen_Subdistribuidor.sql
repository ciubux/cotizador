
/* **** 1 **** */
CREATE TABLE [SUBDISTRIBUIDOR](
	[id_subdistribuidor] [int] IDENTITY(1,1) NOT NULL,
	[codigo] [varchar](20) NULL,
	[nombre] [varchar](100) NOT NULL,
	[estado] [smallint] NOT NULL,
	[usuario_creacion] [uniqueidentifier] NOT NULL,
	[fecha_creacion] [datetime] NOT NULL,
	[usuario_modificacion] [uniqueidentifier] NULL,
	[fecha_modificacion] [datetime] NULL,
 CONSTRAINT [PK_SUBDISTRIBUIDOR] PRIMARY KEY CLUSTERED 
(
	[id_subdistribuidor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


/* **** 2 **** */
CREATE TABLE [dbo].[ORIGEN](
	[id_origen] [int] IDENTITY(1,1) NOT NULL,
	[codigo] [varchar](20) NOT NULL,
	[nombre] [varchar](100) NOT NULL,
	[estado] [smallint] NOT NULL,
	[usuario_creacion] [uniqueidentifier] NOT NULL,
	[fecha_creacion] [datetime] NOT NULL,
	[usuario_modificacion] [uniqueidentifier] NULL,
	[fecha_modificacion] [datetime] NULL,
 CONSTRAINT [PK_ORIGEN] PRIMARY KEY CLUSTERED 
(
	[id_origen] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]




/* **** 3 **** */
CREATE PROCEDURE ps_subdistribuidores
@estado int
AS
BEGIN

	SELECT 
	 id_subdistribuidor
      ,codigo
      ,nombre
      ,estado
	   FROM SUBDISTRIBUIDOR s
	where s.estado = @estado;

END






/* **** 4 **** */
CREATE PROCEDURE ps_origenes
@estado int
AS
BEGIN

	SELECT 
	 id_origen
      ,codigo
      ,nombre
      ,estado
	   FROM ORIGEN o
	where o.estado = @estado;

END





/* **** 5 **** */
CREATE PROCEDURE ps_subdistribuidor
@idSubDistribuidor int
AS
BEGIN

	SELECT 
	 id_subdistribuidor
      ,codigo
      ,nombre
      ,estado
	   FROM SUBDISTRIBUIDOR s
	where s.id_subdistribuidor = @idSubDistribuidor;

END





/* **** 6 **** */
CREATE PROCEDURE ps_origen
@idOrigen int
AS
BEGIN

	SELECT 
	 id_origen
      ,codigo
      ,nombre
      ,estado
	   FROM ORIGEN o
	where o.id_origen = @idOrigen;

END