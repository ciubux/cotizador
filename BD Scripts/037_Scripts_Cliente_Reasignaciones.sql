/* **** 1 **** */
CREATE TABLE [CLIENTE_HISTORIAL_REASIGNACION](
	[id_cliente_historial_reasignacion] [uniqueidentifier] NOT NULL,
	[id_cliente] [uniqueidentifier] NOT NULL,
	[fecha_inicio_vigencia] [date] NULL,
	[campo] [varchar](100) NULL,
	[valor] [varchar](250) NULL,
	[observacion] [varchar](500) NOT NULL,
	[estado] [smallint] NOT NULL,
	[usuario_modificacion] [uniqueidentifier] NULL,
	[fecha_modificacion] [datetime] NULL
 CONSTRAINT [PK_CLIENTE_HISTORIAL_REASIGNACIONES] PRIMARY KEY CLUSTERED 
(
	[id_cliente_historial_reasignacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



/* **** 2 **** */
CREATE PROCEDURE [dbo].[pi_cliente_historial_reasignacion] 
@idCliente uniqueidentifier,
@campo varchar(100),
@valor varchar(250),
@fechaInicioVigencia date,
@observacion varchar(500),
@idUsuario uniqueidentifier,
@newId uniqueidentifier OUTPUT 

AS
BEGIN
	SET NOCOUNT ON
	SET @newId = NEWID();
	
	INSERT INTO CLIENTE_HISTORIAL_REASIGNACION
           (id_cliente_historial_reasignacion
			,id_cliente 
			,fecha_inicio_vigencia
			,campo
			,valor
			,observacion
			,estado
			,usuario_modificacion
			,fecha_modificacion
		   )
     VALUES
           (@newId,
			@idCliente,
			@fechaInicioVigencia,
			@campo,
			@valor,
			@observacion,
			@estado,
			@idUsuario,
			dbo.getlocaldate()
			);
END


/* **** 3 **** */
CREATE PROCEDURE [dbo].[ps_cliente_historial_reasignacion_vendedor] 
@idCliente uniqueidentifier,
@campo varchar(100)
AS
BEGIN
	SELECT cr.valor, cr.observacion, cr.fecha_inicio_vigencia, cr.fecha_modificacion, v.nombre
	FROM CLIENTE_HISTORIAL_REASIGNACION cr
	INNER JOIN VENDEDOR v 
	WHERE cr.id_cliente = @idCliente and cr.campo = @campo 
END










