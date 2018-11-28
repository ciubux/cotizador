/* Obtener horarios */
ALTER PROCEDURE [dbo].[ps_cliente] 
@idCliente uniqueidentifier 
AS
BEGIN

SELECT cl.id_cliente, cl.codigo, cl.razon_social,
cl.nombre_comercial, cl.contacto1, cl.telefono_contacto1, cl.email_contacto1, cl.contacto2, cl.ruc,
cl.domicilio_legal, 
/*Si el cliente no tiene correo entonces se obtiene de algún pedido que tenga correo*/
CASE cl.correo_envio_factura WHEN '' THEN 
(SELECT TOP 1 correo_contacto_pedido FROM PEDIDO where id_cliente = cl.id_cliente
AND correo_contacto_pedido IS NOT NULL AND correo_contacto_pedido NOT IN ( '','.') )
ELSE cl.correo_envio_factura END AS correo_envio_factura, 

cl.razon_social_sunat, cl.nombre_comercial_sunat, 
cl.direccion_domicilio_legal_sunat, cl.estado_contribuyente_sunat, 
cl.condicion_contribuyente_sunat,
ub.codigo as codigo_ubigeo,
ub.provincia, ub.departamento, ub.distrito, cl.plazo_credito,

cl.forma_pago_factura, 
cl.sede_principal, 
cl.negociacion_multiregional, 
cl.id_ciudad,
ci.nombre as ciudad_nombre,
cl.tipo_documento,
/*PLAZO CREDITO*/
cl.plazo_credito_solicitado, --plazo credito aprobado
cl.tipo_pago_factura, --plazo credito aprobado
cl.sobre_plazo,
/*MONTO CREDITO*/
cl.credito_solicitado,
cl.credito_aprobado,
cl.sobre_giro, 
/*FLAG VENDEDORES*/
cl.vendedores_asignados,

/*Turnos entrega*/
cl.hora_inicio_primer_turno_entrega,
cl.hora_fin_primer_turno_entrega,
cl.hora_inicio_segundo_turno_entrega,
cl.hora_fin_segundo_turno_entrega,

--VENDEDORES,
verc.id_vendedor as responsable_comercial_id_vendedor,
verc.codigo as responsable_comercial_codigo,
verc.descripcion as responsable_comercial_descripcion,

vesc.id_vendedor as supervisor_comercial_id_vendedor,
vesc.codigo as supervisor_comercial_codigo,
vesc.descripcion as supervisor_comercial_descripcion,

veasc.id_vendedor as asistente_servicio_cliente_id_vendedor,
veasc.codigo as asistente_servicio_cliente_codigo,
veasc.descripcion as asistente_servicio_cliente_descripcion,

cl.observaciones_credito, 
cl.observaciones, 
cl.bloqueado,

cl.pertenece_canal_multiregional,
cl.pertenece_canal_lima,
cl.pertenece_canal_provincia,
cl.pertenece_canal_pcp,
cl.pertenece_canal_ordon,
cl.es_sub_distribuidor,

clgr.id_grupo_cliente ,
gr.grupo as grupo_nombre

FROM CLIENTE AS cl 
INNER JOIN CIUDAD AS ci ON cl.id_ciudad = ci.id_ciudad
LEFT JOIN UBIGEO AS ub ON cl.ubigeo = ub.codigo
LEFT JOIN VENDEDOR AS verc ON cl.id_responsable_comercial = verc.id_vendedor
LEFT JOIN VENDEDOR AS vesc ON cl.id_supervisor_comercial = vesc.id_vendedor
LEFT JOIN VENDEDOR AS veasc ON cl.id_asistente_servicio_cliente = veasc.id_vendedor
LEFT JOIN CLIENTE_GRUPO_CLIENTE AS clgr ON clgr.id_cliente = cl.id_cliente
LEFT JOIN GRUPO_CLIENTE AS gr ON gr.id_grupo_cliente = clgr.id_grupo_cliente 
WHERE cl.estado = 1 AND cl.id_cliente = @idCliente 

END





/* Agregar columnas segundo turno */
ALTER TABLE PEDIDO 
ADD hora_entrega_adicional_desde TIME(7),
 hora_entrega_adicional_hasta TIME(7);






/* Agrega segundo turno */
USE [cotizadormp]
GO
/****** Object:  StoredProcedure [dbo].[pi_pedido]    Script Date: 27/11/2018 7:36:33 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO








ALTER PROCEDURE [dbo].[pi_pedido] 
@numeroGrupo bigint, 
@idCotizacion uniqueidentifier, 
@idCiudad uniqueidentifier, 
@idCliente uniqueidentifier, 
@numeroReferenciaCliente varchar(100),
@idDireccionEntrega uniqueidentifier,
@direccionEntrega varchar(200),
@contactoEntrega varchar(100),
@telefonoContactoEntrega varchar(100),
@codigoCliente varchar(30),
@codigoMP varchar(30),
@nombre varchar(250),
@observacionesDireccionEntrega varchar(250),
@fechaSolicitud  datetime,
@fechaEntregaDesde datetime,
@fechaEntregaHasta datetime,
@horaEntregaDesde datetime,
@horaEntregaHasta datetime,
@horaEntregaAdicionalDesde datetime,
@horaEntregaAdicionalHasta datetime,
@idSolicitante uniqueIdentifier,
@contactoPedido varchar(100),
@telefonoContactoPedido varchar(100),
@correoContactoPedido varchar(100),
@incluidoIGV smallint, 
@tasaIGV decimal(18,2), 
@igv decimal(18,2), 
@total decimal(18,2), 
@observaciones varchar(500), 
@idUsuario uniqueidentifier,
@estado smallint,
@estadoCrediticio smallint,
@esPagoContado bit,
@observacionSeguimientoPedido varchar(500),
@observacionSeguimientoCrediticioPedido varchar(500),
@ubigeoEntrega char(6),
@tipoPedido char(1),
@observacionesGuiaRemision varchar(200),
@observacionesFactura varchar(200),
@otrosCargos Decimal(12,2),
@tipo char(1),
@newId uniqueidentifier OUTPUT, 
@numero bigint OUTPUT 
AS
BEGIN

SET NOCOUNT ON




DECLARE @countDireccionEntrega int;
DECLARE @countSolicitante int;



SET @newId = NEWID();
SET @numero = NEXT VALUE FOR dbo.SEQ_PEDIDO_NUMERO;












/*Si el numero de Grupo es distinto de null se actualiza en el Pedido Origen*/
/*IF @numeroGrupo IS NOT NULL 
BEGIN
	UPDATE PEDIDO SET numero_grupo = @numeroGrupo where numero = @numeroGrupo;
END*/


