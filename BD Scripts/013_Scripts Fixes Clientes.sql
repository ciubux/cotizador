
ALTER PROCEDURE [dbo].[ps_clientes] 
@codigo varchar(4),
@idCiudad uniqueIdentifier,
@textoBusqueda varchar(50),
@idResponsableComercial int,
@idSupervisorComercial int,
@idAsistenteServicioCliente int,
@sinPlazoCreditoAprobado int, 
@sinAsesorValidado int, 
@bloqueado int,
@idGrupoCliente int
AS
BEGIN

IF @codigo IS NULL OR @codigo = ''
BEGIN
	SELECT 
	cl.id_cliente, 
	cl.codigo,
	cl.sede_principal, 
	cl.negociacion_multiregional, 
	cl.pertenece_canal_multiregional,
	cl.pertenece_canal_lima,
	cl.pertenece_canal_provincia,
	cl.pertenece_canal_pcp,
	cl.pertenece_canal_ordon,
	cl.es_sub_distribuidor,
	ci.id_ciudad,
	ci.nombre as ciudad_nombre, 
	
	CASE cl.tipo_documento WHEN 6 
		THEN ISNULL(cl.razon_social_sunat,cl.razon_social)
	ELSE '' END razon_social_sunat,

	ISNULL(cl.nombre_comercial,'') nombre_comercial,
	cl.tipo_documento, 
	cl.ruc,
		--VENDEDORES,
	verc.codigo as responsable_comercial_codigo,
	ISNULL(verc.descripcion,'') as responsable_comercial_descripcion,

	vesc.codigo as supervisor_comercial_codigo,
	ISNULL(vesc.descripcion,'') as supervisor_comercial_descripcion,

	veasc.codigo as asistente_servicio_cliente_codigo,
	ISNULL(veasc.descripcion,'') as asistente_servicio_cliente_descripcion,

	cl.tipo_pago_factura, --plazo credito aprobado
	cl.credito_aprobado,
	cl.bloqueado,
	cgc.id_grupo_cliente,
	ISNULL(gc.grupo,'-') grupo
	FROM CLIENTE AS cl
	INNER JOIN CIUDAD AS ci ON cl.id_ciudad = ci.id_ciudad 
	--LEFT JOIN UBIGEO AS ub ON cl.ubigeo = ub.codigo
	LEFT JOIN VENDEDOR AS verc ON cl.id_responsable_comercial = verc.id_vendedor
	LEFT JOIN VENDEDOR AS vesc ON cl.id_supervisor_comercial = vesc.id_vendedor
	LEFT JOIN VENDEDOR AS veasc ON cl.id_asistente_servicio_cliente = veasc.id_vendedor
	LEFT JOIN CLIENTE_GRUPO_CLIENTE  AS cgc ON cl.id_cliente = cgc.id_cliente
	LEFT JOIN GRUPO_CLIENTE AS gc ON gc.id_grupo_cliente = cgc.id_grupo_cliente
	WHERE 
	(
	(REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(cl.nombre_comercial, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  LIKE '%'+@textoBusqueda+'%' OR
	cl.ruc LIKE '%'+@textoBusqueda+'%' OR
	REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(cl.razon_social, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  LIKE '%'+@textoBusqueda+'%'
	OR REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(cl.razon_social_sunat, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  LIKE '%'+@textoBusqueda+'%'
	)
	OR @textoBusqueda IS NULL
	OR @textoBusqueda = ''
	)
	AND (cl.id_responsable_comercial = @idResponsableComercial OR @idResponsableComercial = 0)
	AND (cl.id_supervisor_comercial = @idSupervisorComercial OR @idSupervisorComercial = 0)
	AND (cl.id_asistente_servicio_cliente = @idAsistenteServicioCliente OR @idAsistenteServicioCliente = 0)
	AND (cl.bloqueado = 1 OR @bloqueado = 0 )
	AND (@sinAsesorValidado = 0 OR cl.vendedores_asignados = 0) 
	AND cl.estado = 1
	AND (cl.id_ciudad = @idCiudad OR @idCiudad = '00000000-0000-0000-0000-000000000000')
	AND (@idGrupoCliente = 0 OR @idGrupoCliente = cgc.id_grupo_cliente)
	--FALTA sinPlazoCreditoAprobado

END
ELSE
BEGIN 

	SELECT 
	cl.id_cliente, 
	cl.codigo,
	ci.id_ciudad,
	cl.sede_principal, 
	cl.negociacion_multiregional, 
	cl.pertenece_canal_multiregional,
	cl.pertenece_canal_lima,
	cl.pertenece_canal_provincia,
	cl.pertenece_canal_pcp,
	cl.pertenece_canal_ordon,
	cl.es_sub_distribuidor,
	ci.nombre as ciudad_nombre, 
	
	CASE cl.tipo_documento WHEN 6 
		THEN cl.razon_social_sunat
		ELSE '' END razon_social_sunat,

	ISNULL(cl.nombre_comercial,'') nombre_comercial,
	cl.tipo_documento, 
	cl.ruc,
		--VENDEDORES,
	verc.codigo as responsable_comercial_codigo,
	ISNULL(verc.descripcion,'') as responsable_comercial_descripcion,

	vesc.codigo as supervisor_comercial_codigo,
	ISNULL(vesc.descripcion,'') as supervisor_comercial_descripcion,

	veasc.codigo as asistente_servicio_cliente_codigo,
	ISNULL(veasc.descripcion,'') as asistente_servicio_cliente_descripcion,

	cl.tipo_pago_factura, --plazo credito aprobado
	cl.credito_aprobado,
	cl.bloqueado,
	cgc.id_grupo_cliente,
	ISNULL(gc.grupo,'-') grupo
	FROM CLIENTE AS cl
	INNER JOIN CIUDAD AS ci ON cl.id_ciudad = ci.id_ciudad 
	--LEFT JOIN UBIGEO AS ub ON cl.ubigeo = ub.codigo
	LEFT JOIN VENDEDOR AS verc ON cl.id_responsable_comercial = verc.id_vendedor
	LEFT JOIN VENDEDOR AS vesc ON cl.id_supervisor_comercial = vesc.id_vendedor
	LEFT JOIN VENDEDOR AS veasc ON cl.id_asistente_servicio_cliente = veasc.id_vendedor
	LEFT JOIN CLIENTE_GRUPO_CLIENTE  AS cgc ON cl.id_cliente = cgc.id_cliente
	LEFT JOIN GRUPO_CLIENTE AS gc ON gc.id_grupo_cliente = cgc.id_grupo_cliente
	WHERE cl.codigo = @codigo AND cl.estado = 1;
END


END











ALTER PROCEDURE [dbo].[ps_productos]
@sku varchar(100),
@skuProveedor varchar(100),
@descripcion varchar(500),
@familia varchar(200),
@proveedor varchar(10),
@estado int
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
		  and p.estado = @estado;
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
		  and p.estado = @estado;
	
END
END






ALTER TABLE USUARIO  
ADD modifica_producto SMALLINT default 0;









/*Agrega modifica_producto */
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


crea_documentos_venta, 
crea_factura_consolidada_local,
crea_factura_consolidada_multiregional,
visualiza_guias_pendientes_facturacion,
administra_documentos_venta_lima, administra_documentos_venta_provincias,
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
realiza_carga_masiva_pedidos,
modifica_pedido_fecha_entrega_extendida,
bloquea_clientes,
modifica_negociacion_multiregional,
busca_sedes_grupo_cliente,
modifica_canales,
modifica_producto


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

SELECT id_producto, sku, descripcion FROM PRODUCTO 
WHERE tipo = 6 --(6 descuento)

END








ALTER PROCEDURE [dbo].[ps_producto]
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
	where p.id_producto = @idProducto;
END

/*UNIDAD ESTANDAR 
ES DESCUENTO (check) 
ES CARGO (check)*/


