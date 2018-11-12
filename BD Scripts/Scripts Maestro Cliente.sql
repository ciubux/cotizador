ALTER TABLE CLIENTE 
ADD telefono_contacto1 VARCHAR(50),
 email_contacto1 VARCHAR(50), 
  negociacion_multiregional BIT default 'FALSE';


UPDATE CLIENTE
SET negociacion_multiregional = 'FALSE';






/* Agregar checks, email y correo contacto compras */
ALTER PROCEDURE [dbo].[pi_clienteSunat] 
@idUsuario uniqueidentifier,
@razonSocial  varchar(200),
@nombreComercial  varchar(200),
@ruc  varchar(20),
@contacto1  varchar(100),
@idCiudad uniqueidentifier,
@correoEnvioFactura varchar(1000),
@razonSocialSunat varchar(500),
@nombreComercialSunat varchar(500),
@direccionDomicilioLegalSunat varchar(1000),
@estadoContribuyente varchar(50),
@condicionContribuyente varchar(50),
@ubigeo varchar(6),
@formaPagoFactura int,
/*Plazo credito*/
@plazoCreditoSolicitado int,
@tipoPagoFactura int,
@sobrePlazo int, 
/*Monto Crédito*/
@creditoSolicitado decimal(12,2),
@creditoAprobado decimal(12,2),
@sobreGiro decimal(12,2),
/*Vendedores*/
@idResponsableComercial int,
@idAsistenteServicioCliente int,
@idSupervisorComercial int,

@tipoDocumento char(1),
@observacionesCredito varchar(1000),
@observaciones varchar(1000),
@vendedoresAsignados smallint,
@bloqueado smallint,
@perteneceCanalMultiregional smallint,
@perteneceCanalLima smallint,
@perteneceCanalProvincias smallint,
@perteneceCanalPCP smallint,
@esSubDistribuidor smallint,

@idGrupoCliente int,
@horaInicioPrimerTurnoEntrega datetime,
@horaFinPrimerTurnoEntrega datetime,
@horaInicioSegundoTurnoEntrega datetime,
@horaFinSegundoTurnoEntrega datetime,

/* Campos agregados  */
@sedePrincipal bit,
@negociacionMultiregional bit,
@telefonoContacto1 varchar(50),
@emailContacto1 varchar(50),

@newId uniqueidentifier OUTPUT, 
@codigoAlterno int OUTPUT,
@codigo VARCHAR(4) OUTPUT

AS
BEGIN TRAN


Select @codigo = siguiente_codigo_cliente FROM CIUDAD where id_ciudad = @idCiudad;


SET NOCOUNT ON
SET @newId = NEWID();
SET @codigoAlterno = NEXT VALUE FOR SEQ_CODIGO_ALTERNO_CLIENTE;

IF @tipoDocumento <> 6
BEGIN 
	SET @razonSocial = @nombreComercial;
END

INSERT INTO CLIENTE
           ([id_cliente]
		   ,[codigo_alterno]
           ,[razon_Social]
		   ,[nombre_Comercial]
           ,[ruc]
           ,[contacto1]
		   ,[id_ciudad]
		   ,estado
		   ,[usuario_creacion]
		   ,[fecha_creacion]
		   ,[usuario_modificacion]
		   ,[fecha_modificacion]
		   ,correo_Envio_Factura
		   ,razon_Social_Sunat
		   ,nombre_Comercial_Sunat
		   ,direccion_Domicilio_Legal_Sunat
		   ,estado_Contribuyente_sunat
		   ,condicion_Contribuyente_sunat
		   ,ubigeo
		   ,direccion_despacho
		   ,forma_pago_factura
		   ,tipo_documento
		   ,codigo,
		   /*Plazo credito*/
			plazo_credito_solicitado,
			tipo_pago_factura,
			sobre_plazo, 
			/*Monto Crédito*/
			credito_solicitado,
			credito_aprobado,
			sobre_giro, 
			/*Vendedores*/
			id_responsable_comercial,
			id_asistente_servicio_cliente,
			id_supervisor_comercial,
			observaciones_credito,
			observaciones,
			vendedores_asignados,
			bloqueado,
			pertenece_canal_multiregional,
			pertenece_canal_lima,
			pertenece_canal_provincia,
			pertenece_canal_pcp,
			es_sub_distribuidor,
			hora_inicio_primer_turno_entrega,
			hora_fin_primer_turno_entrega,
			hora_inicio_segundo_turno_entrega,
			hora_fin_segundo_turno_entrega,
			sede_principal,
			negociacion_multiregional,
			telefono_contacto1,
			email_contacto1
		   )
     VALUES
           (@newId,
		    @codigoAlterno,
		    @razonSocial,
			@nombreComercial,
            @ruc,
            @contacto1, 
			@idCiudad,
			1,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE(),
			@correoEnvioFactura,
			@razonSocialSunat,
			@nombreComercialSunat,
			@direccionDomicilioLegalSunat,
			@estadoContribuyente,
			@condicionContribuyente,
			@ubigeo,
			@direccionDomicilioLegalSunat,
		--	@tipoPagoFactura,
			@formaPagoFactura,
			@tipoDocumento,
			@codigo,--codigo
			@plazoCreditoSolicitado,
			@tipoPagoFactura,
			@sobrePlazo, 
			/*Monto Crédito*/
			@creditoSolicitado,
			@creditoAprobado,
			@sobreGiro, 
			/*Vendedores*/
			@idResponsableComercial,
			@idAsistenteServicioCliente ,
			@idSupervisorComercial,
			@observacionesCredito,
			@observaciones,
			@vendedoresAsignados,
			@bloqueado ,
			@perteneceCanalMultiregional ,
			@perteneceCanalLima ,
			@perteneceCanalProvincias ,
			@perteneceCanalPCP ,
			@esSubDistribuidor, 
			@horaInicioPrimerTurnoEntrega,
			@horaFinPrimerTurnoEntrega,
			@horaInicioSegundoTurnoEntrega,
			@horaFinSegundoTurnoEntrega,
			@sedePrincipal,
			@negociacionMultiregional,
			@telefonoContacto1,
			@emailContacto1
			);

