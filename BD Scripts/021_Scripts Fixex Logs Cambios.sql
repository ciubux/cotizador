/* **** 1 **** */
ALTER PROCEDURE [dbo].[pi_cambio_dato] 

@idUsuario uniqueidentifier,
@nombreCatalogoTabla varchar(250),
@nombreCatalogoCampo varchar(250),
@idRegistro varchar(40),
@valor varchar(1000),
@fechaInicioVigencia date


AS 
BEGIN 


DECLARE @newId uniqueidentifier;
DECLARE @idCatalogoTabla int;
DECLARE @idCatalogoCampo int;

SET NOCOUNT ON
SET @newId = NEWID();

SET @idCatalogoTabla = 0;

SELECT @idCatalogoTabla = tab.id_catalogo_Tabla, @idCatalogoCampo = camp.id_catalogo_campo 
FROM CATALOGO_CAMPO camp 
INNER JOIN CATALOGO_TABLA tab ON tab.id_catalogo_Tabla = camp.id_catalogo_tabla 
WHERE camp.nombre like @nombreCatalogoCampo and tab.nombre like @nombreCatalogoTabla; 

IF @idCatalogoTabla > 0  
BEGIN 
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
	SET estado = 0
	WHERE id_catalogo_campo = @idCatalogoCampo and fecha_inicio_vigencia = @fechaInicioVigencia and id_registro = @idRegistro and not id_cambio = @newId;

	/*UPDATE CAMBIO 
	SET fecha_fin_vigencia = @fechaInicioVigencia
	WHERE id_catalogo_campo = @idCatalogoCampo and fecha_fin_vigencia is null and id_registro = @idRegistro and not id_cambio = @newId;*/
END 


END






/* **** 2 **** */
ALTER TABLE CLIENTE  
ADD fecha_inicio_vigencia date;




/* **** 3 **** */
ALTER TABLE PRODUCTO  
ADD fecha_inicio_vigencia date;



/* **** 4 **** */
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
@fechaInicioVigencia date,
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
			,fecha_inicio_vigencia
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
			@tipo,
			@fechaInicioVigencia
			);

END





/* **** 5 **** */
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
@fechaInicioVigencia date,
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
		,fecha_inicio_vigencia = @fechaInicioVigencia
		,tipo = @tipo
	WHERE id_producto like @idProducto;
	
END






/* **** 6 **** */
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
@habilitadoNegociacionGrupal bit,
@sedePrincipal bit,
@negociacionMultiregional bit,
@telefonoContacto1 varchar(50),
@emailContacto1 varchar(50),

@observacionHorarioEntrega varchar(1000), 
@fechaInicioVigencia date,

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
			habilitado_negociacion_grupal,
			telefono_contacto1,
			email_contacto1,
			id_origen,
			id_subdistribuidor,
			fecha_inicio_vigencia,
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
			@habilitadoNegociacionGrupal,
			@telefonoContacto1,
			@emailContacto1,
			@idOrigen,
			@idSubDistribuidor,
			@fechaInicioVigencia,
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


	
IF @negociacionMultiregional = 'FALSE'
BEGIN
	UPDATE CLIENTE 
	SET sede_principal = 'FALSE'
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
WHERE ruc like @ruc;

IF @idGrupoCliente > 0 
BEGIN
	INSERT INTO CLIENTE_GRUPO_CLIENTE 
	VALUES (@newId, @idGrupoCliente, GETDATE(), 1, @idUsuario, GETDATE(), @idUsuario, GETDATE())
END


COMMIT








/* **** 7 **** */
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
@habilitadoNegociacionGrupal bit,
@sedePrincipal bit,
@negociacionMultiregional bit,
@telefonoContacto1 varchar(50),
@emailContacto1 varchar(50),
@observacionHorarioEntrega varchar(1000), 
@fechaInicioVigencia date,

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
			,habilitado_negociacion_grupal = @habilitadoNegociacionGrupal
			,telefono_contacto1 = @telefonoContacto1
			,email_contacto1 = @emailContacto1
			,usuario_solicitante_credito = @usuarioSolicitanteCredito
			,observacion_horario_entrega = @observacionHorarioEntrega
			,id_origen = @idOrigen
			,id_subdistribuidor = @idSubDistribuidor
			,fecha_inicio_vigencia = @fechaInicioVigencia
     WHERE 
          id_cliente = @idCliente;

		  
/* IF Agregado */

	
	IF @negociacionMultiregional = 'FALSE'
	BEGIN
		UPDATE CLIENTE 
		SET sede_principal = 'FALSE'
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
	WHERE ruc like @ruc;


DELETE CLIENTE_GRUPO_CLIENTE 
where id_cliente = @idCliente;


IF @idGrupoCliente > 0 
BEGIN
	INSERT INTO CLIENTE_GRUPO_CLIENTE 
	VALUES (@idCliente, @idGrupoCliente, GETDATE(), 1, @idUsuario, GETDATE(), @idUsuario, GETDATE())
END



END







/* **** 8 **** */
CREATE TRIGGER ti_producto ON PRODUCTO
AFTER INSERT AS
BEGIN


DECLARE @idUsuario uniqueidentifier;
DECLARE @idRegistro uniqueidentifier;
DECLARE @sku varchar(250);
DECLARE @descripcion varchar(500);
DECLARE @fechaIngreso date;
DECLARE @fechaFin date;
DECLARE @skuProveedor varchar(100);
DECLARE @estado int;
DECLARE @precio numeric(18,2);
DECLARE @precioProvincia numeric(18,2);
DECLARE @costo numeric(18,2);
DECLARE @familia varchar(200);
DECLARE @clase varchar(200);
DECLARE @marca varchar(200);
DECLARE @proveedor varchar(10);
DECLARE @unidad varchar(200);
DECLARE @unidadAlternativa varchar(200);
DECLARE @equivalencia int;
DECLARE @unidadProveedor varchar(300);
DECLARE @equivalenciaProveedor int;
DECLARE @monedaCompra varchar(300);
DECLARE @monedaVenta varchar(300);
DECLARE @costoOriginal numeric(18,2);
DECLARE @precioOriginal numeric(18,2);
DECLARE @precioProvinciaOriginal numeric(18,2);
DECLARE @unidadConteo varchar(200);
DECLARE @unidadEstandarInternacional varchar(200);
DECLARE @unidadAlternativaInternacional varchar(200);
DECLARE @equivalenciaUnidadConteoEstandar int;
DECLARE @equivalenciaUnidadConteoAlternativa int;
DECLARE @exoneradoIgv smallint;
DECLARE @inafecto int;
DECLARE @tipo int;

DECLARE @fechaInicioVigencia date;


