/*********** Create ps_lista_venta - Lista las ventas por guia de remision (CORRECCION DE NOMBRE DE VENDEDOR)********/
CREATE procedure  [dbo].[ps_lista_venta]
(
@idCiudad uniqueidentifier,
@idCliente uniqueidentifier,
@idUsuario uniqueidentifier,
@tipoDocumento int,
@numeroFactura bigint,
@numeroPedido bigint,
@numeroGuia bigint, 
@fechaTrasladoDesde datetime,
@fechaTrasladoHasta datetime,
@sku varchar(10) = NULL
)
as
begin 
	if  @numeroGuia = 0 AND @numeroPedido = 0 and @numeroFactura =0
	begin 
	select 	
	venta.id_venta,venta.id_movimiento_almacen,	
	PEDIDO.numero,
	CONCAT(CPE_CABECERA_BE.serie,'-',CPE_CABECERA_BE.correlativo) as doc_venta,
	concat('G',MOVIMIENTO_ALMACEN.serie_documento,'-',MOVIMIENTO_ALMACEN.numero_documento) as mov_almacen ,
	usuario.nombre,
	MOVIMIENTO_ALMACEN.fecha_emision,
	CLIENTE.codigo,
	cliente.razon_social,
	cliente.ruc,
	concat('MP',CIUDAD.codigo_sede) as sede,
	venta.total,
	ve.descripcion as vendedor
	
from VENTA	
	left JOIN CPE_CABECERA_BE on venta.id_documento_venta=CPE_CABECERA_BE.id_cpe_cabecera_be 
	inner join pedido on PEDIDO.id_pedido=VENTA.id_pedido 
	INNER JOIN MOVIMIENTO_ALMACEN on MOVIMIENTO_ALMACEN.id_movimiento_almacen=VENTA.id_movimiento_almacen
	inner join cliente on cliente.id_cliente=VENTA.id_cliente	
	inner join USUARIO on usuario.id_usuario=MOVIMIENTO_ALMACEN.usuario_creacion
	inner join CIUDAD on ciudad.id_ciudad=MOVIMIENTO_ALMACEN.id_sede_origen
	LEFT JOIN VENDEDOR AS ve ON cliente.id_responsable_comercial = ve.id_vendedor

where MOVIMIENTO_ALMACEN.tipo_documento = 'GR' and 
	MOVIMIENTO_ALMACEN.tipo_movimiento = 'S' AND 
	MOVIMIENTO_ALMACEN.motivo_traslado='V' AND    
	VENTA.estado=1 and
	MOVIMIENTO_ALMACEN.estado=1 AND
    MOVIMIENTO_ALMACEN.fecha_emision >= @fechaTrasladoDesde AND
	MOVIMIENTO_ALMACEN.fecha_emision <=  @fechaTrasladoHasta  	
	AND CIUDAD.id_ciudad = (case when @idCiudad='00000000-0000-0000-0000-000000000000' then CIUDAD.id_ciudad else @idCiudad end) 
	
	AND MOVIMIENTO_ALMACEN.id_cliente = (case when @idCliente='00000000-0000-0000-0000-000000000000' then MOVIMIENTO_ALMACEN.id_cliente else @idCliente end) 

	AND (@sku IS NULL OR  @sku = '' OR	EXISTS (SELECT	mad.id_movimiento_almacen_detalle
		FROM MOVIMIENTO_ALMACEN_DETALLE mad 
		INNER JOIN PRODUCTO pr ON mad.id_producto = pr.id_producto
		WHERE mad.id_movimiento_almacen = MOVIMIENTO_ALMACEN.id_movimiento_almacen 
		AND pr.sku like @sku)	)
	order by VENTA.numero asc ;
end 

