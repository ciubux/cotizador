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






/* **** 2 **** */
ALTER PROCEDURE [dbo].[pi_producto] 
@idUsuario uniqueidentifier,
@sku  varchar(100),
@descripcion  varchar(500),
@skuProveedor  varchar(100),
@estado  int,
@imagen image,
@monedaVenta varchar(1),
@monedaCompra varchar(1),
@precioOriginal numeric(18,2),
@precioProvinciaOriginal numeric(18,2),
@costoOriginal numeric(18,2),
@precio numeric(18,2),
@precioProvincia numeric(18,2),
@costo numeric(18,2),
@tipoCambio numeric(18,2),
@familia varchar(200),
@proveedor varchar(10),
@unidad varchar(200),
@unidadAlternativa varchar(200),
@unidadAlternativaInternacional varchar(200),
@equivalencia int,
@unidadProveedor varchar(300),
@equivalenciaProveedor int,
@unidadConteo varchar(300),
@unidadProveedorInternacional varchar(300),
@unidadEstandarInternacional varchar(3),
@equivalenciaUnidadAlternativaUnidadConteo int,
@equivalenciaUnidadEstandarUnidadConteo int,
@equivalenciaUnidadProveedorUnidadConteo int,
@codigoSunat varchar(8),
@exoneradoIgv smallint,
@inafecto smallint,
@fechaInicioVigencia date,
@tipo int,
@esCargaMasiva smallint,
@newId uniqueidentifier OUTPUT 

AS
BEGIN

SET NOCOUNT ON
SET @newId = NEWID();


	INSERT INTO PRODUCTO
           (id_producto
			,sku 
			,fecha_ingreso
			,fecha_fin
			,descripcion
			,sku_proveedor
			,estado
			,imagen
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
			,tipo
			,carga_masiva
			,fecha_inicio_vigencia
			,moneda_compra
			,moneda_venta
			,unidad_alternativa_internacional
			,costo_original
			,precio_original
			,precio_provincia_original
			,tipo_cambio
			,equivalencia_unidad_alternativa_unidad_conteo
			,equivalencia_unidad_estandar_unidad_conteo
			,equivalencia_unidad_proveedor_unidad_conteo
			,unidad_conteo
			,unidad_proveedor_internacional
			,codigo_sunat
		   )
     VALUES
           (@newId,
		    @sku,
			GETDATE(),
			DATEADD(mm, 6, GETDATE()),
			@descripcion,
			@skuProveedor,
			@estado,
			@imagen,
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
			@inafecto,
			@tipo,
			@esCargaMasiva,
			@fechaInicioVigencia,
			@monedaCompra,
			@monedaVenta,
			@unidadAlternativaInternacional,
			@costoOriginal,
			@precioOriginal,
			@precioProvinciaOriginal,
			@tipoCambio,
			@equivalenciaUnidadAlternativaUnidadConteo,
			@equivalenciaUnidadEstandarUnidadConteo,
			@equivalenciaUnidadProveedorUnidadConteo,
			@unidadConteo,
			@unidadProveedorInternacional,
			@codigoSunat
			);

END




/* **** 3 **** */
ALTER PROCEDURE [dbo].[pu_producto] 

@idProducto uniqueidentifier,
@idUsuario uniqueidentifier,
@sku  varchar(100),
@descripcion  varchar(500),
@skuProveedor  varchar(100),
@estado  int,
@imagen image,
@monedaVenta varchar(1),
@monedaCompra varchar(1),
@precioOriginal numeric(18,2),
@precioProvinciaOriginal numeric(18,2),
@costoOriginal numeric(18,2),
@precio numeric(18,2),
@precioProvincia numeric(18,2),
@costo numeric(18,2),
@tipoCambio numeric(18,2),
@familia varchar(200),
@proveedor varchar(10),
@unidad varchar(200),
@unidadAlternativa varchar(200),
@unidadAlternativaInternacional varchar(200),
@equivalencia int,
@unidadProveedor varchar(300),
@equivalenciaProveedor int,
@unidadConteo varchar(300),
@unidadProveedorInternacional varchar(300),
@unidadEstandarInternacional varchar(3),
@equivalenciaUnidadAlternativaUnidadConteo int,
@equivalenciaUnidadEstandarUnidadConteo int,
@equivalenciaUnidadProveedorUnidadConteo int,
@codigoSunat varchar(8),
@exoneradoIgv smallint,
@inafecto smallint,
@fechaInicioVigencia date,
@tipo int,
@esCargaMasiva smallint

	