select @idUsuario = usuario_modificacion, @idRegistro = id_producto, 
     @sku = sku, @descripcion = descripcion,  @fechaIngreso = fecha_ingreso, 
	 @fechaFin = fecha_fin, @skuProveedor = sku_proveedor, @estado = estado, @precio = precio,
	 @precioProvincia = precio_provincia, @costo = costo, @familia = familia, @clase = clase,
	 @marca = marca, @proveedor = proveedor, @unidad = unidad, @unidadAlternativa = unidad_alternativa,
	 @equivalencia = equivalencia, @unidadProveedor = unidad_proveedor, @equivalenciaProveedor = equivalencia_proveedor, 
	 @monedaCompra = moneda_compra, @monedaVenta = moneda_compra, @costoOriginal = moneda_compra,
	 @precioOriginal = precio_original, @precioProvinciaOriginal = precio_provincia_original, @unidadConteo = unidad_conteo,
	 @unidadEstandarInternacional = unidad_estandar_internacional, @unidadAlternativaInternacional = unidad_alternativa_internacional, 
	 @equivalenciaUnidadConteoEstandar = equivalencia_unidad_conteo_estandar, @equivalenciaUnidadConteoAlternativa = equivalencia_unidad_conteo_alternativa,
	 @exoneradoIgv = exonerado_igv, @inafecto = inafecto, @tipo = tipo,
	 @fechaInicioVigencia = fecha_inicio_vigencia  
from INSERTED;

EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'sku', @idRegistro, @sku, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'descripcion', @idRegistro, @descripcion, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'fecha_ingreso', @idRegistro, @fechaIngreso, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'fecha_fin', @idRegistro, @fechaFin, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'sku_proveedor', @idRegistro, @skuProveedor, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'estado', @idRegistro, @estado, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'precio', @idRegistro, @precio, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'precio_provincia', @idRegistro, @precioProvincia, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'costo', @idRegistro, @costo, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'familia', @idRegistro, @familia, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'clase', @idRegistro, @clase, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'marca', @idRegistro, @marca, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'proveedor', @idRegistro, @proveedor, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad', @idRegistro, @unidad, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad_alternativa', @idRegistro, @unidadAlternativa, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'equivalencia', @idRegistro, @equivalencia, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad_proveedor', @idRegistro, @unidadProveedor, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'equivalencia_proveedor', @idRegistro, @equivalenciaProveedor, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'moneda_compra', @idRegistro, @monedaCompra, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'moneda_venta', @idRegistro, @monedaVenta, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'costo_original', @idRegistro, @costoOriginal, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'precio_original', @idRegistro, @precioOriginal, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'precio_provincia_original', @idRegistro, @precioProvinciaOriginal, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad_conteo', @idRegistro, @unidadConteo, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad_estandar_internacional', @idRegistro, @unidadEstandarInternacional, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad_alternativa_internacional', @idRegistro, @unidadAlternativaInternacional, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'equivalencia_unidad_conteo_estandar', @idRegistro, @equivalenciaUnidadConteoEstandar, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'equivalencia_unidad_conteo_alternativa', @idRegistro, @equivalenciaUnidadConteoAlternativa, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'exonerado_igv', @idRegistro, @exoneradoIgv, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'inafecto', @idRegistro, @inafecto, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'tipo', @idRegistro, @tipo, @fechaInicioVigencia ;



END
GO






/* **** 9 **** */
CREATE TRIGGER tu_producto ON PRODUCTO
AFTER UPDATE AS
BEGIN


DECLARE @idUsuario uniqueidentifier;
DECLARE @idRegistro uniqueidentifier;
DECLARE @sku varchar(250);
DECLARE @descripcion varchar(500);
DECLARE @fechaIngreso date;
DECLARE @fechaFin date;
DECLARE @skuProveedor varchar(100);
DECLARE @estado int;
DECLARE @precio numeric(18,2);
DECLARE @precioProvincia numeric(18,2);
DECLARE @costo numeric(18,2);
DECLARE @familia varchar(200);
DECLARE @clase varchar(200);
DECLARE @marca varchar(200);
DECLARE @proveedor varchar(10);
DECLARE @unidad varchar(200);
DECLARE @unidadAlternativa varchar(200);
DECLARE @equivalencia int;
DECLARE @unidadProveedor varchar(300);
DECLARE @equivalenciaProveedor int;
DECLARE @monedaCompra varchar(300);
DECLARE @monedaVenta varchar(300);
DECLARE @costoOriginal numeric(18,2);
DECLARE @precioOriginal numeric(18,2);
DECLARE @precioProvinciaOriginal numeric(18,2);
DECLARE @unidadConteo varchar(200);
DECLARE @unidadEstandarInternacional varchar(200);
DECLARE @unidadAlternativaInternacional varchar(200);
DECLARE @equivalenciaUnidadConteoEstandar int;
DECLARE @equivalenciaUnidadConteoAlternativa int;
DECLARE @exoneradoIgv smallint;
DECLARE @inafecto int;
DECLARE @tipo int;


DECLARE @skuPrev varchar(250);
DECLARE @descripcionPrev varchar(500);
DECLARE @fechaIngresoPrev date;
DECLARE @fechaFinPrev date;
DECLARE @skuProveedorPrev varchar(100);
DECLARE @estadoPrev int;
DECLARE @precioPrev numeric(18,2);
DECLARE @precioProvinciaPrev numeric(18,2);
DECLARE @costoPrev numeric(18,2);
DECLARE @familiaPrev varchar(200);
DECLARE @clasePrev varchar(200);
DECLARE @marcaPrev varchar(200);
DECLARE @proveedorPrev varchar(10);
DECLARE @unidadPrev varchar(200);
DECLARE @unidadAlternativaPrev varchar(200);
DECLARE @equivalenciaPrev int;
DECLARE @unidadProveedorPrev varchar(300);
DECLARE @equivalenciaProveedorPrev int;
DECLARE @monedaCompraPrev varchar(300);
DECLARE @monedaVentaPrev varchar(300);
DECLARE @costoOriginalPrev numeric(18,2);
DECLARE @precioOriginalPrev numeric(18,2);
DECLARE @precioProvinciaOriginalPrev numeric(18,2);
DECLARE @unidadConteoPrev varchar(200);
DECLARE @unidadEstandarInternacionalPrev varchar(200);
DECLARE @unidadAlternativaInternacionalPrev varchar(200);
DECLARE @equivalenciaUnidadConteoEstandarPrev int;
DECLARE @equivalenciaUnidadConteoAlternativaPrev int;
DECLARE @exoneradoIgvPrev smallint;
DECLARE @inafectoPrev int;
DECLARE @tipoPrev int;

DECLARE @fechaInicioVigencia date;

DECLARE @tempPrice varchar(20);


