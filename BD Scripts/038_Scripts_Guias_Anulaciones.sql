/* **** 1 **** */
ALTER TABLE MOVIMIENTO_ALMACEN 
add anulacion_solicitada int null;


/* **** 2 **** */
ALTER TABLE MOVIMIENTO_ALMACEN 
add usuario_solicita_anulacion uniqueidentifier null;


/* **** 3 **** */
ALTER TABLE MOVIMIENTO_ALMACEN 
add usuario_anulacion uniqueidentifier null;


/* **** 4 **** */
ALTER TABLE MOVIMIENTO_ALMACEN 
add anulacion_aprobada int null;


/* **** 5 **** */
ALTER TABLE MOVIMIENTO_ALMACEN 
add usuario_aprueba_anulacion uniqueidentifier null;



/* **** 6 **** */
ALTER TABLE MOVIMIENTO_ALMACEN 
add comentario_solicitud_anulacion varchar(500) null;


/* **** 7 **** */
ALTER PROCEDURE [dbo].[pu_jobDiario] 
AS
BEGIN

UPDATE CPE_CABECERA_BE 
SET permite_anulacion = 0
where CAST(FEC_EMI AS DATE) <DATEADD(day, -5, [dbo].[getlocaldate]()) and permite_anulacion = 1;

UPDATE MOVIMIENTO_ALMACEN 
SET permite_anulacion = 0
where CAST(fecha_emision AS DATE) <DATEADD(day, -4, [dbo].[getlocaldate]()) and permite_anulacion = 1;

END





/* **** 8 **** */

