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
ELSE
BEGIN
	SET @razonSocial = @razonSocialSunat;
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
			es_proveedor,
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
			1, --ES PROVEEDOR
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




ALTER PROCEDURE [dbo].[ps_cotizaciones] 

@codigo bigint,
@id_cliente uniqueidentifier,
@id_ciudad uniqueidentifier,
@id_usuario uniqueidentifier,
@idGrupoCliente int,
@buscaSedesGrupoCliente bit,
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
	
	
	and (ci.id_ciudad = @id_ciudad OR (@buscaSedesGrupoCliente = 'TRUE' AND @idGrupoCliente > 0) or @id_ciudad = '00000000-0000-0000-0000-000000000000')
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




ALTER PROCEDURE [dbo].[pi_cotizacion] 
@fecha datetime, 
@fechaLimiteValidezOferta datetime,
@fechaInicioVigenciaPrecios datetime,
@fechaFinVigenciaPrecios datetime,
@incluidoIgv smallint, 
@consideraCantidades smallint, 
@idCliente uniqueidentifier, 
@idGrupo uniqueidentifier, 
@idCiudad uniqueidentifier, 
@porcentajeFlete decimal(18,2), 
@igv decimal(18,2), 
@total decimal(18,2), 
@observaciones varchar(1000), 
@idUsuario uniqueidentifier,
@contacto varchar(100),
@mostrarCodigoProveedor smallint,
@mostrarValidezOfertaDias int,
@estado smallint,
@fechaEsModificada smallint,
@observacionSeguimientoCotizacion varchar(500),
@aplicaSedes bit,
@esPagoContado bit,
@newId uniqueidentifier OUTPUT, 
@codigo bigint OUTPUT 
AS
BEGIN

SET NOCOUNT ON

DECLARE @serie int;
DECLARE @aprueba_cotizaciones int;

SET @newId = NEWID();
SET @codigo = NEXT VALUE FOR dbo.SEQ_CODIGO_COTIZACION;
SET @serie = 1;


SELECT @aprueba_cotizaciones = aprueba_cotizaciones_lima FROM USUARIO WHERE id_usuario = @idUsuario;

--Si el usuario es probador la serie es 0
if @aprueba_cotizaciones = 1
BEGIN
	SET @serie = 0;
END

INSERT INTO [COTIZACION]
           ([id_cotizacion]
		   ,[codigo]
           ,[fecha]
		   ,[fecha_inicio_vigencia_precios]
		   ,[fecha_fin_vigencia_precios]
		   ,[fecha_limite_validez_oferta]
           ,[incluido_igv]
           ,[considera_cantidades]
           ,[id_cliente]
		   ,[id_grupo]
           ,[id_ciudad]
           ,[porcentaje_flete]
		   ,[igv]
		   ,[total]
		   ,[observaciones]
		   ,[contacto]
		   ,[estado]
		   ,[usuario_creacion]
		   ,[fecha_creacion]
		   ,[usuario_modificacion]
		   ,[fecha_modificacion]
		   ,[mostrar_codigo_proveedor]
		   ,[mostrar_validez_oferta_dias]
		   ,[serie]
		   ,[fecha_Es_modificada]
		   ,[aplica_sedes]
		   ,[es_pago_contado]
		   )
     VALUES
           (@newId,
		    @codigo,
		    @fecha,
			@fechaInicioVigenciaPrecios,
			@fechaFinVigenciaPrecios,
			@fechaLimiteValidezOferta,
            @incluidoIgv,
            @consideraCantidades, 
            @idCliente, 
			@idGrupo, 
            @idCiudad, 
            @porcentajeFlete,
			@igv,
			@total,
			@observaciones,
			@contacto,
			1,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE(),
			@mostrarCodigoProveedor,
			@mostrarValidezOfertaDias,
			@serie,
			@fechaEsModificada,
			@aplicaSedes,
			@esPagoContado
			);