IF @codigo = 'LY99'
BEGIN
	UPDATE CIUDAD set siguiente_codigo_cliente = 'LV01'	where id_ciudad = @idCiudad;
END 
ELSE 
BEGIN
	UPDATE CIUDAD set siguiente_codigo_cliente = 
	CONCAT( LEFT(siguiente_codigo_cliente,2),
	FORMAT (CAST(SUBSTRing (siguiente_codigo_cliente,3,2) AS INT) + 1 , '00' ))
	where id_ciudad = @idCiudad
END

UPDATE CLIENTE 
SET negociacion_multiregional = @negociacionMultiregional
WHERE ruc like @ruc;
	
IF @negociacionMultiregional = 'FALSE'
BEGIN
	UPDATE CLIENTE 
	SET sede_principal = 'FALSE'
	WHERE ruc like @ruc;
END

IF @idGrupoCliente > 0 
BEGIN
	INSERT INTO CLIENTE_GRUPO_CLIENTE 
	VALUES (@newId, @idGrupoCliente, GETDATE(), 1, @idUsuario, GETDATE(), @idUsuario, GETDATE())
END

COMMIT




/* */

ALTER PROCEDURE [dbo].[pu_clienteSunat] 
@idCliente uniqueidentifier,
@idUsuario uniqueidentifier,
@razonSocial  varchar(200),
@nombreComercial  varchar(200),
@contacto1  varchar(100),
@idCiudad uniqueidentifier,
@correoEnvioFactura varchar(1000),
@razonSocialSunat varchar(500),
@nombreComercialSunat varchar(500),
@direccionDomicilioLegalSunat varchar(1000),
@estadoContribuyente varchar(50),
@condicionContribuyente varchar(50),
@ubigeo varchar(6),
@formaPagoFactura int,

/*Plazo credito*/
@plazoCreditoSolicitado int,
@tipoPagoFactura int,
@sobrePlazo int, 
/*Monto Crédito*/
@creditoSolicitado decimal(12,2),
@creditoAprobado decimal(12,2),
@sobreGiro decimal(12,2),
/*Vendedores*/
@idResponsableComercial int,
@idAsistenteServicioCliente int,
@idSupervisorComercial int,

@observacionesCredito varchar(1000),
@observaciones varchar(1000),
@vendedoresAsignados smallint,
@bloqueado smallint,
@perteneceCanalMultiregional smallint,
@perteneceCanalLima smallint,
@perteneceCanalProvincias smallint,
@perteneceCanalPCP smallint,
@esSubDistribuidor smallint,
@idGrupoCliente int,
@horaInicioPrimerTurnoEntrega datetime,
@horaFinPrimerTurnoEntrega datetime,
@horaInicioSegundoTurnoEntrega datetime,
@horaFinSegundoTurnoEntrega datetime,
/* Campos agregados  */
@sedePrincipal bit,
@negociacionMultiregional bit,
@telefonoContacto1 varchar(50),
@emailContacto1 varchar(50)

AS
BEGIN

DECLARE @ruc varchar(20); /* Agregado */
DECLARE @nrAnterior bit; /* Agregado */

SET NOCOUNT ON

IF (SELECT tipo_documento FROM CLIENTE where id_cliente = @idCliente) <> 6
BEGIN 
	SET @razonSocial = @nombreComercial;
END


SELECT @ruc = ruc, @nrAnterior = negociacion_multiregional FROM CLIENTE WHERE id_cliente = @idCliente AND estado = 1; /* Agregado */

