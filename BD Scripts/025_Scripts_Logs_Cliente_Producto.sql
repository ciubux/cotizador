/* **** 1 **** */
ALTER TABLE cambio ALTER COLUMN fecha_fin_vigencia DATE NULL;


ALTER TRIGGER tu_producto ON PRODUCTO
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


IF @sku <> @skuPrev OR (@sku IS NULL AND @skuPrev IS NOT NULL) OR (@skuPrev IS NULL AND @sku IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'sku', @idRegistro, @sku, @fechaInicioVigencia ;
END 


IF @descripcion <> @descripcionPrev OR (@descripcion IS NULL AND @descripcionPrev IS NOT NULL) OR (@descripcionPrev IS NULL AND @descripcion IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'descripcion', @idRegistro, @descripcion, @fechaInicioVigencia ;
END 


IF @fechaIngreso <> @fechaIngresoPrev OR (@fechaIngreso IS NULL AND @fechaIngresoPrev IS NOT NULL) OR (@fechaIngresoPrev IS NULL AND @fechaIngreso IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'fecha_ingreso', @idRegistro, @fechaIngreso, @fechaInicioVigencia ;
END 


IF @fechaFin <> @fechaFinPrev OR (@fechaFin IS NULL AND @fechaFinPrev IS NOT NULL) OR (@fechaFinPrev IS NULL AND @fechaFin IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'fecha_fin', @idRegistro, @fechaFin, @fechaInicioVigencia ;
END 


IF @skuProveedor <> @skuProveedorPrev OR (@skuProveedor IS NULL AND @skuProveedorPrev IS NOT NULL) OR (@skuProveedorPrev IS NULL AND @skuProveedor IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'sku_proveedor', @idRegistro, @skuProveedor, @fechaInicioVigencia ;
END 


IF @estado <> @estadoPrev OR (@estado IS NULL AND @estadoPrev IS NOT NULL) OR (@estadoPrev IS NULL AND @estado IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'estado', @idRegistro, @estado, @fechaInicioVigencia ;
END 


IF @precio <> @precioPrev OR (@precio IS NULL AND @precioPrev IS NOT NULL) OR (@precioPrev IS NULL AND @precio IS NOT NULL) 
BEGIN
	set @tempPrice = cast(@precio as varchar(20));
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'precio', @idRegistro,  @tempPrice, @fechaInicioVigencia ;
END 



IF @precioProvincia <> @precioProvinciaPrev OR (@precioProvincia IS NULL AND @precioProvinciaPrev IS NOT NULL) OR (@precioProvinciaPrev IS NULL AND @precioProvincia IS NOT NULL) 
BEGIN
	set @tempPrice = cast(@precioProvincia as varchar(20));
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'precio_provincia', @idRegistro, @tempPrice, @fechaInicioVigencia ;
END 


IF @costo <> @costoPrev OR (@costo IS NULL AND @costoPrev IS NOT NULL) OR (@costoPrev IS NULL AND @costo IS NOT NULL) 
BEGIN
	set @tempPrice = cast(@costo as varchar(20));
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'costo', @idRegistro, @tempPrice, @fechaInicioVigencia ;
END 


IF @familia <> @familiaPrev OR (@familia IS NULL AND @familiaPrev IS NOT NULL) OR (@familiaPrev IS NULL AND @familia IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'familia', @idRegistro, @familia, @fechaInicioVigencia ;
END 


IF @clase <> @clasePrev OR (@clase IS NULL AND @clasePrev IS NOT NULL) OR (@clasePrev IS NULL AND @clase IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'clase', @idRegistro, @clase, @fechaInicioVigencia ;
END 



IF @marca <> @marcaPrev OR (@marca IS NULL AND @marcaPrev IS NOT NULL) OR (@marcaPrev IS NULL AND @marca IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'marca', @idRegistro, @marca, @fechaInicioVigencia ;
END 



IF @proveedor <> @proveedorPrev OR (@proveedor IS NULL AND @proveedorPrev IS NOT NULL) OR (@proveedorPrev IS NULL AND @proveedor IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'proveedor', @idRegistro, @proveedor, @fechaInicioVigencia ;
END 



IF @unidad <> @unidadPrev OR (@unidad IS NULL AND @unidadPrev IS NOT NULL) OR (@unidadPrev IS NULL AND @unidad IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad', @idRegistro, @unidad, @fechaInicioVigencia ;
END 



IF @unidadAlternativa <> @unidadAlternativaPrev OR (@unidadAlternativa IS NULL AND @unidadAlternativaPrev IS NOT NULL) OR (@unidadAlternativaPrev IS NULL AND @unidadAlternativa IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad_alternativa', @idRegistro, @unidadAlternativa, @fechaInicioVigencia ;
END 



IF @equivalencia <> @equivalenciaPrev OR (@equivalencia IS NULL AND @equivalenciaPrev IS NOT NULL) OR (@equivalenciaPrev IS NULL AND @equivalencia IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'equivalencia', @idRegistro, @equivalencia, @fechaInicioVigencia ;
END 



IF @unidadProveedor <> @unidadProveedorPrev OR (@unidadProveedor IS NULL AND @unidadProveedorPrev IS NOT NULL) OR (@unidadProveedorPrev IS NULL AND @unidadProveedor IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad_proveedor', @idRegistro, @unidadProveedor, @fechaInicioVigencia ;
END 



IF @equivalenciaProveedor <> @equivalenciaProveedorPrev OR (@equivalenciaProveedor IS NULL AND @equivalenciaProveedorPrev IS NOT NULL) OR (@equivalenciaProveedorPrev IS NULL AND @equivalenciaProveedor IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'equivalencia_proveedor', @idRegistro, @equivalenciaProveedor, @fechaInicioVigencia ;
END 



IF @monedaCompra <> @monedaCompraPrev OR (@monedaCompra IS NULL AND @monedaCompraPrev IS NOT NULL) OR (@monedaCompraPrev IS NULL AND @monedaCompra IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'moneda_compra', @idRegistro, @monedaCompra, @fechaInicioVigencia ;
END 



IF @monedaVenta <> @monedaVentaPrev OR (@monedaVenta IS NULL AND @monedaVentaPrev IS NOT NULL) OR (@monedaVentaPrev IS NULL AND @monedaVenta IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'moneda_venta', @idRegistro, @monedaVenta, @fechaInicioVigencia ;
END 



IF @costoOriginal <> @costoOriginalPrev OR (@costoOriginal IS NULL AND @costoOriginalPrev IS NOT NULL) OR (@costoOriginalPrev IS NULL AND @costoOriginal IS NOT NULL) 
BEGIN
	set @tempPrice = cast(@costoOriginal as varchar(20));
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'costo_original', @idRegistro, @tempPrice, @fechaInicioVigencia ;
END 


IF @precioOriginal <> @precioOriginalPrev OR (@precioOriginal IS NULL AND @precioOriginalPrev IS NOT NULL) OR (@precioOriginalPrev IS NULL AND @precioOriginal IS NOT NULL) 
BEGIN
	set @tempPrice = cast(@precioOriginal as varchar(20));
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'precio_original', @idRegistro, @tempPrice, @fechaInicioVigencia ;
END 



IF @precioProvinciaOriginal <> @precioProvinciaOriginalPrev OR (@precioProvinciaOriginal IS NULL AND @precioProvinciaOriginalPrev IS NOT NULL) OR (@precioProvinciaOriginalPrev IS NULL AND @precioProvinciaOriginal IS NOT NULL) 
BEGIN
	set @tempPrice = cast(@precioProvinciaOriginal as varchar(20));
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'precio_provincia_original', @idRegistro, @tempPrice, @fechaInicioVigencia ;
END 



IF @unidadConteo <> @unidadConteoPrev OR (@unidadConteo IS NULL AND @unidadConteoPrev IS NOT NULL) OR (@unidadConteoPrev IS NULL AND @unidadConteo IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad_conteo', @idRegistro, @unidadConteo, @fechaInicioVigencia ;
END 



IF @unidadEstandarInternacional <> @unidadEstandarInternacionalPrev OR (@unidadEstandarInternacional IS NULL AND @unidadEstandarInternacionalPrev IS NOT NULL) OR (@unidadEstandarInternacionalPrev IS NULL AND @unidadEstandarInternacional IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad_estandar_internacional', @idRegistro, @unidadEstandarInternacional, @fechaInicioVigencia ;
END 



IF @unidadAlternativaInternacional <> @unidadAlternativaInternacionalPrev OR (@unidadAlternativaInternacional IS NULL AND @unidadAlternativaInternacionalPrev IS NOT NULL) OR (@unidadAlternativaInternacionalPrev IS NULL AND @unidadAlternativaInternacional IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad_alternativa_internacional', @idRegistro, @unidadAlternativaInternacional, @fechaInicioVigencia ;
END 



IF @equivalenciaUnidadConteoEstandar <> @equivalenciaUnidadConteoEstandarPrev OR (@equivalenciaUnidadConteoEstandar IS NULL AND @equivalenciaUnidadConteoEstandarPrev IS NOT NULL) OR (@equivalenciaUnidadConteoEstandarPrev IS NULL AND @equivalenciaUnidadConteoEstandar IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'equivalencia_unidad_conteo_estandar', @idRegistro, @equivalenciaUnidadConteoEstandar, @fechaInicioVigencia ;
END 



IF @equivalenciaUnidadConteoAlternativa <> @equivalenciaUnidadConteoAlternativaPrev OR (@equivalenciaUnidadConteoAlternativa IS NULL AND @equivalenciaUnidadConteoAlternativaPrev IS NOT NULL) OR (@equivalenciaUnidadConteoAlternativaPrev IS NULL AND @equivalenciaUnidadConteoAlternativa IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'equivalencia_unidad_conteo_alternativa', @idRegistro, @equivalenciaUnidadConteoAlternativa, @fechaInicioVigencia ;
END 



IF @exoneradoIgv <> @exoneradoIgvPrev OR (@exoneradoIgv IS NULL AND @exoneradoIgvPrev IS NOT NULL) OR (@exoneradoIgvPrev IS NULL AND @exoneradoIgv IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'exonerado_igv', @idRegistro, @exoneradoIgv, @fechaInicioVigencia ;
END 



IF @inafecto <> @inafectoPrev OR (@inafecto IS NULL AND @inafectoPrev IS NOT NULL) OR (@inafectoPrev IS NULL AND @inafecto IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'inafecto', @idRegistro, @inafecto, @fechaInicioVigencia ;
END 



IF @tipo <> @tipoPrev OR (@tipo IS NULL AND @tipoPrev IS NOT NULL) OR (@tipoPrev IS NULL AND @tipo IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'tipo', @idRegistro, @tipo, @fechaInicioVigencia ;
END 




END
GO





/* **** 2 **** */
ALTER TRIGGER tu_cliente ON CLIENTE
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
DECLARE @idGrupoCliente int;

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
DECLARE @idGrupoClientePrev int;


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
	 @fechaInicioVigencia = fecha_inicio_vigencia ,
	 @idGrupoCliente = id_grupo_cliente
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
	 @idSubdistribuidorPrev = id_subdistribuidor, @idOrigenPrev = id_origen, @habilitadoNegociacionGrupalPrev = habilitado_negociacion_grupal ,
	 @idGrupoClientePrev = id_grupo_cliente
from DELETED;


IF @codigo <> @codigoPrev OR (@codigo IS NULL AND @codigoPrev IS NOT NULL) OR (@codigoPrev IS NULL AND @codigo IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'codigo', @idRegistro, @codigo, @fechaInicioVigencia ;
END 


IF @codigoAlterno <> @codigoAlternoPrev OR (@codigoAlterno IS NULL AND @codigoAlternoPrev IS NOT NULL) OR (@codigoAlternoPrev IS NULL AND @codigoAlterno IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'codigo_alterno', @idRegistro, @codigoAlterno, @fechaInicioVigencia ;
END 


IF @razonSocial <> @razonSocialPrev OR (@razonSocial IS NULL AND @razonSocialPrev IS NOT NULL) OR (@razonSocialPrev IS NULL AND @razonSocial IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'razon_social', @idRegistro, @razonSocial, @fechaInicioVigencia ;
END 


IF @nombreComercial <> @nombreComercialPrev OR (@nombreComercial IS NULL AND @nombreComercialPrev IS NOT NULL) OR (@nombreComercialPrev IS NULL AND @nombreComercial IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'nombre_comercial', @idRegistro, @nombreComercial, @fechaInicioVigencia ;
END 


IF @contacto1 <> @contacto1Prev OR (@contacto1 IS NULL AND @contacto1Prev IS NOT NULL) OR (@contacto1Prev IS NULL AND @contacto1 IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'contacto1', @idRegistro, @contacto1, @fechaInicioVigencia ;
END 


IF @contacto2 <> @contacto2Prev OR (@contacto2 IS NULL AND @contacto2Prev IS NOT NULL) OR (@contacto2Prev IS NULL AND @contacto2 IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'contacto2', @idRegistro, @contacto2, @fechaInicioVigencia ;
END 



IF @ruc <> @rucPrev OR (@ruc IS NULL AND @rucPrev IS NOT NULL) OR (@rucPrev IS NULL AND @ruc IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'ruc', @idRegistro, @ruc, @fechaInicioVigencia ;
END


IF @idGrupo <> @idGrupoPrev OR (@idGrupo IS NULL AND @idGrupoPrev IS NOT NULL) OR (@idGrupoPrev IS NULL AND @idGrupo IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_grupo', @idRegistro, @idGrupo, @fechaInicioVigencia ;
END


IF @idCiudad <> @idCiudadPrev OR (@idCiudad IS NULL AND @idCiudadPrev IS NOT NULL) OR (@idCiudadPrev IS NULL AND @idCiudad IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_ciudad', @idRegistro, @idCiudad, @fechaInicioVigencia ;
END


IF @sede <> @sedePrev OR (@sede IS NULL AND @sedePrev IS NOT NULL) OR (@sedePrev IS NULL AND @sede IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'sede', @idRegistro, @sede, @fechaInicioVigencia ;
END


IF @vendedor <> @vendedorPrev OR (@vendedor IS NULL AND @vendedorPrev IS NOT NULL) OR (@vendedorPrev IS NULL AND @vendedor IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'vendedor', @idRegistro, @vendedor, @fechaInicioVigencia ;
END


IF @domicilioLegal <> @domicilioLegalPrev OR (@domicilioLegal IS NULL AND @domicilioLegalPrev IS NOT NULL) OR (@domicilioLegalPrev IS NULL AND @domicilioLegal IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'domicilio_legal', @idRegistro, @domicilioLegal, @fechaInicioVigencia ;
END


IF @ubigeo <> @ubigeoPrev OR (@ubigeo IS NULL AND @ubigeoPrev IS NOT NULL) OR (@ubigeoPrev IS NULL AND @ubigeo IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'ubigeo', @idRegistro, @ubigeo, @fechaInicioVigencia ;
END


IF @distrito <> @distritoPrev OR (@distrito IS NULL AND @distritoPrev IS NOT NULL) OR (@distritoPrev IS NULL AND @distrito IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'distrito', @idRegistro, @distrito, @fechaInicioVigencia ;
END


IF @direccionDespacho <> @direccionDespachoPrev OR (@direccionDespacho IS NULL AND @direccionDespachoPrev IS NOT NULL) OR (@direccionDespachoPrev IS NULL AND @direccionDespacho IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'direccion_despacho', @idRegistro, @direccionDespacho, @fechaInicioVigencia ;
END


IF @rubro <> @rubroPrev OR (@rubro IS NULL AND @rubroPrev IS NOT NULL) OR (@rubroPrev IS NULL AND @rubro IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'rubro', @idRegistro, @rubro, @fechaInicioVigencia ;
END


IF @emailFacturaElectronica <> @emailFacturaElectronicaPrev OR (@emailFacturaElectronica IS NULL AND @emailFacturaElectronicaPrev IS NOT NULL) OR (@emailFacturaElectronicaPrev IS NULL AND @emailFacturaElectronica IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'email_factura_electronica', @idRegistro, @emailFacturaElectronica, @fechaInicioVigencia ;
END


IF @estado <> @estadoPrev OR (@estado IS NULL AND @estadoPrev IS NOT NULL) OR (@estadoPrev IS NULL AND @estado IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'estado', @idRegistro, @estado, @fechaInicioVigencia ;
END


IF @razonSocialSunat <> @razonSocialSunatPrev OR (@razonSocialSunat IS NULL AND @razonSocialSunatPrev IS NOT NULL) OR (@razonSocialSunatPrev IS NULL AND @razonSocialSunat IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'razon_social_sunat', @idRegistro, @razonSocialSunat, @fechaInicioVigencia ;
END


IF @nombreComercialSunat <> @nombreComercialSunatPrev OR (@nombreComercialSunat IS NULL AND @nombreComercialSunatPrev IS NOT NULL) OR (@nombreComercialSunatPrev IS NULL AND @nombreComercialSunat IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'nombre_comercial_sunat', @idRegistro, @nombreComercialSunat, @fechaInicioVigencia ;
END


IF @direccionDomicilioLegalSunat <> @direccionDomicilioLegalSunatPrev OR (@direccionDomicilioLegalSunat IS NULL AND @direccionDomicilioLegalSunatPrev IS NOT NULL) OR (@direccionDomicilioLegalSunatPrev IS NULL AND @direccionDomicilioLegalSunat IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'direccion_domicilio_legal_sunat', @idRegistro, @direccionDomicilioLegalSunat, @fechaInicioVigencia ;
END


IF @estadoContribuyenteSunat <> @estadoContribuyenteSunatPrev OR (@estadoContribuyenteSunat IS NULL AND @estadoContribuyenteSunatPrev IS NOT NULL) OR (@estadoContribuyenteSunatPrev IS NULL AND @estadoContribuyenteSunat IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'estado_contribuyente_sunat', @idRegistro, @estadoContribuyenteSunat, @fechaInicioVigencia ;
END


IF @condicionContribuyenteSunat <> @condicionContribuyenteSunatPrev OR (@condicionContribuyenteSunat IS NULL AND @condicionContribuyenteSunatPrev IS NOT NULL) OR (@condicionContribuyenteSunatPrev IS NULL AND @condicionContribuyenteSunat IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'condicion_contribuyente_sunat', @idRegistro, @condicionContribuyenteSunat, @fechaInicioVigencia ;
END


IF @validado <> @validadoPrev OR (@validado IS NULL AND @validadoPrev IS NOT NULL) OR (@validadoPrev IS NULL AND @validado IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'validado', @idRegistro, @validado, @fechaInicioVigencia ;
END


IF @correoEnvioFactura <> @correoEnvioFacturaPrev OR (@correoEnvioFactura IS NULL AND @correoEnvioFacturaPrev IS NOT NULL) OR (@correoEnvioFacturaPrev IS NULL AND @correoEnvioFactura IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'correo_envio_factura', @idRegistro, @correoEnvioFactura, @fechaInicioVigencia ;
END


IF @plazoCredito <> @plazoCreditoPrev OR (@plazoCredito IS NULL AND @plazoCreditoPrev IS NOT NULL) OR (@plazoCreditoPrev IS NULL AND @plazoCredito IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'plazo_credito', @idRegistro, @plazoCredito, @fechaInicioVigencia ;
END


IF @tipoPagoFactura <> @tipoPagoFacturaPrev OR (@tipoPagoFactura IS NULL AND @tipoPagoFacturaPrev IS NOT NULL) OR (@tipoPagoFacturaPrev IS NULL AND @tipoPagoFactura IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'tipo_pago_factura', @idRegistro, @tipoPagoFactura, @fechaInicioVigencia ;
END


IF @formaPagoFactura <> @formaPagoFacturaPrev OR (@formaPagoFactura IS NULL AND @formaPagoFacturaPrev IS NOT NULL) OR (@formaPagoFacturaPrev IS NULL AND @formaPagoFactura IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'forma_pago_factura', @idRegistro, @formaPagoFactura, @fechaInicioVigencia ;
END


IF @tipoDocumento <> @tipoDocumentoPrev OR (@tipoDocumento IS NULL AND @tipoDocumentoPrev IS NOT NULL) OR (@tipoDocumentoPrev IS NULL AND @tipoDocumento IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'tipo_documento', @idRegistro, @tipoDocumento, @fechaInicioVigencia ;
END


IF @esProveedor <> @esProveedorPrev OR (@esProveedor IS NULL AND @esProveedorPrev IS NOT NULL) OR (@esProveedorPrev IS NULL AND @esProveedor IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'es_proveedor', @idRegistro, @esProveedor, @fechaInicioVigencia ;
END


IF @creditoSolicitado <> @creditoSolicitadoPrev OR (@creditoSolicitado IS NULL AND @creditoSolicitadoPrev IS NOT NULL) OR (@creditoSolicitadoPrev IS NULL AND @creditoSolicitado IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'credito_solicitado', @idRegistro, @creditoSolicitado, @fechaInicioVigencia ;
END


IF @creditoAprobado <> @creditoAprobadoPrev OR (@creditoAprobado IS NULL AND @creditoAprobadoPrev IS NOT NULL) OR (@creditoAprobadoPrev IS NULL AND @creditoAprobado IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'credito_aprobado', @idRegistro, @creditoAprobado, @fechaInicioVigencia ;
END


IF @idResponsableComercial <> @idResponsableComercialPrev OR (@idResponsableComercial IS NULL AND @idResponsableComercialPrev IS NOT NULL) OR (@idResponsableComercialPrev IS NULL AND @idResponsableComercial IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_responsable_comercial', @idRegistro, @idResponsableComercial, @fechaInicioVigencia ;
END


IF @idAsistenteServicioCliente <> @idAsistenteServicioClientePrev OR (@idAsistenteServicioCliente IS NULL AND @idAsistenteServicioClientePrev IS NOT NULL) OR (@idAsistenteServicioClientePrev IS NULL AND @idAsistenteServicioCliente IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_asistente_servicio_cliente', @idRegistro, @idAsistenteServicioCliente, @fechaInicioVigencia ;
END


IF @idSupervisorComercial <> @idSupervisorComercialPrev OR (@idSupervisorComercial IS NULL AND @idSupervisorComercialPrev IS NOT NULL) OR (@idSupervisorComercialPrev IS NULL AND @idSupervisorComercial IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_supervisor_comercial', @idRegistro, @idSupervisorComercial, @fechaInicioVigencia ;
END


IF @sobreGiro <> @sobreGiroPrev OR (@sobreGiro IS NULL AND @sobreGiroPrev IS NOT NULL) OR (@sobreGiroPrev IS NULL AND @sobreGiro IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'sobre_giro', @idRegistro, @sobreGiro, @fechaInicioVigencia ;
END


IF @vendedoresAsignados <> @vendedoresAsignadosPrev OR (@vendedoresAsignados IS NULL AND @vendedoresAsignadosPrev IS NOT NULL) OR (@vendedoresAsignadosPrev IS NULL AND @vendedoresAsignados IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'vendedores_asignados', @idRegistro, @vendedoresAsignados, @fechaInicioVigencia ;
END


IF @plazoCreditoSolicitado <> @plazoCreditoSolicitadoPrev OR (@plazoCreditoSolicitado IS NULL AND @plazoCreditoSolicitadoPrev IS NOT NULL) OR (@plazoCreditoSolicitadoPrev IS NULL AND @plazoCreditoSolicitado IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'plazo_credito_solicitado', @idRegistro, @plazoCreditoSolicitado, @fechaInicioVigencia ;
END


IF @sobrePlazo <> @sobrePlazoPrev OR (@sobrePlazo IS NULL AND @sobrePlazoPrev IS NOT NULL) OR (@sobrePlazoPrev IS NULL AND @sobrePlazo IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'sobre_plazo', @idRegistro, @sobrePlazo, @fechaInicioVigencia ;
END


IF @observacionesCredito <> @observacionesCreditoPrev OR (@observacionesCredito IS NULL AND @observacionesCreditoPrev IS NOT NULL) OR (@observacionesCreditoPrev IS NULL AND @observacionesCredito IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'observaciones_credito', @idRegistro, @observacionesCredito, @fechaInicioVigencia ;
END


IF @observaciones <> @observacionesPrev OR (@observaciones IS NULL AND @observacionesPrev IS NOT NULL) OR (@observacionesPrev IS NULL AND @observaciones IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'observaciones', @idRegistro, @observaciones, @fechaInicioVigencia ;
END


IF @bloqueado <> @bloqueadoPrev OR (@bloqueado IS NULL AND @bloqueadoPrev IS NOT NULL) OR (@bloqueadoPrev IS NULL AND @bloqueado IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'bloqueado', @idRegistro, @bloqueado, @fechaInicioVigencia ;
END


IF @perteneceCanalMultiregional <> @perteneceCanalMultiregionalPrev OR (@perteneceCanalMultiregional IS NULL AND @perteneceCanalMultiregionalPrev IS NOT NULL) OR (@perteneceCanalMultiregionalPrev IS NULL AND @perteneceCanalMultiregional IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'pertenece_canal_multiregional', @idRegistro, @perteneceCanalMultiregional, @fechaInicioVigencia ;
END


IF @perteneceCanalLima <> @perteneceCanalLimaPrev OR (@perteneceCanalLima IS NULL AND @perteneceCanalLimaPrev IS NOT NULL) OR (@perteneceCanalLimaPrev IS NULL AND @perteneceCanalLima IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'pertenece_canal_lima', @idRegistro, @perteneceCanalLima, @fechaInicioVigencia ;
END


IF @perteneceCanalProvincia <> @perteneceCanalProvinciaPrev OR (@perteneceCanalProvincia IS NULL AND @perteneceCanalProvinciaPrev IS NOT NULL) OR (@perteneceCanalProvinciaPrev IS NULL AND @perteneceCanalProvincia IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'pertenece_canal_provincia', @idRegistro, @perteneceCanalProvincia, @fechaInicioVigencia ;
END


IF @perteneceCanalPcp <> @perteneceCanalPcpPrev OR (@perteneceCanalPcp IS NULL AND @perteneceCanalPcpPrev IS NOT NULL) OR (@perteneceCanalPcpPrev IS NULL AND @perteneceCanalPcp IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'pertenece_canal_pcp', @idRegistro, @perteneceCanalPcp, @fechaInicioVigencia ;
END


IF @esSubDistribuidor <> @esSubDistribuidorPrev OR (@esSubDistribuidor IS NULL AND @esSubDistribuidorPrev IS NOT NULL) OR (@esSubDistribuidorPrev IS NULL AND @esSubDistribuidor IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'es_sub_distribuidor', @idRegistro, @esSubDistribuidor, @fechaInicioVigencia ;
END


IF @horaInicioPrimerTurnoEntrega <> @horaInicioPrimerTurnoEntregaPrev OR (@horaInicioPrimerTurnoEntrega IS NULL AND @horaInicioPrimerTurnoEntregaPrev IS NOT NULL) OR (@horaInicioPrimerTurnoEntregaPrev IS NULL AND @horaInicioPrimerTurnoEntrega IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'hora_inicio_primer_turno_entrega', @idRegistro, @horaInicioPrimerTurnoEntrega, @fechaInicioVigencia ;
END


IF @horaFinPrimerTurnoEntrega <> @horaFinPrimerTurnoEntregaPrev OR (@horaFinPrimerTurnoEntrega IS NULL AND @horaFinPrimerTurnoEntregaPrev IS NOT NULL) OR (@horaFinPrimerTurnoEntregaPrev IS NULL AND @horaFinPrimerTurnoEntrega IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'hora_fin_primer_turno_entrega', @idRegistro, @horaFinPrimerTurnoEntrega, @fechaInicioVigencia ;
END


IF @horaInicioSegundoTurnoEntrega <> @horaInicioSegundoTurnoEntregaPrev OR (@horaInicioSegundoTurnoEntrega IS NULL AND @horaInicioSegundoTurnoEntregaPrev IS NOT NULL) OR (@horaInicioSegundoTurnoEntregaPrev IS NULL AND @horaInicioSegundoTurnoEntrega IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'hora_inicio_segundo_turno_entrega', @idRegistro, @horaInicioSegundoTurnoEntrega, @fechaInicioVigencia ;
END


IF @horaFinSegundoTurnoEntrega <> @horaFinSegundoTurnoEntregaPrev OR (@horaFinSegundoTurnoEntrega IS NULL AND @horaFinSegundoTurnoEntregaPrev IS NOT NULL) OR (@horaFinSegundoTurnoEntregaPrev IS NULL AND @horaFinSegundoTurnoEntrega IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'hora_fin_segundo_turno_entrega', @idRegistro, @horaFinSegundoTurnoEntrega, @fechaInicioVigencia ;
END


IF @sedePrincipal <> @sedePrincipalPrev OR (@sedePrincipal IS NULL AND @sedePrincipalPrev IS NOT NULL) OR (@sedePrincipalPrev IS NULL AND @sedePrincipal IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'sede_principal', @idRegistro, @sedePrincipal, @fechaInicioVigencia ;
END


IF @telefonoContacto1 <> @telefonoContacto1Prev OR (@telefonoContacto1 IS NULL AND @telefonoContacto1Prev IS NOT NULL) OR (@telefonoContacto1Prev IS NULL AND @telefonoContacto1 IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'telefono_contacto1', @idRegistro, @telefonoContacto1, @fechaInicioVigencia ;
END


IF @emailContacto1 <> @emailContacto1Prev OR (@emailContacto1 IS NULL AND @emailContacto1Prev IS NOT NULL) OR (@emailContacto1Prev IS NULL AND @emailContacto1 IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'email_contacto1', @idRegistro, @emailContacto1, @fechaInicioVigencia ;
END

IF @negociacionMultiregional <> @negociacionMultiregionalPrev OR (@negociacionMultiregional IS NULL AND @negociacionMultiregionalPrev IS NOT NULL) OR (@negociacionMultiregionalPrev IS NULL AND @negociacionMultiregional IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'negociacion_multiregional', @idRegistro, @negociacionMultiregional, @fechaInicioVigencia ;
END


IF @usuarioSolicitanteCredito <> @usuarioSolicitanteCreditoPrev OR (@usuarioSolicitanteCredito IS NULL AND @usuarioSolicitanteCreditoPrev IS NOT NULL) OR (@usuarioSolicitanteCreditoPrev IS NULL AND @usuarioSolicitanteCredito IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'usuario_solicitante_credito', @idRegistro, @usuarioSolicitanteCredito, @fechaInicioVigencia ;
END


IF @observacionHorarioEntrega <> @observacionHorarioEntregaPrev OR (@observacionHorarioEntrega IS NULL AND @observacionHorarioEntregaPrev IS NOT NULL) OR (@observacionHorarioEntregaPrev IS NULL AND @observacionHorarioEntrega IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'observacion_horario_entrega', @idRegistro, @observacionHorarioEntrega, @fechaInicioVigencia ;
END


IF @idSubdistribuidor <> @idSubdistribuidorPrev OR (@idSubdistribuidor IS NULL AND @idSubdistribuidorPrev IS NOT NULL) OR (@idSubdistribuidorPrev IS NULL AND @idSubdistribuidor IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_subdistribuidor', @idRegistro, @idSubdistribuidor, @fechaInicioVigencia ;
END


IF @idOrigen <> @idOrigenPrev OR (@idOrigen IS NULL AND @idOrigenPrev IS NOT NULL) OR (@idOrigenPrev IS NULL AND @idOrigen IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_origen', @idRegistro, @idOrigen, @fechaInicioVigencia ;
END


IF @habilitadoNegociacionGrupal <> @habilitadoNegociacionGrupalPrev OR (@habilitadoNegociacionGrupal IS NULL AND @habilitadoNegociacionGrupalPrev IS NOT NULL) OR (@habilitadoNegociacionGrupalPrev IS NULL AND @habilitadoNegociacionGrupal IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'habilitado_negociacion_grupal', @idRegistro, @habilitadoNegociacionGrupal, @fechaInicioVigencia ;
END



IF @idGrupoCliente <> @idGrupoClientePrev OR (@idGrupoCliente IS NULL AND @idGrupoClientePrev IS NOT NULL) OR (@idGrupoClientePrev IS NULL AND @idGrupoCliente IS NOT NULL) 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_grupo_cliente', @idRegistro, @idGrupoCliente, @fechaInicioVigencia ;
END

END
GO