ALTER PROCEDURE [dbo].[ps_guiaRemision] 
@idMovimientoAlmacen uniqueIdentifier
AS
BEGIN

	SELECT 
	--GUIA_REMISION
	ma.serie_documento, ma.numero_documento,ma.id_movimiento_almacen,
	ma.fecha_emision, ma.fecha_traslado,
	ma.atencion_parcial, ma.ultima_atencion_parcial, ma.id_sede_origen, 
	ma.observaciones, ma.anulado, ma.motivo_traslado,  ma.facturado,
	ma.tipo_extorno,
	--PEDIDO
	pe.id_pedido, pe.numero, pe.numero_referencia_cliente, 
	CONCAT(ma.direccion_entrega,' ',ub.departamento,' - ',ub.provincia,' - ', ub.distrito)  
	 as direccion_entrega,

	--pe.direccion_entrega,
	--CLIENTE
	cl.codigo,  
	cl.id_cliente,



	ma.cliente_razon_social as razon_social,

	ma.cliente_numero_documento as ruc, 

	ma.cliente_domicilio_legal as domicilio_legal,

	ma.permite_anulacion, ma.anulacion_aprobada, ma.anulacion_solicitada,
	ma.comentario_solicitud_anulacion, 
	ma.usuario_solicita_anulacion, usa.nombre,
    ma.usuario_aprueba_anulacion, uaa.nombre,


	--USUARIO
	us.nombre as nombre_usuario, us.id_usuario,
	--CIUDAD
	ci.id_ciudad, ci.nombre as nombre_ciudad, 
	ci.direccion_punto_partida, ci.es_provincia, ci.codigo_sede as sede,
	--TRANSPORTISTA
	ma.nombre_transportista,
	ma.brevete_transportista, ma.ruc_transportista,
	  ma.direccion_transportista,
	ma.placa_vehiculo,ma.certificado_inscripcion, 
	--UBIGEO
	ma.ubigeo_entrega, ub.departamento, ub.provincia, ub.distrito,


	--SEGUIMIENTO
	/*
	sc.estado_movimiento_almacen as estado_seguimiento,
	us2.nombre as usuario_seguimiento, sc.observacion as observacion_seguimiento,
	us2.id_usuario as id_usuario_seguimiento	,*/

	---EXTORNO
	ma.motivo_extorno,
	ma.sustento_extorno,
	ma.id_movimiento_almacen_extornado,
	mae.serie_documento as movimiento_almacen_extorno_serie_documento,
	mae.numero_documento as movimiento_almacen_extorno_numero_documento,

	ma.ingresado
	FROM MOVIMIENTO_ALMACEN as ma
	INNER JOIN CIUDAD AS ci ON ma.id_sede_origen = ci.id_ciudad
	INNER JOIN USUARIO AS us on ma.usuario_creacion = us.id_usuario
	/*INNER JOIN SEGUIMIENTO_MOVIMIENTO_ALMACEN sc on sc.id_movimiento_almacen = ma.id_movimiento_almacen
	INNER JOIN USUARIO AS us2 on sc.id_usuario = us2.id_usuario*/
	INNER JOIN CLIENTE AS cl ON cl.id_cliente = ma.id_cliente
	INNER JOIN UBIGEO ub ON CAST(ma.ubigeo_entrega AS CHAR(6)) = ub.codigo
	LEFT JOIN PEDIDO AS pe ON ma.id_pedido = pe.id_pedido
	LEFT JOIN MOVIMIENTO_ALMACEN as mae ON ma.id_movimiento_almacen_extornado = mae.id_movimiento_almacen
	LEFT JOIN USUARIO AS usa on ma.usuario_solicita_anulacion = usa.id_usuario 
	LEFT JOIN USUARIO AS uaa on ma.usuario_aprueba_anulacion = uaa.id_usuario 

	where   
	ma.id_movimiento_almacen = @idMovimientoAlmacen
	--order by ma.numero_documento asc ;

	SELECT  mad.id_movimiento_almacen_detalle, mad.cantidad, 
	mad.unidad, pr.id_producto, pr.sku, 
	CONCAT(ISNULL(mad.descripcion_adicional,''),
	pr.descripcion) descripcion, pr.sku_proveedor
	FROM MOVIMIENTO_ALMACEN_DETALLE as mad 
	INNER JOIN MOVIMIENTO_ALMACEN as ma ON mad.id_movimiento_almacen = ma.id_movimiento_almacen
	INNER JOIN PRODUCTO pr ON mad.id_producto = pr.id_producto
	WHERE mad.id_movimiento_almacen = @idMovimientoAlmacen
	--AND mad.estado = 1
	ORDER BY mad.fecha_creacion asc;
	END



	SELECT dv.serie, dv.correlativo, CAST(dv.fec_emi AS DateTime) as fecha_emision 
	FROM CPE_CABECERA_BE dv
	INNER JOIN VENTA ve ON ve.id_documento_venta = dv.id_cpe_cabecera_be
	INNER JOIN MOVIMIENTO_ALMACEN ma ON ma.id_movimiento_almacen = ve.id_movimiento_almacen
	WHERE 
	ma.id_movimiento_almacen = @idMovimientoAlmacen
	AND ve.estado = 1;
GO






/* **** 10 **** */
CREATE procedure pu_solicita_anulacion_guia_remision
@idMovimientoAlmacen uniqueIdentifier,
@idUsuario uniqueIdentifier,
@comentario varchar(500)

AS
BEGIN
	update MOVIMIENTO_ALMACEN
	set anulacion_solicitada = 1, comentario_solicitud_anulacion = @comentario, usuario_solicita_anulacion = @idUsuario
	where id_movimiento_almacen = @idMovimientoAlmacen and (anulacion_solicitada is null or anulacion_solicitada = 0);
END





/* **** 11 **** */
CREATE procedure pu_aprueba_anulacion_guia_remision
@idMovimientoAlmacen uniqueIdentifier,
@idUsuario uniqueIdentifier

AS
BEGIN
	update MOVIMIENTO_ALMACEN
	set anulacion_aprobada = 1, usuario_aprueba_anulacion = @idUsuario
	where id_movimiento_almacen = @idMovimientoAlmacen and anulacion_solicitada = 1;
END

