else	
	select 	
	VENTA.id_venta,venta.id_movimiento_almacen,	
	pe.numero,
	CONCAT(CPE_CABECERA_BE.serie,'-',CPE_CABECERA_BE.correlativo) as doc_venta,
	concat('G',ma.serie_documento,'-',ma.numero_documento) as mov_almacen ,
	us.nombre,
	ma.fecha_emision,
	cl.codigo,
	cl.razon_social,
	cl.ruc,
	concat('MP',CIUDAD.codigo_sede) as sede,
	venta.total,
	ve.descripcion as vendedor
	FROM VENTA
	left JOIN CPE_CABECERA_BE on venta.id_documento_venta=CPE_CABECERA_BE.id_cpe_cabecera_be 
	inner join pedido AS pe on pe.id_pedido=VENTA.id_pedido 
	INNER JOIN MOVIMIENTO_ALMACEN as ma on ma.id_movimiento_almacen=VENTA.id_movimiento_almacen
	inner join CIUDAD on ciudad.id_ciudad=ma.id_sede_origen	
	INNER JOIN USUARIO AS us on VENTA.usuario_creacion = us.id_usuario
	INNER JOIN CLIENTE AS cl ON cl.id_cliente = VENTA.id_cliente
	LEFT JOIN VENDEDOR AS ve ON cl.id_responsable_comercial = ve.id_vendedor

	where 
		ma.tipo_movimiento = 'S'
	AND ma.tipo_documento = 'GR'
	AND ma.motivo_traslado='V'
	AND ma.estado=1
	AND VENTA.estado=1 	
	AND	(CPE_CABECERA_BE.TIP_CPE in (case when @tipoDocumento=0 then 1 end,case when @tipoDocumento=0 then 3 end,
										case when @tipoDocumento=1 then 1 end,
										case when @tipoDocumento=3 then 3 end)  
	OR CPE_CABECERA_BE.TIP_CPE IS NULL)	   	
	AND (ma.id_sede_origen = @idCiudad 	OR (ma.id_sede_origen = (Select id_ciudad FROM USUARIO where id_usuario = @idUsuario)))
	AND (ma.numero_documento = @numeroGuia OR pe.numero = @numeroPedido or CONVERT(INT,CPE_CABECERA_BE.CORRELATIVO)=@numeroFactura)	
	end
		
/*************** Create ps_venta_lista_detalle - Obtiene los detalles de una venta por guia de remision CORRECCION FILTRADO DE DETALLE DE PEDIDO CON ID_VENTA************************************/