select @idUsuario = usuario_modificacion, @idRegistro = id_producto, 
     @sku = sku, @descripcion = descripcion,  @fechaIngreso = fecha_ingreso, 
	 @fechaFin = fecha_fin, @skuProveedor = sku_proveedor, @estado = estado, @precio = precio,
	 @precioProvincia = precio_provincia, @costo = costo, @familia = familia, @clase = clase,
	 @marca = marca, @proveedor = proveedor, @unidad = unidad, @unidadAlternativa = unidad_alternativa,
	 @equivalencia = equivalencia, @unidadProveedor = unidad_proveedor, @equivalenciaProveedor = equivalencia_proveedor, 
	 @monedaCompra = moneda_compra, @monedaVenta = moneda_venta, @costoOriginal = costo_original,
	 @precioOriginal = precio_original, @precioProvinciaOriginal = precio_provincia_original, @unidadConteo = unidad_conteo,
	 @unidadEstandarInternacional = unidad_estandar_internacional, @unidadAlternativaInternacional = unidad_alternativa_internacional, 
	 @equivalenciaUnidadConteoEstandar = equivalencia_unidad_conteo_estandar, @equivalenciaUnidadConteoAlternativa = equivalencia_unidad_conteo_alternativa,
	 @exoneradoIgv = exonerado_igv, @inafecto = inafecto, @tipo = tipo,
	 @fechaInicioVigencia = fecha_inicio_vigencia  
from INSERTED;


SELECT 
     @skuPrev = sku, @descripcionPrev = descripcion,  @fechaIngresoPrev = fecha_ingreso, 
	 @fechaFinPrev = fecha_fin, @skuProveedorPrev = sku_proveedor, @estadoPrev = estado, @precioPrev = precio,
	 @precioProvinciaPrev = precio_provincia, @costoPrev = costo, @familiaPrev = familia, @clasePrev = clase,
	 @marcaPrev = marca, @proveedorPrev = proveedor, @unidadPrev = unidad, @unidadAlternativaPrev = unidad_alternativa,
	 @equivalenciaPrev = equivalencia, @unidadProveedorPrev = unidad_proveedor, @equivalenciaProveedorPrev = equivalencia_proveedor, 
	 @monedaCompraPrev = moneda_compra, @monedaVenta = moneda_venta, @costoOriginal = costo_original, 
	 @precioOriginalPrev = precio_original, @precioProvinciaOriginalPrev = precio_provincia_original, @unidadConteoPrev = unidad_conteo,
	 @unidadEstandarInternacionalPrev = unidad_estandar_internacional, @unidadAlternativaInternacionalPrev = unidad_alternativa_internacional, 
	 @equivalenciaUnidadConteoEstandarPrev = equivalencia_unidad_conteo_estandar, @equivalenciaUnidadConteoAlternativaPrev = equivalencia_unidad_conteo_alternativa,
	 @exoneradoIgvPrev = exonerado_igv, @inafectoPrev = inafecto, @tipoPrev = tipo
FROM DELETED;


IF @sku <> @skuPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'sku', @idRegistro, @sku, @fechaInicioVigencia ;
END 


IF @descripcion <> @descripcionPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'descripcion', @idRegistro, @descripcion, @fechaInicioVigencia ;
END 


IF @fechaIngreso <> @fechaIngresoPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'fecha_ingreso', @idRegistro, @fechaIngreso, @fechaInicioVigencia ;
END 


IF @fechaFin <> @fechaFinPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'fecha_fin', @idRegistro, @fechaFin, @fechaInicioVigencia ;
END 


IF @skuProveedor <> @skuProveedorPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'sku_proveedor', @idRegistro, @skuProveedor, @fechaInicioVigencia ;
END 


IF @estado <> @estadoPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'estado', @idRegistro, @estado, @fechaInicioVigencia ;
END 


IF @precio <> @precioPrev 
BEGIN
	set @tempPrice = cast(@precio as varchar(20));
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'precio', @idRegistro,  @tempPrice, @fechaInicioVigencia ;
END 



IF @precioProvincia <> @precioProvinciaPrev 
BEGIN
	set @tempPrice = cast(@precioProvincia as varchar(20));
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'precio_provincia', @idRegistro, @tempPrice, @fechaInicioVigencia ;
END 


IF @costo <> @costoPrev 
BEGIN
	set @tempPrice = cast(@costo as varchar(20));
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'costo', @idRegistro, @tempPrice, @fechaInicioVigencia ;
END 


IF @familia <> @familiaPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'familia', @idRegistro, @familia, @fechaInicioVigencia ;
END 


IF @clase <> @clasePrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'clase', @idRegistro, @clase, @fechaInicioVigencia ;
END 



IF @marca <> @marcaPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'marca', @idRegistro, @marca, @fechaInicioVigencia ;
END 



IF @proveedor <> @proveedorPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'proveedor', @idRegistro, @proveedor, @fechaInicioVigencia ;
END 



IF @unidad <> @unidadPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad', @idRegistro, @unidad, @fechaInicioVigencia ;
END 



IF @unidadAlternativa <> @unidadAlternativaPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad_alternativa', @idRegistro, @unidadAlternativa, @fechaInicioVigencia ;
END 



IF @equivalencia <> @equivalenciaPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'equivalencia', @idRegistro, @equivalencia, @fechaInicioVigencia ;
END 



IF @unidadProveedor <> @unidadProveedorPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad_proveedor', @idRegistro, @unidadProveedor, @fechaInicioVigencia ;
END 



IF @equivalenciaProveedor <> @equivalenciaProveedorPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'equivalencia_proveedor', @idRegistro, @equivalenciaProveedor, @fechaInicioVigencia ;
END 



IF @monedaCompra <> @monedaCompraPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'moneda_compra', @idRegistro, @monedaCompra, @fechaInicioVigencia ;
END 



IF @monedaVenta <> @monedaVentaPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'moneda_venta', @idRegistro, @monedaVenta, @fechaInicioVigencia ;
END 



IF @costoOriginal <> @costoOriginalPrev 
BEGIN
	set @tempPrice = cast(@costoOriginal as varchar(20));
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'costo_original', @idRegistro, @tempPrice, @fechaInicioVigencia ;
END 


IF @precioOriginal <> @precioOriginalPrev 
BEGIN
	set @tempPrice = cast(@precioOriginal as varchar(20));
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'precio_original', @idRegistro, @tempPrice, @fechaInicioVigencia ;
END 



IF @precioProvinciaOriginal <> @precioProvinciaOriginalPrev 
BEGIN
	set @tempPrice = cast(@precioProvinciaOriginal as varchar(20));
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'precio_provincia_original', @idRegistro, @tempPrice, @fechaInicioVigencia ;
END 



IF @unidadConteo <> @unidadConteoPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad_conteo', @idRegistro, @unidadConteo, @fechaInicioVigencia ;
END 



IF @unidadEstandarInternacional <> @unidadEstandarInternacionalPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad_estandar_internacional', @idRegistro, @unidadEstandarInternacional, @fechaInicioVigencia ;
END 



IF @unidadAlternativaInternacional <> @unidadAlternativaInternacionalPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad_alternativa_internacional', @idRegistro, @unidadAlternativaInternacional, @fechaInicioVigencia ;
END 



IF @equivalenciaUnidadConteoEstandar <> @equivalenciaUnidadConteoEstandarPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'equivalencia_unidad_conteo_estandar', @idRegistro, @equivalenciaUnidadConteoEstandar, @fechaInicioVigencia ;
END 



