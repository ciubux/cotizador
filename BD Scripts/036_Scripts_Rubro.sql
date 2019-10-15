
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
 CONSTRAINT [PK_RUBRO] PRIMARY KEY CLUSTERED 
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

UPDATE RUBRO  
	SET codigo = @codigo 
		,nombre = @nombre
		,estado = @estado
		,usuario_modificacion = @idUsuario
		,fecha_modificacion = dbo.getlocaldate()
	WHERE id_rubro = @idRubro;

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




/* **** 8 **** */
ALTER PROCEDURE [dbo].[ps_cliente] 
@idCliente uniqueidentifier 
AS
BEGIN

SELECT
cl.usuario_creacion,
 cl.id_cliente, cl.codigo, cl.razon_social,
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
verc.id_usuario as responsable_comercial_id_usuario,

vesc.id_vendedor as supervisor_comercial_id_vendedor,
vesc.codigo as supervisor_comercial_codigo,
vesc.descripcion as supervisor_comercial_descripcion,
vesc.id_usuario as supervisor_comercial_id_usuario,

veasc.id_vendedor as asistente_servicio_cliente_id_vendedor,
veasc.codigo as asistente_servicio_cliente_codigo,
veasc.descripcion as asistente_servicio_cliente_descripcion,
veasc.id_usuario as asistente_servicio_id_usuario,

cl.observaciones_credito, 
cl.observaciones, 
cs.estado_liberacion_creditica,

cl.pertenece_canal_multiregional,
cl.pertenece_canal_lima,
cl.pertenece_canal_provincia,
cl.pertenece_canal_pcp,
cl.pertenece_canal_ordon,
cl.es_sub_distribuidor,
cl.observacion_horario_entrega,
cl.configuraciones,
cl.habilitado_negociacion_grupal,
ISNULL(cs.habilitado_modificar_direccion_entrega,1) habilitado_modificar_direccion_entrega, 
cl.id_subdistribuidor,
sub.nombre nombre_subdistribuidor, 
sub.codigo codigo_subdistribuidor, 
cl.id_rubro,
rub.nombre nombre_rubro, 
rub.codigo codigo_rubro, 
cl.id_origen,
ori.nombre nombre_origen, 
ori.codigo codigo_origen, 

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
LEFT JOIN RUBRO AS rub ON rub.id_rubro = cl.id_rubro  
LEFT JOIN ORIGEN AS ori ON ori.id_origen = cl.id_origen 
LEFT JOIN CLIENTE_SUNAT AS cs ON cs.id_cliente_sunat = cl.id_cliente_sunat
WHERE cl.estado > 0 AND cl.id_cliente = @idCliente 


/*ARCHIVOS ADJUNTOS*/
SELECT  arch.id_archivo_adjunto,  nombre--, arch.adjunto,
,checksum
FROM ARCHIVO_ADJUNTO arch
WHERE id_cliente = @idCliente
AND estado = 1
AND informacion_cliente = 'TRUE';


END




/* **** 9 **** */
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

@tipoDocumento int,
@observacionesCredito varchar(1000),
@observaciones varchar(1000),
@vendedoresAsignados smallint,
@estadoLiberacionCrediticia int,
@perteneceCanalMultiregional smallint,
@perteneceCanalLima smallint,
@perteneceCanalProvincias smallint,
@perteneceCanalPCP smallint,
@esSubDistribuidor smallint,

@idOrigen int,
@idSubDistribuidor int,
@idRubro int,

@idGrupoCliente int,
@horaInicioPrimerTurnoEntrega datetime,
@horaFinPrimerTurnoEntrega datetime,
@horaInicioSegundoTurnoEntrega datetime,
@horaFinSegundoTurnoEntrega datetime,


/* Campos agregados  */
@habilitadoNegociacionGrupal bit,
@sedePrincipal bit,
@negociacionMultiregional bit,
@telefonoContacto1 varchar(50),
@emailContacto1 varchar(50),

@observacionHorarioEntrega varchar(1000), 
@fechaInicioVigencia date,

@configuraciones text,