UPDATE CLIENTE SET [razon_Social] = @razonSocial
		   ,[nombre_Comercial] = @nombreComercial   
		   ,[id_ciudad] = @idCiudad
		   ,[usuario_modificacion] = @idUsuario
		   ,[fecha_modificacion] = GETDATE()
		   ,correo_Envio_Factura = @correoEnvioFactura
		   ,razon_Social_Sunat = @razonSocialSunat
		   ,nombre_Comercial_Sunat = nombre_Comercial_Sunat
		   ,direccion_Domicilio_Legal_Sunat = @direccionDomicilioLegalSunat
		   ,estado_Contribuyente_sunat = @estadoContribuyente
		   ,condicion_Contribuyente_sunat = @condicionContribuyente
		   ,ubigeo = @ubigeo
		   ,forma_pago_factura = @formaPagoFactura

		   /*Plazo credito*/
			,plazo_credito_solicitado = @plazoCreditoSolicitado
			,tipo_pago_factura = @tipoPagoFactura
			,sobre_plazo = @sobrePlazo
			/*Monto Crédito*/
			,credito_solicitado = @creditoSolicitado
			,credito_aprobado = @creditoAprobado
			,sobre_giro = @sobreGiro
			/*Vendedores*/
			,id_responsable_comercial = @idResponsableComercial
			,id_asistente_Servicio_cliente = @idAsistenteServicioCliente
			,id_supervisor_comercial = @idSupervisorComercial
			,observaciones_credito = @observacionesCredito
			,observaciones = @observaciones
			,vendedores_asignados = @vendedoresAsignados
			,bloqueado = @bloqueado
			,pertenece_canal_multiregional = @perteneceCanalMultiregional
			,pertenece_canal_lima = @perteneceCanalLima
			,pertenece_canal_provincia = @perteneceCanalProvincias
			,pertenece_canal_pcp =@perteneceCanalPCP
			,es_sub_distribuidor = @esSubDistribuidor
			,hora_inicio_primer_turno_entrega = @horaInicioPrimerTurnoEntrega
			,hora_fin_primer_turno_entrega = @horaFinPrimerTurnoEntrega
			,hora_inicio_segundo_turno_entrega = @horaInicioSegundoTurnoEntrega
			,hora_fin_segundo_turno_entrega = @horaFinSegundoTurnoEntrega
			,sede_principal = @sedePrincipal
			,negociacion_multiregional = @negociacionMultiregional
			,telefono_contacto1 = @telefonoContacto1
			,email_contacto1 = @emailContacto1
     WHERE 
          id_cliente = @idCliente;

		  
/* IF Agregado */
IF @nrAnterior != @negociacionMultiregional 
BEGIN
	UPDATE CLIENTE 
	SET negociacion_multiregional = @negociacionMultiregional
	WHERE ruc like @ruc;
	
	IF @negociacionMultiregional = 'FALSE'
	BEGIN
		UPDATE CLIENTE 
		SET sede_principal = 'FALSE'
		WHERE ruc like @ruc;
	END
END

DELETE CLIENTE_GRUPO_CLIENTE 
where id_cliente = @idCliente;


IF @idGrupoCliente > 0 
BEGIN
	INSERT INTO CLIENTE_GRUPO_CLIENTE 
	VALUES (@idCliente, @idGrupoCliente, GETDATE(), 1, @idUsuario, GETDATE(), @idUsuario, GETDATE())
END
			/*
IF @idGrupoCliente > 0 
BEGIN

	MERGE dbo.CLIENTE_GRUPO_CLIENTE AS target  
	USING (SELECT id_grupo_cliente, id_cliente
	FROM CLIENTE_GRUPO_CLIENTE 
	where id_grupo_cliente = @idGrupoCliente
	AND id_cliente = @idCliente
	) AS source --(ProductID, OrderQty)  
	ON (target.id_grupo_cliente = source.id_grupo_cliente
	AND target.id_cliente = source.id_cliente )  
	WHEN NOT MATCHED THEN
	 INSERT (ID_CLIENTE, ID_GRUPO_CLIENTE, FECHA_INGRESO, ESTADO, USUARIO_CREACION,
		FECHA_CREACION, USUARIO_MODIFICACIOn, FECHA_MODIFICACION)
			VALUES (@idCliente, @idGrupoCliente, GETDATE(), 1, @idUsuario, GETDATE(), @idUsuario, GETDATE())
	WHEN MATCHED   
		THEN UPDATE SET
		target.ESTADO = 1,   
		target.USUARIO_MODIFICACION = @idUsuario, 
		target.FECHA_MODIFICACION = GETDATE();

END*/

END




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




/* Buscar cotizaciones por grupo cliente */
ALTER PROCEDURE [dbo].[ps_cotizaciones] 

@codigo bigint,
@id_cliente uniqueidentifier,
@id_ciudad uniqueidentifier,
@id_usuario uniqueidentifier,
@idGrupoCliente int,
@fechaDesde datetime,
@fechaHasta datetime, 
@estado smallint
AS
BEGIN