IF @equivalenciaUnidadConteoAlternativa <> @equivalenciaUnidadConteoAlternativaPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'equivalencia_unidad_conteo_alternativa', @idRegistro, @equivalenciaUnidadConteoAlternativa, @fechaInicioVigencia ;
END 



IF @exoneradoIgv <> @exoneradoIgvPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'exonerado_igv', @idRegistro, @exoneradoIgv, @fechaInicioVigencia ;
END 



IF @inafecto <> @inafectoPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'inafecto', @idRegistro, @inafecto, @fechaInicioVigencia ;
END 



IF @tipo <> @tipoPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'tipo', @idRegistro, @tipo, @fechaInicioVigencia ;
END 




END
GO










/* **** 10 **** */
CREATE TRIGGER ti_cliente ON CLIENTE
AFTER INSERT AS
BEGIN


DECLARE @idUsuario uniqueidentifier;
DECLARE @idRegistro uniqueidentifier;

DECLARE @codigo varchar(250);
DECLARE @codigoAlterno int;
DECLARE @razonSocial varchar(200);
DECLARE @nombreComercial varchar(200);
DECLARE @contacto1 varchar(200);
DECLARE @contacto2 varchar(200);
DECLARE @ruc varchar(20);
DECLARE @idGrupo uniqueidentifier;
DECLARE @idCiudad uniqueidentifier;
DECLARE @sede varchar(10);
DECLARE @vendedor varchar(10);
DECLARE @domicilioLegal varchar(200);
DECLARE @ubigeo varchar(10);
DECLARE @distrito varchar(200);
DECLARE @direccionDespacho varchar(200);
DECLARE @rubro varchar(200);
DECLARE @emailFacturaElectronica varchar(1000);
DECLARE @estado int;
DECLARE @razonSocialSunat varchar(500);
DECLARE @nombreComercialSunat varchar(500);
DECLARE @direccionDomicilioLegalSunat  varchar(1000);
DECLARE @estadoContribuyenteSunat varchar(50);
DECLARE @condicionContribuyenteSunat varchar(50);
DECLARE @validado int;
DECLARE @correoEnvioFactura  varchar(1000);
DECLARE @plazoCredito varchar(50);
DECLARE @tipoPagoFactura int;
DECLARE @formaPagoFactura int;
DECLARE @tipoDocumento varchar(10);
DECLARE @esProveedor smallint;
DECLARE @creditoSolicitado decimal(12,2);
DECLARE @creditoAprobado decimal(12,2);
DECLARE @idResponsableComercial int;
DECLARE @idAsistenteServicioCliente int;
DECLARE @idSupervisorComercial int;
DECLARE @sobreGiro decimal(12,2);
DECLARE @vendedoresAsignados smallint;
DECLARE @plazoCreditoSolicitado int;
DECLARE @sobrePlazo int;
DECLARE @observacionesCredito varchar(1000);
DECLARE @observaciones varchar(1000);
DECLARE @bloqueado int;
DECLARE @perteneceCanalMultiregional smallint;
DECLARE @perteneceCanalLima smallint;
DECLARE @perteneceCanalProvincia smallint;
DECLARE @perteneceCanalPcp smallint;
DECLARE @esSubDistribuidor smallint;
DECLARE @horaInicioPrimerTurnoEntrega time(7);
DECLARE @horaFinPrimerTurnoEntrega time(7);
DECLARE @horaInicioSegundoTurnoEntrega time(7);
DECLARE @horaFinSegundoTurnoEntrega time(7);
DECLARE @sedePrincipal bit;
DECLARE @telefonoContacto1 varchar(50);
DECLARE @emailContacto1 varchar(50);
DECLARE @negociacionMultiregional bit;
DECLARE @usuarioSolicitanteCredito uniqueidentifier;
DECLARE @observacionHorarioEntrega varchar(1000);
DECLARE @idSubdistribuidor int;
DECLARE @idOrigen int;
DECLARE @habilitadoNegociacionGrupal bit;

DECLARE @fechaInicioVigencia date;


select @idUsuario = usuario_modificacion, @idRegistro = id_cliente, 
     @codigo = codigo, @codigoAlterno = codigo_alterno,  @razonSocial = razon_social, 
	 @nombreComercial = nombre_comercial, @contacto1 = contacto1, @contacto2 = contacto2, @ruc = ruc,
	 @idGrupo = id_grupo, @idCiudad = id_ciudad, @sede = sede, @vendedor = vendedor,
	 @domicilioLegal = domicilio_legal, @ubigeo = ubigeo, @distrito = distrito, @direccionDespacho = direccion_despacho,
	 @rubro = rubro, @emailFacturaElectronica = email_factura_electronica, @estado = estado, 
	 @razonSocialSunat = razon_social_sunat, @nombreComercialSunat = nombre_comercial_sunat, @direccionDomicilioLegalSunat = direccion_domicilio_legal_sunat,
	 @estadoContribuyenteSunat = estado_contribuyente_sunat, @condicionContribuyenteSunat = condicion_contribuyente_sunat, @validado = validado,
	 @correoEnvioFactura = correo_envio_factura, @plazoCredito = plazo_credito, @tipoPagoFactura = tipo_pago_factura,  
	 @formaPagoFactura = forma_pago_factura, @tipoDocumento = tipo_documento, @esProveedor = es_proveedor,
	 @creditoSolicitado = credito_solicitado, @creditoAprobado = credito_aprobado, @idResponsableComercial = id_responsable_comercial,
	 @idAsistenteServicioCliente = id_asistente_servicio_cliente, @idSupervisorComercial = id_supervisor_comercial, @sobreGiro = sobre_giro, 
	 @vendedoresAsignados = vendedores_asignados, @plazoCreditoSolicitado = plazo_credito_solicitado, @sobrePlazo = sobre_plazo, 
	 @observacionesCredito = observaciones_credito, @observaciones = observaciones, @bloqueado = bloqueado, 
	 @perteneceCanalMultiregional = pertenece_canal_multiregional, @perteneceCanalLima = pertenece_canal_lima, @perteneceCanalProvincia = pertenece_canal_provincia, 
	 @perteneceCanalPcp = pertenece_canal_pcp, @esSubDistribuidor = es_sub_distribuidor, @horaInicioPrimerTurnoEntrega = hora_inicio_primer_turno_entrega, 
	 @horaFinPrimerTurnoEntrega = hora_fin_primer_turno_entrega, @horaInicioSegundoTurnoEntrega = hora_inicio_segundo_turno_entrega, @horaFinSegundoTurnoEntrega = hora_fin_segundo_turno_entrega, 
	 @sedePrincipal = sede_principal, @telefonoContacto1 = telefono_contacto1, @emailContacto1 = email_contacto1,
	 @negociacionMultiregional = negociacion_multiregional, @usuarioSolicitanteCredito = usuario_solicitante_credito, @observacionHorarioEntrega = observacion_horario_entrega, 
	 @idSubdistribuidor = id_subdistribuidor, @idOrigen = id_origen, @habilitadoNegociacionGrupal = habilitado_negociacion_grupal,  
	 @fechaInicioVigencia = fecha_inicio_vigencia  