/*Si no se cuenta con Dirección Entrega*/
IF @idDireccionEntrega = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)
BEGIN 

	IF @codigoCliente IS NULL
	BEGIN 
		SELECT @countDireccionEntrega = COUNT(*) FROM DIRECCION_ENTREGA
		WHERE id_cliente = @idCliente 
		AND estado = 1 
		AND REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(@direccionEntrega), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  = 
		REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(descripcion), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ') 
	
		IF @countDireccionEntrega = 1
		BEGIN 
			SELECT @idDireccionEntrega = id_direccion_entrega FROM DIRECCION_ENTREGA
			WHERE id_cliente = @idCliente 
			AND estado = 1 
			AND REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(@direccionEntrega), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  = 
			REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(descripcion), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ') 
		END 
	END
	ELSE 
	BEGIN 
		SELECT @countDireccionEntrega = COUNT(*) FROM DIRECCION_ENTREGA
		WHERE id_cliente = @idCliente 
		AND estado = 1 
		AND codigo_cliente = @codigoCliente
	
		IF @countDireccionEntrega = 1
		BEGIN 
			SELECT @idDireccionEntrega = id_direccion_entrega FROM DIRECCION_ENTREGA
			WHERE id_cliente = @idCliente 
			AND estado = 1 
			AND codigo_cliente = @codigoCliente
		END 
	END
END

IF @idDireccionEntrega = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)
BEGIN 
	SET @idDireccionEntrega  = NEWID();
	INSERT INTO DIRECCION_ENTREGA
	(id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono, estado, usuario_creacion,
	fecha_creacion, usuario_modificacion, fecha_modificacion,
	codigo_cliente, codigo_mp, nombre, observaciones)
	 VALUES(@idDireccionEntrega, @idCliente, @ubigeoEntrega, 
	@direccionEntrega,@contactoEntrega,@telefonoContactoEntrega,1,@idUsuario, 
	GETDATE(), @idUsuario, GETDATE(), @codigoCliente, @codigoMP, @nombre, @observacionesDireccionEntrega);
END 
ELSE
BEGIN
	UPDATE DIRECCION_ENTREGA SET 
	descripcion = @direccionEntrega, 
	ubigeo = @ubigeoEntrega, 
	contacto = @contactoEntrega,
	telefono = @telefonoContactoEntrega,
	usuario_modificacion = @idUsuario,
	fecha_modificacion = GETDATE(), 
	codigo_cliente = @codigoCliente,
	codigo_mp = @codigoMP,
	nombre =@nombre,
	observaciones = @observacionesDireccionEntrega
	where id_direccion_entrega = @idDireccionEntrega;
END 


IF @idSolicitante = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)
BEGIN 
	SELECT @countSolicitante = COUNT(*) FROM SOLICITANTE
	WHERE id_cliente = @idCliente 
	AND estado = 1 
	AND REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(@contactoPedido), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')   = 
	REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(nombre), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ') 
	
	IF @countSolicitante = 1
	BEGIN 
		SELECT @idSolicitante = id_solicitante FROM SOLICITANTE
		WHERE id_cliente = @idCliente 
		AND estado = 1 
		AND REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(@contactoPedido), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')   = 
		REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(nombre), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ') 
	END 
END


IF @idSolicitante = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)
BEGIN 
	SET @idSolicitante  = NEWID();
	INSERT INTO SOLICITANTE 
	(id_solicitante, id_cliente, nombre, telefono, correo, estado, 
	usuario_creacion, fecha_creacion, usuario_modificacion, fecha_modificacion)
	VALUES(@idSolicitante, @idCliente, @contactoPedido, @telefonoContactoPedido, @correoContactoPedido,1,
	@idUsuario,GETDATE(), @idUsuario, GETDATE());
END 
ELSE
BEGIN
	UPDATE SOLICITANTE SET 
	nombre = @contactoPedido, 
	telefono = @telefonoContactoPedido, 
	correo = @correoContactoPedido,
	usuario_modificacion = @idUsuario,
	fecha_modificacion = GETDATE() 
	where id_solicitante = @idSolicitante;
END 



INSERT INTO [PEDIDO]
           ([id_pedido]
		   ,id_Cotizacion
		   ,[numero]
		   ,numero_grupo
			,[id_ciudad] 
			,[id_cliente] 
			,numero_referencia_cliente
			,id_direccion_entrega
			,direccion_entrega
			,contacto_entrega
			,telefono_contacto_entrega
			,[fecha_solicitud]
			,[fecha_entrega_desde]
			,[fecha_entrega_hasta]
			,[hora_entrega_desde]
			,[hora_entrega_hasta]
			,[hora_entrega_adicional_desde]
			,[hora_entrega_adicional_hasta]
			,contacto_pedido
			,telefono_contacto_pedido
			,correo_contacto_pedido
			,incluido_igv 
			,tasa_igv
			,igv
			,total
			,observaciones
		   ,[estado]
		   ,[usuario_creacion]
		   ,[fecha_creacion]
		   ,[usuario_modificacion]
		   ,[fecha_modificacion]
		   ,ubigeo_entrega
		   ,tipo_pedido
		   ,observaciones_guia_remision
		   ,observaciones_factura
		   ,otros_cargos
		   ,id_solicitante
		   ,tipo
		   ,es_pago_contado
		   )
     VALUES
           (@newId
		   ,@idCotizacion
		    ,@numero
		    ,@numeroGrupo
			,@idCiudad
			,@idCliente 
			,@numeroReferenciaCliente
			,@idDireccionEntrega
			,@direccionEntrega
			,@contactoEntrega
			,@telefonoContactoEntrega
			,@fechaSolicitud
			,@fechaEntregaDesde
			,@fechaEntregaHasta
			,@horaEntregaDesde
			,@horaEntregaHasta
			,@horaEntregaAdicionalDesde
			,@horaEntregaAdicionalHasta
			,@contactoPedido
			,@telefonoContactoPedido
			,@correoContactoPedido
			,@incluidoIGV
			,@tasaIGV
			,@igv
			,@total
			,@observaciones
			,1
			,@idUsuario
			,GETDATE()
			,@idUsuario
			,GETDATE()
			,@ubigeoEntrega	
			,@tipoPedido
		   ,@observacionesGuiaRemision
		   ,@observacionesFactura
		   ,@otrosCargos
		   ,@idSolicitante

		   ,@tipo --[V|C|A]
		   ,@esPagoContado
			);

INSERT INTO SEGUIMIENTO_PEDIDO 
(
		id_seguimiento_pedido, 
		id_usuario ,
		id_pedido, 
		estado_pedido,
		observacion ,
		estado ,
		usuario_creacion ,
		fecha_creacion ,
		usuario_modifiacion ,
		fecha_modificacion 
)
VALUES(
			NEWID(),
			@idUsuario,
			@newId,
			@estado,
			@observacionSeguimientoPedido,
			1,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE()
);

INSERT INTO SEGUIMIENTO_CREDITICIO_PEDIDO 
(
		id_seguimiento_crediticio_pedido, 
		id_usuario ,
		id_pedido, 
		estado_pedido,
		observacion ,
		estado ,
		usuario_creacion ,
		fecha_creacion ,
		usuario_modifiacion ,
		fecha_modificacion 
)
VALUES(
			NEWID(),
			@idUsuario,
			@newId,
			@estadoCrediticio,
			@observacionSeguimientoCrediticioPedido,
			1,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE()
);


END









