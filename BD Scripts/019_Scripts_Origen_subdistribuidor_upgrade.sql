/* **** 1 **** */
ALTER TABLE USUARIO  
ADD modifica_subdistribuidor SMALLINT,
 modifica_origen SMALLINT;
 
 
 
  
/* **** 2 **** */ 
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
modifica_subdistribuidor,
modifica_origen,
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





/* **** 3 **** */
ALTER PROCEDURE [dbo].[pi_subdistribuidor] 
@codigo varchar(20),
@nombre  varchar(500),
@estado  int,
@idUsuario uniqueidentifier,
@newId int OUTPUT 

AS
BEGIN

	INSERT INTO SUBDISTRIBUIDOR
           (codigo
			,nombre 
			,estado
			,usuario_creacion
			,fecha_creacion
			,usuario_modificacion
			,fecha_modificacion
		   )
     VALUES
           (@codigo,
			@nombre,
			@estado,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE()
			);

	SET NOCOUNT ON
	SET @newId = SCOPE_IDENTITY();

END




/* **** 4 **** */
ALTER PROCEDURE [dbo].[pi_origen] 
@codigo varchar(20),
@nombre  varchar(500),
@estado  int,
@idUsuario uniqueidentifier,
@newId int OUTPUT 

AS
BEGIN

	INSERT INTO ORIGEN
           (codigo
			,nombre 
			,estado
			,usuario_creacion
			,fecha_creacion
			,usuario_modificacion
			,fecha_modificacion
		   )
     VALUES
           (@codigo,
			@nombre,
			@estado,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE()
			);

	SET NOCOUNT ON
	SET @newId = SCOPE_IDENTITY();

END





/* **** 5 **** */
ALTER PROCEDURE [dbo].[pu_origen] 
@idOrigen int,
@codigo varchar(20),
@nombre  varchar(500),
@estado  int,
@idUsuario uniqueidentifier

AS
BEGIN

UPDATE ORIGEN  
	SET codigo = @codigo 
		,nombre = @nombre
		,estado = @estado
		,usuario_modificacion = @idUsuario
		,fecha_modificacion = GETDATE()
	WHERE id_origen like @idOrigen;

END







/* **** 6 **** */
ALTER PROCEDURE [dbo].[pu_subdistribuidor] 
@idSubDistribuidor int,
@codigo varchar(20),
@nombre  varchar(500),
@estado  int,
@idUsuario uniqueidentifier

AS
BEGIN

UPDATE SUBDISTRIBUIDOR  
	SET codigo = @codigo 
		,nombre = @nombre
		,estado = @estado
		,usuario_modificacion = @idUsuario
		,fecha_modificacion = GETDATE()
	WHERE id_subdistribuidor like @idSubDistribuidor;

END