if  @codigo = 0 

	SELECT 
	--COTIZACION
	co.codigo as cod_cotizacion, co.id_cotizacion, co.fecha, co.incluido_igv, co.considera_cantidades,
	co.porcentaje_flete, co.igv, co.total, co.observaciones,co.contacto, 
	--CLIENTE
	cl.codigo,  cl.id_cliente,cl.razon_social, cl.ruc, 
	 --USUARIO
	us.nombre as nombre_usuario, us.id_usuario,
	 --CIUDAD
	ci.id_ciudad, ci.nombre as nombre_ciudad, 
	--SEGUIMIENTO
	sc.estado_cotizacion as estado_seguimiento,
	us2.nombre as usuario_seguimiento, sc.observacion as observacion_seguimiento,
	us2.id_usuario as id_usuario_seguimiento,
	--DETALLES
	(SELECT max(porcentaje_descuento) from COTIZACION_DETALLE  WHERE estado = 1 AND id_cotizacion = co.id_cotizacion) as maximo_porcentaje_descuento
/*	(
	SELECT CAST(MIN(((1 - (CASE cd.es_precio_alternativo WHEN 1 THEN pr.costo/pr.equivalencia ELSE pr.costo END )/( CASE cd.precio_neto WHEN 0 THEN 1 ELSE cd.precio_neto END ))*100)) AS DECIMAL(12,1)) from COTIZACION_DETALLE cd INNER JOIN PRODUCTO pr
	ON pr.id_producto = cd.id_producto
	 WHERE cd.estado = 1 AND cd.id_cotizacion = co.id_cotizacion) as minimo_margen
	 */
--	(1 - costoLista / ( precioNeto==0?1: precioNeto))    *100));*/

--	SELECT TOP 5 * FROM COTIZACION_DETALLE
	
	 FROM COTIZACION as co
	INNER JOIN CLIENTE AS cl ON co.id_cliente = cl.id_cliente
	INNER JOIN CIUDAD AS ci ON co.id_ciudad = ci.id_ciudad
	INNER JOIN USUARIO AS us on co.usuario_creacion = us.id_usuario
	INNER JOIN SEGUIMIENTO_COTIZACION sc on sc.id_cotizacion = co.id_cotizacion
	INNER JOIN USUARIO AS us2 on sc.id_usuario = us2.id_usuario
	where   co.fecha > @fechaDesde 
	and co.fecha <=  @fechaHasta
	and (
		co.id_cliente = @id_cliente OR
		(@id_cliente = '00000000-0000-0000-0000-000000000000' AND @idGrupoCliente <= 0) OR
		(@idGrupoCliente > 0 AND co.id_cliente in (select id_cliente from CLIENTE_GRUPO_CLIENTE where id_grupo_cliente = @idGrupoCliente))
	)
	
	
	and (ci.id_ciudad = @id_ciudad or @id_ciudad = '00000000-0000-0000-0000-000000000000')
	and (us.id_usuario = @id_usuario or @id_usuario = '00000000-0000-0000-0000-000000000000'
	OR @id_usuario IN (SELECT id_usuario FROM USUARIO where visualiza_cotizaciones = 1 )
	)
	AND (sc.estado_cotizacion = @estado or @estado = -1  )
	AND sc.estado = 1
	and co.estado = 1
	order by co.codigo asc ;

else

	SELECT 
	--COTIZACION
	co.codigo as cod_cotizacion, co.id_cotizacion, co.fecha, co.incluido_igv, co.considera_cantidades,
	co.porcentaje_flete, co.igv, co.total, co.observaciones,co.contacto, 
	--CLIENTE
	 cl.codigo,  cl.id_cliente,cl.razon_social, cl.ruc, 
	 --USUARIO
	 us.nombre as nombre_usuario, us.id_usuario,
	 --CIUDAD
	ci.id_ciudad, ci.nombre as nombre_ciudad, 
	--SEGUIMIENTO
	sc.estado_cotizacion as estado_seguimiento,
	us2.nombre as usuario_seguimiento, sc.observacion as observacion_seguimiento,
		us2.id_usuario as id_usuario_seguimiento,
	--DETALLES
	(SELECT max(porcentaje_descuento) from COTIZACION_DETALLE  WHERE estado = 1 AND id_cotizacion = co.id_cotizacion) as maximo_porcentaje_descuento
	
	 FROM COTIZACION as co
	INNER JOIN CLIENTE AS cl ON co.id_cliente = cl.id_cliente
	INNER JOIN CIUDAD AS ci ON co.id_ciudad = ci.id_ciudad
	INNER JOIN USUARIO AS us on co.usuario_creacion = us.id_usuario
	INNER JOIN SEGUIMIENTO_COTIZACION sc on sc.id_cotizacion = co.id_cotizacion
	INNER JOIN USUARIO AS us2 on sc.id_usuario = us2.id_usuario
	where co.codigo = @codigo 
	--filtro que evita que un usuario pueda obtener una cotización de otro usuario a través del codigo
	--and (us.id_usuario = @id_usuario or @id_usuario = '00000000-0000-0000-0000-000000000000')
--	AND (sc.estado_cotizacion = @estado or @estado = -1  )
	AND sc.estado = 1
	and co.estado = 1


END




