/* **** 1 **** */
ALTER TABLE [GRUPO_CLIENTE] 
add 	[contacto] [varchar](100) NULL,
	[plazo_credito_solicitado] [int] NULL,
	[plazo_credito_aprobado] [int] NULL,
	[id_ciudad] [uniqueidentifier] NULL,
	[telefono_contacto] [varchar](50) NULL,
	[email_contacto] [varchar](100) NULL,
	[credito_solicitado] [decimal](12, 2) NULL,
	[credito_aprobado] [decimal](12, 2) NULL,
	[sobre_giro] [decimal](12, 2) NULL,
	[sobre_plazo] [int] NULL,
	[observaciones_credito] [varchar](1000) NULL;
	
	
	
	
/* **** 2 **** */	
CREATE PROCEDURE [dbo].[pi_grupoCliente] 
@idUsuario uniqueidentifier,
@nombre  varchar(200),
@contacto  varchar(100),
@telefonoContacto varchar(50),
@emailContacto varchar(50),
@idCiudad uniqueidentifier,
/*Plazo credito*/
@plazoCreditoSolicitado int,
@plazoCreditoAprobado int,
@sobrePlazo int, 
/*Monto Crédito*/
@creditoSolicitado decimal(12,2),
@creditoAprobado decimal(12,2),
@sobreGiro decimal(12,2),

@observacionesCredito varchar(1000),
@observaciones varchar(1000),
@codigo VARCHAR(4),

@newId int OUTPUT
AS
BEGIN TRAN

INSERT INTO GRUPO_CLIENTE
           (grupo
           ,[contacto]
		   ,codigo
		   ,telefono_contacto
		   ,email_contacto
		   ,[id_ciudad]
		   ,estado
		   ,[usuario_creacion]
		   ,[fecha_creacion]
		   ,[usuario_modificacion]
		   ,[fecha_modificacion],
		   /*Plazo credito*/
			plazo_credito_solicitado,
			plazo_credito_aprobado,
			sobre_plazo, 
			/*Monto Crédito*/
			credito_solicitado,
			credito_aprobado,
			sobre_giro, 

			observaciones_credito,
			observaciones
		   )
     VALUES
           (
		    @nombre,
		    @contacto,
			@codigo,
			@telefonoContacto,
            @emailContacto,
			@idCiudad,
			1,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE(),		
			@plazoCreditoSolicitado,
			@plazoCreditoAprobado,
			@sobrePlazo, 
			/*Monto Crédito*/
			@creditoSolicitado,
			@creditoAprobado,
			@sobreGiro, 		
			@observacionesCredito,
			@observaciones
			);

SET NOCOUNT ON
	SET @newId = SCOPE_IDENTITY();

COMMIT





/* **** 3 **** */	
CREATE PROCEDURE ps_findGruposCliente

@codigo varchar(4),
@idCiudad uniqueIdentifier,
@grupo varchar(50),
@sinPlazoCreditoAprobado int, 
@estado int
AS
BEGIN


	SELECT 
	  gc.id_grupo_cliente
      ,gc.codigo
      ,gc.grupo
      ,gc.estado
      ,gc.plazo_credito_solicitado
      ,gc.plazo_credito_aprobado
      ,gc.credito_solicitado
      ,gc.credito_aprobado
	  ,ci.id_ciudad
	  ,ci.nombre as ciudad_nombre
	  
	   FROM GRUPO_CLIENTE gc
	   INNER JOIN CIUDAD ci on ci.id_ciudad = gc.id_ciudad
	where 
	gc.codigo LIKE @codigo 
	OR
	(@codigo = '' AND
	(
	REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(gc.grupo, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  LIKE '%'+@grupo+'%' 
	OR @grupo IS NULL
	OR @grupo = ''
	)
	AND (ci.id_ciudad = @idCiudad OR @idCiudad = '00000000-0000-0000-0000-000000000000')
	AND gc.estado = @estado
	);

END




/* **** 4 **** */	
CREATE PROCEDURE pu_grupoCliente 
@idGrupoCliente int,
@idUsuario uniqueidentifier,
@grupo  varchar(200),
@contacto  varchar(100),
@telefonoContacto varchar(50),
@emailContacto varchar(50),
@idCiudad uniqueidentifier,
/*Plazo credito*/
@plazoCreditoSolicitado int,
@plazoCreditoAprobado int,
@sobrePlazo int, 
/*Monto Crédito*/
@creditoSolicitado decimal(12,2),
@creditoAprobado decimal(12,2),
@sobreGiro decimal(12,2),

@observacionesCredito varchar(1000),
@observaciones varchar(1000),
@codigo VARCHAR(4),
@estado int

AS
BEGIN

UPDATE GRUPO_CLIENTE  
	SET codigo = @codigo 
		,grupo = @grupo
		,contacto = @contacto
		,telefono_contacto = @telefonoContacto
		,email_contacto = @emailContacto
		,id_ciudad = @idCiudad
		,plazo_credito_solicitado = @plazoCreditoSolicitado
		,plazo_credito_aprobado = @plazoCreditoAprobado
		,credito_solicitado = @creditoSolicitado
		,credito_aprobado = @creditoAprobado
		,sobre_giro = @sobreGiro
		,observaciones_credito = @observacionesCredito
		,observaciones = @observaciones

		,estado = @estado
		,usuario_modificacion = @idUsuario
		,fecha_modificacion = GETDATE()
	WHERE id_grupo_cliente like @idGrupoCliente;

END






/* **** 5 **** */	
CREATE PROCEDURE ps_grupoCliente
@idGrupoCliente int
AS
BEGIN


	SELECT 
	  gc.id_grupo_cliente
      ,gc.codigo
      ,gc.grupo
      ,gc.codigo_negociacion_compra
      ,gc.id_responsable_comercial
      ,gc.canal
      ,gc.fecha_resgistro
      ,gc.observaciones
      ,gc.estado
      ,gc.contacto
      ,gc.plazo_credito_solicitado
      ,gc.plazo_credito_aprobado
      ,gc.id_ciudad
      ,gc.telefono_contacto
      ,gc.email_contacto
      ,gc.credito_solicitado
      ,gc.credito_aprobado
      ,gc.sobre_giro
      ,gc.sobre_plazo
      ,gc.observaciones_credito
	  ,ci.id_ciudad
	  ,ci.nombre as ciudad_nombre
	   FROM GRUPO_CLIENTE gc
	   left join CIUDAD ci on ci.id_ciudad = gc.id_ciudad

	where gc.id_grupo_cliente = @idGrupoCliente;

END