/* Agrega segundo turno */
USE [cotizadormp]
GO
/****** Object:  StoredProcedure [dbo].[pu_pedido]    Script Date: 27/11/2018 7:38:41 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO








ALTER PROCEDURE [dbo].[pu_pedido] 
@idPedido uniqueidentifier, 
@numeroGrupo bigint, 
@idCotizacion uniqueidentifier, 
@idCiudad uniqueidentifier, 
@idCliente uniqueidentifier, 
@numeroReferenciaCliente varchar(100),
@idDireccionEntrega uniqueidentifier,
@direccionEntrega varchar(200),
@contactoEntrega varchar(100),
@telefonoContactoEntrega varchar(100),
@codigoCliente varchar(30),
@codigoMP varchar(30),
@nombre varchar(250),
@observacionesDireccionEntrega varchar(250),
@fechaSolicitud  datetime,
@fechaEntregaDesde datetime,
@fechaEntregaHasta datetime,
@horaEntregaDesde datetime,
@horaEntregaHasta datetime,
@horaEntregaAdicionalDesde datetime,
@horaEntregaAdicionalHasta datetime,
@idSolicitante uniqueIdentifier,
@contactoPedido varchar(100),
@telefonoContactoPedido varchar(100),
@correoContactoPedido varchar(100),
@incluidoIGV smallint, 
@tasaIGV decimal(18,2), 
@igv decimal(18,2), 
@total decimal(18,2), 
@observaciones varchar(500), 
@idUsuario uniqueidentifier,
@estado smallint,
@estadoCrediticio smallint,
@esPagoContado bit,
@observacionSeguimientoPedido varchar(500),
@observacionSeguimientoCrediticioPedido varchar(500),
@tipoPedido char(1),
@observacionesGuiaRemision varchar(200),
@observacionesFactura varchar(200),
@ubigeoEntrega char(6),
@otrosCargos decimal(12,2)
AS
BEGIN

DECLARE @countDireccionEntrega int;
DECLARE @countSolicitante int;

SET NOCOUNT ON

IF @numeroGrupo IS NOT NULL 
BEGIN
	UPDATE PEDIDO SET numero_grupo = @numeroGrupo where numero = @numeroGrupo;
END


IF @idDireccionEntrega = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)
BEGIN 

	IF @codigoCliente IS NULL
	BEGIN 
		SELECT @countDireccionEntrega = COUNT(*) FROM DIRECCION_ENTREGA
		WHERE id_cliente = @idCliente 
		AND estado = 1 
		AND REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(@direccionEntrega), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  = 
		REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(descripcion), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ') 
	
		IF @countDireccionEntrega = 1
		BEGIN 
			SELECT @idDireccionEntrega = id_direccion_entrega FROM DIRECCION_ENTREGA
			WHERE id_cliente = @idCliente 
			AND estado = 1 
			AND REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(@direccionEntrega), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  = 
			REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(descripcion), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ') 
		END 
	END
	ELSE 
	BEGIN 
		SELECT @countDireccionEntrega = COUNT(*) FROM DIRECCION_ENTREGA
		WHERE id_cliente = @idCliente 
		AND estado = 1 
		AND codigo_cliente = @codigoCliente
	
		IF @countDireccionEntrega = 1
		BEGIN 
			SELECT @idDireccionEntrega = id_direccion_entrega FROM DIRECCION_ENTREGA
			WHERE id_cliente = @idCliente 
			AND estado = 1 
			AND codigo_cliente = @codigoCliente
		END 
	END
END

IF @idDireccionEntrega = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)
BEGIN 
	SET @idDireccionEntrega  = NEWID();
	INSERT INTO DIRECCION_ENTREGA
	(id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono, estado, usuario_creacion,
	fecha_creacion, usuario_modificacion, fecha_modificacion,
	codigo_cliente, codigo_mp, nombre, observaciones)
	 VALUES(@idDireccionEntrega, @idCliente, @ubigeoEntrega, 
	@direccionEntrega,@contactoEntrega,@telefonoContactoEntrega,1,@idUsuario, 
	GETDATE(), @idUsuario, GETDATE(), @codigoCliente, @codigoMP, @nombre, @observacionesDireccionEntrega);
END 
ELSE
BEGIN
	UPDATE DIRECCION_ENTREGA SET 
	descripcion = @direccionEntrega, 
	ubigeo = @ubigeoEntrega, 
	contacto = @contactoEntrega,
	telefono = @telefonoContactoEntrega,
	usuario_modificacion = @idUsuario,
	fecha_modificacion = GETDATE(), 
	codigo_cliente = @codigoCliente,
	codigo_mp = @codigoMP,
	nombre =@nombre,
	observaciones = @observacionesDireccionEntrega
	where id_direccion_entrega = @idDireccionEntrega;
END 


IF @idSolicitante = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)
BEGIN 
	SELECT @countSolicitante = COUNT(*) FROM SOLICITANTE
	WHERE id_cliente = @idCliente 
	AND estado = 1 
	AND REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(@contactoPedido), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')   = 
	REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(nombre), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ') 
	
	IF @countSolicitante = 1
	BEGIN 
		SELECT @idSolicitante = id_solicitante FROM SOLICITANTE
		WHERE id_cliente = @idCliente 
		AND estado = 1 
		AND REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(@contactoPedido), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')   = 
		REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(nombre), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ') 
	END 
END


IF @idSolicitante = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)
BEGIN 
	SET @idSolicitante  = NEWID();
	INSERT INTO SOLICITANTE 
	(id_solicitante, id_cliente, nombre, telefono, correo, estado, 
	usuario_creacion, fecha_creacion, usuario_modificacion, fecha_modificacion)
	VALUES(@idSolicitante, @idCliente, @contactoPedido, @telefonoContactoPedido, @correoContactoPedido,1,
	@idUsuario,GETDATE(), @idUsuario, GETDATE());
END 
ELSE
BEGIN
	UPDATE SOLICITANTE SET 
	nombre = @contactoPedido, 
	telefono = @telefonoContactoPedido, 
	correo = @correoContactoPedido,
	usuario_modificacion = @idUsuario,
	fecha_modificacion = GETDATE() 
	where id_solicitante = @idSolicitante;
END 


UPDATE PEDIDO_DETALLE SET ESTADO = 0 ,
[usuario_modificacion] = @idUsuario, [fecha_modificacion] = GETDATE()
WHERE  id_pedido = @idPedido;


UPDATE ARCHIVO_ADJUNTO SET estado = 0 where id_archivo_adjunto IN (
SELECT id_archivo_adjunto FROM PEDIDO_ARCHIVO WHERE  id_pedido = @idPedido);

UPDATE PEDIDO_ARCHIVO SET estado = 0 WHERE  id_pedido = @idPedido

UPDATE [PEDIDO] SET
id_cotizacion = @idCotizacion,
[id_ciudad]  = @idCiudad
,[id_cliente] = @idCliente 
,numero_referencia_cliente = @numeroReferenciaCliente
,ubigeo_entrega = @ubigeoEntrega
,id_direccion_entrega = @idDireccionEntrega
,direccion_entrega = @direccionEntrega
,contacto_entrega = @contactoEntrega
,telefono_contacto_entrega = @telefonoContactoEntrega
,[fecha_solicitud] = @fechaSolicitud
,[fecha_entrega_desde] = @fechaEntregaDesde
,[fecha_entrega_hasta] = @fechaEntregaHasta
,[hora_entrega_desde] = @horaEntregaDesde
,[hora_entrega_hasta] = @horaEntregaHasta
,[hora_entrega_adicional_desde] = @horaEntregaAdicionalDesde
,[hora_entrega_adicional_hasta] = @horaEntregaAdicionalHasta
,contacto_pedido = @contactoPedido
,telefono_contacto_pedido = @telefonoContactoPedido
,correo_contacto_pedido = @correoContactoPedido
,incluido_igv = @incluidoIGV
,tasa_igv = @tasaIGV
,igv = @igv
,total = @total
,observaciones = @observaciones
,tipo_pedido = @tipoPedido
,observaciones_guia_remision = @observacionesGuiaRemision
,observaciones_factura = @observacionesFactura
,[estado] = 1
,[usuario_modificacion] = @idUsuario
,[fecha_modificacion] = GETDATE()
,[otros_cargos] = @otrosCargos
,id_solicitante = @idSolicitante
,es_pago_contado = @esPagoContado
 WHERE id_pedido = @idPedido;

UPDATE SEGUIMIENTO_PEDIDO set estado = 0 where id_pedido = @idPedido;
UPDATE SEGUIMIENTO_CREDITICIO_PEDIDO set estado = 0 where id_pedido = @idPedido;

INSERT INTO SEGUIMIENTO_PEDIDO 
(
		id_seguimiento_pedido, 
		id_usuario ,
		id_pedido, 
		estado_pedido,
		observacion ,
		estado ,
		usuario_creacion ,
		fecha_creacion ,
		usuario_modifiacion ,
		fecha_modificacion 
)
VALUES(
			NEWID(),
			@idUsuario,
			@idPedido,
			@estado,
			@observacionSeguimientoPedido,
			1,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE()
);

INSERT INTO SEGUIMIENTO_CREDITICIO_PEDIDO 
(
		id_seguimiento_crediticio_pedido, 
		id_usuario ,
		id_pedido, 
		estado_pedido,
		observacion ,
		estado ,
		usuario_creacion ,
		fecha_creacion ,
		usuario_modifiacion ,
		fecha_modificacion 
)
VALUES(
			NEWID(),
			@idUsuario,
			@idPedido,
			@estadoCrediticio,
			@observacionSeguimientoCrediticioPedido,
			1,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE()
);



END


/* Agrega columnas hora_entrega_adicional_desde y hora_entrega_adicional_hasta*/
ALTER PROCEDURE [dbo].[ps_pedidos] 

