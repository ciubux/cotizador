
/* **** 1 **** */
CREATE TABLE [RUBRO](
	[id_rubro] [int] IDENTITY(1,1) NOT NULL,
	[codigo] [varchar](3) NULL,
	[nombre] [varchar](200) NOT NULL,
	[estado] [smallint] NOT NULL,
	[usuario_creacion] [uniqueidentifier] NOT NULL,
	[fecha_creacion] [datetime] NOT NULL,
	[usuario_modificacion] [uniqueidentifier] NULL,
	[fecha_modificacion] [datetime] NULL,
 CONSTRAINT [PK_SUBDISTRIBUIDOR] PRIMARY KEY CLUSTERED 
(
	[id_rubro] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


/* **** 2 **** */
CREATE PROCEDURE ps_rubros
@estado int
AS
BEGIN

	SELECT 
	 id_rubro
      ,codigo
      ,nombre
      ,estado
	   FROM RUBRO r
	where r.estado = @estado;

END



/* **** 3 **** */
CREATE PROCEDURE ps_rubro
@idRubro int
AS
BEGIN

	SELECT 
	 id_rubro
      ,codigo
      ,nombre
      ,estado
	   FROM RUBRO r
	where r.id_rubro = @idRubro;

END




/* **** 4 **** */
CREATE PROCEDURE [dbo].[pi_rubro] 
@codigo varchar(3),
@nombre  varchar(200),
@estado  int,
@idUsuario uniqueidentifier,
@newId int OUTPUT 

AS
BEGIN

	INSERT INTO RUBRO
           (codigo
			,nombre 
			,estado
			,usuario_creacion
			,fecha_creacion
			,usuario_modificacion
			,fecha_modificacion
		   )
     VALUES
           (@codigo,
			@nombre,
			@estado,
			@idUsuario,
			dbo.getlocaldate(),
			@idUsuario,
			dbo.getlocaldate()
			);

	SET NOCOUNT ON
	SET @newId = SCOPE_IDENTITY();

END





/* **** 5 **** */
CREATE PROCEDURE [dbo].[pu_rubro] 
@idRubro int,
@codigo varchar(20),
@nombre  varchar(500),
@estado  int,
@idUsuario uniqueidentifier

AS
BEGIN

UPDATE SUBDISTRIBUIDOR  
	SET codigo = @codigo 
		,nombre = @nombre
		,estado = @estado
		,usuario_modificacion = @idUsuario
		,fecha_modificacion = dbo.getlocaldate()
	WHERE id_rubro like @idRubro;

END




/* **** 6 **** */
ALTER TABLE CLIENTE
ADD id_rubro int null;



/* **** 7 **** */
CREATE PROCEDURE ps_contarRubros
AS
BEGIN

	SELECT 
	  COUNT(id_rubro) as cantidadRubros 
	   FROM RUBRO r;

END