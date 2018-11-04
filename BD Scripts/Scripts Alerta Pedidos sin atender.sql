CREATE PROCEDURE [dbo].[ps_pedidos_sin_atencion] 

AS
BEGIN

SELECT pe.id_pedido

FROM PEDIDO_DETALLE as pd 
INNER JOIN PEDIDO as pe ON pd.id_pedido = pe.id_pedido 
INNER JOIN SEGUIMIENTO_PEDIDO sp ON pe.id_pedido = sp.id_pedido AND sp.estado_pedido IN (5,6,9, 11)
LEFT JOIN 
(
SELECT mad.id_pedido_detalle, SUM(cantidad) as cantidadAtendida  FROM
MOVIMIENTO_ALMACEN_DETALLE AS mad 
INNER JOIN MOVIMIENTO_ALMACEN ma ON mad.id_movimiento_almacen = ma.id_movimiento_almacen
WHERE mad.estado = 1 AND  ma.estado = 1 AND ma.anulado = 0
GROUP BY mad.id_pedido_detalle
) AS  mad ON mad.id_pedido_detalle  = pd.id_pedido_detalle
	
	
where pe.estado = 1 and pd.estado = 1 and
	  pe.fecha_entrega_hasta <= GETDATE() 
	  and (pd.cantidad - COALESCE(mad.cantidadAtendida,0)) > 0 
group by pe.id_pedido
order by pe.id_pedido;

--  pe.fecha_entrega_hasta >= Convert(datetime, '2018-11-01' )

END





/* Consulta la información del pedido para enviarla por email */

create PROCEDURE [dbo].[ps_pedido_email] 
@idPedido uniqueIdentifier
AS
BEGIN

SELECT 
--PEDIDO
pe.numero, pe.numero_grupo, pe.fecha_solicitud,  
pe.fecha_entrega_desde, pe.fecha_entrega_hasta,
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