INSERT INTO SEGUIMIENTO_COTIZACION 
(
		id_estado_seguimiento, 
		id_usuario ,
		id_cotizacion, 
		estado_cotizacion ,
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
			@observacionSeguimientoCotizacion,
			1,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE()
);


UPDATE CLIENTE set contacto1 = @contacto where id_cliente = @idCliente;


END



ALTER PROCEDURE [dbo].[pu_cotizacion] 
@fecha datetime, 
@fechaLimiteValidezOferta datetime,
@fechaInicioVigenciaPrecios datetime,
@fechaFinVigenciaPrecios datetime,
@fechaModificacion datetime,
@incluidoIgv smallint, 
@consideraCantidades smallint, 
@idCliente uniqueidentifier, 
@idGrupo uniqueidentifier, 
@idCiudad uniqueidentifier, 
@porcentajeFlete decimal(18,2), 
@igv decimal(18,2), 
@total decimal(18,2), 
@observaciones varchar(1000), 
@idUsuario uniqueidentifier,
@contacto varchar(100),
@codigo bigint,
@mostrarCodigoProveedor smallint,
@mostrarValidezOfertaDias int,
@estado int,
@fechaEsModificada smallint,
@observacionSeguimientoCotizacion varchar(500),
@aplicaSedes bit,
@esPagoContado bit,
@fechaModificacionActual datetime OUTPUT
AS
BEGIN

DECLARE @id_cotizacion uniqueidentifier;





SELECT @id_cotizacion =id_cotizacion, @fechaModificacionActual = fecha_modificacion FROM COTIZACION WHERE codigo = @codigo AND estado = 1;

/*if(convert(varchar,@fechaModificacionActual, 120)  = convert(varchar,@fechaModificacion, 120)) 
Begin*/

	UPDATE [COTIZACION_DETALLE] SET ESTADO = 0 ,
	[usuario_modificacion] = @idUsuario, [fecha_modificacion] = GETDATE()
	WHERE id_cotizacion = @id_cotizacion;


	UPDATE [COTIZACION] SET
			   [fecha] =  @fecha
			   ,[fecha_Limite_Validez_Oferta] = @fechaLimiteValidezOferta
			   ,[fecha_Inicio_Vigencia_Precios] = @fechaInicioVigenciaPrecios
			   ,[fecha_Fin_Vigencia_Precios] = @fechaFinVigenciaPrecios
			   ,[incluido_igv] = @incluidoIgv
			   ,[considera_cantidades] = @consideraCantidades
			   ,[id_cliente] = @idCliente
			   ,[id_grupo] = @idGrupo
			   ,[id_ciudad] = @idCiudad
			   ,[porcentaje_flete] = @porcentajeFlete
			   ,[igv] = @igv
			   ,[total] = @total
			   ,[observaciones] = @observaciones
			   ,[contacto] = @contacto
			   ,[usuario_modificacion] = @idUsuario
			   ,[fecha_modificacion] = GETDATE()
			--   ,[estado_aprobacion] = @estadoAprobacion
			   ,[mostrar_codigo_proveedor] = @mostrarCodigoProveedor
			   ,[mostrar_Validez_Oferta_Dias] = @mostrarValidezOfertaDias
			   ,[fecha_es_modificada]  = @fechaEsModificada
			   ,[aplica_sedes]  = @aplicaSedes
			   ,[es_pago_contado]  = @esPagoContado
			     WHERE id_cotizacion = @id_cotizacion ;
		             
	UPDATE SEGUIMIENTO_COTIZACION set estado = 0 where id_cotizacion = @id_cotizacion;
				

	INSERT INTO SEGUIMIENTO_COTIZACION 
	(
			id_estado_seguimiento, 
			id_usuario ,
			id_cotizacion, 
			estado_cotizacion ,
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
				@id_cotizacion,
				@estado,
				@observacionSeguimientoCotizacion,
				1,
				@idUsuario,
				GETDATE(),
				@idUsuario,
				GETDATE()
	);
--end;

END







