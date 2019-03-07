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
	[fecha_creacion] [datetime] NOT NULL,
	[usuario_creacion] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_CANASTA_CLIENTE_PRODUCTO] UNIQUE NONCLUSTERED 
(
	[id_cliente] ASC,
	[id_producto] ASC
));





/* **** 6 **** */
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



/* **** 9 **** */
CREATE PROCEDURE ps_getclientesruc_allsearch
@textoBusqueda varchar(50)

AS
BEGIN

SELECT DISTINCT 
   REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(razon_social_sunat, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  as razon_social ,
   ruc
FROM CLIENTE c
WHERE estado > 0
AND 
ruc LIKE '%'+@textoBusqueda+'%' 

END




/* **** 10 **** */
CREATE PROCEDURE [dbo].[ps_clientes_ruc] 
@ruc varchar(20) 
AS
BEGIN

SELECT cl.id_cliente, cl.codigo, cl.razon_social,
cl.nombre_comercial, cl.contacto1, cl.telefono_contacto1, cl.email_contacto1, cl.contacto2, cl.ruc,
cl.domicilio_legal, 
/*Si el cliente no tiene correo entonces se obtiene de algún pedido que tenga correo*/
CASE cl.correo_envio_factura WHEN '' THEN 
(SELECT TOP 1 correo_contacto_pedido FROM PEDIDO where id_cliente = cl.id_cliente
AND correo_contacto_pedido IS NOT NULL AND correo_contacto_pedido NOT IN ( '','.') )
ELSE cl.correo_envio_factura END AS correo_envio_factura, 

cl.razon_social_sunat, cl.nombre_comercial_sunat, 
cl.direccion_domicilio_legal_sunat, cl.estado_contribuyente_sunat, 
cl.condicion_contribuyente_sunat,
ub.codigo as codigo_ubigeo,
ub.provincia, ub.departamento, ub.distrito, cl.plazo_credito,

cl.forma_pago_factura, 
cl.sede_principal, 
cl.negociacion_multiregional, 
cl.id_ciudad,
ci.nombre as ciudad_nombre,
cl.tipo_documento,
/*PLAZO CREDITO*/
cl.plazo_credito_solicitado, --plazo credito aprobado
cl.tipo_pago_factura, --plazo credito aprobado
cl.sobre_plazo,
/*MONTO CREDITO*/
cl.credito_solicitado,
cl.credito_aprobado,
cl.sobre_giro, 
/*FLAG VENDEDORES*/
cl.vendedores_asignados,

/*Turnos entrega*/
cl.hora_inicio_primer_turno_entrega,
cl.hora_fin_primer_turno_entrega,
cl.hora_inicio_segundo_turno_entrega,
cl.hora_fin_segundo_turno_entrega,

--VENDEDORES,
verc.id_vendedor as responsable_comercial_id_vendedor,
verc.codigo as responsable_comercial_codigo,
verc.descripcion as responsable_comercial_descripcion,

vesc.id_vendedor as supervisor_comercial_id_vendedor,
vesc.codigo as supervisor_comercial_codigo,
vesc.descripcion as supervisor_comercial_descripcion,

veasc.id_vendedor as asistente_servicio_cliente_id_vendedor,
veasc.codigo as asistente_servicio_cliente_codigo,
veasc.descripcion as asistente_servicio_cliente_descripcion,

cl.observaciones_credito, 
cl.observaciones, 
cl.bloqueado,

cl.pertenece_canal_multiregional,
cl.pertenece_canal_lima,
cl.pertenece_canal_provincia,
cl.pertenece_canal_pcp,
cl.pertenece_canal_ordon,
cl.es_sub_distribuidor,
cl.observacion_horario_entrega,
cl.habilitado_negociacion_grupal,

cl.id_subdistribuidor,
sub.nombre nombre_subdistribuidor, 
sub.codigo codigo_subdistribuidor, 
cl.id_origen,
ori.nombre nombre_origen, 
ori.codigo codigo_origen, 

clgr.id_grupo_cliente ,
gr.codigo as codigo_grupo_cliente,
gr.grupo as grupo_nombre

FROM CLIENTE AS cl 
INNER JOIN CIUDAD AS ci ON cl.id_ciudad = ci.id_ciudad
LEFT JOIN UBIGEO AS ub ON cl.ubigeo = ub.codigo
LEFT JOIN VENDEDOR AS verc ON cl.id_responsable_comercial = verc.id_vendedor
LEFT JOIN VENDEDOR AS vesc ON cl.id_supervisor_comercial = vesc.id_vendedor
LEFT JOIN VENDEDOR AS veasc ON cl.id_asistente_servicio_cliente = veasc.id_vendedor
LEFT JOIN CLIENTE_GRUPO_CLIENTE AS clgr ON clgr.id_cliente = cl.id_cliente
LEFT JOIN GRUPO_CLIENTE AS gr ON gr.id_grupo_cliente = clgr.id_grupo_cliente 
LEFT JOIN SUBDISTRIBUIDOR AS sub ON sub.id_subdistribuidor = cl.id_subdistribuidor 
LEFT JOIN ORIGEN AS ori ON ori.id_origen = cl.id_origen 
WHERE cl.estado > 0 AND cl.ruc like '%' + @ruc + '%';


END






/* **** 11 **** */
ALTER PROCEDURE [dbo].[pi_grupoCliente] 
@idUsuario uniqueidentifier,
@nombre  varchar(200),
@contacto  varchar(100),
@telefonoContacto varchar(50),
@emailContacto varchar(50),
@idCiudad uniqueidentifier,
/*Plazo credito*/
@plazoCreditoSolicitado int,
@plazoCreditoAprobado int,
@sobrePlazo int, 
/*Monto Crédito*/
@creditoSolicitado decimal(12,2),
@creditoAprobado decimal(12,2),
@sobreGiro decimal(12,2),

@observacionesCredito varchar(1000),
@observaciones varchar(1000),
@codigo VARCHAR(4),

@newId int OUTPUT
AS
BEGIN TRAN

INSERT INTO GRUPO_CLIENTE
           (grupo
           ,[contacto]
		   ,codigo
		   ,telefono_contacto
		   ,email_contacto
		   ,[id_ciudad]
		   ,estado
		   ,[usuario_creacion]
		   ,[fecha_creacion]
		   ,[usuario_modificacion]
		   ,[fecha_modificacion],
		   /*Plazo credito*/
			plazo_credito_solicitado,
			plazo_credito_aprobado,
			sobre_plazo, 
			/*Monto Crédito*/
			credito_solicitado,
			credito_aprobado,
			sobre_giro, 

			observaciones_credito,
			observaciones
		   )
     VALUES
           (
		    @nombre,
		    @contacto,
			@codigo,
			@telefonoContacto,
            @emailContacto,
			@idCiudad,
			1,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE(),		
			@plazoCreditoSolicitado,
			@plazoCreditoAprobado,
			@sobrePlazo, 
			/*Monto Crédito*/
			@creditoSolicitado,
			@creditoAprobado,
			@sobreGiro, 		
			@observacionesCredito,
			@observaciones
			);

SET NOCOUNT ON
	SET @newId = SCOPE_IDENTITY();

COMMIT













/* ****  12 **** */
CREATE TABLE [dbo].[CANASTA_GRUPO_CLIENTE_PRODUCTO](
	[id_grupo_cliente] [int] NOT NULL,
	[id_producto] [uniqueidentifier] NOT NULL,
	[posicion] [int] NULL,
	[estado] [int] NOT NULL,
	[fecha_creacion] [datetime] NOT NULL,
	[usuario_creacion] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_CANASTA_GRUPO_CLIENTE_PRODUCTO] UNIQUE NONCLUSTERED 
(
	[id_grupo_cliente] ASC,
	[id_producto] ASC
));





