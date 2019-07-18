ALTER TABLE USUARIO  
ADD modifica_pedido_fecha_entrega_extendida SMALLINT;

ALTER TABLE PEDIDO 
ADD fecha_entrega_extendida DATE;

INSERT INTO PARAMETRO VALUES (newid(), 'FECHA_INICIO_BUSQUEDA_PEDIDOS_PENDIENTES', 'Fecha que sirve como límite inferior para la búsqueda de pedidos pendientes de atención', '2018-11-01',1, null, GETDATE(), null, GETDATE())

CREATE PROCEDURE [dbo].[ps_pedidos_sin_atencion] 

AS
BEGIN

SELECT pe.id_pedido

FROM PEDIDO_DETALLE as pd 
INNER JOIN PEDIDO as pe ON pd.id_pedido = pe.id_pedido 
INNER JOIN SEGUIMIENTO_PEDIDO sp ON pe.id_pedido = sp.id_pedido AND sp.estado_pedido IN (1,3,5,6)
LEFT JOIN 
(
SELECT mad.id_pedido_detalle, SUM(cantidad) as cantidadAtendida  FROM
MOVIMIENTO_ALMACEN_DETALLE AS mad 
INNER JOIN MOVIMIENTO_ALMACEN ma ON mad.id_movimiento_almacen = ma.id_movimiento_almacen
WHERE mad.estado = 1 AND  ma.estado = 1 AND ma.anulado = 0
GROUP BY mad.id_pedido_detalle
) AS  mad ON mad.id_pedido_detalle  = pd.id_pedido_detalle
	
	
where pe.estado = 1 and pd.estado = 1 and sp.estado = 1 and 
	  ((pe.fecha_entrega_extendida IS NULL AND pe.fecha_entrega_hasta <= GETDATE()) OR (pe.fecha_entrega_extendida <= GETDATE()))
	  and (pd.cantidad - COALESCE(mad.cantidadAtendida,0)) > 0 
group by pe.id_pedido
order by pe.id_pedido;


pe.fecha_entrega_hasta >= Convert(datetime, (SELECT valor FROM PARAMETRO WHERE codigo = 'FECHA_INICIO_BUSQUEDA_PEDIDOS_PENDIENTES') )



END





/* Consulta la información del pedido para enviarla por email */

create PROCEDURE [dbo].[ps_pedido_email] 
@idPedido uniqueIdentifier
AS
BEGIN

SELECT 
--PEDIDO
pe.numero, pe.numero_grupo, pe.fecha_solicitud, 
pe.fecha_entrega_desde, pe.fecha_entrega_hasta, pe.fecha_entrega_extendida,
pe.hora_entrega_desde, pe.hora_entrega_hasta,
pe.numero_referencia_cliente, pe.id_direccion_entrega, pe.direccion_entrega, pe.contacto_entrega,
pe.telefono_contacto_entrega, 
pe.contacto_pedido,pe.telefono_contacto_pedido, pe.correo_contacto_pedido,
pe.fecha_creacion as fecha_registro,
pe.tipo,
--UBIGEO
pe.ubigeo_entrega, ub.departamento, ub.provincia, ub.distrito,
---CLIENTE
cl.id_cliente, cl.codigo, cl.razon_social, cl.ruc, cic.id_ciudad as id_ciudad_cliente, cic.nombre as nombre_ciudad_cliente,

--CIUDAD
ci.id_ciudad, ci.nombre as nombre_ciudad ,

--USUARIO
us.nombre  as nombre_usuario, us.cargo, us.contacto as contacto_usuario, us.email,

--SEGUIMIENTO
sp.estado_pedido as estado_seguimiento,
us2.nombre as usuario_seguimiento, sp.observacion as observacion_seguimiento,
us2.id_usuario as id_usuario_seguimiento,


--VENDEDORES,
verc.codigo as responsable_comercial_codigo,
verc.descripcion as responsable_comercial_descripcion,
ISNULL(uverc.email,'') as responsable_comercial_email,

veasc.codigo as asistente_servicio_cliente_codigo,
veasc.descripcion as asistente_servicio_cliente_descripcion,
ISNULL(uveasc.email,'') as asistente_servicio_cliente_email

