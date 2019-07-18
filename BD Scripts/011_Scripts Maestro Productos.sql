CREATE PROCEDURE ps_productosVigentesCliente 
@idCliente uniqueidentifier
AS
BEGIN

	--DETALLE DE COTIZACION
	SELECT 
	* FROM 
	(SELECT 
		--DATOS DEL PRODUCTO
		p.id_producto,
		REPLACE(p.sku,'"','''''') as  sku, 
		REPLACE(p.descripcion,'"','''''')  as  descripcion,
		p.proveedor,
		p.familia,
		p.sku_proveedor,
		p.imagen,
		--REVISAR PARA PROVINCIAS
		p.precio AS precio_sin_igv, 
		p.precio_provincia AS precio_provincia_sin_igv, 
		p.costo AS costo_sin_igv,
	
		--PRECIO REGISTRADOS DEL PRODUCTO
		pc.es_unidad_alternativa AS es_precio_alternativo,
		pc.unidad, 
		pc.precio_neto, 
		pc.flete, 
		pc.precio_unitario,  
		pc.equivalencia,
		pc.id_precio_cliente_producto,
		pc.fecha_inicio_vigencia,
		pc.fecha_fin_vigencia,
		pc.id_cliente,
		--pc.fecha_inicio_vigencia desc, co.codigo desc
	ROW_NUMBER() OVER(PARTITION BY p.id_producto, pc.id_cliente ORDER BY 
	pc.fecha_inicio_vigencia DESC, co.codigo desc) AS RowNumber
	FROM (SELECT * FROM 
	PRECIO_CLIENTE_PRODUCTO 
	WHERE fecha_inicio_vigencia IS NOT NULL 
	AND fecha_inicio_vigencia < GETDATE()
	AND (fecha_fin_vigencia is NULL OR fecha_fin_vigencia >= GETDATE())
	) pc
	INNER JOIN PRODUCTO p ON pc.id_producto = p.id_producto
	LEFT JOIN COTIZACION co ON pc.id_cotizacion = co.id_cotizacion
	WHERE pc.id_cliente = @idCliente
	AND pc.fecha_inicio_vigencia IS NOT NULL 
	AND pc.estado = 1
	AND pc.unidad IS NOT NULL) SQuery 
	
	where RowNumber = 1;

END








CREATE PROCEDURE [dbo].[ps_productos]
@sku varchar(100),
@skuProveedor varchar(100),
@descripcion varchar(500),
@familia varchar(200),
@proveedor varchar(10)

AS
BEGIN

IF @sku IS NULL OR @sku = ''
BEGIN
	SELECT 
	 id_producto
      ,sku
      ,descripcion
      ,sku_proveedor
      ,imagen
      ,precio
      ,precio_provincia
      ,costo
      ,familia
      ,proveedor
      ,unidad
      ,unidad_alternativa
      ,equivalencia
      ,unidad_proveedor
      ,equivalencia_proveedor
	   FROM PRODUCTO p
	where p.sku_proveedor LIKE '%'+@skuProveedor+'%' 
		  and p.descripcion LIKE '%'+@descripcion+'%' 
		  and (@familia LIKE 'Todas' OR p.familia LIKE @familia) 
		  and (@proveedor LIKE 'Todos' OR p.proveedor LIKE @proveedor) 
		  and p.estado = 1;
END
ELSE
BEGIN 

SELECT 
	 id_producto
      ,sku
      ,descripcion
      ,sku_proveedor
      ,imagen
      ,precio
      ,precio_provincia
      ,costo
      ,familia
      ,proveedor
      ,unidad
      ,unidad_alternativa
      ,equivalencia
      ,unidad_proveedor
      ,equivalencia_proveedor
	   FROM PRODUCTO p
	where p.sku LIKE @sku 
		  and p.estado = 1;
	
END
END



CREATE PROCEDURE [dbo].[ps_producto]
@idProducto uniqueIdentifier

AS
BEGIN
	SELECT 
	 id_producto
    ,sku 
    ,descripcion
	,sku_proveedor
	,imagen
	,estado
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
	FROM PRODUCTO p
	where p.id_producto = @idProducto and 
		  p.estado = 1;
END

/*UNIDAD ESTANDAR 
ES DESCUENTO (check) 
ES CARGO (check)*/







CREATE PROCEDURE [dbo].[pi_producto] 
@sku  varchar(100),
@descripcion  varchar(500),
@skuProveedor  varchar(100),
@estado  int,
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
		   )
     VALUES
           (@newId,
		    @sku,
			@descripcion,
			@skuProveedor,
			@estado,
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
			@inafecto
			);

END







CREATE PROCEDURE pu_producto 
@idProducto uniqueidentifier,
@idUsuario uniqueidentifier,
@sku  varchar(100),
@descripcion  varchar(500),
@skuProveedor  varchar(100),
@estado  int,
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
@inafecto smallint


AS
BEGIN

	UPDATE PRODUCTO  
	SET sku = @sku 
		,descripcion = @descripcion
		,sku_proveedor = @skuProveedor
		,estado = @estado
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
	WHERE id_producto like @idProducto;
	
END