/* ****  13 **** */
ALTER PROCEDURE ps_productosVigentesGrupo 
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
		gclp.estado estado_canasta,
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
	LEFT JOIN CANASTA_GRUPO_CLIENTE_PRODUCTO gclp ON gclp.id_grupo_cliente = pc.id_grupo_cliente AND gclp.id_producto = p.id_producto
	WHERE pc.id_grupo_cliente = @idGrupoCliente
	AND pc.fecha_inicio_vigencia IS NOT NULL 
	AND pc.estado = 1
	AND pc.unidad IS NOT NULL) SQuery 
	
	where RowNumber = 1;

END









/* **** 14 **** */
CREATE PROCEDURE [dbo].[pi_canastaGrupoClienteProducto] 
@idProducto uniqueidentifier,
@idGrupoCliente int,
@idUsuario uniqueidentifier

AS
BEGIN


	INSERT INTO CANASTA_GRUPO_CLIENTE_PRODUCTO
           (id_producto
			,id_grupo_cliente 
			,estado
			,usuario_creacion
			,fecha_creacion
		   )
     VALUES
           (@idProducto,
		    @idGrupoCliente,
		    1,
			@idUsuario,
			GETDATE()
			);

END





/* **** 15 **** */
CREATE PROCEDURE pd_canastaGrupoClienteProducto 
@idProducto uniqueidentifier,
@idGrupoCliente int

AS
BEGIN


	DELETE FROM CANASTA_GRUPO_CLIENTE_PRODUCTO
    WHERE id_producto = @idProducto AND id_grupo_cliente = @idGrupoCliente;

END






/* **** 16 **** */
ALTER PROCEDURE [dbo].[ps_cliente] 
@idCliente uniqueidentifier 
AS
BEGIN