ALTER PROCEDURE [dbo].[ps_cotizacion] 
@codigo bigint
AS
BEGIN

SELECT 
--COTIZACION
co.id_cotizacion, co.fecha,  co.fecha_limite_validez_oferta ,  co.fecha_inicio_vigencia_precios, co.fecha_fin_vigencia_precios, 
co.incluido_igv, co.considera_cantidades,  co.mostrar_validez_oferta_dias, co.contacto,
co.porcentaje_flete, co.igv, co.total, co.observaciones, co.mostrar_codigo_proveedor, co.fecha_modificacion,
co.fecha_es_modificada, co.aplica_sedes, co.es_pago_contado,
---CLIENTE
cl.id_cliente, cl.codigo, cl.razon_social, cl.ruc, cl.sede_principal,
cl.plazo_credito_solicitado, cl.tipo_pago_factura,
--CIUDAD
ci.id_ciudad, ci.nombre as nombre_ciudad , ci.es_provincia,
--USUARIO
us.nombre  as nombre_usuario, us.cargo, us.contacto as contacto_usuario, us.email,
--GRUPO
gr.id_grupo, gr.codigo as codigo_grupo, gr.nombre as nombre_grupo, gr.contacto as contacto_grupo,
--SEGUIMIENTO
sc.estado_cotizacion as estado_seguimiento,
us2.nombre as usuario_seguimiento, sc.observacion as observacion_seguimiento,
us2.id_usuario as id_usuario_seguimiento,
--DETALLE
(SELECT max(porcentaje_descuento) from COTIZACION_DETALLE  WHERE estado = 1 AND id_cotizacion = co.id_cotizacion) as maximo_porcentaje_descuento



FROM COTIZACION as co
INNER JOIN CIUDAD AS ci ON co.id_ciudad = ci.id_ciudad
INNER JOIN USUARIO AS us on co.usuario_creacion = us.id_usuario
INNER JOIN SEGUIMIENTO_COTIZACION sc ON co.id_cotizacion = sc.id_cotizacion
INNER JOIN USUARIO AS us2 on sc.id_usuario = us2.id_usuario
LEFT JOIN CLIENTE AS cl ON co.id_cliente = cl.id_cliente
LEFT JOIN GRUPO AS gr ON co.id_grupo = gr.id_grupo  
where co.codigo = @codigo and co.estado = 1
AND sc.estado = 1;



select * from (
	SELECT cd.id_cotizacion_detalle, 
	cd.cantidad, 
	cd.precio_sin_igv, 
	cd.costo_sin_igv, 
	cd.equivalencia,
	cd.unidad, 
	cd.porcentaje_descuento,
	cd.precio_neto, 
	cd.es_precio_alternativo, 
	cd.observaciones,
	cd.fecha_modificacion,
	cd.flete,
	pr.id_producto, 
	pr.sku, 
	pr.descripcion, 
	pr.sku_proveedor, 
	pr.imagen, 
	pr.proveedor, 
	pr.costo as producto_costo, 
	pr.precio as producto_precio,
	pr.precio_provincia as producto_precio_provincia,

	--pc.unidad, 
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
	
	ROW_NUMBER() OVER(PARTITION BY cd.id_producto,co.id_cliente ORDER BY 
	pc.fecha_inicio_vigencia DESC, pc.codigo DESC) AS RowNumber

	FROM COTIZACION_DETALLE as cd INNER JOIN 
	COTIZACION as co ON cd.id_cotizacion = co.id_cotizacion 
	INNER JOIN PRODUCTO pr ON cd.id_producto = pr.id_producto
	LEFT JOIN 
	(SELECT pc.*, co.codigo FROM 
		PRECIO_CLIENTE_PRODUCTO pc 
		LEFT JOIN COTIZACION co ON pc.id_cotizacion = co.id_cotizacion
		WHERE /*fecha_inicio_vigencia < GETDATE()
		AND fecha_inicio_vigencia >= DATEADD(month,-6,GETDATE())  
		AND (fecha_fin_vigencia is NULL OR fecha_fin_vigencia >= GETDATE())*/
		 fecha_inicio_vigencia > DATEADD(day, cast((SELECT valor FROM PARAMETRO where codigo = 'DIAS_MAX_BUSQUEDA_PRECIOS') as int) * -1 , GETDATE()) 

		--ORDER BY fecha_inicio_vigencia DESC
	) pc ON pc.id_producto = pr.id_producto 
	AND co.id_cliente = pc.id_cliente AND cd.equivalencia = pc.equivalencia
--	AND cd.es_precio_alternativo = pc.es_unidad_alternativa
	where co.codigo = @codigo and cd.estado = 1 ) SQuery 
	where RowNumber = 1
	ORDER BY fecha_modificacion ASC;