/* Busacar guias remision por grupo cliente */
ALTER PROCEDURE [dbo].[ps_guiasRemision] 
@numeroDocumento bigint,
@idCiudad uniqueidentifier,
@idCliente uniqueidentifier,
@idGrupoCliente int,
@idUsuario uniqueidentifier,
@fechaTrasladoDesde datetime,
@fechaTrasladoHasta datetime, 
@anulado smallint, 
@facturado smallint, 
@numeroPedido bigint,
@motivoTraslado char(1)
AS
BEGIN

if  @numeroDocumento = 0 AND @numeroPedido = 0
BEGIN
	SELECT 
	--GUIA_REMISION
	ma.serie_documento, ma.numero_documento,ma.id_movimiento_almacen,
	ma.fecha_emision, ma.fecha_traslado,
	ma.atencion_parcial, ma.ultima_atencion_parcial,  ma.anulado, ma.facturado,
	ma.no_entregado, ma.tipo_extorno, ma.motivo_traslado, ma.ingresado,
	--PEDIDO
	pe.id_pedido, pe.numero as numero_pedido,
	--CLIENTE
	 cl.codigo,  cl.id_cliente,cl.razon_social, cl.ruc, 
	 --USUARIO
	 us.nombre as nombre_usuario, us.id_usuario,
	 --CIUDAD
	ci.id_ciudad, ci.nombre as nombre_ciudad--, 
	--SEGUIMIENTO
	--sc.estado_movimiento_almacen as estado_seguimiento,
--	us2.nombre as usuario_seguimiento, sc.observacion as observacion_seguimiento,
--	us2.id_usuario as id_usuario_seguimiento	

	
	
	FROM MOVIMIENTO_ALMACEN as ma
	INNER JOIN CIUDAD AS ci ON ma.id_sede_origen = ci.id_ciudad
	INNER JOIN USUARIO AS us on ma.usuario_creacion = us.id_usuario
	--LEFT JOIN SEGUIMIENTO_MOVIMIENTO_ALMACEN sc on sc.id_movimiento_almacen = ma.id_movimiento_almacen
	/*INNER JOIN USUARIO AS us2 on sc.id_usuario = us2.id_usuario*/
	LEFT JOIN PEDIDO AS pe ON ma.id_pedido = pe.id_pedido
	LEFT JOIN CLIENTE AS cl ON cl.id_cliente = ma.id_cliente
	where   ma.fecha_traslado >= @fechaTrasladoDesde 
	and ma.fecha_traslado <=  @fechaTrasladoHasta
	and (cl.id_cliente = @idCliente OR
	(@idCliente = '00000000-0000-0000-0000-000000000000' AND @idGrupoCliente <= 0) OR 
	(@idGrupoCliente > 0 AND cl.id_cliente in (select id_cliente from CLIENTE_GRUPO_CLIENTE where id_grupo_cliente = @idGrupoCliente))
	)
	and (ci.id_ciudad = @idCiudad  or @idCiudad = '00000000-0000-0000-0000-000000000000')
	--and (us.id_usuario = @idUsuario or @idUsuario = '00000000-0000-0000-0000-000000000000')
	--AND (sc.estado_movimiento_almacen = @estado or @estado = -1  )
	--AND sc.estado = 1
	and ma.estado = 1
	and (ma.anulado = 0 OR  ma.anulado = @anulado)
	and (ma.facturado = 0 OR  ma.facturado = @facturado)
	and ma.tipo_movimiento = 'S'
	AND ma.tipo_documento = 'GR'
	AND (@motivoTraslado = '0' OR @motivoTraslado = ma.motivo_traslado)
	order by ma.numero_documento asc ;
END
else

--SELECT * FROM MOVIMIENTO_ALMACEN


		SELECT 
	--GUIA_REMISION
	ma.serie_documento, ma.numero_documento,ma.id_movimiento_almacen,
	ma.fecha_emision, ma.fecha_traslado,
	ma.atencion_parcial,  ma.ultima_atencion_parcial,  ma.anulado,ma.facturado,
	ma.no_entregado, ma.tipo_extorno, ma.motivo_traslado, ma.ingresado,
	--PEDIDO
	pe.id_pedido, pe.numero as numero_pedido,
	--CLIENTE
	 cl.codigo,  cl.id_cliente,cl.razon_social, cl.ruc, 
	 --USUARIO
	 us.nombre as nombre_usuario, us.id_usuario,
	 --CIUDAD
	ci.id_ciudad, ci.nombre as nombre_ciudad--, 
	--SEGUIMIENTO
--	sc.estado_movimiento_almacen as estado_seguimiento,
--	us2.nombre as usuario_seguimiento, sc.observacion as observacion_seguimiento,
--	us2.id_usuario as id_usuario_seguimiento	
	
	FROM MOVIMIENTO_ALMACEN as ma
	INNER JOIN CIUDAD AS ci ON ma.id_sede_origen = ci.id_ciudad
	INNER JOIN USUARIO AS us on ma.usuario_creacion = us.id_usuario
