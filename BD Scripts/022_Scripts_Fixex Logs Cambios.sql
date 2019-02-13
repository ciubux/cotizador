
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



IF @idGrupoCliente <> @idGrupoClientePrev 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_grupo_cliente', @idRegistro, @idGrupoCliente, @fechaInicioVigencia ;
END

END
GO






/* **** 10 **** */
ALTER TRIGGER ti_cliente ON CLIENTE
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
DECLARE @idGrupoCliente int;

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
	 @fechaInicioVigencia = fecha_inicio_vigencia , @idGrupoCliente = id_grupo_cliente
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
EXEC  pi_cambio_dato  @idUsuario, 'CLIENTE', 'id_grupo_cliente', @idRegistro, @idGrupoCliente, @fechaInicioVigencia ;


END
GO

