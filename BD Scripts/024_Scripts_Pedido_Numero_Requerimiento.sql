ALTER TABLE PEDIDO ADD numero_requerimiento varchar(50)

/****** Object:  StoredProcedure [dbo].[pu_actualizarPedido2]    Script Date: 20/02/2019 8:47:18 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



/* Actualizar fecha_entrega_extendida */
ALTER PROCEDURE [dbo].[pu_actualizarPedido] 
@idPedido uniqueidentifier,
@numeroReferenciaCliente varchar(20),
@numeroReferenciaAdicional varchar(100),
@fechaEntregaExtendida datetime,
@observaciones varchar(500),
@observacionesGuiaRemision varchar(200),
@observacionesFactura varchar(200),
@numeroGrupoPedido bigint,
@numeroRequerimiento varchar(200)
AS
BEGIN

SET NOCOUNT ON

UPDATE PEDIDO SET
numero_referencia_cliente = @numeroReferenciaCliente,
numero_referencia_adicional = @numeroReferenciaAdicional,
observaciones = @observaciones,
observaciones_guia_remision = @observacionesGuiaRemision,
observaciones_factura = @observacionesFactura,
fecha_entrega_extendida = @fechaEntregaExtendida,
numero_grupo = @numeroGrupoPedido,
numero_requerimiento = @numeroRequerimiento
WHERE id_pedido = @idPedido


UPDATE ARCHIVO_ADJUNTO SET estado = 0 where id_archivo_adjunto IN (
SELECT id_archivo_adjunto FROM PEDIDO_ARCHIVO WHERE  id_pedido = @idPedido);

UPDATE PEDIDO_ARCHIVO SET estado = 0 WHERE  id_pedido = @idPedido


END




/****** Object:  StoredProcedure [dbo].[ps_pedido]    Script Date: 20/02/2019 8:51:12 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ps_pedido] 
@idPedido uniqueIdentifier
AS
BEGIN

SELECT 
--PEDIDO
pe.numero, pe.numero_grupo, pe.fecha_solicitud,  
pe.fecha_entrega_desde, pe.fecha_entrega_hasta, pe.fecha_entrega_extendida,
pe.hora_entrega_desde, pe.hora_entrega_hasta, pe.hora_entrega_adicional_desde, pe.hora_entrega_adicional_hasta,
pe.incluido_igv,  pe.igv, pe.total, pe.observaciones,  pe.fecha_modificacion,
pe.numero_referencia_cliente, pe.id_direccion_entrega, pe.direccion_entrega, pe.contacto_entrega,
pe.telefono_contacto_entrega, 
pe.fecha_programacion,
pe.tipo_pedido, pe.observaciones_factura, pe.observaciones_guia_remision,
pe.contacto_pedido,pe.telefono_contacto_pedido, pe.correo_contacto_pedido,
pe.otros_cargos,
pe.numero_referencia_adicional,
pe.fecha_creacion as fecha_registro,
/*cpe.serie AS serie_factura,
cpe.CORRELATIVO AS numero_factura,*/
pe.id_solicitante,
pe.tipo,
pe.es_pago_contado,
pe.numero_requerimiento,
--UBIGEO
pe.ubigeo_entrega, ub.departamento, ub.provincia, ub.distrito,
---CLIENTE
cl.id_cliente, cl.codigo, cl.razon_social, cl.ruc, cic.id_ciudad as id_ciudad_cliente, cic.nombre as nombre_ciudad_cliente,
cl.razon_social_sunat, cl.direccion_domicilio_legal_sunat, cl.correo_envio_factura, cl.plazo_credito,
cl.tipo_pago_factura,
cl.forma_pago_factura,
cl.bloqueado,


---VENTA
/*
ve.igv as igv_venta,
ve.sub_total as sub_total_venta,
ve.total as total_venta,
ve.id_venta,*/

--CIUDAD
ci.id_ciudad, ci.nombre as nombre_ciudad , ci.es_provincia,
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
ISNULL(uveasc.email,'') as asistente_servicio_cliente_email,

di.codigo_cliente as direccion_cliente_codigo_cliente,
di.codigo_mp as direccion_cliente_codigo_mp,
di.nombre as direccion_cliente_nombre