AS
BEGIN

	UPDATE PRODUCTO  
	SET sku = @sku 
		,descripcion = @descripcion
		,sku_proveedor = @skuProveedor
		,estado = @estado
		,imagen = @imagen
		,moneda_compra = @monedaCompra
		,moneda_venta = @monedaVenta
		,costo_original = @costoOriginal 
		,precio_original = @precioOriginal
		,precio_provincia_original = @precioProvinciaOriginal
		,precio = @precio
		,precio_provincia = @precioProvincia
		,costo = @costo
		,tipo_cambio = @tipoCambio
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
		,unidad_alternativa_internacional = @unidadAlternativaInternacional
		,unidad_conteo = @unidadConteo
		,unidad_proveedor_internacional = @unidadProveedorInternacional
		,equivalencia_unidad_alternativa_unidad_conteo = @equivalenciaUnidadAlternativaUnidadConteo
		,equivalencia_unidad_estandar_unidad_conteo = @equivalenciaUnidadEstandarUnidadConteo
		,equivalencia_unidad_proveedor_unidad_conteo = @equivalenciaUnidadProveedorUnidadConteo
		,exonerado_igv = @exoneradoIgv
		,inafecto = @inafecto
		,fecha_inicio_vigencia = @fechaInicioVigencia
		,tipo = @tipo
		,codigo_sunat = @codigoSunat
		,carga_masiva = @esCargaMasiva
	WHERE id_producto like @idProducto;
	
END





/* **** 4 **** */
ALTER PROCEDURE [dbo].[ps_producto]
@idProducto uniqueIdentifier

AS
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
		usuario_creacion,
		fecha_creacion,
		usuario_modificacion,
		fecha_modificacion,

		imagen,
		estado
	FROM PRODUCTO p
	where p.id_producto = @idProducto;
END




/* **** 5 **** */
CREATE PROCEDURE [dbo].[ps_allProductoId]
@sku varchar(10)
AS
BEGIN

SELECT id_producto FROM PRODUCTO where sku = @sku;

END



/* **** 6 **** */
CREATE PROCEDURE [dbo].[pi_cambio_dato_programado_forprocedures] 

@idUsuario uniqueidentifier,
@nombreCatalogoTabla varchar(250),
@nombreCatalogoCampo varchar(250),
@persisteCambio smallint,
@idRegistro varchar(40),
@valor varchar(1000),
@fechaInicioVigencia date

AS 
BEGIN 

DECLARE @newId uniqueidentifier;

SET NOCOUNT ON
SET @newId = NEWID();
DECLARE @idCatalogoTabla int;
DECLARE @idCatalogoCampo int;

SET @idCatalogoTabla = 0;

SELECT @idCatalogoTabla = tab.id_catalogo_Tabla, @idCatalogoCampo = camp.id_catalogo_campo 
FROM CATALOGO_CAMPO camp 
INNER JOIN CATALOGO_TABLA tab ON tab.id_catalogo_Tabla = camp.id_catalogo_tabla 
WHERE camp.nombre like @nombreCatalogoCampo and tab.nombre like @nombreCatalogoTabla AND camp.estado = 1; 

IF @idCatalogoTabla > 0  
BEGIN 
	INSERT INTO CAMBIO_PROGRAMADO
           (id_cambio_programado
		   ,id_catalogo_tabla
           ,id_catalogo_campo
		   ,id_registro
           ,valor
		   ,persiste_cambio
           ,estado
		   ,fecha_inicio_vigencia
		   ,usuario_modificacion
		   ,fecha_modificacion
		   )
     VALUES
           (@newId,
		    @idCatalogoTabla,
		    @idCatalogoCampo,
			@idRegistro,
            @valor,
			@persisteCambio,
            1, 
			@fechaInicioVigencia,
			@idUsuario,
			GETDATE()
			);


	UPDATE CAMBIO_PROGRAMADO
	SET estado = 0
	WHERE id_catalogo_campo = @idCatalogoCampo and fecha_inicio_vigencia = @fechaInicioVigencia and id_registro = @idRegistro and not id_cambio_programado = @newId;
END 


END