from INSERTED;


EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'codigo', @idRegistro, @codigo, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'codigo_alterno', @idRegistro, @codigoAlterno, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'razon_social', @idRegistro, @razonSocial, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'nombre_comercial', @idRegistro, @nombreComercial, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'contacto1', @idRegistro, @contacto1, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'contacto2', @idRegistro, @contacto2, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'ruc', @idRegistro, @ruc, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_grupo', @idRegistro, @idGrupo, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_ciudad', @idRegistro, @idCiudad, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'sede', @idRegistro, @sede, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'vendedor', @idRegistro, @vendedor, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'domicilio_legal', @idRegistro, @domicilioLegal, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'ubigeo', @idRegistro, @ubigeo, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'distrito', @idRegistro, @distrito, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'direccion_despacho', @idRegistro, @direccionDespacho, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'rubro', @idRegistro, @rubro, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'email_factura_electronica', @idRegistro, @emailFacturaElectronica, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'estado', @idRegistro, @estado, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'razon_social_sunat', @idRegistro, @razonSocialSunat, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'nombre_comercial_sunat', @idRegistro, @nombreComercialSunat, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'direccion_domicilio_legal_sunat', @idRegistro, @direccionDomicilioLegalSunat, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'estado_contribuyente_sunat', @idRegistro, @estadoContribuyenteSunat, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'condicion_contribuyente_sunat', @idRegistro, @condicionContribuyenteSunat, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'validado', @idRegistro, @validado, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'correo_envio_factura', @idRegistro, @correoEnvioFactura, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'plazo_credito', @idRegistro, @plazoCredito, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'tipo_pago_factura', @idRegistro, @tipoPagoFactura, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'forma_pago_factura', @idRegistro, @formaPagoFactura, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'tipo_documento', @idRegistro, @tipoDocumento, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'es_proveedor', @idRegistro, @esProveedor, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'credito_solicitado', @idRegistro, @creditoSolicitado, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'credito_aprobado', @idRegistro, @creditoAprobado, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_responsable_comercial', @idRegistro, @idResponsableComercial, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_asistente_servicio_cliente', @idRegistro, @idAsistenteServicioCliente, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_supervisor_comercial', @idRegistro, @idSupervisorComercial, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'sobre_giro', @idRegistro, @sobreGiro, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'vendedores_asignados', @idRegistro, @vendedoresAsignados, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'plazo_credito_solicitado', @idRegistro, @plazoCreditoSolicitado, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'sobre_giro', @idRegistro, @sobreGiro, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'observaciones_credito', @idRegistro, @observacionesCredito, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'observaciones', @idRegistro, @observaciones, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'bloqueado', @idRegistro, @bloqueado, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'pertenece_canal_multiregional', @idRegistro, @perteneceCanalMultiregional, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'pertenece_canal_lima', @idRegistro, @perteneceCanalLima, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'pertenece_canal_provincia', @idRegistro, @perteneceCanalProvincia, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'pertenece_canal_pcp', @idRegistro, @perteneceCanalPcp, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'es_sub_distribuidor', @idRegistro, @esSubDistribuidor, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'hora_inicio_primer_turno_entrega', @idRegistro, @horaInicioPrimerTurnoEntrega, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'hora_fin_primer_turno_entrega', @idRegistro, @horaFinPrimerTurnoEntrega, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'hora_inicio_segundo_turno_entrega', @idRegistro, @horaInicioSegundoTurnoEntrega, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'hora_fin_segundo_turno_entrega', @idRegistro, @horaFinSegundoTurnoEntrega, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'sede_principal', @idRegistro, @sedePrincipal, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'telefono_contacto1', @idRegistro, @telefonoContacto1, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'email_contacto1', @idRegistro, @emailContacto1, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'negociacion_multiregional', @idRegistro, @negociacionMultiregional, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'usuario_solicitante_credito', @idRegistro, @usuarioSolicitanteCredito, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'observacion_horario_entrega', @idRegistro, @observacionHorarioEntrega, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_subdistribuidor', @idRegistro, @idSubdistribuidor, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_origen', @idRegistro, @idOrigen, @fechaInicioVigencia ;
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'habilitado_negociacion_grupal', @idRegistro, @habilitadoNegociacionGrupal, @fechaInicioVigencia ;


END
GO







/* **** 11 **** */
CREATE TRIGGER tu_cliente ON CLIENTE
AFTER UPDATE AS
BEGIN


DECLARE @idUsuario uniqueidentifier;
DECLARE @idRegistro uniqueidentifier;

DECLARE @codigo varchar(250);
DECLARE @codigoAlterno int;
DECLARE @razonSocial varchar(200);
DECLARE @nombreComercial varchar(200);
DECLARE @contacto1 varchar(200);
DECLARE @contacto2 varchar(200);
DECLARE @ruc varchar(20);
DECLARE @idGrupo uniqueidentifier;
DECLARE @idCiudad uniqueidentifier;
DECLARE @sede varchar(10);
DECLARE @vendedor varchar(10);
DECLARE @domicilioLegal varchar(200);
DECLARE @ubigeo varchar(10);
DECLARE @distrito varchar(200);
DECLARE @direccionDespacho varchar(200);
DECLARE @rubro varchar(200);
DECLARE @emailFacturaElectronica varchar(1000);
DECLARE @estado int;
DECLARE @razonSocialSunat varchar(500);
DECLARE @nombreComercialSunat varchar(500);
DECLARE @direccionDomicilioLegalSunat  varchar(1000);
DECLARE @estadoContribuyenteSunat varchar(50);
DECLARE @condicionContribuyenteSunat varchar(50);
DECLARE @validado int;
DECLARE @correoEnvioFactura  varchar(1000);
DECLARE @plazoCredito varchar(50);
DECLARE @tipoPagoFactura int;
DECLARE @formaPagoFactura int;
DECLARE @tipoDocumento varchar(10);
DECLARE @esProveedor smallint;
DECLARE @creditoSolicitado decimal(12,2);
DECLARE @creditoAprobado decimal(12,2);
DECLARE @idResponsableComercial int;
DECLARE @idAsistenteServicioCliente int;
DECLARE @idSupervisorComercial int;
DECLARE @sobreGiro decimal(12,2);
DECLARE @vendedoresAsignados smallint;
DECLARE @plazoCreditoSolicitado int;
DECLARE @sobrePlazo int;
DECLARE @observacionesCredito varchar(1000);
DECLARE @observaciones varchar(1000);
DECLARE @bloqueado int;
DECLARE @perteneceCanalMultiregional smallint;
DECLARE @perteneceCanalLima smallint;
DECLARE @perteneceCanalProvincia smallint;
DECLARE @perteneceCanalPcp smallint;
DECLARE @esSubDistribuidor smallint;
DECLARE @horaInicioPrimerTurnoEntrega time(7);
DECLARE @horaFinPrimerTurnoEntrega time(7);
DECLARE @horaInicioSegundoTurnoEntrega time(7);
DECLARE @horaFinSegundoTurnoEntrega time(7);
DECLARE @sedePrincipal bit;
DECLARE @telefonoContacto1 varchar(50);
DECLARE @emailContacto1 varchar(50);
DECLARE @negociacionMultiregional bit;
DECLARE @usuarioSolicitanteCredito uniqueidentifier;
DECLARE @observacionHorarioEntrega varchar(1000);
DECLARE @idSubdistribuidor int;
DECLARE @idOrigen int;
DECLARE @habilitadoNegociacionGrupal bit;

