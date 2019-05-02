/* **** 1 **** */
ALTER PROCEDURE [dbo].[ps_productos]
@sku varchar(100),
@skuProveedor varchar(100),
@descripcion varchar(500),
@familia varchar(200),
@proveedor varchar(10),
@tipo int,
@estado int
AS
BEGIN

IF @sku IS NULL OR @sku = ''
BEGIN
	SELECT 
	 id_producto,
		precio,
		costo,
		precio_provincia,
		sku,
		sku_proveedor,
		descripcion,
		familia,
		proveedor,
		unidad,
		unidad_alternativa,

		equivalencia,
		equivalencia_proveedor,
		moneda_compra,
		moneda_venta,
		unidad_proveedor,
		unidad_estandar_internacional,
		unidad_alternativa_internacional,
		inafecto,
		tipo,

		costo_original,
		precio_original,
		precio_provincia_original,
		tipo_cambio,
		equivalencia_unidad_alternativa_unidad_conteo,
		equivalencia_unidad_estandar_unidad_conteo,
		equivalencia_unidad_proveedor_unidad_conteo,
		unidad_conteo,
		unidad_proveedor_internacional,
		codigo_sunat,
		exonerado_igv,

		estado,

      imagen,
	  fecha_modificacion

	   FROM PRODUCTO p
	where p.sku_proveedor LIKE '%'+@skuProveedor+'%' 
		  and p.descripcion LIKE '%'+@descripcion+'%' 
		  and (@familia LIKE 'Todas' OR p.familia LIKE @familia) 
		  and (@proveedor LIKE 'Todos' OR p.proveedor LIKE @proveedor) 
		  and (@tipo = 0 OR tipo = @tipo) 
		  and (p.estado = @estado OR @estado = -1);
END
ELSE
BEGIN 

SELECT 
	 id_producto,
		precio,
		costo,
		precio_provincia,
		sku,
		sku_proveedor,
		descripcion,
		familia,
		proveedor,
		unidad,
		unidad_alternativa,

		equivalencia,
		equivalencia_proveedor,
		moneda_compra,
		moneda_venta,
		unidad_proveedor,
		unidad_estandar_internacional,
		unidad_alternativa_internacional,
		inafecto,
		tipo,

		costo_original,
		precio_original,
		precio_provincia_original,
		tipo_cambio,
		equivalencia_unidad_alternativa_unidad_conteo,
		equivalencia_unidad_estandar_unidad_conteo,
		equivalencia_unidad_proveedor_unidad_conteo,
		unidad_conteo,
		unidad_proveedor_internacional,
		codigo_sunat,
		exonerado_igv,

		estado,

      imagen,
	  fecha_modificacion
	   FROM PRODUCTO p
	where p.sku LIKE @sku;
	
END
END