@numero bigint,
@numeroGrupo bigint,
@idCliente uniqueidentifier,
@idCiudad uniqueidentifier,
@idUsuario uniqueidentifier,
@idUsuarioBusqueda uniqueidentifier,
@numeroReferenciaCliente varchar(20),
@tipoPedido char(1),
@fechaCreacionDesde datetime,
@fechaCreacionHasta datetime, 
@fechaEntregaDesde datetime,
@fechaEntregaHasta datetime, 
@fechaProgramacionDesde datetime,
@fechaProgramacionHasta datetime, 
@tipo char(1),
@idGrupoCliente int,
@estado smallint,
@estadoCrediticio smallint
AS
BEGIN



DECLARE @aprobadorPedidosLima int;
DECLARE @aprobadorPedidosProvincias int;
DECLARE @EST_PEDIDO_EDICION int;
DECLARE @EST_PEDIDO_PEND_APROBACION int;
DECLARE @EST_PEDIDO_INGRESADO int;
DECLARE @EST_PEDIDO_DENEGADO int;
DECLARE @EST_PEDIDO_PROGRAMADO int;
DECLARE @EST_PEDIDO_ATENDIDO_PARCIALMENTE int;

SET @EST_PEDIDO_EDICION = 6;
SET @EST_PEDIDO_PEND_APROBACION = 0;
SET @EST_PEDIDO_INGRESADO = 1;
SET @EST_PEDIDO_DENEGADO = 2;
SET @EST_PEDIDO_PROGRAMADO = 3;
SET @EST_PEDIDO_ATENDIDO_PARCIALMENTE = 5;