--	INNER JOIN SEGUIMIENTO_MOVIMIENTO_ALMACEN sc on sc.id_movimiento_almacen = ma.id_movimiento_almacen
	--INNER JOIN USUARIO AS us2 on sc.id_usuario = us2.id_usuario
	LEFT JOIN PEDIDO AS pe ON ma.id_pedido = pe.id_pedido
	LEFT JOIN CLIENTE AS cl ON cl.id_cliente = ma.id_cliente
	where ((ma.numero_documento = @numeroDocumento
	OR pe.numero = @numeroPedido )
	AND (ma.id_sede_origen = (Select id_ciudad FROM USUARIO where id_usuario = @idUsuario)
	OR motivo_traslado = 'T'
	OR ma.id_sede_origen = @idCiudad)
	)
	and ma.estado = 1
	and ma.tipo_movimiento = 'S'
	AND ma.tipo_documento = 'GR'


END;



/* Buscar facturas por grupo cliente */
ALTER PROCEDURE [dbo].[ps_facturas] 

@numero varchar(8),
@idCliente uniqueidentifier,
@idGrupoCliente int,
@idCiudad uniqueidentifier,
@idUsuario uniqueidentifier,
@fechaDesde datetime,
@fechaHasta datetime,
@soloSolicitudAnulacion int,
@estado int,
@numeroPedido bigint,
@numeroGuiaRemision bigint,
@tipoDocumento int
AS
BEGIN


if ( @numero IS NULL OR @numero = '') AND  @numeroPedido  = 0 AND @numeroGuiaRemision = 0

	SELECT 
	--FACTURA
	dv.id_cpe_cabecera_be as id_documento_venta, 
	dv.SERIE, dv.CORRELATIVO, 
	dv.MNT_TOT MNT_TOT_PRC_VTA,
	dv.solicitud_anulacion,
	dv.comentario_solicitud_anulacion,
	CAST(dv.TIP_CPE AS INT) TIP_CPE,
	CASE  dv.COD_ESTD_SUNAT 
	WHEN '101' THEN 101
	WHEN '102' THEN 102
	WHEN '103' THEN 103
	WHEN '104' THEN 104
	WHEN '105' THEN 105
	WHEN '106' THEN 106
	WHEN '108' THEN 108
	ELSE 0 END as estado ,
	--USUARIO
	us.nombre as nombre_usuario, us.id_usuario,
	CONVERT(datetime, FEC_EMI, 101) as fecha_emision,
	--CLIENTE
	cl.codigo ,
	cl.id_cliente,
	dv.NOM_RCT AS razon_social, dv.NRO_DOC_RCT AS ruc, 
	  --CIUDAD
	ci.id_ciudad, ci.nombre as nombre_ciudad,
	--PEDIDO
	p.numero,
	--MOVIMIENTO
	ma.serie_documento,
	ma.numero_documento,
	dv.permite_anulacion,
	v.id_venta_afectacion

	FROM CPE_CABECERA_BE as dv
	INNER JOIN SERIE_DOCUMENTO_ELECTRONICO as se ON SUBSTRING(dv.SERIE,3,2) = SUBSTRING(se.serie,2,2) 
	INNER JOIN CIUDAD AS ci ON se.id_sede_mp = ci.id_ciudad
	LEFT JOIN CLIENTE AS cl ON (
	cl.ruc = dv.NRO_DOC_RCT AND
	cl.id_ciudad = ci.id_ciudad AND cl.estado = 1 ) 
	LEFT JOIN VENTA v ON dv.id_venta = v.id_venta
	LEFT JOIN MOVIMIENTO_ALMACEN ma ON v.id_movimiento_almacen = ma.id_movimiento_almacen
	LEFT JOIN PEDIDO p ON v.id_pedido = p.id_pedido 
	LEFT JOIN USUARIO AS us ON dv.usuario_creacion = us.id_usuario
	where  CONVERT(datetime, FEC_EMI, 101)  >= @fechaDesde 
	and CONVERT(datetime, FEC_EMI, 101)  <=  @fechaHasta
	and (cl.id_cliente = @idCliente OR  
		(@idCliente = '00000000-0000-0000-0000-000000000000' AND @idGrupoCliente <= 0) OR 
		(@idGrupoCliente > 0 AND cl.id_cliente in (select id_cliente from CLIENTE_GRUPO_CLIENTE where id_grupo_cliente = @idGrupoCliente))
	)
	and (ci.id_ciudad = @idCiudad  or @idCiudad = '00000000-0000-0000-0000-000000000000'
	)
	AND dv.estado = 1
	AND (dv.TIP_CPE = @tipoDocumento 
		OR @tipoDocumento = 0 
	)
	AND (
		dv.COD_ESTD_SUNAT = @estado 
		OR @estado = 0 
		OR (@estado = 205 AND dv.COD_ESTD_SUNAT IN ('102','103') 
		OR (@estado = 1 AND ( dv.COD_ESTD_SUNAT NOT IN ('101','102', '103','104', '105', '108') OR dv.COD_ESTD_SUNAT IS NULL  ) )
		)
	)
	AND dv.ENVIADO_A_EOL  = 1
	AND (
			@soloSolicitudAnulacion = 0
			OR
			(	solicitud_anulacion = @soloSolicitudAnulacion 
			)
		)

	order  by dv.SERIE asc, dv.CORRELATIVO asc;
		