END






ALTER PROCEDURE [dbo].[pi_seguimiento_cotizacion] 
@codigo bigint,
@idUsuario uniqueidentifier,
@estado int,
@observacion varchar(2000),
@fechaModificacion datetime,
@fechaModificacionActual datetime OUTPUT
AS
BEGIN

DECLARE @id_cotizacion uniqueidentifier;
DECLARE @id_cliente uniqueidentifier;
DECLARE @id_cliente_sede uniqueidentifier;

DECLARE @aplica_sedes bit;
DECLARE @ruc varchar(20);


SELECT @id_cotizacion =id_cotizacion, @id_cliente = id_cliente, @aplica_sedes = aplica_sedes,  @fechaModificacionActual = fecha_modificacion FROM COTIZACION WHERE codigo = @codigo AND estado = 1;
/*
if(convert(varchar,@fechaModificacionActual, 120)  = convert(varchar,@fechaModificacion, 120)) 
Begin*/
	UPDATE SEGUIMIENTO_COTIZACION set estado = 0 where id_cotizacion = @id_cotizacion;



	INSERT INTO SEGUIMIENTO_COTIZACION 
	(
			id_estado_seguimiento, 
			id_usuario ,
			id_cotizacion, 
			estado_cotizacion ,
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
				@id_cotizacion,
				@estado,
				@observacion,
				1,
				@idUsuario,
				GETDATE(),
				@idUsuario,
				GETDATE()
	);
	--Si estado es Aceptar
	IF (@estado = 3)
	BEGIN

		--SELECT * FROM COTIZACION_DETALLE
		IF (@aplica_sedes = 'TRUE')
		BEGIN

		SELECT @ruc = ruc FROM CLIENTE WHERE id_cliente = @id_cliente AND estado = 1;


		DECLARE cursor_sedes CURSOR FAST_FORWARD
		FOR SELECT id_cliente FROM CLIENTE WHERE ruc like @ruc AND negociacion_multiregional = 'TRUE' 

		OPEN cursor_sedes
		FETCH NEXT FROM cursor_sedes INTO @id_cliente_sede

			WHILE @@FETCH_STATUS = 0
			BEGIN

				INSERT INTO PRECIO_CLIENTE_PRODUCTO
					(id_precio_cliente_producto, id_cliente, id_producto,
					fecha_inicio_vigencia, fecha_fin_vigencia, unidad, moneda, 
					precio_neto, flete, precio_unitario, id_cotizacion,
					estado, usuario_creacion, fecha_creacion, usuario_modifiacion, fecha_modificacion,
					equivalencia, es_unidad_alternativa )


					SELECT NEWID(), @id_cliente_sede, p.id_producto, 
					CASE WHEN co.fecha_inicio_vigencia_precios IS NULL 
					THEN GETDATE() ELSE co.fecha_inicio_vigencia_precios END,
					co.fecha_fin_vigencia_precios, dc.unidad, 
					'S', dc.precio_neto, dc.flete, dc.precio_neto + dc.flete, @id_cotizacion,1, @idUsuario, GETDATE(), @idUsuario, GETDATE(),
					 dc.equivalencia, dc.es_precio_alternativo
					 FROM  COTIZACION  co
					 INNER JOIN COTIZACION_DETALLE dc ON co.id_cotizacion = dc.id_cotizacion
					 INNER JOIN PRODUCTO p ON dc.id_producto = p.id_producto
					 INNER JOIN CLIENTE c ON co.id_cliente = c.id_cliente
					 WHERE co.id_cotizacion = @id_cotizacion                                              
					 AND dc.estado = 1 AND co.estado = 1
					 AND c.estado = 1;

				FETCH NEXT FROM cursor_sedes INTO @id_cliente_sede
			END

		CLOSE cursor_sedes
		DEALLOCATE cursor_sedes
	
		--SELECT * FROM COTIZACION_DETALLE
		END

		ELSE 

		BEGIN
			INSERT INTO PRECIO_CLIENTE_PRODUCTO
			(id_precio_cliente_producto, id_cliente, id_producto,
			fecha_inicio_vigencia, fecha_fin_vigencia, unidad, moneda, 
			precio_neto, flete, precio_unitario, id_cotizacion,
			estado, usuario_creacion, fecha_creacion, usuario_modifiacion, fecha_modificacion,
			equivalencia, es_unidad_alternativa )


			SELECT NEWID(), c.id_cliente, p.id_producto, 
			CASE WHEN co.fecha_inicio_vigencia_precios IS NULL 
			THEN GETDATE() ELSE co.fecha_inicio_vigencia_precios END,
			co.fecha_fin_vigencia_precios, dc.unidad, 
			'S', dc.precio_neto, dc.flete, dc.precio_neto + dc.flete, @id_cotizacion,1, @idUsuario, GETDATE(), @idUsuario, GETDATE(),
			 dc.equivalencia, dc.es_precio_alternativo
			 FROM  COTIZACION  co
			 INNER JOIN COTIZACION_DETALLE dc ON co.id_cotizacion = dc.id_cotizacion
			 INNER JOIN PRODUCTO p ON dc.id_producto = p.id_producto
			 INNER JOIN CLIENTE c ON co.id_cliente = c.id_cliente
			 WHERE co.id_cotizacion = @id_cotizacion                                              
			 AND dc.estado = 1 AND co.estado = 1
			 AND c.estado = 1;

		END 
	END