SELECT cl.id_cliente, cl.codigo, cl.razon_social,
cl.nombre_comercial, cl.contacto1, cl.telefono_contacto1, cl.email_contacto1, cl.contacto2, cl.ruc,
cl.domicilio_legal, 
/*Si el cliente no tiene correo entonces se obtiene de algún pedido que tenga correo*/
CASE cl.correo_envio_factura WHEN '' THEN 
(SELECT TOP 1 correo_contacto_pedido FROM PEDIDO where id_cliente = cl.id_cliente
AND correo_contacto_pedido IS NOT NULL AND correo_contacto_pedido NOT IN ( '','.') )
ELSE cl.correo_envio_factura END AS correo_envio_factura, 

cl.razon_social_sunat, cl.nombre_comercial_sunat, 
cl.direccion_domicilio_legal_sunat, cl.estado_contribuyente_sunat, 
cl.condicion_contribuyente_sunat,
ub.codigo as codigo_ubigeo,
ub.provincia, ub.departamento, ub.distrito, cl.plazo_credito,

cl.forma_pago_factura, 
cl.sede_principal, 
cl.negociacion_multiregional, 
cl.id_ciudad,
ci.nombre as ciudad_nombre,
cl.tipo_documento,
/*PLAZO CREDITO*/
cl.plazo_credito_solicitado, --plazo credito aprobado
cl.tipo_pago_factura, --plazo credito aprobado
cl.sobre_plazo,
/*MONTO CREDITO*/
cl.credito_solicitado,
cl.credito_aprobado,
cl.sobre_giro, 
/*FLAG VENDEDORES*/
cl.vendedores_asignados,

/*Turnos entrega*/
cl.hora_inicio_primer_turno_entrega,
cl.hora_fin_primer_turno_entrega,
cl.hora_inicio_segundo_turno_entrega,
cl.hora_fin_segundo_turno_entrega,

--VENDEDORES,
verc.id_vendedor as responsable_comercial_id_vendedor,
verc.codigo as responsable_comercial_codigo,
verc.descripcion as responsable_comercial_descripcion,
verc.id_usuario as responsable_comercial_id_usuario,

vesc.id_vendedor as supervisor_comercial_id_vendedor,
vesc.codigo as supervisor_comercial_codigo,
vesc.descripcion as supervisor_comercial_descripcion,
vesc.id_usuario as supervisor_comercial_id_usuario,

veasc.id_vendedor as asistente_servicio_cliente_id_vendedor,
veasc.codigo as asistente_servicio_cliente_codigo,
veasc.descripcion as asistente_servicio_cliente_descripcion,
veasc.id_usuario as asistente_servicio_id_usuario,

cl.observaciones_credito, 
cl.observaciones, 
cl.bloqueado,

cl.pertenece_canal_multiregional,
cl.pertenece_canal_lima,
cl.pertenece_canal_provincia,
cl.pertenece_canal_pcp,
cl.pertenece_canal_ordon,
cl.es_sub_distribuidor,
cl.observacion_horario_entrega,
cl.habilitado_negociacion_grupal,

cl.id_subdistribuidor,
sub.nombre nombre_subdistribuidor, 
sub.codigo codigo_subdistribuidor, 
cl.id_origen,
ori.nombre nombre_origen, 
ori.codigo codigo_origen, 

clgr.id_grupo_cliente ,
gr.codigo as codigo_grupo_cliente,
gr.grupo as grupo_nombre

FROM CLIENTE AS cl 
INNER JOIN CIUDAD AS ci ON cl.id_ciudad = ci.id_ciudad
LEFT JOIN UBIGEO AS ub ON cl.ubigeo = ub.codigo
LEFT JOIN VENDEDOR AS verc ON cl.id_responsable_comercial = verc.id_vendedor
LEFT JOIN VENDEDOR AS vesc ON cl.id_supervisor_comercial = vesc.id_vendedor
LEFT JOIN VENDEDOR AS veasc ON cl.id_asistente_servicio_cliente = veasc.id_vendedor
LEFT JOIN CLIENTE_GRUPO_CLIENTE AS clgr ON clgr.id_cliente = cl.id_cliente
LEFT JOIN GRUPO_CLIENTE AS gr ON gr.id_grupo_cliente = clgr.id_grupo_cliente 
LEFT JOIN SUBDISTRIBUIDOR AS sub ON sub.id_subdistribuidor = cl.id_subdistribuidor 
LEFT JOIN ORIGEN AS ori ON ori.id_origen = cl.id_origen 
WHERE cl.estado > 0 AND cl.id_cliente = @idCliente 

SELECT  arch.id_archivo_adjunto,  nombre--, arch.adjunto,
FROM ARCHIVO_ADJUNTO arch
WHERE id_cliente = @idCliente
AND estado = 1
AND informacion_cliente = 'TRUE';

END