/* **** 7 **** */
CREATE PROCEDURE pp_actualiza_tipo_cambio_producto
@aplicaCosto smallint, 
@aplicaPrecio smallint, 
@aplicaPrecioProvincia smallint, 
@tipoCambio decimal(6,4), 
@idUsuario uniqueidentifier,
@fechaInicioVigencia date

AS
BEGIN


DECLARE @c_id_producto uniqueidentifier
DECLARE @c_fiv date  
DECLARE @c_co numeric(18,2)
DECLARE @c_po numeric(18,2)
DECLARE @c_ppo numeric(18,2)
DECLARE @c_ep int

DECLARE @n_costo numeric(18,2)
DECLARE @n_precio numeric(18,2)
DECLARE @n_precioP numeric(18,2)
declare @tempPrice as varchar(20)

DECLARE db_cursor CURSOR FOR 
SELECT id_producto , fecha_inicio_vigencia, costo_original, precio_original, precio_provincia_original, equivalencia_proveedor FROM producto 
WHERE (moneda_venta = 'D' AND (@aplicaPrecioProvincia = 1 OR @aplicaPrecio = 1)) OR (moneda_compra = 'D' AND @aplicaCosto = 1);


OPEN db_cursor  
FETCH NEXT FROM db_cursor INTO @c_id_producto, @c_fiv, @c_co, @c_po, @c_ppo, @c_ep

WHILE @@FETCH_STATUS = 0  
BEGIN  
    SET @n_costo = 0
	SET @n_precio = 0
	SET @n_precioP = 0

	IF @aplicaCosto = 1 
	BEGIN 
		SET @n_costo = @c_co
		IF @c_ep > 0 
		BEGIN
			SET @n_costo = @c_co / @c_ep
		END
		SET @n_costo = @n_costo * @tipoCambio
	END

	IF @aplicaPrecio = 1 
	BEGIN 
		SET @n_precio = @c_po * @tipoCambio
	END

	IF @aplicaPrecioProvincia = 1 
	BEGIN 
		SET @n_precioP = @c_ppo * @tipoCambio
	END

	
	IF @c_fiv IS NULL OR @fechaInicioVigencia >= @c_fiv 
	BEGIN 
		-- Registrar en log programado
		IF @aplicaCosto = 1 
		BEGIN
			set @tempPrice = cast(@n_costo as varchar(20));
			EXEC  pi_cambio_dato_programado_forprocedures  @idUsuario, 'PRODUCTO', 'costo', 1, @c_id_producto,  @tempPrice, @fechaInicioVigencia ;
		END 

		IF @aplicaPrecio = 1 
		BEGIN
			set @tempPrice = cast(@n_precio as varchar(20));
			EXEC  pi_cambio_dato_programado_forprocedures  @idUsuario, 'PRODUCTO', 'precio', 1, @c_id_producto,  @tempPrice, @fechaInicioVigencia ;
		END 

		IF @aplicaPrecioProvincia = 1 
		BEGIN
			set @tempPrice = cast(@n_precioP as varchar(20));
			EXEC  pi_cambio_dato_programado_forprocedures  @idUsuario, 'PRODUCTO', 'precio_provincia', 1, @c_id_producto,  @tempPrice, @fechaInicioVigencia ;
		END 

		set @tempPrice = cast(@tipoCambio as varchar(20));
		EXEC  pi_cambio_dato_programado_forprocedures  @idUsuario, 'PRODUCTO', 'tipo_cambio', 1, @c_id_producto,  @tempPrice, @fechaInicioVigencia ;
	END 
	ELSE 
	BEGIN
		-- Registrar log anterior
		IF @aplicaCosto = 1 
		BEGIN
			set @tempPrice = cast(@n_costo as varchar(20));
			EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'costo', @c_id_producto,  @tempPrice, @fechaInicioVigencia ;
		END 

		IF @aplicaPrecio = 1 
		BEGIN
			set @tempPrice = cast(@n_precio as varchar(20));
			EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'precio', @c_id_producto,  @tempPrice, @fechaInicioVigencia ;
		END 

		IF @aplicaPrecioProvincia = 1 
		BEGIN
			set @tempPrice = cast(@n_precioP as varchar(20));
			EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'precio_provincia', @c_id_producto,  @tempPrice, @fechaInicioVigencia ;
		END 

		set @tempPrice = cast(@tipoCambio as varchar(20));
		EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'tipo_cambio', @c_id_producto,  @tempPrice, @fechaInicioVigencia ;
	END

	FETCH NEXT FROM db_cursor INTO @c_id_producto, @c_fiv, @c_co, @c_po, @c_ppo, @c_ep