if  @numero = 0 
BEGIN



	SELECT 
	--PEDIDO
	pe.numero as numero_pedido, pe.numero_grupo as numero_grupo_pedido,
    pe.id_pedido, pe.fecha_solicitud, pe.incluido_igv, 
	pe.igv, pe.total, ISNULL(pe.observaciones,'') observaciones,
	pe.fecha_creacion, pe.fecha_entrega_desde, pe.fecha_entrega_hasta, 
	pe.hora_entrega_desde, pe.hora_entrega_hasta, pe.hora_entrega_adicional_desde, pe.hora_entrega_adicional_hasta,
	pe.fecha_creacion as fecha_registro,
	pe.fecha_programacion,
	pe.stock_confirmado,
	REPLACE(COALESCE(pe.numero_referencia_cliente,''),'O/C N°','')as numero_referencia_cliente,
	--CLIENTE
	COALESCE( cl.codigo,'') as codigo,  cl.id_cliente,cl.razon_social, cl.ruc,
	 --USUARIO
	 us.nombre as nombre_usuario, us.id_usuario,
	 --CIUDAD
	ci.id_ciudad, ci.nombre as nombre_ciudad, 
	--SEGUIMIENTO
	sc.estado_pedido as estado_seguimiento,
	us2.nombre as usuario_seguimiento, sc.observacion as observacion_seguimiento,
	us2.id_usuario as id_usuario_seguimiento,
	--SEGUIMIENTO CREDITICIO
	scp.estado_pedido as estado_seguimiento_crediticio,
	us3.nombre as usuario_seguimiento_Crediticio, scp.observacion as observacion_seguimiento_Crediticio,
	us3.id_usuario as id_usuario_seguimiento_crediticio,
	--DETALLES
	--(SELECT max(porcentaje_descuento) from COTIZACION_DETALLE  WHERE estado = 1 AND id_cotizacion = pe.id_pedido) as maximo_porcentaje_descuento
	ub.codigo codigo_ubigeo,
	ISNULl(ub.distrito,'-') distrito

	 FROM PEDIDO as pe
	INNER JOIN CLIENTE AS cl ON pe.id_cliente = cl.id_cliente
	INNER JOIN CIUDAD AS ci ON pe.id_ciudad = ci.id_ciudad
	INNER JOIN USUARIO AS us on pe.usuario_creacion = us.id_usuario
	INNER JOIN SEGUIMIENTO_PEDIDO sc on sc.id_pedido = pe.id_pedido
	INNER JOIN USUARIO AS us2 on sc.id_usuario = us2.id_usuario
	INNER JOIN SEGUIMIENTO_CREDITICIO_PEDIDO scp on scp.id_pedido = pe.id_pedido
	INNER JOIN USUARIO AS us3 on scp.id_usuario = us3.id_usuario
	LEFT JOIN UBIGEO AS ub ON pe.ubigeo_entrega = ub.codigo 
	where   pe.fecha_creacion >= @fechaCreacionDesde 
	and pe.fecha_creacion <=  @fechaCreacionHasta


	and (pe.fecha_entrega_desde >= @fechaEntregaDesde or  @fechaEntregaDesde IS NULL)
	and (pe.fecha_entrega_desde <= @fechaEntregaHasta or  @fechaEntregaHasta IS NULL)
	and (pe.fecha_programacion >= @fechaProgramacionDesde OR @fechaProgramacionDesde IS NULL)
	and (pe.fecha_programacion <=  @fechaProgramacionHasta OR @fechaProgramacionHasta IS NULL)
	and (
	cl.id_cliente = @idCliente or 
	
	(@idCliente = '00000000-0000-0000-0000-000000000000' AND @idGrupoCliente <= 0
		AND (
				(	(SELECT es_cliente FROM USUARIO where id_usuario = @idUsuario ) = 0
					OR 
					(	(SELECT es_cliente FROM USUARIO where id_usuario = @idUsuario ) = 1
						AND cl.id_cliente in 
						(SELECT id_cliente FROM CLIENTE where ruc IN 
							(SELECT RUC FROM USUARIO_CLIENTE WHERE id_usuario =@idUsuario)
						)
					)
				)	
					
			)
	
	)
	OR 
	(@idGrupoCliente > 0 AND pe.id_cliente in (select id_cliente from CLIENTE_GRUPO_CLIENTE where id_grupo_cliente = @idGrupoCliente))
	)
	and (ci.id_ciudad = @idCiudad 
		OR
		(@idCiudad = '00000000-0000-0000-0000-000000000000'
		AND @idUsuario IN (SELECT id_usuario FROM USUARIO where (aprueba_pedidos_lima = 1 AND aprueba_pedidos_provincias = 1) OR es_cliente = 1 ))
	)
	and (pe.usuario_creacion = @idUsuarioBusqueda OR @idUsuarioBusqueda = '00000000-0000-0000-0000-000000000000')
	

	/*
	and (ci.id_ciudad = @idCiudad 
		OR pe.usuario_creacion = @idUsuario
		OR
		(@idCiudad = '00000000-0000-0000-0000-000000000000'
		AND @idUsuario IN (SELECT id_usuario FROM USUARIO where aprueba_pedidos_lima = 1 AND aprueba_pedidos_provincias = 1 ))
	)*/

	AND 
		(sc.estado_pedido = @estado 
		OR @estado = -1  
		OR (@estado = -2 AND (sc.estado_pedido IN (@EST_PEDIDO_EDICION,
														@EST_PEDIDO_PEND_APROBACION,
														@EST_PEDIDO_INGRESADO,
														@EST_PEDIDO_DENEGADO,
														@EST_PEDIDO_PROGRAMADO,
														@EST_PEDIDO_ATENDIDO_PARCIALMENTE)
									OR 	pe.no_entregado = 1				
									)
		)
		
		
		)
	AND (
		scp.estado_pedido = @estadoCrediticio 
		OR @estadoCrediticio = -1	)
		
	AND (@numeroReferenciaCliente = '' OR 
		@numeroReferenciaCliente IS NULL OR 
		pe.numero_referencia_cliente LIKE '%'+@numeroReferenciaCliente+'%')
	AND sc.estado = 1 AND scp.estado = 1
	AND pe.estado = 1

	AND pe.tipo = @tipo
	AND (@tipoPedido = '0' OR  pe.tipo_pedido = @tipoPedido)

	order by pe.numero asc ;
	END
else
BEGIN
	SELECT 
	--PEDIDO
	pe.numero as numero_pedido, pe.numero_grupo as numero_grupo_pedido,
    pe.id_pedido, pe.fecha_solicitud, pe.incluido_igv, 
	pe.igv, pe.total, ISNULL(pe.observaciones,'') observaciones,
	pe.fecha_creacion, pe.fecha_entrega_desde, pe.fecha_entrega_hasta, 
	pe.hora_entrega_desde, pe.hora_entrega_hasta,
	pe.fecha_creacion as fecha_registro,
	pe.fecha_programacion,
	pe.stock_confirmado,
	REPLACE(COALESCE(pe.numero_referencia_cliente,''),'O/C N°','')as numero_referencia_cliente,
	--CLIENTE
	COALESCE( cl.codigo,'') as codigo,  cl.id_cliente,cl.razon_social, cl.ruc, 
	 --USUARIO
	 us.nombre as nombre_usuario, us.id_usuario,
	 --CIUDAD
	ci.id_ciudad, ci.nombre as nombre_ciudad, 
	--SEGUIMIENTO
	sc.estado_pedido as estado_seguimiento,
	us2.nombre as usuario_seguimiento, sc.observacion as observacion_seguimiento,
	us2.id_usuario as id_usuario_seguimiento,
	--SEGUIMIENTO CREDITICIO
	scp.estado_pedido as estado_seguimiento_crediticio,
	us3.nombre as usuario_seguimiento_Crediticio, scp.observacion as observacion_seguimiento_Crediticio,
	us3.id_usuario as id_usuario_seguimiento_crediticio,
	--DETALLES
	--(SELECT max(porcentaje_descuento) from COTIZACION_DETALLE  WHERE estado = 1 AND id_cotizacion = co.id_cotizacion) as maximo_porcentaje_descuento
	ub.codigo codigo_ubigeo,
	ISNULl(ub.distrito,'-') distrito

	 FROM PEDIDO as pe
	INNER JOIN CLIENTE AS cl ON pe.id_cliente = cl.id_cliente
	INNER JOIN CIUDAD AS ci ON pe.id_ciudad = ci.id_ciudad
	INNER JOIN USUARIO AS us on pe.usuario_creacion = us.id_usuario
	INNER JOIN SEGUIMIENTO_PEDIDO sc on sc.id_pedido = pe.id_pedido
	INNER JOIN USUARIO AS us2 on sc.id_usuario = us2.id_usuario
	INNER JOIN SEGUIMIENTO_CREDITICIO_PEDIDO scp on scp.id_pedido = pe.id_pedido
	INNER JOIN USUARIO AS us3 on scp.id_usuario = us3.id_usuario
	LEFT JOIN UBIGEO AS ub ON pe.ubigeo_entrega = ub.codigo 
	where pe.numero = @numero 
	--filtro que evita que un usuario pueda obtener una cotización de otro usuario a través del codigo
--	and (us.id_usuario = @idUsuario or @idUsuario = '00000000-0000-0000-0000-000000000000'
--	OR (SELECT USUARIO @idUsuario)
	
	--)
--	AND (sc.estado_pedido = @estado or @estado = -1  )
	AND sc.estado = 1 AND scp.estado = 1
	AND pe.estado = 1

	AND pe.tipo = @tipo;
	END
END







