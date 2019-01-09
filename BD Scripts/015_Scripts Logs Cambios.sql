
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
SET fecha_fin_vigencia = @fechaInicioVigencia
WHERE id_catalogo_campo = @idCatalogoCampo and fecha_fin_vigencia is null and not id_registro = @idRegistro;

END

















USE [cotizadormp]
GO
/****** Object:  StoredProcedure [dbo].[pi_clienteSunat]    Script Date: 6/01/2019 5:45:52 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



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