DECLARE @fechaInicioVigencia date;


DECLARE @codigoPrev varchar(250);
DECLARE @codigoAlternoPrev int;
DECLARE @razonSocialPrev varchar(200);
DECLARE @nombreComercialPrev varchar(200);
DECLARE @contacto1Prev varchar(200);
DECLARE @contacto2Prev varchar(200);
DECLARE @rucPrev varchar(20);
DECLARE @idGrupoPrev uniqueidentifier;
DECLARE @idCiudadPrev uniqueidentifier;
DECLARE @sedePrev varchar(10);
DECLARE @vendedorPrev varchar(10);
DECLARE @domicilioLegalPrev varchar(200);
DECLARE @ubigeoPrev varchar(10);
DECLARE @distritoPrev varchar(200);
DECLARE @direccionDespachoPrev varchar(200);
DECLARE @rubroPrev varchar(200);
DECLARE @emailFacturaElectronicaPrev varchar(1000);
DECLARE @estadoPrev int;
DECLARE @razonSocialSunatPrev varchar(500);
DECLARE @nombreComercialSunatPrev varchar(500);
DECLARE @direccionDomicilioLegalSunatPrev varchar(1000);
DECLARE @estadoContribuyenteSunatPrev varchar(50);
DECLARE @condicionContribuyenteSunatPrev varchar(50);
DECLARE @validadoPrev int;
DECLARE @correoEnvioFacturaPrev varchar(1000);
DECLARE @plazoCreditoPrev varchar(50);
DECLARE @tipoPagoFacturaPrev int;
DECLARE @formaPagoFacturaPrev int;
DECLARE @tipoDocumentoPrev varchar(10);
DECLARE @esProveedorPrev smallint;
DECLARE @creditoSolicitadoPrev decimal(12,2);
DECLARE @creditoAprobadoPrev decimal(12,2);
DECLARE @idResponsableComercialPrev int;
DECLARE @idAsistenteServicioClientePrev int;
DECLARE @idSupervisorComercialPrev int;
DECLARE @sobreGiroPrev decimal(12,2);
DECLARE @vendedoresAsignadosPrev smallint;
DECLARE @plazoCreditoSolicitadoPrev int;
DECLARE @sobrePlazoPrev int;
DECLARE @observacionesCreditoPrev varchar(1000);
DECLARE @observacionesPrev varchar(1000);
DECLARE @bloqueadoPrev int;
DECLARE @perteneceCanalMultiregionalPrev smallint;
DECLARE @perteneceCanalLimaPrev smallint;
DECLARE @perteneceCanalProvinciaPrev smallint;
DECLARE @perteneceCanalPcpPrev smallint;
DECLARE @esSubDistribuidorPrev smallint;
DECLARE @horaInicioPrimerTurnoEntregaPrev time(7);
DECLARE @horaFinPrimerTurnoEntregaPrev time(7);
DECLARE @horaInicioSegundoTurnoEntregaPrev time(7);
DECLARE @horaFinSegundoTurnoEntregaPrev time(7);
DECLARE @sedePrincipalPrev bit;
DECLARE @telefonoContacto1Prev varchar(50);
DECLARE @emailContacto1Prev varchar(50);
DECLARE @negociacionMultiregionalPrev bit;
DECLARE @usuarioSolicitanteCreditoPrev uniqueidentifier;
DECLARE @observacionHorarioEntregaPrev varchar(1000);
DECLARE @idSubdistribuidorPrev int;
DECLARE @idOrigenPrev int;
DECLARE @habilitadoNegociacionGrupalPrev bit;


select @idUsuario = usuario_modificacion, @idRegistro = id_cliente, 
     @codigo = codigo, @codigoAlterno = codigo_alterno,  @razonSocial = razon_social, 
	 @nombreComercial = nombre_comercial, @contacto1 = contacto1, @contacto2 = contacto2, @ruc = ruc,
	 @idGrupo = id_grupo, @idCiudad = id_ciudad, @sede = sede, @vendedor = vendedor,
	 @domicilioLegal = domicilio_legal, @ubigeo = ubigeo, @distrito = distrito, @direccionDespacho = direccion_despacho,
	 @rubro = rubro, @emailFacturaElectronica = email_factura_electronica, @estado = estado, 
	 @razonSocialSunat = razon_social_sunat, @nombreComercialSunat = nombre_comercial_sunat, @direccionDomicilioLegalSunat = direccion_domicilio_legal_sunat,
	 @estadoContribuyenteSunat = estado_contribuyente_sunat, @condicionContribuyenteSunat = condicion_contribuyente_sunat, @validado = validado,
	 @correoEnvioFactura = correo_envio_factura, @plazoCredito = plazo_credito, @tipoPagoFactura = tipo_pago_factura,  
	 @formaPagoFactura = forma_pago_factura, @tipoDocumento = tipo_documento, @esProveedor = es_proveedor,
	 @creditoSolicitado = credito_solicitado, @creditoAprobado = credito_aprobado, @idResponsableComercial = id_responsable_comercial,
	 @idAsistenteServicioCliente = id_asistente_servicio_cliente, @idSupervisorComercial = id_supervisor_comercial, @sobreGiro = sobre_giro, 
	 @vendedoresAsignados = vendedores_asignados, @plazoCreditoSolicitado = plazo_credito_solicitado, @sobrePlazo = sobre_plazo, 
	 @observacionesCredito = observaciones_credito, @observaciones = observaciones, @bloqueado = bloqueado, 
	 @perteneceCanalMultiregional = pertenece_canal_multiregional, @perteneceCanalLima = pertenece_canal_lima, @perteneceCanalProvincia = pertenece_canal_provincia, 
	 @perteneceCanalPcp = pertenece_canal_pcp, @esSubDistribuidor = es_sub_distribuidor, @horaInicioPrimerTurnoEntrega = hora_inicio_primer_turno_entrega, 
	 @horaFinPrimerTurnoEntrega = hora_fin_primer_turno_entrega, @horaInicioSegundoTurnoEntrega = hora_inicio_segundo_turno_entrega, @horaFinSegundoTurnoEntrega = hora_fin_segundo_turno_entrega, 
	 @sedePrincipal = sede_principal, @telefonoContacto1 = telefono_contacto1, @emailContacto1 = email_contacto1,
	 @negociacionMultiregional = negociacion_multiregional, @usuarioSolicitanteCredito = usuario_solicitante_credito, @observacionHorarioEntrega = observacion_horario_entrega, 
	 @idSubdistribuidor = id_subdistribuidor, @idOrigen = id_origen, @habilitadoNegociacionGrupal = habilitado_negociacion_grupal,  
	 @fechaInicioVigencia = fecha_inicio_vigencia  