@esCargaMasiva smallint = 0,
@newId uniqueidentifier OUTPUT, 
@codigoAlterno int OUTPUT,
@codigo VARCHAR(4) OUTPUT


AS
BEGIN TRAN

DECLARE @id_cliente_sunat int;
DECLARE @id_domicilio_legal int;
DECLARE @existe_cliente bit;
DECLARE @flagFinWhile bit;
DECLARE @letrasEnCodigo CHAR(2);
DECLARE @numeroEnCodigo int;

SET NOCOUNT ON

SET @flagFinWhile = 0;

Select @codigo = siguiente_codigo_cliente FROM CIUDAD where id_ciudad = @idCiudad;


WHILE @flagFinWhile = 0
BEGIN 
	IF (SELECT COUNT(*) FROM CLIENTE WHERE CODIGO = @codigo) = 1
	BEGIN 
		SET @letrasEnCodigo = LEFT(@codigo,2);
		SET @numeroEnCodigo = RIGHT(@codigo,2);
		IF @numeroEnCodigo = 99
		BEGIN 
			BREAK
		END
		ELSE
		BEGIN
			SET @codigo = CONCAT(@letrasEnCodigo, FORMAT(@numeroEnCodigo+ 1  , '00' ) )
		END
	END 
	ELSE
	BEGIN
		BREAK
	END
END




SET @newId = NEWID();
SET @codigoAlterno = NEXT VALUE FOR SEQ_CODIGO_ALTERNO_CLIENTE;

IF @tipoDocumento <> 6
BEGIN 
	SET @razonSocial = @nombreComercial;
END
ELSE
BEGIN
	SET @razonSocial = @razonSocialSunat;
	SELECT @existe_cliente = COUNT(*) FROM CLIENTE_SUNAT WHERE ruc = @ruc

	IF @existe_cliente = 0
	BEGIN --Crear ClienteSunat
		SET @id_cliente_sunat = NEXT VALUE FOR SEQ_ID_CLIENTE_SUNAT;
		SET @id_domicilio_legal = NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL;

		INSERT INTO CLIENTE_SUNAT 
		(id_cliente_sunat, ruc,estado, fecha_creacion, fecha_modificacion,
		razon_social,	nombre_comercial,	estado_contribuyente,	condicion_contribuyente, estado_liberacion_creditica)
		VALUES
		( 
		@id_cliente_sunat, @ruc, 1, [dbo].[getlocaldate](), [dbo].[getlocaldate](), 
		@razonSocialSunat, @nombreComercialSunat, @estadoContribuyente, @condicionContribuyente, @estadoLiberacionCrediticia
		)
		
		INSERT INTO DOMICILIO_LEGAL 
		(id_domicilio_legal, id_cliente_sunat, codigo, direccion, ubigeo, 
		es_establecimiento_anexo,
		estado, fecha_creacion, fecha_modificacion, usuario_creacion, usuario_modificacion
		)
		VALUES
		(@id_domicilio_legal, @id_cliente_sunat, '0000', @direccionDomicilioLegalSunat,	@ubigeo, 0,	
		1, [dbo].[getlocaldate](), [dbo].[getlocaldate](), @idUsuario, @idUsuario	)

		UPDATE CLIENTE_SUNAT set id_domicilio_legal = @id_domicilio_legal where id_cliente_sunat = @id_cliente_sunat;

	END 
	ELSE --Actualizar ClienteSunat
	BEGIN 
		SELECT @id_cliente_sunat = id_cliente_sunat, @id_domicilio_legal = id_domicilio_legal FROM CLIENTE_SUNAT WHERE ruc = @ruc;

		UPDATE CLIENTE_SUNAT set 
		razon_social = @razonSocialSunat,
		nombre_comercial = @nombreComercialSunat, 
		estado_contribuyente = @estadoContribuyente,
		condicion_contribuyente = @condicionContribuyente,
		usuario_modificacion = @idUsuario,
		estado_liberacion_creditica = @estadoLiberacionCrediticia,
		fecha_modificacion = [dbo].[getlocaldate]()
		WHERE id_cliente_sunat = @id_cliente_sunat

		UPDATE DOMICILIO_LEGAL set
		direccion = @direccionDomicilioLegalSunat,
		ubigeo = @ubigeo,
		usuario_modificacion = @idUsuario,
		fecha_modificacion = [dbo].[getlocaldate]()
		WHERE id_domicilio_legal = @id_domicilio_legal

	END

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
			habilitado_negociacion_grupal,
			telefono_contacto1,
			email_contacto1,
			id_origen,
			id_subdistribuidor,
			id_rubro,
			fecha_inicio_vigencia,
			observacion_horario_entrega,
			id_grupo_cliente, 
			id_cliente_sunat,
			configuraciones,
			carga_masiva
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
			[dbo].[getlocaldate](),
			@idUsuario,
			[dbo].[getlocaldate](),
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
			@habilitadoNegociacionGrupal,
			@telefonoContacto1,
			@emailContacto1,
			@idOrigen,
			@idSubDistribuidor,
			@idRubro,
			@fechaInicioVigencia,
			@observacionHorarioEntrega,
			@idGrupoCliente,
			@id_cliente_sunat,
			@configuraciones,
			@esCargaMasiva
			);