FROM PEDIDO as pe
INNER JOIN CIUDAD AS ci ON pe.id_ciudad = ci.id_ciudad
INNER JOIN USUARIO AS us on pe.usuario_creacion = us.id_usuario
INNER JOIN SEGUIMIENTO_PEDIDO sp ON pe.id_pedido = sp.id_pedido
INNER JOIN USUARIO AS us2 on sp.id_usuario = us2.id_usuario
INNER JOIN CLIENTE AS cl ON pe.id_cliente = cl.id_cliente
INNER JOIN CIUDAD AS cic ON cl.id_ciudad = cic.id_ciudad
LEFT JOIN UBIGEO ub ON pe.ubigeo_entrega = ub.codigo
LEFT JOIN SOLICITANTE so ON so.id_solicitante = pe.id_solicitante
LEFT JOIN VENDEDOR AS verc ON cl.id_responsable_comercial = verc.id_vendedor
LEFT JOIN VENDEDOR AS veasc ON cl.id_asistente_servicio_cliente = veasc.id_vendedor
LEFT JOIN USUARIO AS uverc ON uverc.id_usuario = verc.id_usuario
LEFT JOIN USUARIO AS uveasc ON uveasc.id_usuario = veasc.id_usuario
--LEFT JOIN GRUPO AS gr ON pe.id_grupo = gr.id_grupo  
where pe.id_pedido = @idPedido and 
pe.estado = 1
AND sp.estado = 1;



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


	where pd.id_pedido = @idPedido and pd.estado = 1 and (pd.cantidad - COALESCE(mad.cantidadAtendida,0)) > 0
	
	) SQuery 
		where RowNumber = 1
	ORDER BY fecha_modificacion ASC;


--RECUPERA LAS DIRECCIONES DE ENTREGA

SELECT TOP 0 de.id_direccion_entrega, de.descripcion, de.contacto, de.telefono, ubigeo
FROM DIRECCION_ENTREGA de
INNER JOIN PEDIDO pe ON de.id_cliente = pe.id_cliente
where pe.id_pedido = @idPedido
AND de.estado = 1;


END


/* Obtener modifica_pedido_fecha_entrega_extendida */

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
crea_documentos_venta, administra_documentos_venta_lima, administra_documentos_venta_provincias,
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

bloquea_clientes,
modifica_canales,
modifica_pedido_fecha_entrega_extendida




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



/* Actualizar fecha_entrega_extendida */
ALTER PROCEDURE [dbo].[pu_actualizarPedido] 
@idPedido uniqueidentifier,
@numeroReferenciaCliente varchar(20),
@numeroReferenciaAdicional varchar(100),
@fechaEntregaExtendida datetime,
@observaciones varchar(500),
@observacionesGuiaRemision varchar(200),
@observacionesFactura varchar(200)

AS
BEGIN

SET NOCOUNT ON

UPDATE PEDIDO SET
numero_referencia_cliente = @numeroReferenciaCliente,
numero_referencia_adicional = @numeroReferenciaAdicional,
observaciones = @observaciones,
observaciones_guia_remision = @observacionesGuiaRemision,
observaciones_factura = @observacionesFactura,
fecha_entrega_extendida = @fechaEntregaExtendida

WHERE id_pedido = @idPedido


UPDATE ARCHIVO_ADJUNTO SET estado = 0 where id_archivo_adjunto IN (
SELECT id_archivo_adjunto FROM PEDIDO_ARCHIVO WHERE  id_pedido = @idPedido);

UPDATE PEDIDO_ARCHIVO SET estado = 0 WHERE  id_pedido = @idPedido


END







/* Obtener fecha_entrega_extendida */

ALTER PROCEDURE [dbo].[ps_pedido] 
@idPedido uniqueIdentifier
AS
BEGIN

SELECT 
--PEDIDO
pe.numero, pe.numero_grupo, pe.fecha_solicitud,  
pe.fecha_entrega_desde, pe.fecha_entrega_hasta, pe.fecha_entrega_extendida,
pe.hora_entrega_desde, pe.hora_entrega_hasta,
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
LEFT JOIN UBIGEO ub ON pe.ubigeo_entrega = ub.codigo
LEFT JOIN COTIZACION co ON co.id_cotizacion = pe.id_cotizacion
LEFT JOIN VENTA ve ON ve.id_pedido = pe.id_pedido
LEFT JOIN CPE_CABECERA_BE cpe ON cpe.id_cpe_cabecera_be = ve.id_documento_venta
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

SELECT arch.id_archivo_adjunto as id_pedido_adjunto,  nombre--, arch.adjunto,
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


