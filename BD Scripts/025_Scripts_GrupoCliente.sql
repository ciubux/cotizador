/* **** 1 **** */
CREATE PROCEDURE ps_productosVigentesGrupo 
@idGrupoCliente int
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
	WHERE pc.id_grupo_cliente = @idGrupoCliente
	AND pc.fecha_inicio_vigencia IS NOT NULL 
	AND pc.estado = 1
	AND pc.unidad IS NOT NULL) SQuery 
	
	where RowNumber = 1;

END



/* **** 2 **** */
create PROCEDURE [dbo].[ps_getprecioGrupoClienteProducto] 
@idProducto uniqueidentifier ,
@idGrupoCliente int 
AS
BEGIN

SELECT pc.fecha_inicio_vigencia, pc.fecha_fin_vigencia, pc.unidad,
pc.precio_neto, pc.flete, pc.precio_unitario, c.codigo as numero_cotizacion
FROM  PRECIO_CLIENTE_PRODUCTO pc
LEFT JOIN COTIZACION c ON pc.id_cotizacion = c.id_cotizacion
WHERE pc.id_grupo_cliente = @idGrupoCliente  AND pc.id_producto = @idProducto
AND  pc.estado = 1 
 ORDER BY pc.fecha_inicio_vigencia desc, c.codigo desc	

END




/* **** 3 **** */
CREATE PROCEDURE ps_getClientesGrupo 
@idGrupoCliente int
AS
BEGIN

	SELECT 
	cl.id_cliente, 
	cl.codigo,
	ci.id_ciudad,
	ci.nombre as ciudad_nombre, 
	
	CASE cl.tipo_documento WHEN 6 
		THEN ISNULL(cl.razon_social_sunat,cl.razon_social)
	ELSE '' END razon_social_sunat,


	CASE cl.tipo_documento WHEN 1 
		THEN cl.razon_social
	WHEN 4
		THEN cl.razon_social
	ELSE ISNULL(cl.nombre_comercial,'')  END nombre_comercial,
	

	cl.tipo_documento, 
	cl.ruc
	
	FROM CLIENTE AS cl
	INNER JOIN CIUDAD AS ci ON cl.id_ciudad = ci.id_ciudad 
	LEFT JOIN GRUPO_CLIENTE AS gc ON gc.id_grupo_cliente = cl.id_grupo_cliente

	WHERE 
	 cl.estado > 0
	AND @idGrupoCliente = gc.id_grupo_cliente

END





/* **** 4 **** */
CREATE PROCEDURE ps_getclientes_allsearch
@textoBusqueda varchar(50)

AS
BEGIN

SELECT id_cliente,codigo,
 REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(nombre_comercial, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')   as nombre_comercial,
   REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(razon_social, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  as razon_social ,
  contacto1, contacto2, ruc
FROM CLIENTE c
WHERE estado > 0
AND (REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(nombre_comercial, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  LIKE '%'+@textoBusqueda+'%' OR
ruc LIKE '%'+@textoBusqueda+'%' OR
codigo LIKE '%'+@textoBusqueda+'%' OR
REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(razon_social, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  LIKE '%'+@textoBusqueda+'%')


END





/* ****  5 **** */
CREATE TABLE [dbo].[CANASTA_CLIENTE_PRODUCTO](
	[id_cliente] [uniqueidentifier] NOT NULL,
	[id_producto] [uniqueidentifier] NOT NULL,
	[posicion] [int] NULL,
	[estado] [int] NOT NULL,
	[usaurio_creacion] [uniqueidentifier] NOT NULL,
	[fecha_creacion] [datetime] NOT NULL,
	[usuario_creacion] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_CANASTA_CLIENTE_PRODUCTO] UNIQUE NONCLUSTERED 
(
	[id_cliente] ASC,
	[id_producto] ASC
));





/* **** 6 **** */
USE [cotizadormp]
GO
/****** Object:  StoredProcedure [dbo].[ps_productosVigentesCliente]    Script Date: 4/03/2019 6:10:18 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



ALTER PROCEDURE [dbo].[ps_productosVigentesCliente] 
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
		clp.estado estado_canasta,
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
	LEFT JOIN CANASTA_CLIENTE_PRODUCTO clp ON clp.id_cliente = pc.id_cliente AND clp.id_producto = p.id_producto
	WHERE pc.id_cliente = @idCliente
	AND pc.fecha_inicio_vigencia IS NOT NULL 
	AND pc.estado = 1
	AND pc.unidad IS NOT NULL) SQuery 
	
	where RowNumber = 1;

END






/* **** 7 **** */
CREATE PROCEDURE [dbo].[pi_canastaClienteProducto] 
@idProducto uniqueidentifier,
@idCliente uniqueidentifier,
@idUsuario uniqueidentifier

AS
BEGIN


	INSERT INTO CANASTA_CLIENTE_PRODUCTO
           (id_producto
			,id_cliente 
			,estado
			,usuario_creacion
			,fecha_creacion
		   )
     VALUES
           (@idProducto,
		    @idCliente,
		    1,
			@idUsuario,
			GETDATE()
			);

END





/* **** 8 **** */
CREATE PROCEDURE pd_canastaClienteProducto 
@idProducto uniqueidentifier,
@idCliente uniqueidentifier

AS
BEGIN


	DELETE FROM CANASTA_CLIENTE_PRODUCTO
    WHERE id_producto = @idProducto AND id_cliente = @idCliente;

END