create PROCEDURE [dbo].[ps_venta_lista_detalle] 
@idMovimientoAlmacen uniqueIdentifier,
@idVenta uniqueIdentifier
AS
BEGIN
	SELECT 
	concat('G',ma.serie_documento,'-',ma.numero_documento) as mov_almacen,
	pe.id_pedido,
	pe.numero, pe.numero_grupo, pe.fecha_solicitud,  
	pe.fecha_entrega_desde, pe.fecha_entrega_hasta,
	pe.hora_entrega_desde, pe.hora_entrega_hasta,
	pe.incluido_igv,  pe.igv, pe.total, pe.observaciones,  pe.fecha_modificacion,
	pe.numero_referencia_cliente, pe.id_direccion_entrega, pe.direccion_entrega, pe.contacto_entrega,
	pe.telefono_contacto_entrega, 
	pe.fecha_programacion,
	pe.tipo_pedido, pe.observaciones_factura, pe.observaciones_guia_remision,
	pe.contacto_pedido,pe.telefono_contacto_pedido, pe.correo_contacto_pedido,
	pe.otros_cargos,
	pe.numero_referencia_adicional,
	pe.numero_requerimiento,
	--UBIGEO
	pe.ubigeo_entrega, ub.departamento, ub.provincia, ub.distrito,
	---CLIENTE
	cl.id_cliente, cl.codigo, cl.razon_social, cl.ruc, cic.id_ciudad as id_ciudad_cliente, cic.nombre as nombre_ciudad_cliente,
	cl.razon_social_sunat, 
	--11/03/2018
	ISNULL(dl.direccion, ISNULL(dlp.direccion, UPPER(cl.direccion_domicilio_legal_sunat)) ) direccion_domicilio_legal_sunat	
	, cl.correo_envio_factura, cl.plazo_credito,	
	CASE pe.es_pago_contado WHEN 'TRUE' 
	THEN 1 
	ELSE cl.tipo_pago_factura END 
	AS tipo_pago_factura,
	cl.forma_pago_factura,
	cl.tipo_documento,
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
	ma.fecha_emision,
	ma.fecha_traslado,
	--DIRECCION_ENTREGA
	de.nombre as direccion_entrega_nombre,
	de.codigo_mp as direccion_entrega_codigo_mp,
	de.codigo_cliente as direccion_entrega_codigo_cliente,
	--DOCUMENTO_VENTA	
	CPE_CABECERA_BE.SERIE,
	CPE_CABECERA_BE.CORRELATIVO,
	CPE_CABECERA_BE.FEC_EMI,
	CPE_CABECERA_BE.FEC_VCTO,
	CPE_CABECERA_BE.HOR_EMI,
    CPE_CABECERA_BE.id_cpe_cabecera_be
	FROM VENTA as ve
	INNER JOIN MOVIMIENTO_ALMACEN as ma ON ma.id_movimiento_almacen = ve.id_movimiento_almacen
	INNER JOIN PEDIDO pe ON ve.id_pedido = pe.id_pedido
	INNER JOIN CIUDAD AS ci ON pe.id_ciudad = ci.id_ciudad
	INNER JOIN USUARIO AS us on pe.usuario_creacion = us.id_usuario
	INNER JOIN CLIENTE AS cl ON pe.id_cliente = cl.id_cliente
	INNER JOIN CIUDAD AS cic ON cl.id_ciudad = cic.id_ciudad
	LEFT JOIN UBIGEO ub ON CAST(pe.ubigeo_entrega AS CHAR(6)) = ub.codigo
	LEFT JOIN CPE_CABECERA_BE ON CPE_CABECERA_BE.id_cpe_cabecera_be=ve.id_documento_venta
	
	 --11/03/2018
	 --LEFT JOIN DIRECCION_ENTREGA de ON de.id_direccion_entrega = pe.id_direccion_entrega
	LEFT JOIN CLIENTE_SUNAT cs ON cl.id_cliente_sunat = cs.id_cliente_sunat
	LEFT JOIN DIRECCION_ENTREGA de ON pe.id_direccion_entrega = de.id_direccion_entrega
	LEFT JOIN DOMICILIO_LEGAL dl ON dl.id_cliente_sunat = cs.id_cliente_sunat AND dl.id_domicilio_legal = de.id_domicilio_legal
	--domicilio legal principal
	LEFT JOIN DOMICILIO_LEGAL dlp ON  dlp.id_domicilio_legal = cs.id_domicilio_legal
	
	where ve.id_movimiento_almacen = @idMovimientoAlmacen 
	AND ma.estado=1
	AND	pe.estado = 1
	AND ve.estado = 1
	
	--DETALLE PEDIDO
	SELECT vd.id_venta_detalle, 
	vd.cantidad,
	vd.precio_sin_igv, 
	vd.costo_sin_igv, 
	vd.equivalencia as equivalencia,
	vd.unidad, 
	vd.porcentaje_descuento, 
	vd.precio_neto, 
	vd.es_precio_alternativo, 
	vd.flete,
	pr.id_producto, pr.sku, pr.descripcion, pr.sku_proveedor, pr.imagen, pr.proveedor, 
	pr.costo as costo_producto, pr.precio as precio_producto,
	pd.observaciones,
	pd.fecha_modificacion,
	CAST(pd.precio_neto + pd.flete AS decimal(12,2)) as precio_unitario_original,
	vd.precio_unitario as precio_unitario_venta,
	vd.igv_precio_unitario as igv_precio_unitario_venta,
	RECTIFICACION_V_SELL_OUT.excluir,
	RECTIFICACION_V_SELL_OUT.estado
	FROM 
	VENTA_DETALLE as vd 
	INNER JOIN VENTA ve ON vd.id_venta = ve.id_venta
	INNER JOIN PEDIDO as pe ON ve.id_pedido = pe.id_pedido 
	INNER JOIN PRODUCTO pr ON vd.id_producto = pr.id_producto
	LEFT JOIN PEDIDO_DETALLE as pd ON vd.id_pedido_detalle = pd.id_pedido_detalle
	LEFT JOIN RECTIFICACION_V_SELL_OUT on RECTIFICACION_V_SELL_OUT.id_venta_detalle=vd.id_venta_detalle
	WHERE ve.id_venta=@idVenta	and ve.id_movimiento_almacen=@idMovimientoAlmacen
	AND ve.estado = 1
	ORDER BY fecha_modificacion ASC;

	SELECT arch.id_archivo_adjunto, null adjunto, nombre
	FROM ARCHIVO_ADJUNTO arch
	INNER JOIN PEDIDO_ARCHIVO parch ON arch.id_archivo_adjunto = parch.id_archivo_adjunto
	WHERE parch.id_pedido = (SELECT id_pedido FROM MOVIMIENTO_ALMACEN
	WHERE id_movimiento_almacen = @idMovimientoAlmacen   ) AND 
	arch.estado = 1 AND parch.estado = 1;
END

