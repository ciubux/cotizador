/**************************** CREATE TABLE LOG_INICIO_SESION **********************************/
create table LOG_INICIO_SESION
(
id_log_inicio_session uniqueidentifier,
id_usuario uniqueidentifier,
fecha_login datetime,
ip_login varchar(100)
)

/**************************** ALTER ps_usuario - Recive parametro de ip del usuario **********************************/

ALTER PROCEDURE [dbo].[ps_usuario] 
@email varchar(50),
@password varchar(50),
@ip_login varchar(100)
AS
BEGIN

--DECLARE @aprueba_pedidos_lima smallint; 
--DECLARE @aprueba_pedidos smallint; 
DECLARE @id_usuario uniqueidentifier; 

/*
SELECT @id_usuario = id_usuario FROM USUARIO 
WHERE estado = 1
AND email = @email 
AND PWDCOMPARE ( @password,password )  = 1 ;
*/

SELECT @id_usuario = id_usuario FROM USUARIO 
WHERE estado = 1
AND email = @email 
AND ((PWDCOMPARE ( @password,password )  = 1) 
    OR 1 IN (
		select PWDCOMPARE ( @password, usa.password ) from USUARIO usa 
		INNER JOIN ROL_USUARIO rua on rua.id_usuario = usa.id_usuario AND rua.id_rol = 1
	) 
);



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
modifica_producto,
crea_cotizaciones_grupales,
aprueba_cotizaciones_grupales,
modifica_grupo_clientes,
visualiza_grupo_clientes, 
visualiza_clientes, 
visualiza_productos, 
visualiza_subdistribuidores, 
visualiza_origenes,
realiza_carga_masiva_clientes, 
realiza_carga_masiva_productos,
elimina_cotizaciones_aceptadas

FROM USUARIO 
WHERE estado = 1
AND email = @email 
AND id_usuario = @id_usuario; 
/* PWDCOMPARE ( @password,password )  = 1 ;*/

--PARAMETROS POR USUARIO
SELECT codigo, valor FROM PARAMETRO where estado = 1
UNION 
SELECT 'CPE_CABECERA_BE_ID' as codigo, CPE_CABECERA_BE_ID as valor FROM PARAMETROS_AMBIENTE_EOL
UNION 
SELECT 'CPE_CABECERA_BE_COD_GPO' as codigo, CPE_CABECERA_BE_COD_GPO as valor FROM PARAMETROS_AMBIENTE_EOL;


--USUARIOS A CARGO COTIZACION
IF (@id_usuario IS NOT NULL)
BEGIN
	SELECT distinct us.id_usuario,  us.nombre, es_provincia
	FROM USUARIO us
	left JOIN USUARIO_PERMISO up ON us.id_usuario = up.id_usuario
	left JOIN ROL_USUARIO ru ON ru.id_usuario = us.id_usuario
	left JOIN ROL_PERMISO rp ON rp.id_rol = ru.id_rol 
	left JOIN PERMISO pe ON pe.id_permiso = up.id_permiso OR pe.id_permiso = rp.id_permiso

	 LEFT JOIN CIUDAD ci ON us.id_ciudad = ci.id_ciudad
	WHERE us.estado = 1 AND us.id_usuario != '9936aba3-2085-4fa7-ae98-5531fea8ac31' 
	AND (NOT up.id_usuario IS NULL OR NOT rp.id_rol IS NULL)
	AND pe.codigo  IN ('P031','P012','P055','P054') 
	ORDER BY us.nombre
END

--USUARIOS A CARGO PEDIDO
IF (@id_usuario IS NOT NULL)
BEGIN
	SELECT distinct us.id_usuario, us.nombre, es_provincia
	FROM USUARIO us 
	INNER JOIN USUARIO_PERMISO up ON us.id_usuario = up.id_usuario
	INNER JOIN PERMISO pe ON pe.id_permiso = up.id_permiso
	LEFT JOIN CIUDAD ci ON us.id_ciudad = ci.id_ciudad
	WHERE us.estado = 1 --AND us.id_usuario != @id_usuario
	AND us.id_usuario NOT IN ('412ACDEE-FE20-4539-807C-D00CD71359D6')
	AND pe.codigo  IN ('P013','P032')
	ORDER BY us.nombre;
END


--USUARIOS A CARGO GUIAS
IF (@id_usuario IS NOT NULL)
BEGIN
	SELECT id_usuario, us.nombre, es_provincia
	FROM USUARIO us INNER JOIN 
	CIUDAD ci ON us.id_ciudad = ci.id_ciudad
	WHERE us.estado = 1 AND us.crea_guias = 1
	AND id_usuario != @id_usuario 
	ORDER BY us.nombre;
END

--USUARIOS A CARGO DOCUMENTOS VENTA
IF (@id_usuario IS NOT NULL)
BEGIN
	SELECT id_usuario, us.nombre, es_provincia
	FROM USUARIO us INNER JOIN 
	CIUDAD ci ON us.id_ciudad = ci.id_ciudad
	WHERE us.estado = 1 AND us.crea_documentos_venta = 1
	AND id_usuario != @id_usuario 
	ORDER BY us.nombre;
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
WHERE tipo = 5 --(5 descuento)

SELECT id_producto, sku, descripcion FROM PRODUCTO 
WHERE tipo = 4 --(4 cargos)


SELECT pe.id_permiso, pe.codigo,pe.descripcion_corta, pe.descripcion_larga,   
       CASE WHEN NOT up.id_usuario IS NULL THEN 1 ELSE 0 END as es_usuario, 
	   CASE WHEN NOT rp.id_rol IS NULL THEN 1 ELSE 0 END as es_rol 
FROM PERMISO PE
LEFT JOIN ROL_USUARIO ru ON ru.id_usuario = @id_usuario
LEFT JOIN ROL_PERMISO rp ON rp.id_rol = ru.id_rol AND pe.id_permiso = rp.id_permiso 
LEFT JOIN USUARIO_PERMISO up ON pe.id_permiso = up.id_permiso and up.id_usuario = @id_usuario and up.estado = 1
where NOT up.id_usuario IS NULL OR NOT rp.id_rol IS NULL 

if @ip_login is not  null
begin
insert into LOG_INICIO_SESION values(NEWID(),@id_usuario,dbo.getlocaldate(),@ip_login) 

 DELETE FROM LOG_INICIO_SESION
 WHERE id_log_inicio_session NOT IN (SELECT TOP 20 id_log_inicio_session FROM LOG_INICIO_SESION where id_usuario=@id_usuario order by fecha_login desc )
 and id_usuario=@id_usuario 
 end
END