from INSERTED;


select
     @codigoPrev = codigo, @codigoAlternoPrev = codigo_alterno,  @razonSocialPrev = razon_social, 
	 @nombreComercialPrev = nombre_comercial, @contacto1Prev = contacto1, @contacto2Prev = contacto2, @rucPrev = ruc,
	 @idGrupoPrev = id_grupo, @idCiudadPrev = id_ciudad, @sedePrev = sede, @vendedorPrev = vendedor,
	 @domicilioLegalPrev = domicilio_legal, @ubigeoPrev = ubigeo, @distritoPrev = distrito, @direccionDespachoPrev = direccion_despacho,
	 @rubroPrev = rubro, @emailFacturaElectronicaPrev = email_factura_electronica, @estadoPrev = estado, 
	 @razonSocialSunatPrev = razon_social_sunat, @nombreComercialSunatPrev = nombre_comercial_sunat, @direccionDomicilioLegalSunatPrev = direccion_domicilio_legal_sunat,
	 @estadoContribuyenteSunatPrev = estado_contribuyente_sunat, @condicionContribuyenteSunatPrev = condicion_contribuyente_sunat, @validadoPrev = validado,
	 @correoEnvioFacturaPrev = correo_envio_factura, @plazoCreditoPrev = plazo_credito, @tipoPagoFacturaPrev = tipo_pago_factura,  
	 @formaPagoFacturaPrev = forma_pago_factura, @tipoDocumentoPrev = tipo_documento, @esProveedorPrev = es_proveedor,
	 @creditoSolicitadoPrev = credito_solicitado, @creditoAprobadoPrev = credito_aprobado, @idResponsableComercialPrev = id_responsable_comercial,
	 @idAsistenteServicioClientePrev = id_asistente_servicio_cliente, @idSupervisorComercialPrev = id_supervisor_comercial, @sobreGiroPrev = sobre_giro, 
	 @vendedoresAsignadosPrev = vendedores_asignados, @plazoCreditoSolicitadoPrev = plazo_credito_solicitado, @sobrePlazoPrev = sobre_plazo, 
	 @observacionesCreditoPrev = observaciones_credito, @observacionesPrev = observaciones, @bloqueadoPrev = bloqueado, 
	 @perteneceCanalMultiregionalPrev = pertenece_canal_multiregional, @perteneceCanalLimaPrev = pertenece_canal_lima, @perteneceCanalProvinciaPrev = pertenece_canal_provincia, 
	 @perteneceCanalPcpPrev = pertenece_canal_pcp, @esSubDistribuidorPrev = es_sub_distribuidor, @horaInicioPrimerTurnoEntregaPrev = hora_inicio_primer_turno_entrega, 
	 @horaFinPrimerTurnoEntregaPrev = hora_fin_primer_turno_entrega, @horaInicioSegundoTurnoEntregaPrev = hora_inicio_segundo_turno_entrega, @horaFinSegundoTurnoEntregaPrev = hora_fin_segundo_turno_entrega, 
	 @sedePrincipalPrev = sede_principal, @telefonoContacto1Prev = telefono_contacto1, @emailContacto1Prev = email_contacto1,
	 @negociacionMultiregionalPrev = negociacion_multiregional, @usuarioSolicitanteCreditoPrev = usuario_solicitante_credito, @observacionHorarioEntregaPrev = observacion_horario_entrega, 
	 @idSubdistribuidorPrev = id_subdistribuidor, @idOrigenPrev = id_origen, @habilitadoNegociacionGrupalPrev = habilitado_negociacion_grupal 
from DELETED;


IF @codigo <> @codigoPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'codigo', @idRegistro, @codigo, @fechaInicioVigencia ;
END 


IF @codigoAlterno <> @codigoAlternoPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'codigo_alterno', @idRegistro, @codigoAlterno, @fechaInicioVigencia ;
END 


IF @razonSocial <> @razonSocialPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'razon_social', @idRegistro, @razonSocial, @fechaInicioVigencia ;
END 


IF @nombreComercial <> @nombreComercialPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'nombre_comercial', @idRegistro, @nombreComercial, @fechaInicioVigencia ;
END 


IF @contacto1 <> @contacto1Prev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'contacto1', @idRegistro, @contacto1, @fechaInicioVigencia ;
END 


IF @contacto2 <> @contacto2Prev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'contacto2', @idRegistro, @contacto2, @fechaInicioVigencia ;
END 



IF @ruc <> @rucPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'ruc', @idRegistro, @ruc, @fechaInicioVigencia ;
END


IF @idGrupo <> @idGrupoPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_grupo', @idRegistro, @idGrupo, @fechaInicioVigencia ;
END


IF @idCiudad <> @idCiudadPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_ciudad', @idRegistro, @idCiudad, @fechaInicioVigencia ;
END


IF @sede <> @sedePrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'sede', @idRegistro, @sede, @fechaInicioVigencia ;
END


IF @vendedor <> @vendedorPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'vendedor', @idRegistro, @vendedor, @fechaInicioVigencia ;
END


IF @domicilioLegal <> @domicilioLegalPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'domicilio_legal', @idRegistro, @domicilioLegal, @fechaInicioVigencia ;
END


IF @ubigeo <> @ubigeoPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'ubigeo', @idRegistro, @ubigeo, @fechaInicioVigencia ;
END


IF @distrito <> @distritoPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'distrito', @idRegistro, @distrito, @fechaInicioVigencia ;
END


IF @direccionDespacho <> @direccionDespachoPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'direccion_despacho', @idRegistro, @direccionDespacho, @fechaInicioVigencia ;
END


IF @rubro <> @rubroPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'rubro', @idRegistro, @rubro, @fechaInicioVigencia ;
END


IF @emailFacturaElectronica <> @emailFacturaElectronicaPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'email_factura_electronica', @idRegistro, @emailFacturaElectronica, @fechaInicioVigencia ;
END


IF @estado <> @estadoPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'estado', @idRegistro, @estado, @fechaInicioVigencia ;
END


IF @razonSocialSunat <> @razonSocialSunatPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'razon_social_sunat', @idRegistro, @razonSocialSunat, @fechaInicioVigencia ;
END


IF @nombreComercialSunat <> @nombreComercialSunatPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'nombre_comercial_sunat', @idRegistro, @nombreComercialSunat, @fechaInicioVigencia ;
END


IF @direccionDomicilioLegalSunat <> @direccionDomicilioLegalSunatPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'direccion_domicilio_legal_sunat', @idRegistro, @direccionDomicilioLegalSunat, @fechaInicioVigencia ;
END