/* Agrega columnas hora_entrega_adicional_desde y hora_entrega_adicional_hasta*/
USE [cotizadormp]
GO
/****** Object:  StoredProcedure [dbo].[ps_pedido]    Script Date: 27/11/2018 7:44:22 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO









ALTER PROCEDURE [dbo].[ps_pedido] 
@idPedido uniqueIdentifier
AS
BEGIN

SELECT 
--PEDIDO
pe.numero, pe.numero_grupo, pe.fecha_solicitud,  
pe.fecha_entrega_desde, pe.fecha_entrega_hasta, pe.fecha_entrega_extendida,
pe.hora_entrega_desde, pe.hora_entrega_hasta, pe.hora_entrega_adicional_desde, pe.hora_entrega_adicional_hasta,
pe.incluido_igv,  pe.igv, pe.total, pe.observaciones,  pe.fecha_modificacion,
pe.numero_referencia_cliente, pe.id_direccion_entrega, pe.direccion_entrega, pe.contacto_entrega,
pe.telefono_contacto_entrega, 
pe.fecha_programacion,
pe.tipo_pedido, pe.observaciones_factura, pe.observaciones_guia_remision,
pe.contacto_pedido,pe.telefono_contacto_pedido, pe.correo_contacto_pedido,
pe.otros_cargos,
pe.numero_referencia_adicional,
pe.fecha_creacion as fecha_registro,
/*cpe.serie AS serie_factura,
cpe.CORRELATIVO AS numero_factura,*/
pe.id_solicitante,
pe.tipo,
pe.es_pago_contado,
--UBIGEO
pe.ubigeo_entrega, ub.departamento, ub.provincia, ub.distrito,
---CLIENTE
cl.id_cliente, cl.codigo, cl.razon_social, cl.ruc, cic.id_ciudad as id_ciudad_cliente, cic.nombre as nombre_ciudad_cliente,
cl.razon_social_sunat, cl.direccion_domicilio_legal_sunat, cl.correo_envio_factura, cl.plazo_credito,
cl.tipo_pago_factura,
cl.forma_pago_factura,
cl.bloqueado,


---VENTA
/*
ve.igv as igv_venta,
ve.sub_total as sub_total_venta,
ve.total as total_venta,
ve.id_venta,*/

--CIUDAD
ci.id_ciudad, ci.nombre as nombre_ciudad , ci.es_provincia,
--USUARIO
us.nombre  as nombre_usuario, us.cargo, us.contacto as contacto_usuario, us.email,
--SEGUIMIENTO
sp.estado_pedido as estado_seguimiento,
us2.nombre as usuario_seguimiento, sp.observacion as observacion_seguimiento,
us2.id_usuario as id_usuario_seguimiento,
--SEGUIMIENTO CREDITICIO
spc.estado_pedido as estado_seguimiento_crediticio,
us3.nombre as usuario_seguimiento_crediticio, spc.observacion as observacion_seguimiento_crediticio,
us3.id_usuario as id_usuario_seguimiento_crediticio,

--COTIZACION
co.id_cotizacion,
co.codigo as cotizacion_codigo,
--VENDEDORES,
verc.codigo as responsable_comercial_codigo,
verc.descripcion as responsable_comercial_descripcion,
ISNULL(uverc.email,'') as responsable_comercial_email,

vesc.codigo as supervisor_comercial_codigo,
vesc.descripcion as supervisor_comercial_descripcion,
ISNULL(uvesc.email,'') as supervisor_comercial_email,

veasc.codigo as asistente_servicio_cliente_codigo,
veasc.descripcion as asistente_servicio_cliente_descripcion,
ISNULL(uveasc.email,'') as asistente_servicio_cliente_email

FROM PEDIDO as pe
INNER JOIN CIUDAD AS ci ON pe.id_ciudad = ci.id_ciudad
INNER JOIN USUARIO AS us on pe.usuario_creacion = us.id_usuario
INNER JOIN SEGUIMIENTO_PEDIDO sp ON pe.id_pedido = sp.id_pedido
INNER JOIN USUARIO AS us2 on sp.id_usuario = us2.id_usuario
INNER JOIN SEGUIMIENTO_CREDITICIO_PEDIDO spc ON pe.id_pedido = spc.id_pedido
INNER JOIN USUARIO AS us3 on spc.id_usuario = us3.id_usuario
INNER JOIN CLIENTE AS cl ON pe.id_cliente = cl.id_cliente
INNER JOIN CIUDAD AS cic ON cl.id_ciudad = cic.id_ciudad
LEFT JOIN UBIGEO ub ON CAST(pe.ubigeo_entrega AS char(6)) = ub.codigo
LEFT JOIN COTIZACION co ON co.id_cotizacion = pe.id_cotizacion
/*LEFT JOIN VENTA ve ON ve.id_pedido = pe.id_pedido
LEFT JOIN CPE_CABECERA_BE cpe ON cpe.id_cpe_cabecera_be = ve.id_documento_venta*/
LEFT JOIN SOLICITANTE so ON so.id_solicitante = pe.id_solicitante
LEFT JOIN VENDEDOR AS verc ON cl.id_responsable_comercial = verc.id_vendedor
LEFT JOIN VENDEDOR AS vesc ON cl.id_supervisor_comercial = vesc.id_vendedor
LEFT JOIN VENDEDOR AS veasc ON cl.id_asistente_servicio_cliente = veasc.id_vendedor
LEFT JOIN USUARIO AS uverc ON uverc.id_usuario = verc.id_usuario
LEFT JOIN USUARIO AS uvesc ON uvesc.id_usuario = vesc.id_usuario
LEFT JOIN USUARIO AS uveasc ON uveasc.id_usuario = veasc.id_usuario
--LEFT JOIN GRUPO AS gr ON pe.id_grupo = gr.id_grupo  
where pe.id_pedido = @idPedido and 
pe.estado = 1
AND sp.estado = 1
AND spc.estado = 1;



--RECUPERA EL DETALLE DEL PEDIDO



	SELECT 
	pd.id_pedido_detalle, pd.cantidad, pd.precio_sin_igv, pd.costo_sin_igv, 


	pd.equivalencia as equivalencia,
	pd.unidad, pd.porcentaje_descuento, pd.precio_neto, pd.es_precio_alternativo, pd.flete,
	pr.id_producto, pr.sku, pr.descripcion, pr.sku_proveedor, null as imagen, pr.proveedor, 
	pr.costo as costo_producto, pr.precio as precio_producto,
	pd.observaciones,
	pd.fecha_modificacion,


















































	null as fecha_fin_vigencia,






	(pd.cantidad - COALESCE(mad.cantidadAtendida,0)) AS  cantidadPendienteAtencion





	FROM PEDIDO_DETALLE as pd 
	INNER JOIN PEDIDO as pe ON pd.id_pedido = pe.id_pedido 
	INNER JOIN PRODUCTO pr ON pd.id_producto = pr.id_producto
	LEFT JOIN 
	(
	SELECT mad.id_pedido_detalle, SUM(cantidad) as cantidadAtendida  FROM
	MOVIMIENTO_ALMACEN_DETALLE AS mad 
	INNER JOIN MOVIMIENTO_ALMACEN ma ON mad.id_movimiento_almacen = ma.id_movimiento_almacen
	INNER JOIN PEDIDO pe ON pe.id_pedido = ma.id_pedido
	WHERE ma.id_pedido = @idPedido
	AND mad.estado = 1 AND  ma.estado = 1 AND ma.anulado = 0
	AND ((pe.tipo = 'V' AND ma.tipo_movimiento = 'S'	) OR (pe.tipo = 'C' AND ma.tipo_movimiento = 'S')
		OR pe.tipo = 'A' AND ma.tipo_movimiento = 'S'
	) 
	GROUP BY mad.id_pedido_detalle
	) AS  mad ON mad.id_pedido_detalle  = pd.id_pedido_detalle






























	where pd.id_pedido = @idPedido and pd.estado = 1 







	ORDER BY fecha_modificacion ASC;




