INSERT INTO SOLICITANTE 
(id_solicitante, id_cliente, nombre, telefono, correo, estado, 
usuario_creacion, fecha_creacion, usuario_modificacion, fecha_modificacion)
VALUES
(newid(), @newId, @contacto1, @telefonoContacto1, @emailContacto1,1,
@idUsuario,[dbo].[getlocaldate](), @idUsuario, [dbo].[getlocaldate]())


IF @codigo = 'LW99'
BEGIN
	UPDATE CIUDAD set siguiente_codigo_cliente = 'LX01'	where id_ciudad = @idCiudad;
END 
ELSE 
BEGIN
	UPDATE CIUDAD set siguiente_codigo_cliente = @codigo


	where id_ciudad = @idCiudad
END


	
IF @negociacionMultiregional = 'FALSE'
BEGIN
	UPDATE CLIENTE 
	SET sede_principal = 'FALSE',
	fecha_inicio_vigencia = @fechaInicioVigencia
	WHERE ruc like @ruc;
END

UPDATE CLIENTE 
SET   negociacion_multiregional = @negociacionMultiregional, pertenece_canal_multiregional = @perteneceCanalMultiregional
        ,razon_Social_Sunat = @razonSocialSunat
		,nombre_Comercial_Sunat = nombre_Comercial_Sunat
		,direccion_Domicilio_Legal_Sunat = @direccionDomicilioLegalSunat
		,estado_Contribuyente_sunat = @estadoContribuyente
		,condicion_Contribuyente_sunat = @condicionContribuyente
		,es_sub_distribuidor = @esSubDistribuidor
		,id_subdistribuidor = @idSubDistribuidor
		,ubigeo = @ubigeo
		,fecha_inicio_vigencia = @fechaInicioVigencia
WHERE ruc like @ruc;

IF @idGrupoCliente > 0 
BEGIN
	INSERT INTO CLIENTE_GRUPO_CLIENTE 
	VALUES (@newId, @idGrupoCliente, [dbo].[getlocaldate](), 1, @idUsuario, [dbo].[getlocaldate](), @idUsuario, [dbo].[getlocaldate]())
END


COMMIT



GO








/* **** 10 **** */
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
@idRubro int,

@observacionesCredito varchar(1000),
@observaciones varchar(1000),
@vendedoresAsignados smallint,
@estadoLiberacionCrediticia int,
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
@habilitadoNegociacionGrupal bit,
@sedePrincipal bit,
@negociacionMultiregional bit,
@telefonoContacto1 varchar(50),
@emailContacto1 varchar(50),
@observacionHorarioEntrega varchar(1000), 
@fechaInicioVigencia date,
@configuraciones text,
@esCargaMasiva smallint = 0,

@existenCambiosCreditos smallint OUTPUT,
@usuarioSolicitanteCredito uniqueIdentifier OUTPUT,
@correoUsuarioSolicitanteCredito VARCHAR(50) OUTPUT

