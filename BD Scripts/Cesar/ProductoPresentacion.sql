
ALTER TABLE PEDIDO_DETALLE ADD id_producto_presentacion int
ALTER TABLE COTIZACION_DETALLE ADD id_producto_presentacion int
ALTER TABLE MOVIMIENTO_ALMACEN_DETALLE ADD id_producto_presentacion int
ALTER TABLE VENTA_DETALLE ADD id_producto_presentacion int
ALTER TABLE PRECIO_CLIENTE_PRODUCTO ADD id_producto_presentacion int
ALTER TABLE MOVIMIENTO_ALMACEN_DETALLE ADD es_unidad_alternativa bit

UPDATE PEDIDO_DETALLE SET id_producto_presentacion = es_precio_alternativo
UPDATE COTIZACION_DETALLE SET id_producto_presentacion = es_precio_alternativo
UPDATE VENTA_DETALLE SET id_producto_presentacion = es_precio_alternativo
UPDATE PRECIO_CLIENTE_PRODUCTO SET id_producto_presentacion = es_unidad_alternativa
UPDATE MOVIMIENTO_ALMACEN_DETALLE SET es_unidad_alternativa = 0 WHERE equivalencia = 1 
UPDATE MOVIMIENTO_ALMACEN_DETALLE SET es_unidad_alternativa = 1 WHERE equivalencia != 1 


SELECT codigo, equivalencia, presentacion, 
precio/equivalencia precio_lima_sin_igv,
precio_provincia/equivalencia precio_provincias_sin_igv,
costo/equivalencia costo_sin_igv, 
unidad_internacional
 FROM (
	SELECT 1 AS codigo,
	CAST(equivalencia AS decimal(14,12)) AS equivalencia,
	unidad_alternativa AS presentacion, 
	precio,
	precio_provincia,
	costo,
	unidad_alternativa_internacional AS unidad_internacional
	FROM PRODUCTO WHERE id_producto = '4BB9A368-E3AA-42ED-BB28-E3B120189CE0'
	AND equivalencia != 1
	UNION 
	SELECT 2 AS codigo,
	CAST(1/CAST(equivalencia_proveedor AS decimal) AS decimal(14,12))  AS equivalencia, 
	unidad_proveedor AS presentacion,
	precio,
	precio_provincia,
	costo,
	unidad_proveedor_internacional AS unidad_internacional
	FROM PRODUCTO WHERE id_producto = '4BB9A368-E3AA-42ED-BB28-E3B120189CE0'
	AND equivalencia_proveedor  != 1
) sq

SELECT * FROM PRODUCTO_PRESENTACION 

DROP TABLE  PRODUCTO_PRESENTACION

CREATE TABLE PRODUCTO_PRESENTACION (
id_producto_presentacion int primary key identity (1,1),
estado bit not null, 
usuario_creacion uniqueIdentifier,
fecha_creacion datetime,
usuario_modificacion uniqueIdentifier,
fecha_modificacion datetime,
id_producto uniqueIdentifier, 
es_unidad_alternativa bit,
equivalencia decimal(18,14),
presentacion varchar(500), 
precio_lima_sin_igv decimal(12,2),
precio_provincias_sin_igv decimal(12,2),
costo_sin_igv decimal(12,2),
unidad_internacional VARCHAR(3)
)


INSERT INTO PRODUCTO_PRESENTACION (estado,fecha_creacion, fecha_modificacion,equivalencia, es_unidad_alternativa
,presentacion, precio_lima_sin_igv , precio_provincias_sin_igv, costo_sin_igv , unidad_internacional,id_producto)
SELECT 1, GETDATE(), GETDATE(),equivalencia, es_unidad_alternativa, presentacion, 
precio/equivalencia precio_lima_sin_igv, 
precio_provincia/equivalencia precio_provincias_sin_igv,
costo/equivalencia costo_sin_igv, 
unidad_internacional,id_producto
 FROM (
	SELECT 1 AS codigo,
	CAST(equivalencia AS decimal(18,14)) AS equivalencia, 1 AS es_unidad_alternativa,
--	equivalencia,
	unidad_alternativa AS presentacion, 
	precio,
	precio_provincia,
	costo,
	unidad_alternativa_internacional AS unidad_internacional,
	id_producto
	FROM PRODUCTO WHERE 
	--id_producto = '4BB9A368-E3AA-42ED-BB28-E3B120189CE0' AND 
	equivalencia != 1
	--ORDER BY equivalencia


	UNION 
	SELECT 2 AS codigo,
	CAST(1/CAST(equivalencia_proveedor AS decimal) AS decimal(18,14))  AS equivalencia,  1 AS es_unidad_alternativa,
	unidad_proveedor AS presentacion,
	precio,
	precio_provincia,
	costo,
	unidad_proveedor_internacional AS unidad_internacional,
	id_producto
	FROM PRODUCTO WHERE 
	--id_producto = '4BB9A368-E3AA-42ED-BB28-E3B120189CE0' AND 
	equivalencia_proveedor  != 1
	UNION 
	SELECT 0 AS codigo,
	1  AS equivalencia,  0 AS es_unidad_alternativa,
	unidad AS presentacion,
	precio,
	precio_provincia,
	costo,
	unidad_estandar_internacional AS unidad_internacional,
	id_producto
	FROM PRODUCTO WHERE 
	--id_producto = '4BB9A368-E3AA-42ED-BB28-E3B120189CE0' AND 
	equivalencia_proveedor  != 1
) sq