--RECUPERA LAS DIRECCIONES DE ENTREGA

SELECT TOP 0 de.id_direccion_entrega, de.descripcion, de.contacto, de.telefono, ubigeo
FROM DIRECCION_ENTREGA de
INNER JOIN PEDIDO pe ON de.id_cliente = pe.id_cliente
where pe.id_pedido = @idPedido
AND de.estado = 1;


--RECUPERA LAS GUÍAS Y FACTURAS

SELECT 
ma.id_movimiento_almacen, ma.serie_documento, ma.numero_documento, 
ma.fecha_emision,
ma.fecha_traslado,
dv.id_cpe_cabecera_be as id_documento_venta, dv.SERIE, dv.CORRELATIVO, 
 CONVERT( DATETIME,dv.FEC_EMI ,20) AS fecha_emision_factura,
 	CASE  dv.COD_ESTD_SUNAT 
	WHEN '101' THEN 'EN PROCESO'
	WHEN '102' THEN 'ACEPTADO'
	WHEN '103' THEN 'ACEPTADO CON OBS'
	WHEN '104' THEN 'RECHAZADO'
	WHEN '105' THEN 'ANULADO'
	ELSE '---' END as estado ,
mad.id_movimiento_almacen_detalle, mad.cantidad, 
mad.unidad, pr.id_producto, pr.sku, pr.descripcion
FROM MOVIMIENTO_ALMACEN_DETALLE as mad 
INNER JOIN MOVIMIENTO_ALMACEN as ma ON mad.id_movimiento_almacen = ma.id_movimiento_almacen
INNER JOIN PRODUCTO pr ON mad.id_producto = pr.id_producto
INNER JOIN VENTA ve ON ve.id_movimiento_almacen = ma.id_movimiento_almacen
LEFT JOIN CPE_CABECERA_BE dv ON ve.id_documento_venta = dv.id_cpe_cabecera_be
AND mad.estado = 1
WHERE ma.id_pedido = @idPedido
AND ma.estado = 1
AND ma.anulado = 0
ORDER BY ma.numero_documento, mad.fecha_creacion asc;



--RECUPERA LOS ARCHIVOS ADJUNTOS

SELECT  arch.id_archivo_adjunto as id_pedido_adjunto,  nombre--, arch.adjunto,
 FROM ARCHIVO_ADJUNTO arch
 INNER JOIN PEDIDO_ARCHIVO parch ON arch.id_archivo_adjunto = parch.id_archivo_adjunto
   WHERE parch.id_pedido = @idPedido
   AND arch.estado = 1
   AND parch.estado = 1;


SELECT TOP 0 so.id_solicitante, so.nombre, so.telefono, so.correo
FROM SOLICITANTE so
INNER JOIN PEDIDO pe ON so.id_cliente = pe.id_cliente
where pe.id_pedido = @idPedido
AND so.estado = 1;

END