else

	SELECT 
	dv.id_cpe_cabecera_be as id_documento_venta, 
	--FACTURA
	dv.id_cpe_cabecera_be, 
	dv.SERIE, dv.CORRELATIVO, 
	dv.MNT_TOT MNT_TOT_PRC_VTA,
	dv.solicitud_anulacion,
	dv.comentario_solicitud_anulacion,
	CAST(dv.TIP_CPE AS INT) TIP_CPE,
	CASE  dv.COD_ESTD_SUNAT 
	WHEN '101' THEN 101
	WHEN '102' THEN 102
	WHEN '103' THEN 103
	WHEN '104' THEN 104
	WHEN '105' THEN 105
	WHEN '106' THEN 106
	WHEN '108' THEN 108
	ELSE 0 END as estado ,
	--USUARIO
	us.nombre as nombre_usuario, us.id_usuario,
	CONVERT(datetime, FEC_EMI, 101) as fecha_emision, 
	--CLIENTE
	cl.codigo ,--	 COALESCE(cl.codigo,' ') as codigo,  
	cl.id_cliente, --COALESCE(cl.id_cliente,' ') as id_cliente,
	 dv.NOM_RCT AS razon_social, dv.NRO_DOC_RCT AS ruc, 
	  --CIUDAD
	ci.id_ciudad, ci.nombre as nombre_ciudad,
	--PEDIDO
	p.numero,
	--MOVIMIENTO
	ma.serie_documento,
	ma.numero_documento,
	dv.permite_anulacion,
	v.id_venta_afectacion
	FROM CPE_CABECERA_BE as dv
	INNER JOIN SERIE_DOCUMENTO_ELECTRONICO as se ON SUBSTRING(dv.SERIE,3,2) = SUBSTRING(se.serie,2,2) 
	INNER JOIN CIUDAD AS ci ON se.id_sede_mp = ci.id_ciudad
	LEFT JOIN CLIENTE AS cl ON (
	cl.ruc = dv.NRO_DOC_RCT AND
	cl.id_ciudad = ci.id_ciudad AND cl.estado = 1 ) 
	LEFT JOIN VENTA v ON dv.id_venta = v.id_venta
	LEFT JOIN MOVIMIENTO_ALMACEN ma ON v.id_movimiento_almacen = ma.id_movimiento_almacen
	LEFT JOIN PEDIDO p ON v.id_pedido = p.id_pedido 
--	LEFT JOIN CIUDAD AS ci ON v.id_ciudad = ci.id_ciudad
	LEFT JOIN USUARIO AS us ON dv.usuario_creacion = us.id_usuario
	where  -- dv.fecha_creacion >= @fechaDesde 
--	and dv.fecha_creacion <=  @fechaHasta
--	and
	 (ci.id_ciudad = @idCiudad )
	AND (p.numero = @numeroPedido
	OR  dv.CORRELATIVO = @numero OR ma.numero_documento = @numeroGuiaRemision)
	AND dv.estado = 1
--	and v.estado = 1
	AND dv.ENVIADO_A_EOL  = 1
	AND (dv.TIP_CPE = @tipoDocumento 
		OR @tipoDocumento = 0 
	)
	order by dv.SERIE asc, dv.CORRELATIVO asc;

END





/* Buscar notas de ingreso por grupo cliente */
ALTER PROCEDURE [dbo].[ps_notasIngreso] 
@numeroDocumento bigint,
@idCiudad uniqueidentifier,
@idCliente uniqueidentifier,
@idGrupoCliente int,
@idUsuario uniqueidentifier,
@fechaTrasladoDesde datetime,
@fechaTrasladoHasta datetime, 
@anulado smallint, 
@numeroGuiaReferencia int, 
@numeroPedido bigint,
@motivoTraslado char(1)
AS
BEGIN

if  @numeroDocumento = 0 AND @numeroPedido = 0
BEGIN
	SELECT 
	--GUIA_REMISION
	ma.serie_documento, ma.numero_documento,ma.id_movimiento_almacen,
	ma.fecha_emision, ma.fecha_traslado,
	ma.atencion_parcial, ma.ultima_atencion_parcial,  ma.anulado, ma.facturado,
	ma.no_entregado, ma.tipo_extorno,ma.motivo_traslado,
	--PEDIDO
	pe.id_pedido, pe.numero as numero_pedido,
	--CLIENTE
	 cl.codigo,  cl.id_cliente,cl.razon_social, cl.ruc, 
	 --USUARIO
	 us.nombre as nombre_usuario, us.id_usuario,
	 --CIUDAD
	ci.id_ciudad, ci.nombre as nombre_ciudad--, 
	--SEGUIMIENTO
	--sc.estado_movimiento_almacen as estado_seguimiento,
