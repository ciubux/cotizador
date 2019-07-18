
/* **** TABLAS LOG CAMBIOS **** */
CREATE TABLE CATALOGO_TABLA(
	id_catalogo_Tabla int IDENTITY(1,1) NOT NULL,
	codigo char(4) NULL,
	nombre varchar(250) NULL,
	estado smallint NULL
PRIMARY KEY CLUSTERED 
(
	id_catalogo_Tabla ASC
)
) ON [PRIMARY]
GO






CREATE TABLE CATALOGO_CAMPO(
	id_catalogo_campo int IDENTITY(1,1) NOT NULL,
	id_catalogo_tabla int not null,
	codigo char(8) NULL,
	nombre varchar(250) NULL,
	estado smallint NULL
PRIMARY KEY CLUSTERED 
(
	id_catalogo_campo ASC
)
) ON [PRIMARY]
GO






CREATE TABLE CAMBIO(
	id_cambio uniqueidentifier  NOT NULL,
	id_catalogo_tabla int null,
	id_catalogo_campo int not null,
	id_registro varchar(40),
	valor varchar(1000) ,
	estado smallint NULL,
	fecha_inicio_vigencia date NOT NULL,
	fecha_fin_vigencia date NULL,
	usuario_modificacion uniqueidentifier NOT NULL,
	fecha_modificacion datetime NOT NULL,
	 CONSTRAINT [PK_CAMBIO] PRIMARY KEY CLUSTERED 
(
	id_cambio ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



INSERT INTO CATALOGO_TABLA(codigo, nombre, estado) VALUES('0001', 'CLIENTE', 1);
INSERT INTO CATALOGO_TABLA(codigo, nombre, estado) VALUES('0002', 'PRODUCTO', 1);





INSERT INTO CATALOGO_CAMPO(id_catalogo_tabla, codigo, nombre, estado) VALUES(1, '00010001', 'id_grupo', 1);
INSERT INTO CATALOGO_CAMPO(id_catalogo_tabla, codigo, nombre, estado) VALUES(1, '00010002', 'id_responsable_comercial', 1);
INSERT INTO CATALOGO_CAMPO(id_catalogo_tabla, codigo, nombre, estado) VALUES(1, '00010003', 'id_asistente_servicio_cliente', 1);
INSERT INTO CATALOGO_CAMPO(id_catalogo_tabla, codigo, nombre, estado) VALUES(1, '00010004', 'id_supervisor_comercial', 1);
INSERT INTO CATALOGO_CAMPO(id_catalogo_tabla, codigo, nombre, estado) VALUES(1, '00010005', 'plazo_credito', 1);
INSERT INTO CATALOGO_CAMPO(id_catalogo_tabla, codigo, nombre, estado) VALUES(1, '00010006', 'credito_aprobado', 1);
INSERT INTO CATALOGO_CAMPO(id_catalogo_tabla, codigo, nombre, estado) VALUES(1, '00010007', 'negociacion_multiregional', 1);
INSERT INTO CATALOGO_CAMPO(id_catalogo_tabla, codigo, nombre, estado) VALUES(1, '00010008', 'sede_principal', 1);








CREATE PROCEDURE [dbo].[pi_cambio_dato] 

@idUsuario uniqueidentifier,
@idCatalogoTabla int,
@idCatalogoCampo int,
@idRegistro varchar(40),
@valor varchar(1000),
@fechaInicioVigencia date


AS 
BEGIN 

DECLARE @newId uniqueidentifier;
SET NOCOUNT ON
SET @newId = NEWID();


INSERT INTO CAMBIO
           (id_cambio
		   ,id_catalogo_tabla
           ,id_catalogo_campo
		   ,id_registro
           ,valor
           ,estado
		   ,fecha_inicio_vigencia
		   ,usuario_modificacion
		   ,fecha_modificacion
		   )
     VALUES
           (@newId,
		    @idCatalogoTabla,
		    @idCatalogoCampo,
			@idRegistro,
            @valor,
            1, 
			@fechaInicioVigencia,
			@idUsuario,
			GETDATE()
			);

UPDATE CAMBIO 
SET fecha_fin_vigencia = @fechaInicioVigencia, estado = 0
WHERE id_catalogo_campo = @idCatalogoCampo and fecha_fin_vigencia is null and not id_registro = @idRegistro;

END


















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


DECLARE @cdFechaInicioVigencia date;
SET @cdFechaInicioVigencia = Convert(date, GETDATE(), 120);

/* Responsable comercial */
EXEC  pi_cambio_dato @idUsuario, 1, 2, @newId, @idResponsableComercial, @cdFechaInicioVigencia;

/* Asistente servicio cliente */
EXEC  pi_cambio_dato @idUsuario, 1, 3, @newId, @idAsistenteServicioCliente, @cdFechaInicioVigencia;

/* Supervisor comercial */
EXEC  pi_cambio_dato @idUsuario, 1, 4, @newId, @idSupervisorComercial, @cdFechaInicioVigencia;

/* Negociacion multiregional */
EXEC  pi_cambio_dato @idUsuario, 1, 7, @newId, @negociacionMultiregional, @cdFechaInicioVigencia;

/* sede principal */
EXEC  pi_cambio_dato @idUsuario, 1, 8, @newId, @sedePrincipal, @cdFechaInicioVigencia;


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


DECLARE @cdFechaInicioVigencia date;
SET @cdFechaInicioVigencia = Convert(date, GETDATE(), 120);

/* Responsable comercial */
IF @idResponsableComercial != @idResponsableComercialAnterior 
BEGIN
	EXEC  pi_cambio_dato @idUsuario, 1, 2, @idCliente, @idResponsableComercial, @cdFechaInicioVigencia;
END


/* Asistente servicio cliente */
IF @idAsistenteServicioCliente != @idAsistenteServicioClienteAnterior 
BEGIN
	EXEC  pi_cambio_dato @idUsuario, 1, 3, @idCliente, @idAsistenteServicioCliente, @cdFechaInicioVigencia;
END


/* Supervisor comercial */
IF @idSupervisorComercial != @idSupervisorComercialAnterior 
BEGIN
	EXEC  pi_cambio_dato @idUsuario, 1, 4, @idCliente, @idSupervisorComercial, @cdFechaInicioVigencia;
END


/* Negociacion multiregional */
IF @negociacionMultiregional != @negociacionMultiregionalAnterior 
BEGIN
	EXEC  pi_cambio_dato @idUsuario, 1, 7, @idCliente, @negociacionMultiregional, @cdFechaInicioVigencia;
END


/* sede principal */
IF @sedePrincipal != @sedePrincipalAnterior 
BEGIN
	EXEC  pi_cambio_dato @idUsuario, 1, 8, @idCliente, @sedePrincipal, @cdFechaInicioVigencia;
END



END