END 

CLOSE db_cursor  
DEALLOCATE db_cursor 


END
 
 
 
 
/* **** 6 **** */
ALTER TRIGGER ti_producto ON PRODUCTO
AFTER INSERT AS
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
DECLARE @monedaCompra varchar(1);
DECLARE @monedaVenta varchar(1);
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

DECLARE @fechaInicioVigencia date;
DECLARE @cargaMasiva smallint;

select @idUsuario = usuario_modificacion, @idRegistro = id_producto, 
     @sku = sku, @descripcion = descripcion,  @fechaIngreso = fecha_ingreso, 
	 @fechaFin = fecha_fin, @skuProveedor = sku_proveedor, @estado = estado, @precio = precio,
	 @precioProvincia = precio_provincia, @costo = costo, @familia = familia, @clase = clase,
	 @marca = marca, @proveedor = proveedor, @unidad = unidad, @unidadAlternativa = unidad_alternativa,
	 @equivalencia = equivalencia, @unidadProveedor = unidad_proveedor, @equivalenciaProveedor = equivalencia_proveedor, 
	 @monedaCompra = moneda_compra, @monedaVenta = moneda_compra, @costoOriginal = costo_original,
	 @precioOriginal = precio_original, @precioProvinciaOriginal = precio_provincia_original, @unidadConteo = unidad_conteo,
	 @unidadEstandarInternacional = unidad_estandar_internacional, @unidadAlternativaInternacional = unidad_alternativa_internacional, 
	 @equivalenciaUnidadConteoEstandar = equivalencia_unidad_conteo_estandar, @equivalenciaUnidadConteoAlternativa = equivalencia_unidad_conteo_alternativa,
	 @exoneradoIgv = exonerado_igv, @inafecto = inafecto, @tipo = tipo,
	 @fechaInicioVigencia = fecha_inicio_vigencia, @cargaMasiva = carga_masiva 
from INSERTED;

IF @cargaMasiva = 0 
BEGIN
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'sku', @idRegistro, @sku, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'descripcion', @idRegistro, @descripcion, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'fecha_ingreso', @idRegistro, @fechaIngreso, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'fecha_fin', @idRegistro, @fechaFin, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'sku_proveedor', @idRegistro, @skuProveedor, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'estado', @idRegistro, @estado, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'precio', @idRegistro, @precio, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'precio_provincia', @idRegistro, @precioProvincia, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'costo', @idRegistro, @costo, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'familia', @idRegistro, @familia, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'clase', @idRegistro, @clase, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'marca', @idRegistro, @marca, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'proveedor', @idRegistro, @proveedor, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad', @idRegistro, @unidad, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad_alternativa', @idRegistro, @unidadAlternativa, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'equivalencia', @idRegistro, @equivalencia, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad_proveedor', @idRegistro, @unidadProveedor, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'equivalencia_proveedor', @idRegistro, @equivalenciaProveedor, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'moneda_compra', @idRegistro, @monedaCompra, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'moneda_venta', @idRegistro, @monedaVenta, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'costo_original', @idRegistro, @costoOriginal, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'precio_original', @idRegistro, @precioOriginal, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'precio_provincia_original', @idRegistro, @precioProvinciaOriginal, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad_conteo', @idRegistro, @unidadConteo, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad_estandar_internacional', @idRegistro, @unidadEstandarInternacional, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'unidad_alternativa_internacional', @idRegistro, @unidadAlternativaInternacional, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'equivalencia_unidad_conteo_estandar', @idRegistro, @equivalenciaUnidadConteoEstandar, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'equivalencia_unidad_conteo_alternativa', @idRegistro, @equivalenciaUnidadConteoAlternativa, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'exonerado_igv', @idRegistro, @exoneradoIgv, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'inafecto', @idRegistro, @inafecto, @fechaInicioVigencia ;
	EXEC  pi_cambio_dato  @idUsuario, 'PRODUCTO', 'tipo', @idRegistro, @tipo, @fechaInicioVigencia ;
END


END
GO