FROM PEDIDO as pe
INNER JOIN CIUDAD AS ci ON pe.id_ciudad = ci.id_ciudad
INNER JOIN USUARIO AS us on pe.usuario_creacion = us.id_usuario
INNER JOIN SEGUIMIENTO_PEDIDO sp ON pe.id_pedido = sp.id_pedido
INNER JOIN USUARIO AS us2 on sp.id_usuario = us2.id_usuario
INNER JOIN SEGUIMIENTO_CREDITICIO_PEDIDO spc ON pe.id_pedido = spc.id_pedido
INNER JOIN USUARIO AS us3 on spc.id_usuario = us3.id_usuario
INNER JOIN CLIENTE AS cl ON pe.id_cliente = cl.id_cliente
INNER JOIN CIUDAD AS cic ON cl.id_ciudad = cic.id_ciudad
LEFT JOIN UBIGEO ub ON CAST(pe.ubigeo_entrega AS char(6)) = ub.codigo
LEFT JOIN COTIZACION co ON co.id_cotizacion = pe.id_cotizacion
/*LEFT JOIN VENTA ve ON ve.id_pedido = pe.id_pedido
LEFT JOIN CPE_CABECERA_BE cpe ON cpe.id_cpe_cabecera_be = ve.id_documento_venta*/
LEFT JOIN SOLICITANTE so ON so.id_solicitante = pe.id_solicitante
LEFT JOIN VENDEDOR AS verc ON cl.id_responsable_comercial = verc.id_vendedor
LEFT JOIN VENDEDOR AS vesc ON cl.id_supervisor_comercial = vesc.id_vendedor
LEFT JOIN VENDEDOR AS veasc ON cl.id_asistente_servicio_cliente = veasc.id_vendedor
LEFT JOIN USUARIO AS uverc ON uverc.id_usuario = verc.id_usuario
LEFT JOIN USUARIO AS uvesc ON uvesc.id_usuario = vesc.id_usuario
LEFT JOIN USUARIO AS uveasc ON uveasc.id_usuario = veasc.id_usuario
LEFT JOIN DIRECCION_ENTREGA AS di ON di.id_direccion_entrega = pe.id_direccion_entrega
--LEFT JOIN GRUPO AS gr ON pe.id_grupo = gr.id_grupo  
where pe.id_pedido = @idPedido and 
pe.estado = 1
AND sp.estado = 1
AND spc.estado = 1;



--RECUPERA EL DETALLE DEL PEDIDO

	SELECT 
	pd.id_pedido_detalle, pd.cantidad, pd.precio_sin_igv, pr.costo costo_sin_igv, 


	pd.equivalencia as equivalencia,
	pd.unidad, pd.porcentaje_descuento, pd.precio_neto, pd.es_precio_alternativo, pd.flete,
	pr.id_producto, pr.sku, pr.descripcion, pr.sku_proveedor, null as imagen, pr.proveedor, 
	pr.costo as costo_producto, pr.precio as precio_producto, pr.tipo tipo_producto,
	pd.observaciones,
	pd.fecha_modificacion,

	null as fecha_fin_vigencia,

	(pd.cantidad - COALESCE(mad.cantidadAtendida,0)) AS  cantidadPendienteAtencion


	FROM PEDIDO_DETALLE as pd 
	INNER JOIN PEDIDO as pe ON pd.id_pedido = pe.id_pedido 
	INNER JOIN PRODUCTO pr ON pd.id_producto = pr.id_producto
	LEFT JOIN 
	(
	SELECT mad.id_pedido_detalle, SUM(cantidad) as cantidadAtendida  FROM
	MOVIMIENTO_ALMACEN_DETALLE AS mad 
	INNER JOIN MOVIMIENTO_ALMACEN ma ON mad.id_movimiento_almacen = ma.id_movimiento_almacen
	INNER JOIN PEDIDO pe ON pe.id_pedido = ma.id_pedido
	WHERE ma.id_pedido = @idPedido
	AND mad.estado = 1 AND  ma.estado = 1 AND ma.anulado = 0
	/*AND ((pe.tipo = 'V' AND ma.tipo_movimiento = 'S'	) OR (pe.tipo = 'C' AND ma.tipo_movimiento = 'S')
		OR pe.tipo = 'A' AND ma.tipo_movimiento = 'S'

	) */
	GROUP BY mad.id_pedido_detalle
	) AS  mad ON mad.id_pedido_detalle  = pd.id_pedido_detalle



	where pd.id_pedido = @idPedido and pd.estado = 1 


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