--	us2.nombre as usuario_seguimiento, sc.observacion as observacion_seguimiento,
--	us2.id_usuario as id_usuario_seguimiento	
	
	FROM MOVIMIENTO_ALMACEN as ma
	INNER JOIN CIUDAD AS ci ON ma.id_sede_destino = ci.id_ciudad
	INNER JOIN USUARIO AS us on ma.usuario_creacion = us.id_usuario
	--LEFT JOIN SEGUIMIENTO_MOVIMIENTO_ALMACEN sc on sc.id_movimiento_almacen = ma.id_movimiento_almacen
	/*INNER JOIN USUARIO AS us2 on sc.id_usuario = us2.id_usuario*/
	LEFT JOIN PEDIDO AS pe ON ma.id_pedido = pe.id_pedido
	LEFT JOIN CLIENTE AS cl ON cl.id_cliente = ma.id_cliente
	LEFT JOIN MOVIMIENTO_ALMACEN as mag ON mag.id_movimiento_almacen = ma.id_movimiento_almacen_ingresado
	where   ma.fecha_traslado > @fechaTrasladoDesde 
	and ma.fecha_traslado <=  @fechaTrasladoHasta
	and (cl.id_cliente = @idCliente OR  
		(@idCliente = '00000000-0000-0000-0000-000000000000' AND @idGrupoCliente <= 0) OR 
		(@idGrupoCliente > 0 AND cl.id_cliente in (select id_cliente from CLIENTE_GRUPO_CLIENTE where id_grupo_cliente = @idGrupoCliente)) 
	)
	and (ci.id_ciudad = @idCiudad or @idCiudad = '00000000-0000-0000-0000-000000000000')
	--and (us.id_usuario = @idUsuario or @idUsuario = '00000000-0000-0000-0000-000000000000')
	--AND (sc.estado_movimiento_almacen = @estado or @estado = -1  )
	--AND sc.estado = 1
	and ma.estado = 1
	and (ma.anulado = 0 OR  ma.anulado = @anulado)
	and ma.tipo_movimiento = 'E'
	AND ma.tipo_documento = 'NI'
	AND (ma.numero_guia_referencia = @numeroGuiaReferencia OR @numeroGuiaReferencia  = 0
	OR mag.numero_documento = @numeroGuiaReferencia
	)
	AND (@motivoTraslado = '0' OR @motivoTraslado = ma.motivo_traslado)
	order by ma.numero_documento asc ;
END
else

--SELECT TOP 5 * FROM MOVIMIENTO_ALMACEN ORDER  BY fecha_creacion DESC

--SELECT * FROM MOVIMIENTO_ALMACEN


		SELECT 
	--GUIA_REMISION
	ma.serie_documento, ma.numero_documento,ma.id_movimiento_almacen,
	ma.fecha_emision, ma.fecha_traslado,
	 ma.atencion_parcial,  ma.ultima_atencion_parcial,  ma.anulado,ma.facturado,
	 ma.no_entregado,  ma.tipo_extorno,ma.motivo_traslado,
	--PEDIDO
	pe.id_pedido, pe.numero as numero_pedido,
	--CLIENTE
	 cl.codigo,  cl.id_cliente,cl.razon_social, cl.ruc, 
	 --USUARIO
	 us.nombre as nombre_usuario, us.id_usuario,
	 --CIUDAD
	ci.id_ciudad, ci.nombre as nombre_ciudad--, 
	--SEGUIMIENTO
--	sc.estado_movimiento_almacen as estado_seguimiento,
--	us2.nombre as usuario_seguimiento, sc.observacion as observacion_seguimiento,
--	us2.id_usuario as id_usuario_seguimiento	
	
	FROM MOVIMIENTO_ALMACEN as ma
	INNER JOIN CIUDAD AS ci ON ma.id_sede_destino = ci.id_ciudad
	INNER JOIN USUARIO AS us on ma.usuario_creacion = us.id_usuario
--	INNER JOIN SEGUIMIENTO_MOVIMIENTO_ALMACEN sc on sc.id_movimiento_almacen = ma.id_movimiento_almacen
	--INNER JOIN USUARIO AS us2 on sc.id_usuario = us2.id_usuario
	LEFT JOIN PEDIDO AS pe ON ma.id_pedido = pe.id_pedido
	LEFT JOIN CLIENTE AS cl ON cl.id_cliente = ma.id_cliente
	where (ma.numero_documento = @numeroDocumento
	OR pe.numero = @numeroPedido 
	)
	--filtro que evita que un usuario pueda obtener una cotización de otro usuario a través del codigo
--	and (us.id_usuario = @idUsuario or @idUsuario = '00000000-0000-0000-0000-000000000000')
--	AND (sc.estado_cotizacion = @estado or @estado = -1  )
--	AND sc.estado = 1
	and ma.estado = 1
	and ma.tipo_movimiento = 'E'
	AND ma.tipo_documento = 'NI'
END;











