/* **** 1 **** */
ALTER TABLE CLIENTE  
ADD observacion_horario_entrega VARCHAR(1000);



/* **** 2 **** */
/* Agregar observacion_horario_entrega */
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

@observacionHorarioEntrega varchar(1000), 

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
			email_contacto1,
			observacion_horario_entrega
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
			@emailContacto1,
			@observacionHorarioEntrega
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








/* **** 3 **** */
/* Agregar observacion_horario_entrega */
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
@emailContacto1 varchar(50),
@observacionHorarioEntrega varchar(1000), 

@existenCambiosCreditos smallint OUTPUT,
@usuarioSolicitanteCredito uniqueIdentifier OUTPUT,
@correoUsuarioSolicitanteCredito VARCHAR(50) OUTPUT

AS
BEGIN


DECLARE @ruc varchar(20); /* Agregado */
DECLARE @nrAnterior bit; /* Agregado */
DECLARE @plazoCreditoSolicitadoAnterior int;
DECLARE @tipoPagoFacturaAnterior int;
DECLARE @formaPagoFacturaAnterior int;
DECLARE @creditoSolicitadoAnterior decimal(12,2);
DECLARE @creditoAprobadoAnterior decimal(12,2);
DECLARE @usuarioSolicitanteCreditoAnterior uniqueIdentifier;
DECLARE @enviarCorreoCreditos int;
DECLARE @enviarCorreoUsuarioNoCreditos int;
DECLARE @defineMontoCredito smallint;
DECLARE @definePlazoCredito smallint;


SET NOCOUNT ON
SET @existenCambiosCreditos = 0;
SET @usuarioSolicitanteCredito = '00000000-0000-0000-0000-000000000000';
SET @correoUsuarioSolicitanteCredito = '';

IF (SELECT tipo_documento FROM CLIENTE where id_cliente = @idCliente) <> 6
BEGIN 
	SET @razonSocial = @nombreComercial;
END




SELECT @ruc = ruc, @nrAnterior = negociacion_multiregional ,
@plazoCreditoSolicitadoAnterior = plazo_credito_solicitado,
@tipoPagoFacturaAnterior = tipo_pago_factura,
@formaPagoFacturaAnterior = forma_pago_factura,
@creditoSolicitadoAnterior = credito_solicitado,
@creditoAprobadoAnterior =credito_aprobado,
@usuarioSolicitanteCreditoAnterior = usuario_solicitante_credito
FROM CLIENTE WHERE id_cliente = @idCliente; /* Agregado */

IF @plazoCreditoSolicitadoAnterior <>  @plazoCreditoSolicitado
	OR @tipoPagoFacturaAnterior <> @tipoPagoFactura
	OR @formaPagoFacturaAnterior <> @formaPagoFactura
	OR @creditoSolicitadoAnterior <> @creditoSolicitado
	OR @creditoAprobadoAnterior <> @creditoAprobado
BEGIN 
 
	SET @existenCambiosCreditos = 1;

	/*Si no se indica el usuario solicitante entonces se recupera el ultimo solicitanteCredito en caso exista*/
	/*IF @usuarioSolicitanteCredito = '00000000-0000-0000-0000-000000000000'
	BEGIN 
		SET @usuarioSolicitanteCredito = @usuarioSolicitanteCreditoAnterior
	END */
	
	
	SELECT @defineMontoCredito = ISNULL(define_monto_credito,0),
	@definePlazoCredito = ISNULL(define_plazo_credito,0)
	FROM USUARIO where id_usuario = @idUsuario

	IF  @defineMontoCredito = 1 OR @definePlazoCredito = 1
	BEGIN 
		/*Si el usuario es aprobador de creditos se recupera el usuario solicitante anterior*/
		SET @usuarioSolicitanteCredito = @usuarioSolicitanteCreditoAnterior;
		SELECT @correoUsuarioSolicitanteCredito = ISNULL(email,'') FROM USUARIO 
		WHERE id_usuario = @usuarioSolicitanteCreditoAnterior;

	END 
	ELSE
	BEGIN
		/*Si el usuario NO es aprobador de creditos se actualiza con el usuario actual*/
		SELECT @usuarioSolicitanteCredito = @idUsuario,
		@correoUsuarioSolicitanteCredito = email
		FROM USUARIO 
		WHERE id_usuario = @idUsuario;
	END
END 

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
			,usuario_solicitante_credito = @usuarioSolicitanteCredito
			,observacion_horario_entrega = @observacionHorarioEntrega
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