END


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
	/*AND ((pe.tipo = 'V' AND ma.tipo_movimiento = 'S'	) OR (pe.tipo = 'C' AND ma.tipo_movimiento = 'S')
		OR pe.tipo = 'A' AND ma.tipo_movimiento = 'S'
	) */
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
@buscaSedesGrupoCliente bit,
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

if  @numero = 0 AND (@numeroGrupo = 0 OR @numeroGrupo IS NULL)
BEGIN



	SELECT 
	--PEDIDO
	pe.numero as numero_pedido, pe.numero_grupo as numero_grupo_pedido,
    pe.id_pedido, pe.fecha_solicitud, pe.incluido_igv, 
	pe.igv, pe.total, ISNULL(pe.observaciones,'') observaciones,
	pe.fecha_creacion, pe.fecha_entrega_desde, pe.fecha_entrega_hasta, 
	pe.hora_entrega_desde, pe.hora_entrega_hasta, pe.hora_entrega_adicional_desde, pe.hora_entrega_adicional_hasta,
	pe.fecha_entrega_extendida,
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
	and (ci.id_ciudad = @idCiudad  OR (@buscaSedesGrupoCliente = 'TRUE' AND @idGrupoCliente > 0) 

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
	pe.hora_entrega_desde, pe.hora_entrega_hasta,  pe.hora_entrega_adicional_desde, pe.hora_entrega_adicional_hasta,
	pe.fecha_entrega_extendida,
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
	where (pe.numero = @numero 
	OR pe.numero_grupo = @numeroGrupo )
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





ALTER PROCEDURE [dbo].[ps_usuario] 
@email varchar(50),
@password varchar(50)
AS
BEGIN

--DECLARE @aprueba_pedidos_lima smallint; 
--DECLARE @aprueba_pedidos smallint; 
DECLARE @id_usuario uniqueidentifier; 


SELECT @id_usuario = id_usuario FROM USUARIO 
WHERE estado = 1
AND email = @email 
AND PWDCOMPARE ( @password,password )  = 1 ;

--USUARIO
SELECT id_usuario, cargo, nombre , contacto, es_cliente, 
--Cotizaciones
crea_cotizaciones_lima as crea_cotizaciones, aprueba_cotizaciones_lima, aprueba_cotizaciones_provincias,
maximo_porcentaje_descuento_aprobacion, cotizacion_serializada,
visualiza_cotizaciones,
--Pedidos
toma_pedidos_lima as toma_pedidos,  aprueba_pedidos_lima, aprueba_pedidos_provincias, pedido_serializado,
visualiza_pedidos_lima,
visualiza_pedidos_provincias, libera_pedidos, bloquea_pedidos,
visualiza_costos,
visualiza_margen,
confirma_stock,
--Guias
crea_guias, cast(administra_guias_lima as int) as administra_guias_lima,cast (administra_guias_provincias as int) as administra_guias_provincias,
visualiza_guias_remision,
--Documentos Venta


crea_documentos_venta, 
crea_factura_consolidada_local,
crea_factura_consolidada_multiregional,
visualiza_guias_pendientes_facturacion,
administra_documentos_venta_lima, administra_documentos_venta_provincias,
visualiza_documentos_venta,
--Sede
id_ciudad,
crea_cotizaciones_provincias,
aprueba_pedidos_lima,
aprueba_pedidos_provincias,
toma_pedidos_provincias,
programa_pedidos,
modifica_maestro_clientes,
modifica_maestro_productos,
aprueba_anulaciones,
crea_notas_credito, 
crea_notas_debito, 
realiza_refacturacion,
toma_pedidos_compra,
toma_pedidos_almacen,
define_plazo_credito,
define_monto_credito,
define_responsable_comercial,
define_supervisor_comercial,
define_asistente_atencion_cliente,
define_responsable_portafolio,
modifica_pedido_venta_fecha_entrega_hasta,
realiza_carga_masiva_pedidos,
modifica_pedido_fecha_entrega_extendida,
bloquea_clientes,
modifica_negociacion_multiregional,
busca_sedes_grupo_cliente,
modifica_canales


FROM USUARIO 
WHERE estado = 1
AND email = @email 
AND PWDCOMPARE ( @password,password )  = 1 ;

--PARAMETROS POR USUARIO
SELECT codigo, valor FROM PARAMETRO where estado = 1
UNION 
SELECT 'CPE_CABECERA_BE_ID' as codigo, CPE_CABECERA_BE_ID as valor FROM PARAMETROS_AMBIENTE_EOL
UNION 
SELECT 'CPE_CABECERA_BE_COD_GPO' as codigo, CPE_CABECERA_BE_COD_GPO as valor FROM PARAMETROS_AMBIENTE_EOL;


--USUARIOS A CARGO COTIZACION
IF (@id_usuario IS NOT NULL)
BEGIN
	SELECT id_usuario,  us.nombre, es_provincia
	FROM USUARIO us INNER JOIN 
	CIUDAD ci ON us.id_ciudad = ci.id_ciudad
	WHERE us.estado = 1 AND us.crea_cotizaciones_lima = 1
	AND id_usuario != @id_usuario ;
END

--USUARIOS A CARGO PEDIDO
IF (@id_usuario IS NOT NULL)
BEGIN
	SELECT id_usuario, us.nombre, es_provincia
	FROM USUARIO us INNER JOIN 
	CIUDAD ci ON us.id_ciudad = ci.id_ciudad
	WHERE us.estado = 1 AND us.toma_pedidos_lima = 1
	AND us.id_usuario NOT IN ('412ACDEE-FE20-4539-807C-D00CD71359D6')
	AND (us.toma_pedidos_lima = 1 OR us.toma_pedidos_provincias = 1)
	AND id_usuario != @id_usuario ;
END


--USUARIOS A CARGO GUIAS
IF (@id_usuario IS NOT NULL)
BEGIN
	SELECT id_usuario, us.nombre, es_provincia
	FROM USUARIO us INNER JOIN 
	CIUDAD ci ON us.id_ciudad = ci.id_ciudad
	WHERE us.estado = 1 AND us.crea_guias = 1
	AND id_usuario != @id_usuario ;
END

--USUARIOS A CARGO DOCUMENTOS VENTA
IF (@id_usuario IS NOT NULL)
BEGIN
	SELECT id_usuario, us.nombre, es_provincia
	FROM USUARIO us INNER JOIN 
	CIUDAD ci ON us.id_ciudad = ci.id_ciudad
	WHERE us.estado = 1 AND us.crea_documentos_venta = 1
	AND id_usuario != @id_usuario ;
END

IF (@id_usuario IS NOT NULL)
BEGIN
	SELECT cl.id_cliente, cl.razon_social, 
	cl.codigo, cl.ruc, cl.nombre_comercial 
	FROM CLIENTE cl
	INNER JOIN USUARIO_CLIENTE uc	ON cl.ruc = uc.ruc
	INNER JOIN CIUDAD ci ON  cl.id_ciudad = ci.id_ciudad
	WHERE uc.id_usuario = @id_usuario 
	AND ci.es_provincia = 0 ;

	SELECT id_vendedor, codigo, 
	descripcion,
	es_responsable_comercial, es_asistente_servicio_cliente, 
	es_responsable_portafolio, es_supervisor_comercial,
	id_usuario--, us.nombre 
	FROM VENDEDOR ve
--	LEFT JOIN USUARIO us ON ve.id_usuario = us.id_usuario
	WHERE estado = 1;
END

END





ALTER PROCEDURE [dbo].[ps_facturas] 

@numero varchar(8),
@idCliente uniqueidentifier,
@idGrupoCliente int,
@buscaSedesGrupoCliente bit,
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
	and (ci.id_ciudad = @idCiudad OR (@buscaSedesGrupoCliente = 'TRUE' AND @idGrupoCliente > 0)  or @idCiudad = '00000000-0000-0000-0000-000000000000'
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





ALTER PROCEDURE [dbo].[ps_guiasRemision] 
@numeroDocumento bigint,
@idCiudad uniqueidentifier,
@idCliente uniqueidentifier,
@idGrupoCliente int,
@buscaSedesGrupoCliente bit,
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
	and (ci.id_ciudad = @idCiudad OR (@buscaSedesGrupoCliente = 'TRUE' AND @idGrupoCliente > 0)  or @idCiudad = '00000000-0000-0000-0000-000000000000')

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



ALTER PROCEDURE [dbo].[ps_notasIngreso] 
@numeroDocumento bigint,
@idCiudad uniqueidentifier,
@idCliente uniqueidentifier,
@idGrupoCliente int,
@buscaSedesGrupoCliente bit,
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
	and (ci.id_ciudad = @idCiudad OR (@buscaSedesGrupoCliente = 'TRUE' AND @idGrupoCliente > 0) or @idCiudad = '00000000-0000-0000-0000-000000000000')

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



ALTER PROCEDURE [dbo].[ps_venta] 
@idMovimientoAlmacen uniqueIdentifier
AS
BEGIN
	SELECT 
	pe.id_pedido,
	pe.numero, pe.numero_grupo, pe.fecha_solicitud,  
	pe.fecha_entrega_desde, pe.fecha_entrega_hasta,
	pe.hora_entrega_desde, pe.hora_entrega_hasta,
	pe.incluido_igv,  pe.igv, pe.total, pe.observaciones,  pe.fecha_modificacion,
	pe.numero_referencia_cliente, pe.id_direccion_entrega, pe.direccion_entrega, pe.contacto_entrega,
	pe.telefono_contacto_entrega, 
	pe.fecha_programacion,
	pe.tipo_pedido, pe.observaciones_factura, pe.observaciones_guia_remision,
	pe.contacto_pedido,pe.telefono_contacto_pedido, pe.correo_contacto_pedido,
	pe.otros_cargos,
	pe.numero_referencia_adicional,
	--UBIGEO
	pe.ubigeo_entrega, ub.departamento, ub.provincia, ub.distrito,
	---CLIENTE
	cl.id_cliente, cl.codigo, cl.razon_social, cl.ruc, cic.id_ciudad as id_ciudad_cliente, cic.nombre as nombre_ciudad_cliente,
	cl.razon_social_sunat, cl.direccion_domicilio_legal_sunat, cl.correo_envio_factura, cl.plazo_credito,
	CASE pe.es_pago_contado WHEN 'TRUE' 
	THEN 1 
	ELSE cl.tipo_pago_factura END 
	AS tipo_pago_factura,
	cl.forma_pago_factura,
	cl.tipo_documento,
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
	ma.fecha_emision
	FROM VENTA as ve
	INNER JOIN MOVIMIENTO_ALMACEN as ma ON ma.id_movimiento_almacen = ve.id_movimiento_almacen
	INNER JOIN PEDIDO pe ON ve.id_pedido = pe.id_pedido
	INNER JOIN CIUDAD AS ci ON pe.id_ciudad = ci.id_ciudad
	INNER JOIN USUARIO AS us on pe.usuario_creacion = us.id_usuario
	INNER JOIN CLIENTE AS cl ON pe.id_cliente = cl.id_cliente
	INNER JOIN CIUDAD AS cic ON cl.id_ciudad = cic.id_ciudad
	LEFT JOIN UBIGEO ub ON CAST(pe.ubigeo_entrega AS CHAR(6)) = ub.codigo
	where ve.id_movimiento_almacen = @idMovimientoAlmacen 
	AND ve.id_documento_venta IS NULL AND
	pe.estado = 1


	--DETALLE PEDIDO
	SELECT vd.id_venta_detalle, 
	vd.cantidad,
	vd.precio_sin_igv, 
	vd.costo_sin_igv, 
	vd.equivalencia as equivalencia,
	vd.unidad, 
	vd.porcentaje_descuento, 
	vd.precio_neto, 
	vd.es_precio_alternativo, 
	vd.flete,
	pr.id_producto, pr.sku, pr.descripcion, pr.sku_proveedor, pr.imagen, pr.proveedor, 
	pr.costo as costo_producto, pr.precio as precio_producto,
	pd.observaciones,
	pd.fecha_modificacion,
	CAST(pd.precio_neto + pd.flete AS decimal(12,2)) as precio_unitario_original,
	vd.precio_unitario as precio_unitario_venta,
	vd.igv_precio_unitario as igv_precio_unitario_venta

	FROM 
	VENTA_DETALLE as vd 
	INNER JOIN VENTA ve ON vd.id_venta = ve.id_venta
	INNER JOIN PEDIDO as pe ON ve.id_pedido = pe.id_pedido 
	INNER JOIN PRODUCTO pr ON vd.id_producto = pr.id_producto
	LEFT JOIN PEDIDO_DETALLE as pd ON vd.id_pedido_detalle = pd.id_pedido_detalle
	WHERE ve.id_movimiento_almacen = @idMovimientoAlmacen
	AND ve.id_documento_venta IS NULL
	ORDER BY fecha_modificacion ASC;

	SELECT arch.id_archivo_adjunto, null adjunto, nombre
	FROM ARCHIVO_ADJUNTO arch
	INNER JOIN PEDIDO_ARCHIVO parch ON arch.id_archivo_adjunto = parch.id_archivo_adjunto
	WHERE parch.id_pedido = (SELECT id_pedido FROM MOVIMIENTO_ALMACEN
	WHERE id_movimiento_almacen = @idMovimientoAlmacen   ) AND 
	arch.estado = 1 AND parch.estado = 1;



END