/* Agrega columnas hora_entrega_adicional_desde y hora_entrega_adicional_hasta*/
USE [cotizadormp]
GO
/****** Object:  StoredProcedure [dbo].[ps_pedidoParaEditar]    Script Date: 27/11/2018 7:46:30 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



ALTER PROCEDURE [dbo].[ps_pedidoParaEditar] 
@idPedido uniqueIdentifier
AS
BEGIN

SELECT 
--PEDIDO
pe.numero, pe.numero_grupo, pe.fecha_solicitud,  
pe.fecha_entrega_desde, pe.fecha_entrega_hasta,
pe.hora_entrega_desde, pe.hora_entrega_hasta,pe.hora_entrega_adicional_desde, pe.hora_entrega_adicional_hasta,
pe.incluido_igv,  pe.igv, pe.total, pe.observaciones,  pe.fecha_modificacion,
pe.numero_referencia_cliente, pe.id_direccion_entrega, pe.direccion_entrega, pe.contacto_entrega,
pe.telefono_contacto_entrega, 
pe.fecha_programacion,
pe.tipo_pedido, pe.observaciones_factura, pe.observaciones_guia_remision,
pe.contacto_pedido,pe.telefono_contacto_pedido, pe.correo_contacto_pedido,
pe.otros_cargos,
pe.numero_referencia_adicional,
pe.fecha_creacion as fecha_registro,
cpe.serie AS serie_factura,
cpe.CORRELATIVO AS numero_factura,
pe.id_solicitante,
pe.tipo,
pe.es_pago_contado, ---REVISAR
--UBIGEO
pe.ubigeo_entrega, ub.departamento, ub.provincia, ub.distrito,
---CLIENTE
cl.id_cliente, cl.codigo, cl.razon_social, cl.ruc, cic.id_ciudad as id_ciudad_cliente, cic.nombre as nombre_ciudad_cliente,
cl.razon_social_sunat, cl.direccion_domicilio_legal_sunat, cl.correo_envio_factura, cl.plazo_credito,
cl.tipo_pago_factura,
cl.forma_pago_factura,


---VENTA
ve.igv as igv_venta,
ve.sub_total as sub_total_venta,
ve.total as total_venta,
ve.id_venta,

--CIUDAD
ci.id_ciudad, ci.nombre as nombre_ciudad ,
--USUARIO
us.nombre  as nombre_usuario, us.cargo, us.contacto as contacto_usuario, us.email,
--SEGUIMIENTO
sp.estado_pedido as estado_seguimiento,
us2.nombre as usuario_seguimiento, sp.observacion as observacion_seguimiento,
us2.id_usuario as id_usuario_seguimiento,
--SEGUIMIENTO CREDITICIO
spc.estado_pedido as estado_seguimiento_crediticio,
us3.nombre as usuario_seguimiento_crediticio, spc.observacion as observacion_seguimiento_crediticio,
us3.id_usuario as id_usuario_seguimiento_crediticio,
--DETALLE
co.id_cotizacion,
co.codigo as cotizacion_codigo
FROM PEDIDO as pe
INNER JOIN CIUDAD AS ci ON pe.id_ciudad = ci.id_ciudad
INNER JOIN USUARIO AS us on pe.usuario_creacion = us.id_usuario
INNER JOIN SEGUIMIENTO_PEDIDO sp ON pe.id_pedido = sp.id_pedido
INNER JOIN USUARIO AS us2 on sp.id_usuario = us2.id_usuario
INNER JOIN SEGUIMIENTO_CREDITICIO_PEDIDO spc ON pe.id_pedido = spc.id_pedido
INNER JOIN USUARIO AS us3 on spc.id_usuario = us3.id_usuario
INNER JOIN CLIENTE AS cl ON pe.id_cliente = cl.id_cliente
INNER JOIN CIUDAD AS cic ON cl.id_ciudad = cic.id_ciudad
LEFT JOIN UBIGEO ub ON pe.ubigeo_entrega = ub.codigo
LEFT JOIN COTIZACION co ON co.id_cotizacion = pe.id_cotizacion
LEFT JOIN VENTA ve ON ve.id_pedido = pe.id_pedido
LEFT JOIN CPE_CABECERA_BE cpe ON cpe.id_cpe_cabecera_be = ve.id_documento_venta
LEFT JOIN SOLICITANTE so ON so.id_solicitante = pe.id_solicitante
--LEFT JOIN GRUPO AS gr ON pe.id_grupo = gr.id_grupo  
where pe.id_pedido = @idPedido and 
pe.estado = 1
AND sp.estado = 1
AND spc.estado = 1;



--RECUPERA EL DETALLE DEL PEDIDO

select * from (
	SELECT pd.id_pedido_detalle, pd.cantidad, pd.precio_sin_igv, pd.costo_sin_igv, 
	pd.equivalencia as equivalencia,
	pd.unidad, pd.porcentaje_descuento, pd.precio_neto, pd.es_precio_alternativo, pd.flete,
	pr.id_producto, pr.sku, pr.descripcion, pr.sku_proveedor, pr.imagen, pr.proveedor, 
	pr.costo as costo_producto, pr.precio as precio_producto,
	pd.observaciones,
	pd.fecha_modificacion,

	--Si es precio alternativo y el precio registrado es estandar
--	 CASE pd.es_precio_alternativo WHEN 
	CASE pc.es_unidad_alternativa WHEN 1 THEN pc.precio_neto * pc.equivalencia 
	ELSE pc.precio_neto END as precio_neto_vigente, 

	pc.flete as flete_vigente, 

	CASE pc.es_unidad_alternativa WHEN 1 THEN pc.precio_unitario * pc.equivalencia 
	ELSE pc.precio_unitario END as precio_unitario_vigente, 

	pc.equivalencia as equivalencia_vigente,
	pc.id_precio_cliente_producto,
	pc.fecha_inicio_vigencia,
	pc.fecha_fin_vigencia,
	pc.id_cliente,

	vd.precio_unitario as precio_unitario_venta,
	vd.igv_precio_unitario as igv_precio_unitario_venta,	
	vd.id_venta_detalle,
	(pd.cantidad - COALESCE(mad.cantidadAtendida,0)) AS cantidadPendienteAtencion,

	ROW_NUMBER() OVER(PARTITION BY pd.id_producto,pe.id_cliente 
	ORDER BY pc.fecha_inicio_vigencia DESC, pc.fecha_creacion DESC) AS RowNumber


	FROM PEDIDO_DETALLE as pd 
	INNER JOIN PEDIDO as pe ON pd.id_pedido = pe.id_pedido 
	INNER JOIN PRODUCTO pr ON pd.id_producto = pr.id_producto
	LEFT JOIN 
	(
	SELECT mad.id_pedido_detalle, SUM(cantidad) as cantidadAtendida  FROM
	MOVIMIENTO_ALMACEN_DETALLE AS mad 
	INNER JOIN MOVIMIENTO_ALMACEN ma ON mad.id_movimiento_almacen = ma.id_movimiento_almacen
	WHERE ma.id_pedido = @idPedido
	AND mad.estado = 1 AND  ma.estado = 1 AND ma.anulado = 0
	GROUP BY mad.id_pedido_detalle
	) AS  mad ON mad.id_pedido_detalle  = pd.id_pedido_detalle
	
	LEFT JOIN VENTA_DETALLE AS vd ON vd.id_pedido_detalle = mad.id_pedido_detalle


	LEFT JOIN
	(SELECT pc.* FROM 
		PRECIO_CLIENTE_PRODUCTO pc
		WHERE --fecha_inicio_vigencia < GETDATE()
		--AND (fecha_fin_vigencia is NULL OR fecha_fin_vigencia >= GETDATE())
		--AND
		 fecha_inicio_vigencia > DATEADD(day, cast((SELECT valor FROM PARAMETRO where codigo = 'DIAS_MAX_BUSQUEDA_PRECIOS') as int) * -1 , GETDATE()) 
		--ORDER BY fecha_inicio_vigencia DESC
	) pc ON pc.id_producto = pr.id_producto 
	AND pe.id_cliente = pc.id_cliente AND pd.equivalencia = pc.equivalencia
	--AND pd.es_precio_alternativo = pc.es_unidad_alternativa


	where pd.id_pedido = @idPedido and pd.estado = 1 
	
	) SQuery 
		where RowNumber = 1
	ORDER BY fecha_modificacion ASC;


--RECUPERA LAS DIRECCIONES DE ENTREGA

SELECT de.id_direccion_entrega, de.descripcion, de.contacto, de.telefono, ubigeo
FROM DIRECCION_ENTREGA de
INNER JOIN PEDIDO pe ON de.id_cliente = pe.id_cliente
where pe.id_pedido = @idPedido
AND de.estado = 1;


--RECUPERA LAS GUÍAS Y FACTURAS

SELECT 
ma.id_movimiento_almacen, ma.serie_documento, ma.numero_documento, 
ma.fecha_emision,
ma.fecha_traslado,
dv.id_cpe_cabecera_be as id_documento_venta, dv.SERIE, dv.CORRELATIVO, 
 CONVERT( DATETIME,dv.FEC_EMI ,20) AS fecha_emision_factura,
 	CASE  dv.COD_ESTD_SUNAT 
	WHEN '101' THEN 'EN PROCESO'
	WHEN '102' THEN 'ACEPTADO'
	WHEN '103' THEN 'ACEPTADO CON OBS'
	WHEN '104' THEN 'RECHAZADO'
	WHEN '105' THEN 'ANULADO'
	ELSE '---' END as estado ,
mad.id_movimiento_almacen_detalle, mad.cantidad, 
mad.unidad, pr.id_producto, pr.sku, pr.descripcion
FROM MOVIMIENTO_ALMACEN_DETALLE as mad 
INNER JOIN MOVIMIENTO_ALMACEN as ma ON mad.id_movimiento_almacen = ma.id_movimiento_almacen
INNER JOIN PRODUCTO pr ON mad.id_producto = pr.id_producto
INNER JOIN VENTA ve ON ve.id_movimiento_almacen = ma.id_movimiento_almacen
LEFT JOIN CPE_CABECERA_BE dv ON ve.id_documento_venta = dv.id_cpe_cabecera_be
AND mad.estado = 1
WHERE ma.id_pedido = @idPedido
AND ma.estado = 1
AND ma.anulado = 0
ORDER BY ma.numero_documento, mad.fecha_creacion asc;



--RECUPERA LOS ARCHIVOS ADJUNTOS

SELECT arch.id_archivo_adjunto as id_pedido_adjunto,  nombre--, arch.adjunto,
 FROM ARCHIVO_ADJUNTO arch
 INNER JOIN PEDIDO_ARCHIVO parch ON arch.id_archivo_adjunto = parch.id_archivo_adjunto
   WHERE parch.id_pedido = @idPedido
   AND arch.estado = 1
   AND parch.estado = 1;


SELECT so.id_solicitante, so.nombre, so.telefono, so.correo
FROM SOLICITANTE so
INNER JOIN PEDIDO pe ON so.id_cliente = pe.id_cliente
where pe.id_pedido = @idPedido
AND so.estado = 1;

END