AS
BEGIN TRAN

DECLARE @id_cliente_sunat int;
DECLARE @id_domicilio_legal int;
DECLARE @existe_cliente bit;

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

DECLARE @countSolicitante int;
DECLARE @idSolicitante uniqueIdentifier;


SET NOCOUNT ON
SET @existenCambiosCreditos = 0;
SET @usuarioSolicitanteCredito = '00000000-0000-0000-0000-000000000000';
SET @correoUsuarioSolicitanteCredito = '';

IF (SELECT tipo_documento FROM CLIENTE where id_cliente = @idCliente) <> 6
BEGIN 
	SET @razonSocial = @nombreComercial;
END
ELSE
BEGIN 
SET @razonSocial = @razonSocialSunat;

	SELECT @ruc = ruc FROM CLIENTE where id_cliente = @idCliente 
	SELECT @existe_cliente = COUNT(*) FROM CLIENTE_SUNAT WHERE ruc = @ruc

	IF @existe_cliente = 0
	BEGIN --Crear ClienteSunat
		SET @id_cliente_sunat = NEXT VALUE FOR SEQ_ID_CLIENTE_SUNAT;
		SET @id_domicilio_legal = NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL;

		INSERT INTO CLIENTE_SUNAT 
		(id_cliente_sunat, ruc,estado, fecha_creacion, fecha_modificacion,
		razon_social,	nombre_comercial,	estado_contribuyente,	condicion_contribuyente, estado_liberacion_creditica)
		VALUES
		( 
		@id_cliente_sunat, @ruc, 1, [dbo].[getlocaldate](), [dbo].[getlocaldate](), 
		@razonSocialSunat, @nombreComercialSunat, @estadoContribuyente, @condicionContribuyente, @estadoLiberacionCrediticia
		)
		
		INSERT INTO DOMICILIO_LEGAL 
		(id_domicilio_legal, id_cliente_sunat, codigo, direccion, ubigeo, 
		es_establecimiento_anexo,
		estado, fecha_creacion, fecha_modificacion, usuario_creacion, usuario_modificacion
		)
		VALUES
		(@id_domicilio_legal, @id_cliente_sunat, '0000', @direccionDomicilioLegalSunat,	@ubigeo, 0,	
		1, [dbo].[getlocaldate](), [dbo].[getlocaldate](), @idUsuario, @idUsuario	)

		UPDATE CLIENTE_SUNAT set id_domicilio_legal = @id_domicilio_legal where id_cliente_sunat = @id_cliente_sunat;

	END 
	ELSE --Actualizar ClienteSunat
	BEGIN 
		SELECT @id_cliente_sunat = id_cliente_sunat, @id_domicilio_legal = id_domicilio_legal FROM CLIENTE_SUNAT WHERE ruc = @ruc

		UPDATE CLIENTE_SUNAT set 
		razon_social = @razonSocialSunat,
		nombre_comercial = @nombreComercialSunat, 
		estado_contribuyente = @estadoContribuyente,
		condicion_contribuyente = @condicionContribuyente,
		usuario_modificacion = @idUsuario,
		estado_liberacion_creditica = @estadoLiberacionCrediticia,
		fecha_modificacion = [dbo].[getlocaldate]()
		WHERE id_cliente_sunat = @id_cliente_sunat

		UPDATE DOMICILIO_LEGAL set
		direccion = @direccionDomicilioLegalSunat,
		ubigeo = @ubigeo,
		usuario_modificacion = @idUsuario,
		fecha_modificacion = [dbo].[getlocaldate]()
		WHERE id_domicilio_legal = @id_domicilio_legal

	END

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
		   ,[fecha_modificacion] = [dbo].[getlocaldate]()
		   ,correo_Envio_Factura = @correoEnvioFactura
		   ,razon_Social_Sunat = @razonSocialSunat
		   ,nombre_Comercial_Sunat = nombre_Comercial_Sunat
		   ,direccion_Domicilio_Legal_Sunat = @direccionDomicilioLegalSunat
		   ,estado_Contribuyente_sunat = @estadoContribuyente
		   ,condicion_Contribuyente_sunat = @condicionContribuyente
		   ,ubigeo = @ubigeo
		   ,forma_pago_factura = @formaPagoFactura
		   ,contacto1 =@contacto1
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
			,habilitado_negociacion_grupal = @habilitadoNegociacionGrupal
			,telefono_contacto1 = @telefonoContacto1
			,email_contacto1 = @emailContacto1
			,usuario_solicitante_credito = @usuarioSolicitanteCredito
			,observacion_horario_entrega = @observacionHorarioEntrega
			,id_origen = @idOrigen
			,id_subdistribuidor = @idSubDistribuidor
			,id_rubro = @idRubro
			,fecha_inicio_vigencia = @fechaInicioVigencia
			,id_grupo_cliente = @idGrupoCliente
			,id_cliente_sunat = @id_cliente_sunat
			,configuraciones =  @configuraciones
			,carga_masiva = @esCargaMasiva

     WHERE 
          id_cliente = @idCliente;

		  