SELECT  arch.id_archivo_adjunto,  nombre--, arch.adjunto,
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



---NUMEROS DE GRUPOS
SELECT numero_grupo, fecha_solicitud FROM (
SELECT  row_number() over (partition by numero_grupo order by fecha_solicitud) rownumber
 , numero_grupo, fecha_solicitud
FROM PEDIDO where numero_grupo IS NOT NULL AND 
	numero_grupo > 0 AND
	id_cliente IN (
	SELECT id_cliente FROM CLIENTE where estado > 0 
	AND RUC = (
	SELECT cl.ruc FROM 
	PEDIDO pe INNER JOIN CLIENTE cl ON pe.id_cliente = cl.id_cliente
	where id_pedido = @idPedido)
) )  SQ
where rownumber = 1


END



/****** Object:  StoredProcedure [dbo].[ps_pedidoParaEditar]    Script Date: 20/02/2019 8:52:02 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[ps_pedidoParaEditar] 
@idPedido uniqueIdentifier
AS
BEGIN

SELECT 
--PEDIDO
pe.numero, pe.numero_grupo, pe.fecha_solicitud,  
pe.fecha_entrega_desde, pe.fecha_entrega_hasta,
pe.hora_entrega_desde, pe.hora_entrega_hasta,pe.hora_entrega_adicional_desde, pe.hora_entrega_adicional_hasta,
pe.incluido_igv,  pe.igv, pe.total, pe.observaciones,  pe.fecha_modificacion,
pe.numero_referencia_cliente, pe.id_direccion_entrega, pe.direccion_entrega, pe.contacto_entrega,
de.nombre direccion_entrega_nombre, 
de.codigo_mp  direccion_entrega_codigo_mp,
de.codigo_cliente direccion_entrega_codigo_cliente,
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
pe.es_pago_contado, ---REVISAR
pe.numero_requerimiento,
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
--DETALLE
co.id_cotizacion,
co.codigo as cotizacion_codigo
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
LEFT JOIN DIRECCION_ENTREGA de ON de.id_direccion_entrega = pe.id_direccion_entrega
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
	pr.costo as costo_producto, pr.precio as precio_producto, pr.tipo as tipo_producto,
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
		 AND fecha_inicio_vigencia <= GETDATE()
		--ORDER BY fecha_inicio_vigencia DESC
	) pc ON pc.id_producto = pr.id_producto 
	AND (pe.id_cliente = pc.id_cliente 
	OR pc.id_grupo_cliente = (SELECT id_grupo_cliente FROM CLIENTE_GRUPO_CLIENTE where id_cliente = pe.id_cliente)
	)
	AND pd.equivalencia = pc.equivalencia
	--AND pd.es_precio_alternativo = pc.es_unidad_alternativa


	where pd.id_pedido = @idPedido and pd.estado = 1 
	
	) SQuery 
		where RowNumber = 1
	ORDER BY fecha_modificacion ASC;


--RECUPERA LAS DIRECCIONES DE ENTREGA

SELECT de.id_direccion_entrega, de.descripcion, de.contacto, de.telefono, ubigeo,
nombre, codigo_cliente, codigo_mp
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

SELECT arch.id_archivo_adjunto,  nombre--, arch.adjunto,
 FROM ARCHIVO_ADJUNTO arch
 INNER JOIN PEDIDO_ARCHIVO parch ON arch.id_archivo_adjunto = parch.id_archivo_adjunto
   WHERE parch.id_pedido = @idPedido
   AND arch.estado = 1
   AND parch.estado = 1;


SELECT so.id_solicitante, so.nombre, so.telefono, so.correo
FROM SOLICITANTE so
INNER JOIN PEDIDO pe ON so.id_cliente = pe.id_cliente
where pe.id_pedido = @idPedido
AND so.estado = 1;

END



/****** Object:  StoredProcedure [dbo].[pi_pedido]    Script Date: 20/02/2019 8:52:43 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[pi_pedido] 
@numeroGrupo bigint, 
@idCotizacion uniqueidentifier, 
@idCiudad uniqueidentifier, 
@idCliente uniqueidentifier, 
@numeroReferenciaCliente varchar(100),
@idDireccionEntrega uniqueidentifier,
@direccionEntrega varchar(200),
@contactoEntrega varchar(100),
@telefonoContactoEntrega varchar(100),
@codigoCliente varchar(30),
@codigoMP varchar(30),
@nombre varchar(250),
@observacionesDireccionEntrega varchar(250),
@fechaSolicitud  datetime,
@fechaEntregaDesde datetime,
@fechaEntregaHasta datetime,
@horaEntregaDesde datetime,
@horaEntregaHasta datetime,
@horaEntregaAdicionalDesde datetime,
@horaEntregaAdicionalHasta datetime,
@idSolicitante uniqueIdentifier,
@contactoPedido varchar(100),
@telefonoContactoPedido varchar(100),
@correoContactoPedido varchar(100),
@incluidoIGV smallint, 
@tasaIGV decimal(18,2), 
@igv decimal(18,2), 
@total decimal(18,2), 
@observaciones varchar(500), 
@idUsuario uniqueidentifier,
@estado smallint,
@estadoCrediticio smallint,
@esPagoContado bit,
@observacionSeguimientoPedido varchar(500),
@observacionSeguimientoCrediticioPedido varchar(500),
@ubigeoEntrega char(6),
@tipoPedido char(1),
@observacionesGuiaRemision varchar(200),
@observacionesFactura varchar(200),
@otrosCargos Decimal(12,2),
@tipo char(1),
@numeroRequerimiento varchar(50),
@newId uniqueidentifier OUTPUT, 
@numero bigint OUTPUT 
AS
BEGIN
SET NOCOUNT ON

DECLARE @countDireccionEntrega int;
DECLARE @countSolicitante int;

SET @newId = NEWID();
SET @numero = NEXT VALUE FOR dbo.SEQ_PEDIDO_NUMERO;

/*Si el numero de Grupo es distinto de null se actualiza en el Pedido Origen*/
/*IF @numeroGrupo IS NOT NULL 
BEGIN
	UPDATE PEDIDO SET numero_grupo = @numeroGrupo where numero = @numeroGrupo;
END*/