END







/* **** 4 **** */
/* Agregar observacion_horario_entrega, codigo_grupo_cliente */
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
cl.observacion_horario_entrega,

clgr.id_grupo_cliente ,
gr.codigo as codigo_grupo_cliente,
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

SELECT  arch.id_archivo_adjunto,  nombre--, arch.adjunto,
FROM ARCHIVO_ADJUNTO arch
WHERE id_cliente = @idCliente
AND estado = 1
AND informacion_cliente = 'TRUE';

END





/* **** 5 **** */
/* agregar codigo_grupo */
ALTER PROCEDURE [dbo].[ps_clientes] 
@codigo varchar(4),
@idCiudad uniqueIdentifier,
@textoBusqueda varchar(50),
@idResponsableComercial int,
@idSupervisorComercial int,
@idAsistenteServicioCliente int,
@sinPlazoCreditoAprobado int, 
@sinAsesorValidado int, 
@bloqueado int,
@idGrupoCliente int
AS
BEGIN

IF @codigo IS NULL OR @codigo = ''
BEGIN
	SELECT 
	cl.id_cliente, 
	cl.codigo,
	cl.sede_principal, 
	cl.negociacion_multiregional, 
	cl.pertenece_canal_multiregional,
	cl.pertenece_canal_lima,
	cl.pertenece_canal_provincia,
	cl.pertenece_canal_pcp,
	cl.pertenece_canal_ordon,
	cl.es_sub_distribuidor,
	ci.id_ciudad,
	ci.nombre as ciudad_nombre, 
	
	CASE cl.tipo_documento WHEN 6 
		THEN ISNULL(cl.razon_social_sunat,cl.razon_social)
	ELSE '' END razon_social_sunat,

	ISNULL(cl.nombre_comercial,'') nombre_comercial,
	cl.tipo_documento, 
	cl.ruc,
		--VENDEDORES,
	verc.codigo as responsable_comercial_codigo,
	ISNULL(verc.descripcion,'') as responsable_comercial_descripcion,

	vesc.codigo as supervisor_comercial_codigo,
	ISNULL(vesc.descripcion,'') as supervisor_comercial_descripcion,

	veasc.codigo as asistente_servicio_cliente_codigo,
	ISNULL(veasc.descripcion,'') as asistente_servicio_cliente_descripcion,

	cl.tipo_pago_factura, --plazo credito aprobado
	cl.credito_aprobado,
	cl.bloqueado,
	cgc.id_grupo_cliente,
	gc.codigo as codigo_grupo,
	ISNULL(gc.grupo,'-') grupo
	FROM CLIENTE AS cl
	INNER JOIN CIUDAD AS ci ON cl.id_ciudad = ci.id_ciudad 
	--LEFT JOIN UBIGEO AS ub ON cl.ubigeo = ub.codigo
	LEFT JOIN VENDEDOR AS verc ON cl.id_responsable_comercial = verc.id_vendedor
	LEFT JOIN VENDEDOR AS vesc ON cl.id_supervisor_comercial = vesc.id_vendedor
	LEFT JOIN VENDEDOR AS veasc ON cl.id_asistente_servicio_cliente = veasc.id_vendedor
	LEFT JOIN CLIENTE_GRUPO_CLIENTE  AS cgc ON cl.id_cliente = cgc.id_cliente
	LEFT JOIN GRUPO_CLIENTE AS gc ON gc.id_grupo_cliente = cgc.id_grupo_cliente
	WHERE 
	(
	(REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(cl.nombre_comercial, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  LIKE '%'+@textoBusqueda+'%' OR
	cl.ruc LIKE '%'+@textoBusqueda+'%' OR
	REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(cl.razon_social, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  LIKE '%'+@textoBusqueda+'%'
	OR REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(cl.razon_social_sunat, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  LIKE '%'+@textoBusqueda+'%'
	)
	OR @textoBusqueda IS NULL
	OR @textoBusqueda = ''
	)
	AND (cl.id_responsable_comercial = @idResponsableComercial OR @idResponsableComercial = 0)
	AND (cl.id_supervisor_comercial = @idSupervisorComercial OR @idSupervisorComercial = 0)
	AND (cl.id_asistente_servicio_cliente = @idAsistenteServicioCliente OR @idAsistenteServicioCliente = 0)
	AND (cl.bloqueado = 1 OR @bloqueado = 0 )
	AND (@sinAsesorValidado = 0 OR cl.vendedores_asignados = 0) 
	AND cl.estado = 1
	AND (cl.id_ciudad = @idCiudad OR @idCiudad = '00000000-0000-0000-0000-000000000000')
	AND (@idGrupoCliente = 0 OR @idGrupoCliente = cgc.id_grupo_cliente)
	--FALTA sinPlazoCreditoAprobado

END
ELSE
BEGIN 

	SELECT 
	cl.id_cliente, 
	cl.codigo,
	ci.id_ciudad,
	cl.sede_principal, 
	cl.negociacion_multiregional, 
	cl.pertenece_canal_multiregional,
	cl.pertenece_canal_lima,
	cl.pertenece_canal_provincia,
	cl.pertenece_canal_pcp,
	cl.pertenece_canal_ordon,
	cl.es_sub_distribuidor,
	ci.nombre as ciudad_nombre, 
	
	CASE cl.tipo_documento WHEN 6 
		THEN cl.razon_social_sunat
		ELSE '' END razon_social_sunat,

	ISNULL(cl.nombre_comercial,'') nombre_comercial,
	cl.tipo_documento, 
	cl.ruc,
		--VENDEDORES,
	verc.codigo as responsable_comercial_codigo,
	ISNULL(verc.descripcion,'') as responsable_comercial_descripcion,

	vesc.codigo as supervisor_comercial_codigo,
	ISNULL(vesc.descripcion,'') as supervisor_comercial_descripcion,

	veasc.codigo as asistente_servicio_cliente_codigo,
	ISNULL(veasc.descripcion,'') as asistente_servicio_cliente_descripcion,

	cl.tipo_pago_factura, --plazo credito aprobado
	cl.credito_aprobado,
	cl.bloqueado,
	cgc.id_grupo_cliente,
	ISNULL(gc.grupo,'-') grupo,
	gc.codigo as codigo_grupo
	FROM CLIENTE AS cl
	INNER JOIN CIUDAD AS ci ON cl.id_ciudad = ci.id_ciudad 
	--LEFT JOIN UBIGEO AS ub ON cl.ubigeo = ub.codigo
	LEFT JOIN VENDEDOR AS verc ON cl.id_responsable_comercial = verc.id_vendedor
	LEFT JOIN VENDEDOR AS vesc ON cl.id_supervisor_comercial = vesc.id_vendedor
	LEFT JOIN VENDEDOR AS veasc ON cl.id_asistente_servicio_cliente = veasc.id_vendedor
	LEFT JOIN CLIENTE_GRUPO_CLIENTE  AS cgc ON cl.id_cliente = cgc.id_cliente
	LEFT JOIN GRUPO_CLIENTE AS gc ON gc.id_grupo_cliente = cgc.id_grupo_cliente
	WHERE cl.codigo = @codigo AND cl.estado = 1;
END


END








/* **** 6 **** */
ALTER PROCEDURE [dbo].[pi_producto] 
@sku  varchar(100),
@descripcion  varchar(500),
@skuProveedor  varchar(100),
@estado  int,
@imagen image,
@precio numeric(18,2),
@precioProvincia numeric(18,2),
@costo numeric(18,2),
@familia varchar(200),
@proveedor varchar(10),
@unidad varchar(200),
@unidadAlternativa varchar(200),
@equivalencia int,
@idUsuario uniqueidentifier,
@unidadProveedor varchar(300),
@equivalenciaProveedor int,
@unidadEstandarInternacional varchar(3),
@exoneradoIgv smallint,
@inafecto smallint,
@tipo int,
@newId uniqueidentifier OUTPUT 

AS
BEGIN

SET NOCOUNT ON
SET @newId = NEWID();


	INSERT INTO PRODUCTO
           (id_producto
			,sku 
			,descripcion
			,sku_proveedor
			,estado
			,imagen
			,precio
			,precio_provincia
			,costo
			,familia
			,proveedor
			,unidad
			,unidad_alternativa
			,equivalencia
			,usuario_creacion
			,fecha_creacion
			,usuario_modificacion
			,fecha_modificacion
			,unidad_proveedor
			,equivalencia_proveedor
			,unidad_estandar_internacional
			,exonerado_igv
			,inafecto
			,tipo
		   )
     VALUES
           (@newId,
		    @sku,
			@descripcion,
			@skuProveedor,
			@estado,
			@imagen,
			@precio,
			@precioProvincia,
			@costo,
			@familia,
			@proveedor,
			@unidad,
			@unidadAlternativa,
			@equivalencia,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE(),
			@unidadProveedor,
			@equivalenciaProveedor,
			@unidadEstandarInternacional,
			@exoneradoIgv,
			@inafecto,
			@tipo
			);

END











/* **** 7 **** */
ALTER PROCEDURE [dbo].[ps_producto]
@idProducto uniqueIdentifier

AS
BEGIN
	SELECT 
	 id_producto
    ,sku 
    ,descripcion
	,sku_proveedor
	,imagen
	,estado
    ,precio
	,precio_provincia
	,costo
    ,familia
	,proveedor
	,unidad
	,unidad_alternativa
	,equivalencia
	,usuario_creacion
    ,fecha_creacion
    ,usuario_modificacion
    ,fecha_modificacion
    ,unidad_proveedor
    ,equivalencia_proveedor
    ,unidad_estandar_internacional
	,exonerado_igv
	,inafecto
	,tipo
	FROM PRODUCTO p
	where p.id_producto = @idProducto;
END

/*UNIDAD ESTANDAR 
ES DESCUENTO (check) 
ES CARGO (check)*/









/* **** 8 **** */
ALTER PROCEDURE [dbo].[pu_producto] 
@idProducto uniqueidentifier,
@idUsuario uniqueidentifier,
@sku  varchar(100),
@descripcion  varchar(500),
@skuProveedor  varchar(100),
@estado  int,
@imagen image,
@precio numeric(18,2),
@precioProvincia numeric(18,2),
@costo numeric(18,2),
@familia varchar(200),
@proveedor varchar(10),
@unidad varchar(200),
@unidadAlternativa varchar(200),
@equivalencia int,
@unidadProveedor varchar(300),
@equivalenciaProveedor int,
@unidadEstandarInternacional varchar(3),
@exoneradoIgv smallint,
@inafecto smallint,
@tipo int

AS
BEGIN

	UPDATE PRODUCTO  
	SET sku = @sku 
		,descripcion = @descripcion
		,sku_proveedor = @skuProveedor
		,estado = @estado
		,imagen = @imagen
		,precio = @precio
		,precio_provincia = @precioProvincia
		,costo = @costo
		,familia = @familia
		,proveedor = @proveedor
		,unidad = @unidad
		,unidad_alternativa = @unidadAlternativa
		,equivalencia = @equivalencia
		,usuario_modificacion = @idUsuario
		,fecha_modificacion = GETDATE()
		,unidad_proveedor = @unidadProveedor
		,equivalencia_proveedor = @equivalenciaProveedor
		,unidad_estandar_internacional = @unidadEstandarInternacional
		,exonerado_igv = @exoneradoIgv
		,inafecto = @inafecto
		,tipo = @tipo
	WHERE id_producto like @idProducto;
	
END





/* **** 9 **** */
ALTER PROCEDURE [dbo].[ps_productos]
@sku varchar(100),
@skuProveedor varchar(100),
@descripcion varchar(500),
@familia varchar(200),
@proveedor varchar(10),
@tipo int,
@estado int
AS
BEGIN

IF @sku IS NULL OR @sku = ''
BEGIN
	SELECT 
	 id_producto
      ,sku
      ,descripcion
      ,sku_proveedor
      ,imagen
      ,precio
      ,precio_provincia
      ,costo
      ,familia
      ,proveedor
      ,unidad
      ,unidad_alternativa
      ,equivalencia
      ,unidad_proveedor
      ,equivalencia_proveedor
	  ,tipo 
	   FROM PRODUCTO p
	where p.sku_proveedor LIKE '%'+@skuProveedor+'%' 
		  and p.descripcion LIKE '%'+@descripcion+'%' 
		  and (@familia LIKE 'Todas' OR p.familia LIKE @familia) 
		  and (@proveedor LIKE 'Todos' OR p.proveedor LIKE @proveedor) 
		  and (@tipo = 0 OR tipo = @tipo) 
		  and p.estado = @estado;
END
ELSE
BEGIN 

SELECT 
	 id_producto
      ,sku
      ,descripcion
      ,sku_proveedor
      ,imagen
      ,precio
      ,precio_provincia
      ,costo
      ,familia
      ,proveedor
      ,unidad
      ,unidad_alternativa
      ,equivalencia
      ,unidad_proveedor
      ,equivalencia_proveedor
	  ,tipo
	   FROM PRODUCTO p
	where p.sku LIKE @sku 
		  and p.estado = @estado;
	
END
END




