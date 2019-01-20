/* **** 1 **** */
ALTER TABLE cliente 
ADD id_subdistribuidor INT,
 id_origen INT;

 
 
/* **** 2 **** */
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

@idOrigen int,
@idSubDistribuidor int,

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
			id_origen,
			id_subdistribuidor,
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
			@idOrigen,
			@idSubDistribuidor,
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

@idOrigen int,
@idSubDistribuidor int,

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


DECLARE @ruc varchar(20); 
DECLARE @nrAnterior bit; 
DECLARE @idResponsableComercialAnterior int;  /* Agregado */
DECLARE @idAsistenteServicioClienteAnterior int;  /* Agregado */
DECLARE @idSupervisorComercialAnterior int;  /* Agregado */
DECLARE @negociacionMultiregionalAnterior bit;  /* Agregado */
DECLARE @sedePrincipalAnterior bit;  /* Agregado */
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
@idResponsableComercialAnterior = id_responsable_comercial,
@idAsistenteServicioClienteAnterior = id_asistente_Servicio_cliente,
@idSupervisorComercialAnterior = id_supervisor_comercial,
@negociacionMultiregionalAnterior = negociacion_multiregional,
@sedePrincipalAnterior = sede_principal,
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
			,id_origen = @idOrigen
			,id_subdistribuidor = @idSubDistribuidor
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

cl.id_subdistribuidor,
sub.nombre nombre_subdistribuidor, 
cl.id_origen,
ori.nombre nombre_origen, 

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
LEFT JOIN SUBDISTRIBUIDOR AS sub ON sub.id_subdistribuidor = cl.id_subdistribuidor 
LEFT JOIN ORIGEN AS ori ON ori.id_origen = cl.id_origen 
WHERE cl.estado = 1 AND cl.id_cliente = @idCliente 

SELECT  arch.id_archivo_adjunto,  nombre--, arch.adjunto,
FROM ARCHIVO_ADJUNTO arch
WHERE id_cliente = @idCliente
AND estado = 1
AND informacion_cliente = 'TRUE';

END