/* IF Agregado */

	
	IF @negociacionMultiregional = 'FALSE'
	BEGIN
		UPDATE CLIENTE 
		SET sede_principal = 'FALSE',
		fecha_inicio_vigencia = @fechaInicioVigencia	
		WHERE ruc like @ruc;
	END

	UPDATE CLIENTE 
	SET negociacion_multiregional = @negociacionMultiregional, pertenece_canal_multiregional = @perteneceCanalMultiregional
	       ,razon_Social_Sunat = @razonSocialSunat
		   ,nombre_Comercial_Sunat = nombre_Comercial_Sunat
		   ,direccion_Domicilio_Legal_Sunat = @direccionDomicilioLegalSunat
		   ,estado_Contribuyente_sunat = @estadoContribuyente
		   ,condicion_Contribuyente_sunat = @condicionContribuyente
		   ,ubigeo = @ubigeo
		   ,es_sub_distribuidor = @esSubDistribuidor
		   ,id_subdistribuidor = @idSubDistribuidor
		   ,fecha_inicio_vigencia = @fechaInicioVigencia
	WHERE ruc like @ruc;


DELETE CLIENTE_GRUPO_CLIENTE 
where id_cliente = @idCliente;


IF @idGrupoCliente > 0 
BEGIN
	INSERT INTO CLIENTE_GRUPO_CLIENTE 
	VALUES (@idCliente, @idGrupoCliente, [dbo].[getlocaldate](), 1, @idUsuario, [dbo].[getlocaldate](), @idUsuario, [dbo].[getlocaldate]())
END



SELECT @countSolicitante = COUNT(*) FROM SOLICITANTE
WHERE id_cliente = @idCliente 
AND estado = 1 
AND REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(@contacto1), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')   = 
REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(nombre), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ') 
	
IF @countSolicitante = 1
BEGIN 
	SELECT @idSolicitante = id_solicitante FROM SOLICITANTE
	WHERE id_cliente = @idCliente 
	AND estado = 1 
	AND REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(@contacto1), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')   = 
	REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(nombre), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ') 
END 


IF @idSolicitante = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)
BEGIN 
	SET @idSolicitante  = NEWID();
	INSERT INTO SOLICITANTE 
	(id_solicitante, id_cliente, nombre, telefono, correo, estado, 
	usuario_creacion, fecha_creacion, usuario_modificacion, fecha_modificacion)
	VALUES(@idSolicitante, @idCliente, @contacto1, @telefonoContacto1, @emailContacto1,1,
	@idUsuario,[dbo].[getlocaldate](), @idUsuario, [dbo].[getlocaldate]());
END 
ELSE
BEGIN
	UPDATE SOLICITANTE SET 
	nombre = @contacto1, 
	telefono = @telefonoContacto1, 
	correo = @emailContacto1,
	usuario_modificacion = @idUsuario,
	fecha_modificacion = [dbo].[getlocaldate]() 
	where id_solicitante = @idSolicitante;
END 


UPDATE ARCHIVO_ADJUNTO SET estado = 0 where id_cliente = @idCliente AND informacion_cliente = 1;

COMMIT


GO