IF @estadoContribuyenteSunat <> @estadoContribuyenteSunatPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'estado_contribuyente_sunat', @idRegistro, @estadoContribuyenteSunat, @fechaInicioVigencia ;
END


IF @condicionContribuyenteSunat <> @condicionContribuyenteSunatPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'condicion_contribuyente_sunat', @idRegistro, @condicionContribuyenteSunat, @fechaInicioVigencia ;
END


IF @validado <> @validadoPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'validado', @idRegistro, @validado, @fechaInicioVigencia ;
END


IF @correoEnvioFactura <> @correoEnvioFacturaPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'correo_envio_factura', @idRegistro, @correoEnvioFactura, @fechaInicioVigencia ;
END


IF @plazoCredito <> @plazoCreditoPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'plazo_credito', @idRegistro, @plazoCredito, @fechaInicioVigencia ;
END


IF @tipoPagoFactura <> @tipoPagoFacturaPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'tipo_pago_factura', @idRegistro, @tipoPagoFactura, @fechaInicioVigencia ;
END


IF @formaPagoFactura <> @formaPagoFacturaPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'forma_pago_factura', @idRegistro, @formaPagoFactura, @fechaInicioVigencia ;
END


IF @tipoDocumento <> @tipoDocumentoPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'tipo_documento', @idRegistro, @tipoDocumento, @fechaInicioVigencia ;
END


IF @esProveedor <> @esProveedorPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'es_proveedor', @idRegistro, @esProveedor, @fechaInicioVigencia ;
END


IF @creditoSolicitado <> @creditoSolicitadoPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'credito_solicitado', @idRegistro, @creditoSolicitado, @fechaInicioVigencia ;
END


IF @creditoAprobado <> @creditoAprobadoPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'credito_aprobado', @idRegistro, @creditoAprobado, @fechaInicioVigencia ;
END


IF @idResponsableComercial <> @idResponsableComercialPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_responsable_comercial', @idRegistro, @idResponsableComercial, @fechaInicioVigencia ;
END


IF @idAsistenteServicioCliente <> @idAsistenteServicioClientePrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_asistente_servicio_cliente', @idRegistro, @idAsistenteServicioCliente, @fechaInicioVigencia ;
END


IF @idSupervisorComercial <> @idSupervisorComercialPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_supervisor_comercial', @idRegistro, @idSupervisorComercial, @fechaInicioVigencia ;
END


IF @sobreGiro <> @sobreGiroPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'sobre_giro', @idRegistro, @sobreGiro, @fechaInicioVigencia ;
END


IF @vendedoresAsignados <> @vendedoresAsignadosPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'vendedores_asignados', @idRegistro, @vendedoresAsignados, @fechaInicioVigencia ;
END


IF @plazoCreditoSolicitado <> @plazoCreditoSolicitadoPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'plazo_credito_solicitado', @idRegistro, @plazoCreditoSolicitado, @fechaInicioVigencia ;
END


IF @sobrePlazo <> @sobrePlazoPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'sobre_plazo', @idRegistro, @sobrePlazo, @fechaInicioVigencia ;
END


IF @observacionesCredito <> @observacionesCreditoPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'observaciones_credito', @idRegistro, @observacionesCredito, @fechaInicioVigencia ;
END


IF @observaciones <> @observacionesPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'observaciones', @idRegistro, @observaciones, @fechaInicioVigencia ;
END


IF @bloqueado <> @bloqueadoPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'bloqueado', @idRegistro, @bloqueado, @fechaInicioVigencia ;
END


IF @perteneceCanalMultiregional <> @perteneceCanalMultiregionalPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'pertenece_canal_multiregional', @idRegistro, @perteneceCanalMultiregional, @fechaInicioVigencia ;
END


IF @perteneceCanalLima <> @perteneceCanalLimaPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'pertenece_canal_lima', @idRegistro, @perteneceCanalLima, @fechaInicioVigencia ;
END


IF @perteneceCanalProvincia <> @perteneceCanalProvinciaPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'pertenece_canal_provincia', @idRegistro, @perteneceCanalProvincia, @fechaInicioVigencia ;
END


IF @perteneceCanalPcp <> @perteneceCanalPcpPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'pertenece_canal_pcp', @idRegistro, @perteneceCanalPcp, @fechaInicioVigencia ;
END


IF @esSubDistribuidor <> @esSubDistribuidorPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'es_sub_distribuidor', @idRegistro, @esSubDistribuidor, @fechaInicioVigencia ;
END


IF @horaInicioPrimerTurnoEntrega <> @horaInicioPrimerTurnoEntregaPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'hora_inicio_primer_turno_entrega', @idRegistro, @horaInicioPrimerTurnoEntrega, @fechaInicioVigencia ;
END


IF @horaFinPrimerTurnoEntrega <> @horaFinPrimerTurnoEntregaPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'hora_fin_primer_turno_entrega', @idRegistro, @horaFinPrimerTurnoEntrega, @fechaInicioVigencia ;
END


IF @horaInicioSegundoTurnoEntrega <> @horaInicioSegundoTurnoEntregaPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'hora_inicio_segundo_turno_entrega', @idRegistro, @horaInicioSegundoTurnoEntrega, @fechaInicioVigencia ;
END


IF @horaFinSegundoTurnoEntrega <> @horaFinSegundoTurnoEntregaPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'hora_fin_segundo_turno_entrega', @idRegistro, @horaFinSegundoTurnoEntrega, @fechaInicioVigencia ;
END


IF @sedePrincipal <> @sedePrincipalPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'sede_principal', @idRegistro, @sedePrincipal, @fechaInicioVigencia ;
END


IF @telefonoContacto1 <> @telefonoContacto1Prev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'telefono_contacto1', @idRegistro, @telefonoContacto1, @fechaInicioVigencia ;
END


IF @emailContacto1 <> @emailContacto1Prev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'email_contacto1', @idRegistro, @emailContacto1, @fechaInicioVigencia ;
END

IF @negociacionMultiregional <> @negociacionMultiregionalPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'negociacion_multiregional', @idRegistro, @negociacionMultiregional, @fechaInicioVigencia ;
END


IF @usuarioSolicitanteCredito <> @usuarioSolicitanteCreditoPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'usuario_solicitante_credito', @idRegistro, @usuarioSolicitanteCredito, @fechaInicioVigencia ;
END


IF @observacionHorarioEntrega <> @observacionHorarioEntregaPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'observacion_horario_entrega', @idRegistro, @observacionHorarioEntrega, @fechaInicioVigencia ;
END


IF @idSubdistribuidor <> @idSubdistribuidorPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_subdistribuidor', @idRegistro, @idSubdistribuidor, @fechaInicioVigencia ;
END


IF @idOrigen <> @idOrigenPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_origen', @idRegistro, @idOrigen, @fechaInicioVigencia ;
END


IF @habilitadoNegociacionGrupal <> @habilitadoNegociacionGrupalPrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'habilitado_negociacion_grupal', @idRegistro, @habilitadoNegociacionGrupal, @fechaInicioVigencia ;
END



END
GO