/*Si no se cuenta con Dirección Entrega*/
IF @idDireccionEntrega = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)
BEGIN 

	IF @codigoCliente IS NULL AND @nombre IS NULL
	BEGIN 
		SELECT @countDireccionEntrega = COUNT(*) FROM DIRECCION_ENTREGA
		WHERE id_cliente = @idCliente 
		AND estado = 1 
		AND REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(@direccionEntrega), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  = 
		REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(descripcion), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ') 
	
		IF @countDireccionEntrega = 1
		BEGIN 
			SELECT @idDireccionEntrega = id_direccion_entrega FROM DIRECCION_ENTREGA
			WHERE id_cliente = @idCliente 
			AND estado = 1 
			AND REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(@direccionEntrega), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  = 
			REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(descripcion), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ') 
		END 
	END
	ELSE IF @codigoCliente IS NOT  NULL 
	BEGIN 
		SELECT @countDireccionEntrega = COUNT(*) FROM DIRECCION_ENTREGA
		WHERE id_cliente = @idCliente 
		AND estado = 1 
		AND codigo_cliente = @codigoCliente
	
		IF @countDireccionEntrega = 1
		BEGIN 
			SELECT @idDireccionEntrega = id_direccion_entrega FROM DIRECCION_ENTREGA
			WHERE id_cliente = @idCliente 
			AND estado = 1 
			AND codigo_cliente = @codigoCliente
		END 
	END
	ELSE 
	BEGIN 
		SELECT @countDireccionEntrega = COUNT(*) FROM DIRECCION_ENTREGA
		WHERE id_cliente = @idCliente 
		AND estado = 1 
		AND nombre = TRIm(@nombre)
	
		IF @countDireccionEntrega = 1
		BEGIN 
			SELECT @idDireccionEntrega = id_direccion_entrega FROM DIRECCION_ENTREGA
			WHERE id_cliente = @idCliente 
			AND estado = 1 
			AND nombre = TRIm(@nombre)
		END 
	END
END

IF @idDireccionEntrega = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)
BEGIN 
	SET @idDireccionEntrega  = NEWID();
	INSERT INTO DIRECCION_ENTREGA
	(id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono, estado, usuario_creacion,
	fecha_creacion, usuario_modificacion, fecha_modificacion,
	codigo_cliente, codigo_mp, nombre, observaciones)
	 VALUES(@idDireccionEntrega, @idCliente, @ubigeoEntrega, 
	@direccionEntrega,@contactoEntrega,@telefonoContactoEntrega,1,@idUsuario, 
	GETDATE(), @idUsuario, GETDATE(), @codigoCliente, @codigoMP, @nombre, @observacionesDireccionEntrega);
END 
ELSE
BEGIN

	IF @contactoEntrega IS NULL
	BEGIN 
		SELECT @contactoEntrega = contacto, @telefonoContactoEntrega = telefono FROM DIRECCION_ENTREGA WHERE id_direccion_entrega = @idDireccionEntrega;
	END

	UPDATE DIRECCION_ENTREGA SET 
	descripcion = @direccionEntrega, 
	ubigeo = @ubigeoEntrega, 
	contacto = @contactoEntrega,
	telefono = @telefonoContactoEntrega,
	usuario_modificacion = @idUsuario,
	fecha_modificacion = GETDATE(), 
	codigo_cliente = @codigoCliente,
	codigo_mp = @codigoMP,
	nombre =@nombre,
	observaciones = @observacionesDireccionEntrega
	where id_direccion_entrega = @idDireccionEntrega;
END 


IF @idSolicitante = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)
BEGIN 
	SELECT @countSolicitante = COUNT(*) FROM SOLICITANTE
	WHERE id_cliente = @idCliente 
	AND estado = 1 
	AND REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(@contactoPedido), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')   = 
	REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(nombre), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ') 
	
	IF @countSolicitante = 1
	BEGIN 
		SELECT @idSolicitante = id_solicitante FROM SOLICITANTE
		WHERE id_cliente = @idCliente 
		AND estado = 1 
		AND REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(@contactoPedido), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')   = 
		REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(nombre), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ') 
	END 
END


IF @idSolicitante = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)
BEGIN 
	SET @idSolicitante  = NEWID();
	INSERT INTO SOLICITANTE 
	(id_solicitante, id_cliente, nombre, telefono, correo, estado, 
	usuario_creacion, fecha_creacion, usuario_modificacion, fecha_modificacion)
	VALUES(@idSolicitante, @idCliente, @contactoPedido, @telefonoContactoPedido, @correoContactoPedido,1,
	@idUsuario,GETDATE(), @idUsuario, GETDATE());
END 
ELSE
BEGIN
	UPDATE SOLICITANTE SET 
	nombre = @contactoPedido, 
	telefono = @telefonoContactoPedido, 
	correo = @correoContactoPedido,
	usuario_modificacion = @idUsuario,
	fecha_modificacion = GETDATE() 
	where id_solicitante = @idSolicitante;
END 



INSERT INTO [PEDIDO]
           ([id_pedido]
		   ,id_Cotizacion
		   ,[numero]
		   ,numero_grupo
			,[id_ciudad] 
			,[id_cliente] 
			,numero_referencia_cliente
			,id_direccion_entrega
			,direccion_entrega
			,contacto_entrega
			,telefono_contacto_entrega
			,[fecha_solicitud]
			,[fecha_entrega_desde]
			,[fecha_entrega_hasta]
			,[hora_entrega_desde]
			,[hora_entrega_hasta]
			,[hora_entrega_adicional_desde]
			,[hora_entrega_adicional_hasta]
			,contacto_pedido
			,telefono_contacto_pedido
			,correo_contacto_pedido
			,incluido_igv 
			,tasa_igv
			,igv
			,total
			,observaciones
		   ,[estado]
		   ,[usuario_creacion]
		   ,[fecha_creacion]
		   ,[usuario_modificacion]
		   ,[fecha_modificacion]
		   ,ubigeo_entrega
		   ,tipo_pedido
		   ,observaciones_guia_remision
		   ,observaciones_factura
		   ,otros_cargos
		   ,id_solicitante
		   ,tipo
		   ,es_pago_contado
		   ,numero_requerimiento
		   )
     VALUES
           (@newId
		   ,@idCotizacion
		    ,@numero
		    ,@numeroGrupo
			,@idCiudad
			,@idCliente 
			,@numeroReferenciaCliente
			,@idDireccionEntrega
			,@direccionEntrega
			,@contactoEntrega
			,@telefonoContactoEntrega
			,@fechaSolicitud
			,@fechaEntregaDesde
			,@fechaEntregaHasta
			,@horaEntregaDesde
			,@horaEntregaHasta
			,@horaEntregaAdicionalDesde
			,@horaEntregaAdicionalHasta
			,@contactoPedido
			,@telefonoContactoPedido
			,@correoContactoPedido
			,@incluidoIGV
			,@tasaIGV
			,@igv
			,@total
			,@observaciones
			,1
			,@idUsuario
			,GETDATE()
			,@idUsuario
			,GETDATE()
			,@ubigeoEntrega	
			,@tipoPedido
		   ,@observacionesGuiaRemision
		   ,@observacionesFactura
		   ,@otrosCargos
		   ,@idSolicitante

		   ,@tipo --[V|C|A]
		   ,@esPagoContado
		   ,@numeroRequerimiento
			);

INSERT INTO SEGUIMIENTO_PEDIDO 
(
		id_seguimiento_pedido, 
		id_usuario ,
		id_pedido, 
		estado_pedido,
		observacion ,
		estado ,
		usuario_creacion ,
		fecha_creacion ,
		usuario_modifiacion ,
		fecha_modificacion 
)
VALUES(
			NEWID(),
			@idUsuario,
			@newId,
			@estado,
			@observacionSeguimientoPedido,
			1,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE()
);

INSERT INTO SEGUIMIENTO_CREDITICIO_PEDIDO 
(
		id_seguimiento_crediticio_pedido, 
		id_usuario ,
		id_pedido, 
		estado_pedido,
		observacion ,
		estado ,
		usuario_creacion ,
		fecha_creacion ,
		usuario_modifiacion ,
		fecha_modificacion 
)
VALUES(
			NEWID(),
			@idUsuario,
			@newId,
			@estadoCrediticio,
			@observacionSeguimientoCrediticioPedido,
			1,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE()
);


END


/****** Object:  StoredProcedure [dbo].[pu_pedido]    Script Date: 20/02/2019 8:55:06 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[pu_pedido] 
@idPedido uniqueidentifier, 
@numeroGrupo bigint, 
@idCotizacion uniqueidentifier, 
@idCiudad uniqueidentifier, 
@idCliente uniqueidentifier, 
@numeroReferenciaCliente varchar(100),
@idDireccionEntrega uniqueidentifier,
@direccionEntrega varchar(200),
@contactoEntrega varchar(100),
@telefonoContactoEntrega varchar(100),
@codigoCliente varchar(30),
@codigoMP varchar(30),
@nombre varchar(250),
@observacionesDireccionEntrega varchar(250),
@fechaSolicitud  datetime,
@fechaEntregaDesde datetime,
@fechaEntregaHasta datetime,
@horaEntregaDesde datetime,
@horaEntregaHasta datetime,
@horaEntregaAdicionalDesde datetime,
@horaEntregaAdicionalHasta datetime,
@idSolicitante uniqueIdentifier,
@contactoPedido varchar(100),
@telefonoContactoPedido varchar(100),
@correoContactoPedido varchar(100),
@incluidoIGV smallint, 
@tasaIGV decimal(18,2), 
@igv decimal(18,2), 
@total decimal(18,2), 
@observaciones varchar(500), 
@idUsuario uniqueidentifier,
@estado smallint,
@estadoCrediticio smallint,
@esPagoContado bit,
@observacionSeguimientoPedido varchar(500),
@observacionSeguimientoCrediticioPedido varchar(500),
@tipoPedido char(1),
@observacionesGuiaRemision varchar(200),
@observacionesFactura varchar(200),
@ubigeoEntrega char(6),
@otrosCargos decimal(12,2),
@numeroRequerimiento varchar(50)
AS
BEGIN

DECLARE @countDireccionEntrega int;
DECLARE @countSolicitante int;

SET NOCOUNT ON

IF @numeroGrupo IS NOT NULL 
BEGIN
	UPDATE PEDIDO SET numero_grupo = @numeroGrupo where id_pedido  =@idPedido;
END


IF @idDireccionEntrega = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)
BEGIN 

	IF @codigoCliente IS NULL
	BEGIN 
		SELECT @countDireccionEntrega = COUNT(*) FROM DIRECCION_ENTREGA
		WHERE id_cliente = @idCliente 
		AND estado = 1 
		AND REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(@direccionEntrega), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  = 
		REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(descripcion), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ') 
	
		IF @countDireccionEntrega = 1
		BEGIN 
			SELECT @idDireccionEntrega = id_direccion_entrega FROM DIRECCION_ENTREGA
			WHERE id_cliente = @idCliente 
			AND estado = 1 
			AND REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(@direccionEntrega), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  = 
			REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(descripcion), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ') 
		END 
	END
	ELSE 
	BEGIN 
		SELECT @countDireccionEntrega = COUNT(*) FROM DIRECCION_ENTREGA
		WHERE id_cliente = @idCliente 
		AND estado = 1 
		AND codigo_cliente = @codigoCliente
	
		IF @countDireccionEntrega = 1
		BEGIN 
			SELECT @idDireccionEntrega = id_direccion_entrega FROM DIRECCION_ENTREGA
			WHERE id_cliente = @idCliente 
			AND estado = 1 
			AND codigo_cliente = @codigoCliente
		END 
	END
END

IF @idDireccionEntrega = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)
BEGIN 
	SET @idDireccionEntrega  = NEWID();
	INSERT INTO DIRECCION_ENTREGA
	(id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono, estado, usuario_creacion,
	fecha_creacion, usuario_modificacion, fecha_modificacion,
	codigo_cliente, codigo_mp, nombre, observaciones)
	 VALUES(@idDireccionEntrega, @idCliente, @ubigeoEntrega, 
	@direccionEntrega,@contactoEntrega,@telefonoContactoEntrega,1,@idUsuario, 
	GETDATE(), @idUsuario, GETDATE(), @codigoCliente, @codigoMP, @nombre, @observacionesDireccionEntrega);
END 
ELSE
BEGIN
	UPDATE DIRECCION_ENTREGA SET 
	descripcion = @direccionEntrega, 
	ubigeo = @ubigeoEntrega, 
	contacto = @contactoEntrega,
	telefono = @telefonoContactoEntrega,
	usuario_modificacion = @idUsuario,
	fecha_modificacion = GETDATE(), 
	codigo_cliente = @codigoCliente,
	codigo_mp = @codigoMP,
	nombre =@nombre,
	observaciones = @observacionesDireccionEntrega
	where id_direccion_entrega = @idDireccionEntrega;
END 


IF @idSolicitante = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)
BEGIN 
	SELECT @countSolicitante = COUNT(*) FROM SOLICITANTE
	WHERE id_cliente = @idCliente 
	AND estado = 1 
	AND REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(@contactoPedido), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')   = 
	REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(nombre), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ') 
	
	IF @countSolicitante = 1
	BEGIN 
		SELECT @idSolicitante = id_solicitante FROM SOLICITANTE
		WHERE id_cliente = @idCliente 
		AND estado = 1 
		AND REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(@contactoPedido), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')   = 
		REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(nombre), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ') 
	END 
END


IF @idSolicitante = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)
BEGIN 
	SET @idSolicitante  = NEWID();
	INSERT INTO SOLICITANTE 
	(id_solicitante, id_cliente, nombre, telefono, correo, estado, 
	usuario_creacion, fecha_creacion, usuario_modificacion, fecha_modificacion)
	VALUES(@idSolicitante, @idCliente, @contactoPedido, @telefonoContactoPedido, @correoContactoPedido,1,
	@idUsuario,GETDATE(), @idUsuario, GETDATE());
END 
ELSE
BEGIN
	UPDATE SOLICITANTE SET 
	nombre = @contactoPedido, 
	telefono = @telefonoContactoPedido, 
	correo = @correoContactoPedido,
	usuario_modificacion = @idUsuario,
	fecha_modificacion = GETDATE() 
	where id_solicitante = @idSolicitante;
END 


UPDATE PEDIDO_DETALLE SET ESTADO = 0 ,
[usuario_modificacion] = @idUsuario, [fecha_modificacion] = GETDATE()
WHERE  id_pedido = @idPedido;


UPDATE ARCHIVO_ADJUNTO SET estado = 0 where id_archivo_adjunto IN (
SELECT id_archivo_adjunto FROM PEDIDO_ARCHIVO WHERE  id_pedido = @idPedido);

UPDATE PEDIDO_ARCHIVO SET estado = 0 WHERE  id_pedido = @idPedido

UPDATE [PEDIDO] SET
id_cotizacion = @idCotizacion,
[id_ciudad]  = @idCiudad
,[id_cliente] = @idCliente 
,numero_referencia_cliente = @numeroReferenciaCliente
,ubigeo_entrega = @ubigeoEntrega
,id_direccion_entrega = @idDireccionEntrega
,direccion_entrega = @direccionEntrega
,contacto_entrega = @contactoEntrega
,telefono_contacto_entrega = @telefonoContactoEntrega
,[fecha_solicitud] = @fechaSolicitud
,[fecha_entrega_desde] = @fechaEntregaDesde
,[fecha_entrega_hasta] = @fechaEntregaHasta
,[hora_entrega_desde] = @horaEntregaDesde
,[hora_entrega_hasta] = @horaEntregaHasta
,[hora_entrega_adicional_desde] = @horaEntregaAdicionalDesde
,[hora_entrega_adicional_hasta] = @horaEntregaAdicionalHasta
,contacto_pedido = @contactoPedido
,telefono_contacto_pedido = @telefonoContactoPedido
,correo_contacto_pedido = @correoContactoPedido
,incluido_igv = @incluidoIGV
,tasa_igv = @tasaIGV
,igv = @igv
,total = @total
,observaciones = @observaciones
,tipo_pedido = @tipoPedido
,observaciones_guia_remision = @observacionesGuiaRemision
,observaciones_factura = @observacionesFactura
,[estado] = 1
,[usuario_modificacion] = @idUsuario
,[fecha_modificacion] = GETDATE()
,[otros_cargos] = @otrosCargos
,id_solicitante = @idSolicitante
,es_pago_contado = @esPagoContado
,numero_requerimiento = @numeroRequerimiento
 WHERE id_pedido = @idPedido;

UPDATE SEGUIMIENTO_PEDIDO set estado = 0 where id_pedido = @idPedido;
UPDATE SEGUIMIENTO_CREDITICIO_PEDIDO set estado = 0 where id_pedido = @idPedido;

INSERT INTO SEGUIMIENTO_PEDIDO 
(
		id_seguimiento_pedido, 
		id_usuario ,
		id_pedido, 
		estado_pedido,
		observacion ,
		estado ,
		usuario_creacion ,
		fecha_creacion ,
		usuario_modifiacion ,
		fecha_modificacion 
)
VALUES(
			NEWID(),
			@idUsuario,
			@idPedido,
			@estado,
			@observacionSeguimientoPedido,
			1,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE()
);

INSERT INTO SEGUIMIENTO_CREDITICIO_PEDIDO 
(
		id_seguimiento_crediticio_pedido, 
		id_usuario ,
		id_pedido, 
		estado_pedido,
		observacion ,
		estado ,
		usuario_creacion ,
		fecha_creacion ,
		usuario_modifiacion ,
		fecha_modificacion 
)
VALUES(
			NEWID(),
			@idUsuario,
			@idPedido,
			@estadoCrediticio,
			@observacionSeguimientoCrediticioPedido,
			1,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE()
);



END



-----------------------------------------------------------
USE [cotizadormp]
GO
/****** Object:  StoredProcedure [dbo].[ps_getproductos_search]    Script Date: 20/02/2019 10:43:12 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ps_getproductos_search] 
/*@idProveedor uniqueidentifier,
@idFamilia uniqueidentifier, */
@textoBusqueda varchar(50),
@proveedor varchar(5),
@familia varchar(200),
@considerarDescontinuados int,
@tipoPedido char(1)
AS
BEGIN




SELECT id_producto, REPLACE(sku,'"',' ') as sku,
REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(descripcion, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  as descripcion
FROM PRODUCTO 
WHERE  (REPLACE(sku,'"',' ') LIKE '%'+REPLACE(@textoBusqueda,' ','%') +'%' OR REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(descripcion, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  LIKE '%'+REPLACE(@textoBusqueda,' ','%')+'%') 
AND (estado = 1 OR @considerarDescontinuados = 1)
AND (familia = @familia or @familia = 'Todas')
AND (proveedor = @proveedor or @proveedor = 'Todos')
AND (
	@tipoPedido IS NULL 
	OR (@tipoPedido = 'V' AND tipo IN (1,3,4,5) ) 
	OR (@tipoPedido = 'G' AND tipo IN (1) ) 
	OR (@tipoPedido = 'M' AND tipo = 2 ) 
)



/*AND 
(id_proveedor = @idProveedor OR '00000000-0000-0000-0000-000000000000' = @idProveedor ) AND 
(id_familia = @idFamilia OR '00000000-0000-0000-0000-000000000000' = @idFamilia )*/
order by descripcion asc
END
