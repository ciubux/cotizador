ALTER TABLE USUARIO  
ADD busca_sedes_grupo_cliente SMALLINT;



/* Agrega parametro @buscaSedesGrupoCliente */
ALTER PROCEDURE [dbo].[ps_pedidos] 

@numero bigint,
@numeroGrupo bigint,
@idCliente uniqueidentifier,
@idCiudad uniqueidentifier,
@idUsuario uniqueidentifier,
@idUsuarioBusqueda uniqueidentifier,
@numeroReferenciaCliente varchar(20),
@tipoPedido char(1),
@fechaCreacionDesde datetime,
@fechaCreacionHasta datetime, 
@fechaEntregaDesde datetime,
@fechaEntregaHasta datetime, 
@fechaProgramacionDesde datetime,
@fechaProgramacionHasta datetime, 
@tipo char(1),
@idGrupoCliente int,
@buscaSedesGrupoCliente bit,
@estado smallint,
@estadoCrediticio smallint
AS
BEGIN



DECLARE @aprobadorPedidosLima int;
DECLARE @aprobadorPedidosProvincias int;
DECLARE @EST_PEDIDO_EDICION int;
DECLARE @EST_PEDIDO_PEND_APROBACION int;
DECLARE @EST_PEDIDO_INGRESADO int;
DECLARE @EST_PEDIDO_DENEGADO int;
DECLARE @EST_PEDIDO_PROGRAMADO int;
DECLARE @EST_PEDIDO_ATENDIDO_PARCIALMENTE int;

SET @EST_PEDIDO_EDICION = 6;
SET @EST_PEDIDO_PEND_APROBACION = 0;
SET @EST_PEDIDO_INGRESADO = 1;
SET @EST_PEDIDO_DENEGADO = 2;
SET @EST_PEDIDO_PROGRAMADO = 3;
SET @EST_PEDIDO_ATENDIDO_PARCIALMENTE = 5;

if  @numero = 0 
BEGIN



	SELECT 
	--PEDIDO
	pe.numero as numero_pedido, pe.numero_grupo as numero_grupo_pedido,
    pe.id_pedido, pe.fecha_solicitud, pe.incluido_igv, 
	pe.igv, pe.total, ISNULL(pe.observaciones,'') observaciones,
	pe.fecha_creacion, pe.fecha_entrega_desde, pe.fecha_entrega_hasta, 
	pe.hora_entrega_desde, pe.hora_entrega_hasta, pe.hora_entrega_adicional_desde, pe.hora_entrega_adicional_hasta,
	pe.fecha_creacion as fecha_registro,
	pe.fecha_programacion,
	pe.stock_confirmado,
	REPLACE(COALESCE(pe.numero_referencia_cliente,''),'O/C N°','')as numero_referencia_cliente,
	--CLIENTE
	COALESCE( cl.codigo,'') as codigo,  cl.id_cliente,cl.razon_social, cl.ruc,
	 --USUARIO
	 us.nombre as nombre_usuario, us.id_usuario,
	 --CIUDAD
	ci.id_ciudad, ci.nombre as nombre_ciudad, 
	--SEGUIMIENTO
	sc.estado_pedido as estado_seguimiento,
	us2.nombre as usuario_seguimiento, sc.observacion as observacion_seguimiento,
	us2.id_usuario as id_usuario_seguimiento,
	--SEGUIMIENTO CREDITICIO
	scp.estado_pedido as estado_seguimiento_crediticio,
	us3.nombre as usuario_seguimiento_Crediticio, scp.observacion as observacion_seguimiento_Crediticio,
	us3.id_usuario as id_usuario_seguimiento_crediticio,
	--DETALLES
	--(SELECT max(porcentaje_descuento) from COTIZACION_DETALLE  WHERE estado = 1 AND id_cotizacion = pe.id_pedido) as maximo_porcentaje_descuento
	ub.codigo codigo_ubigeo,
	ISNULl(ub.distrito,'-') distrito

	 FROM PEDIDO as pe
	INNER JOIN CLIENTE AS cl ON pe.id_cliente = cl.id_cliente
	INNER JOIN CIUDAD AS ci ON pe.id_ciudad = ci.id_ciudad
	INNER JOIN USUARIO AS us on pe.usuario_creacion = us.id_usuario
	INNER JOIN SEGUIMIENTO_PEDIDO sc on sc.id_pedido = pe.id_pedido
	INNER JOIN USUARIO AS us2 on sc.id_usuario = us2.id_usuario
	INNER JOIN SEGUIMIENTO_CREDITICIO_PEDIDO scp on scp.id_pedido = pe.id_pedido
	INNER JOIN USUARIO AS us3 on scp.id_usuario = us3.id_usuario
	LEFT JOIN UBIGEO AS ub ON pe.ubigeo_entrega = ub.codigo 
	where   pe.fecha_creacion >= @fechaCreacionDesde 
	and pe.fecha_creacion <=  @fechaCreacionHasta


	and (pe.fecha_entrega_desde >= @fechaEntregaDesde or  @fechaEntregaDesde IS NULL)
	and (pe.fecha_entrega_desde <= @fechaEntregaHasta or  @fechaEntregaHasta IS NULL)
	and (pe.fecha_programacion >= @fechaProgramacionDesde OR @fechaProgramacionDesde IS NULL)
	and (pe.fecha_programacion <=  @fechaProgramacionHasta OR @fechaProgramacionHasta IS NULL)
	and (
	cl.id_cliente = @idCliente or 
	
	(@idCliente = '00000000-0000-0000-0000-000000000000' AND @idGrupoCliente <= 0
		AND (
				(	(SELECT es_cliente FROM USUARIO where id_usuario = @idUsuario ) = 0
					OR 
					(	(SELECT es_cliente FROM USUARIO where id_usuario = @idUsuario ) = 1
						AND cl.id_cliente in 
						(SELECT id_cliente FROM CLIENTE where ruc IN 
							(SELECT RUC FROM USUARIO_CLIENTE WHERE id_usuario =@idUsuario)
						)
					)
				)	
					
			)
	
	)
	OR 
	(@idGrupoCliente > 0 AND pe.id_cliente in (select id_cliente from CLIENTE_GRUPO_CLIENTE where id_grupo_cliente = @idGrupoCliente))
	)
	and (ci.id_ciudad = @idCiudad  OR (@buscaSedesGrupoCliente = 'TRUE' AND @idGrupoCliente > 0) 
		OR
		(@idCiudad = '00000000-0000-0000-0000-000000000000'
		AND @idUsuario IN (SELECT id_usuario FROM USUARIO where (aprueba_pedidos_lima = 1 AND aprueba_pedidos_provincias = 1) OR es_cliente = 1 ))
	)
	and (pe.usuario_creacion = @idUsuarioBusqueda OR @idUsuarioBusqueda = '00000000-0000-0000-0000-000000000000')
	

	/*
	and (ci.id_ciudad = @idCiudad 
		OR pe.usuario_creacion = @idUsuario
		OR
		(@idCiudad = '00000000-0000-0000-0000-000000000000'
		AND @idUsuario IN (SELECT id_usuario FROM USUARIO where aprueba_pedidos_lima = 1 AND aprueba_pedidos_provincias = 1 ))
	)*/

	AND 
		(sc.estado_pedido = @estado 
		OR @estado = -1  
		OR (@estado = -2 AND (sc.estado_pedido IN (@EST_PEDIDO_EDICION,
														@EST_PEDIDO_PEND_APROBACION,
														@EST_PEDIDO_INGRESADO,
														@EST_PEDIDO_DENEGADO,
														@EST_PEDIDO_PROGRAMADO,
														@EST_PEDIDO_ATENDIDO_PARCIALMENTE)
									OR 	pe.no_entregado = 1				
									)
		)
		
		
		)
	AND (
		scp.estado_pedido = @estadoCrediticio 
		OR @estadoCrediticio = -1	)
		
	AND (@numeroReferenciaCliente = '' OR 
		@numeroReferenciaCliente IS NULL OR 
		pe.numero_referencia_cliente LIKE '%'+@numeroReferenciaCliente+'%')
	AND sc.estado = 1 AND scp.estado = 1
	AND pe.estado = 1

	AND pe.tipo = @tipo
	AND (@tipoPedido = '0' OR  pe.tipo_pedido = @tipoPedido)

	order by pe.numero asc ;
	END
else
BEGIN
	SELECT 
	--PEDIDO
	pe.numero as numero_pedido, pe.numero_grupo as numero_grupo_pedido,
    pe.id_pedido, pe.fecha_solicitud, pe.incluido_igv, 
	pe.igv, pe.total, ISNULL(pe.observaciones,'') observaciones,
	pe.fecha_creacion, pe.fecha_entrega_desde, pe.fecha_entrega_hasta, 
	pe.hora_entrega_desde, pe.hora_entrega_hasta,
	pe.fecha_creacion as fecha_registro,
	pe.fecha_programacion,
	pe.stock_confirmado,
	REPLACE(COALESCE(pe.numero_referencia_cliente,''),'O/C N°','')as numero_referencia_cliente,
	--CLIENTE
	COALESCE( cl.codigo,'') as codigo,  cl.id_cliente,cl.razon_social, cl.ruc, 
	 --USUARIO
	 us.nombre as nombre_usuario, us.id_usuario,
	 --CIUDAD
	ci.id_ciudad, ci.nombre as nombre_ciudad, 
	--SEGUIMIENTO
	sc.estado_pedido as estado_seguimiento,
	us2.nombre as usuario_seguimiento, sc.observacion as observacion_seguimiento,
	us2.id_usuario as id_usuario_seguimiento,
	--SEGUIMIENTO CREDITICIO
	scp.estado_pedido as estado_seguimiento_crediticio,
	us3.nombre as usuario_seguimiento_Crediticio, scp.observacion as observacion_seguimiento_Crediticio,
	us3.id_usuario as id_usuario_seguimiento_crediticio,
	--DETALLES
	--(SELECT max(porcentaje_descuento) from COTIZACION_DETALLE  WHERE estado = 1 AND id_cotizacion = co.id_cotizacion) as maximo_porcentaje_descuento
	ub.codigo codigo_ubigeo,
	ISNULl(ub.distrito,'-') distrito

	 FROM PEDIDO as pe
	INNER JOIN CLIENTE AS cl ON pe.id_cliente = cl.id_cliente
	INNER JOIN CIUDAD AS ci ON pe.id_ciudad = ci.id_ciudad
	INNER JOIN USUARIO AS us on pe.usuario_creacion = us.id_usuario
	INNER JOIN SEGUIMIENTO_PEDIDO sc on sc.id_pedido = pe.id_pedido
	INNER JOIN USUARIO AS us2 on sc.id_usuario = us2.id_usuario
	INNER JOIN SEGUIMIENTO_CREDITICIO_PEDIDO scp on scp.id_pedido = pe.id_pedido
	INNER JOIN USUARIO AS us3 on scp.id_usuario = us3.id_usuario
	LEFT JOIN UBIGEO AS ub ON pe.ubigeo_entrega = ub.codigo 
	where pe.numero = @numero 
	--filtro que evita que un usuario pueda obtener una cotización de otro usuario a través del codigo
--	and (us.id_usuario = @idUsuario or @idUsuario = '00000000-0000-0000-0000-000000000000'
--	OR (SELECT USUARIO @idUsuario)
	
	--)
--	AND (sc.estado_pedido = @estado or @estado = -1  )
	AND sc.estado = 1 AND scp.estado = 1
	AND pe.estado = 1

	AND pe.tipo = @tipo;
	END
END












/* Agrega columna busca_sedes_grupo_cliente */
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
crea_documentos_venta, administra_documentos_venta_lima, administra_documentos_venta_provincias,
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

bloquea_clientes,
modifica_negociacion_multiregional,
busca_sedes_grupo_cliente,
modifica_canales




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

END








/* Agrega parametro @buscaSedesGrupoCliente */
ALTER PROCEDURE [dbo].[ps_cotizaciones] 

@codigo bigint,
@id_cliente uniqueidentifier,
@id_ciudad uniqueidentifier,
@id_usuario uniqueidentifier,
@idGrupoCliente int,
@buscaSedesGrupoCliente bit,
@fechaDesde datetime,
@fechaHasta datetime, 
@estado smallint
AS
BEGIN

if  @codigo = 0 

	SELECT 
	--COTIZACION
	co.codigo as cod_cotizacion, co.id_cotizacion, co.fecha, co.incluido_igv, co.considera_cantidades,
	co.porcentaje_flete, co.igv, co.total, co.observaciones,co.contacto, 
	--CLIENTE
	cl.codigo,  cl.id_cliente,cl.razon_social, cl.ruc, 
	 --USUARIO
	us.nombre as nombre_usuario, us.id_usuario,
	 --CIUDAD
	ci.id_ciudad, ci.nombre as nombre_ciudad, 
	--SEGUIMIENTO
	sc.estado_cotizacion as estado_seguimiento,
	us2.nombre as usuario_seguimiento, sc.observacion as observacion_seguimiento,
	us2.id_usuario as id_usuario_seguimiento,
	--DETALLES
	(SELECT max(porcentaje_descuento) from COTIZACION_DETALLE  WHERE estado = 1 AND id_cotizacion = co.id_cotizacion) as maximo_porcentaje_descuento
/*	(
	SELECT CAST(MIN(((1 - (CASE cd.es_precio_alternativo WHEN 1 THEN pr.costo/pr.equivalencia ELSE pr.costo END )/( CASE cd.precio_neto WHEN 0 THEN 1 ELSE cd.precio_neto END ))*100)) AS DECIMAL(12,1)) from COTIZACION_DETALLE cd INNER JOIN PRODUCTO pr
	ON pr.id_producto = cd.id_producto
	 WHERE cd.estado = 1 AND cd.id_cotizacion = co.id_cotizacion) as minimo_margen
	 */
--	(1 - costoLista / ( precioNeto==0?1: precioNeto))    *100));*/

--	SELECT TOP 5 * FROM COTIZACION_DETALLE
	
	 FROM COTIZACION as co
	INNER JOIN CLIENTE AS cl ON co.id_cliente = cl.id_cliente
	INNER JOIN CIUDAD AS ci ON co.id_ciudad = ci.id_ciudad
	INNER JOIN USUARIO AS us on co.usuario_creacion = us.id_usuario
	INNER JOIN SEGUIMIENTO_COTIZACION sc on sc.id_cotizacion = co.id_cotizacion
	INNER JOIN USUARIO AS us2 on sc.id_usuario = us2.id_usuario
	where   co.fecha > @fechaDesde 
	and co.fecha <=  @fechaHasta
	and (
		co.id_cliente = @id_cliente OR
		(@id_cliente = '00000000-0000-0000-0000-000000000000' AND @idGrupoCliente <= 0) OR
		(@idGrupoCliente > 0 AND co.id_cliente in (select id_cliente from CLIENTE_GRUPO_CLIENTE where id_grupo_cliente = @idGrupoCliente))
	)
	
	
	and (ci.id_ciudad = @id_ciudad OR (@buscaSedesGrupoCliente = 'TRUE' AND @idGrupoCliente > 0) or @id_ciudad = '00000000-0000-0000-0000-000000000000')
	and (us.id_usuario = @id_usuario or @id_usuario = '00000000-0000-0000-0000-000000000000'
	OR @id_usuario IN (SELECT id_usuario FROM USUARIO where visualiza_cotizaciones = 1 )
	)
	AND (sc.estado_cotizacion = @estado or @estado = -1  )
	AND sc.estado = 1
	and co.estado = 1
	order by co.codigo asc ;

else

	SELECT 
	--COTIZACION
	co.codigo as cod_cotizacion, co.id_cotizacion, co.fecha, co.incluido_igv, co.considera_cantidades,
	co.porcentaje_flete, co.igv, co.total, co.observaciones,co.contacto, 
	--CLIENTE
	 cl.codigo,  cl.id_cliente,cl.razon_social, cl.ruc, 
	 --USUARIO
	 us.nombre as nombre_usuario, us.id_usuario,
	 --CIUDAD
	ci.id_ciudad, ci.nombre as nombre_ciudad, 
	--SEGUIMIENTO
	sc.estado_cotizacion as estado_seguimiento,
	us2.nombre as usuario_seguimiento, sc.observacion as observacion_seguimiento,
		us2.id_usuario as id_usuario_seguimiento,
	--DETALLES
	(SELECT max(porcentaje_descuento) from COTIZACION_DETALLE  WHERE estado = 1 AND id_cotizacion = co.id_cotizacion) as maximo_porcentaje_descuento
	
	 FROM COTIZACION as co
	INNER JOIN CLIENTE AS cl ON co.id_cliente = cl.id_cliente
	INNER JOIN CIUDAD AS ci ON co.id_ciudad = ci.id_ciudad
	INNER JOIN USUARIO AS us on co.usuario_creacion = us.id_usuario
	INNER JOIN SEGUIMIENTO_COTIZACION sc on sc.id_cotizacion = co.id_cotizacion
	INNER JOIN USUARIO AS us2 on sc.id_usuario = us2.id_usuario
	where co.codigo = @codigo 
	--filtro que evita que un usuario pueda obtener una cotización de otro usuario a través del codigo
	--and (us.id_usuario = @id_usuario or @id_usuario = '00000000-0000-0000-0000-000000000000')
--	AND (sc.estado_cotizacion = @estado or @estado = -1  )
	AND sc.estado = 1
	and co.estado = 1

END









/* Agrega parametro @buscaSedesGrupoCliente */
ALTER PROCEDURE [dbo].[ps_facturas] 

@numero varchar(8),
@idCliente uniqueidentifier,
@idGrupoCliente int,
@buscaSedesGrupoCliente bit,
@idCiudad uniqueidentifier,
@idUsuario uniqueidentifier,
@fechaDesde datetime,
@fechaHasta datetime,
@soloSolicitudAnulacion int,
@estado int,
@numeroPedido bigint,
@numeroGuiaRemision bigint,
@tipoDocumento int
AS
BEGIN


if ( @numero IS NULL OR @numero = '') AND  @numeroPedido  = 0 AND @numeroGuiaRemision = 0

	SELECT 
	--FACTURA
	dv.id_cpe_cabecera_be as id_documento_venta, 
	dv.SERIE, dv.CORRELATIVO, 
	dv.MNT_TOT MNT_TOT_PRC_VTA,
	dv.solicitud_anulacion,
	dv.comentario_solicitud_anulacion,
	CAST(dv.TIP_CPE AS INT) TIP_CPE,
	CASE  dv.COD_ESTD_SUNAT 
	WHEN '101' THEN 101
	WHEN '102' THEN 102
	WHEN '103' THEN 103
	WHEN '104' THEN 104
	WHEN '105' THEN 105
	WHEN '106' THEN 106
	WHEN '108' THEN 108
	ELSE 0 END as estado ,
	--USUARIO
	us.nombre as nombre_usuario, us.id_usuario,
	CONVERT(datetime, FEC_EMI, 101) as fecha_emision,
	--CLIENTE
	cl.codigo ,
	cl.id_cliente,
	dv.NOM_RCT AS razon_social, dv.NRO_DOC_RCT AS ruc, 
	  --CIUDAD
	ci.id_ciudad, ci.nombre as nombre_ciudad,
	--PEDIDO
	p.numero,
	--MOVIMIENTO
	ma.serie_documento,
	ma.numero_documento,
	dv.permite_anulacion,
	v.id_venta_afectacion

	FROM CPE_CABECERA_BE as dv
	INNER JOIN SERIE_DOCUMENTO_ELECTRONICO as se ON SUBSTRING(dv.SERIE,3,2) = SUBSTRING(se.serie,2,2) 
	INNER JOIN CIUDAD AS ci ON se.id_sede_mp = ci.id_ciudad
	LEFT JOIN CLIENTE AS cl ON (
	cl.ruc = dv.NRO_DOC_RCT AND
	cl.id_ciudad = ci.id_ciudad AND cl.estado = 1 ) 
	LEFT JOIN VENTA v ON dv.id_venta = v.id_venta
	LEFT JOIN MOVIMIENTO_ALMACEN ma ON v.id_movimiento_almacen = ma.id_movimiento_almacen
	LEFT JOIN PEDIDO p ON v.id_pedido = p.id_pedido 
	LEFT JOIN USUARIO AS us ON dv.usuario_creacion = us.id_usuario
	where  CONVERT(datetime, FEC_EMI, 101)  >= @fechaDesde 
	and CONVERT(datetime, FEC_EMI, 101)  <=  @fechaHasta
	and (cl.id_cliente = @idCliente OR  
		(@idCliente = '00000000-0000-0000-0000-000000000000' AND @idGrupoCliente <= 0) OR 
		(@idGrupoCliente > 0 AND cl.id_cliente in (select id_cliente from CLIENTE_GRUPO_CLIENTE where id_grupo_cliente = @idGrupoCliente))
	)
	and (ci.id_ciudad = @idCiudad OR (@buscaSedesGrupoCliente = 'TRUE' AND @idGrupoCliente > 0)  or @idCiudad = '00000000-0000-0000-0000-000000000000'
	)
	AND dv.estado = 1
	AND (dv.TIP_CPE = @tipoDocumento 
		OR @tipoDocumento = 0 
	)
	AND (
		dv.COD_ESTD_SUNAT = @estado 
		OR @estado = 0 
		OR (@estado = 205 AND dv.COD_ESTD_SUNAT IN ('102','103') 
		OR (@estado = 1 AND ( dv.COD_ESTD_SUNAT NOT IN ('101','102', '103','104', '105', '108') OR dv.COD_ESTD_SUNAT IS NULL  ) )
		)
	)
	AND dv.ENVIADO_A_EOL  = 1
	AND (
			@soloSolicitudAnulacion = 0
			OR
			(	solicitud_anulacion = @soloSolicitudAnulacion 
			)
		)

	order  by dv.SERIE asc, dv.CORRELATIVO asc;
		
else

	SELECT 
	dv.id_cpe_cabecera_be as id_documento_venta, 
	--FACTURA
	dv.id_cpe_cabecera_be, 
	dv.SERIE, dv.CORRELATIVO, 
	dv.MNT_TOT MNT_TOT_PRC_VTA,
	dv.solicitud_anulacion,
	dv.comentario_solicitud_anulacion,
	CAST(dv.TIP_CPE AS INT) TIP_CPE,
	CASE  dv.COD_ESTD_SUNAT 
	WHEN '101' THEN 101
	WHEN '102' THEN 102
	WHEN '103' THEN 103
	WHEN '104' THEN 104
	WHEN '105' THEN 105
	WHEN '106' THEN 106
	WHEN '108' THEN 108
	ELSE 0 END as estado ,
	--USUARIO
	us.nombre as nombre_usuario, us.id_usuario,
	CONVERT(datetime, FEC_EMI, 101) as fecha_emision, 
	--CLIENTE
	cl.codigo ,--	 COALESCE(cl.codigo,' ') as codigo,  
	cl.id_cliente, --COALESCE(cl.id_cliente,' ') as id_cliente,
	 dv.NOM_RCT AS razon_social, dv.NRO_DOC_RCT AS ruc, 
	  --CIUDAD
	ci.id_ciudad, ci.nombre as nombre_ciudad,
	--PEDIDO
	p.numero,
	--MOVIMIENTO
	ma.serie_documento,
	ma.numero_documento,
	dv.permite_anulacion,
	v.id_venta_afectacion
	FROM CPE_CABECERA_BE as dv
	INNER JOIN SERIE_DOCUMENTO_ELECTRONICO as se ON SUBSTRING(dv.SERIE,3,2) = SUBSTRING(se.serie,2,2) 
	INNER JOIN CIUDAD AS ci ON se.id_sede_mp = ci.id_ciudad
	LEFT JOIN CLIENTE AS cl ON (
	cl.ruc = dv.NRO_DOC_RCT AND
	cl.id_ciudad = ci.id_ciudad AND cl.estado = 1 ) 
	LEFT JOIN VENTA v ON dv.id_venta = v.id_venta
	LEFT JOIN MOVIMIENTO_ALMACEN ma ON v.id_movimiento_almacen = ma.id_movimiento_almacen
	LEFT JOIN PEDIDO p ON v.id_pedido = p.id_pedido 
--	LEFT JOIN CIUDAD AS ci ON v.id_ciudad = ci.id_ciudad
	LEFT JOIN USUARIO AS us ON dv.usuario_creacion = us.id_usuario
	where  -- dv.fecha_creacion >= @fechaDesde 
--	and dv.fecha_creacion <=  @fechaHasta
--	and
	 (ci.id_ciudad = @idCiudad )
	AND (p.numero = @numeroPedido
	OR  dv.CORRELATIVO = @numero OR ma.numero_documento = @numeroGuiaRemision)
	AND dv.estado = 1
--	and v.estado = 1
	AND dv.ENVIADO_A_EOL  = 1
	AND (dv.TIP_CPE = @tipoDocumento 
		OR @tipoDocumento = 0 
	)
	order by dv.SERIE asc, dv.CORRELATIVO asc;

END










/* Agrega parametro @buscaSedesGrupoCliente */
ALTER PROCEDURE [dbo].[ps_guiasRemision] 
@numeroDocumento bigint,
@idCiudad uniqueidentifier,
@idCliente uniqueidentifier,
@idGrupoCliente int,
@buscaSedesGrupoCliente bit,
@idUsuario uniqueidentifier,
@fechaTrasladoDesde datetime,
@fechaTrasladoHasta datetime, 
@anulado smallint, 
@facturado smallint, 
@numeroPedido bigint,
@motivoTraslado char(1)
AS
BEGIN

if  @numeroDocumento = 0 AND @numeroPedido = 0
BEGIN
	SELECT 
	--GUIA_REMISION
	ma.serie_documento, ma.numero_documento,ma.id_movimiento_almacen,
	ma.fecha_emision, ma.fecha_traslado,
	ma.atencion_parcial, ma.ultima_atencion_parcial,  ma.anulado, ma.facturado,
	ma.no_entregado, ma.tipo_extorno, ma.motivo_traslado, ma.ingresado,
	--PEDIDO
	pe.id_pedido, pe.numero as numero_pedido,
	--CLIENTE
	 cl.codigo,  cl.id_cliente,cl.razon_social, cl.ruc, 
	 --USUARIO
	 us.nombre as nombre_usuario, us.id_usuario,
	 --CIUDAD
	ci.id_ciudad, ci.nombre as nombre_ciudad--, 
	--SEGUIMIENTO
	--sc.estado_movimiento_almacen as estado_seguimiento,
--	us2.nombre as usuario_seguimiento, sc.observacion as observacion_seguimiento,
--	us2.id_usuario as id_usuario_seguimiento	

	
	
	FROM MOVIMIENTO_ALMACEN as ma
	INNER JOIN CIUDAD AS ci ON ma.id_sede_origen = ci.id_ciudad
	INNER JOIN USUARIO AS us on ma.usuario_creacion = us.id_usuario
	--LEFT JOIN SEGUIMIENTO_MOVIMIENTO_ALMACEN sc on sc.id_movimiento_almacen = ma.id_movimiento_almacen
	/*INNER JOIN USUARIO AS us2 on sc.id_usuario = us2.id_usuario*/
	LEFT JOIN PEDIDO AS pe ON ma.id_pedido = pe.id_pedido
	LEFT JOIN CLIENTE AS cl ON cl.id_cliente = ma.id_cliente
	where   ma.fecha_traslado >= @fechaTrasladoDesde 
	and ma.fecha_traslado <=  @fechaTrasladoHasta
	and (cl.id_cliente = @idCliente OR
	(@idCliente = '00000000-0000-0000-0000-000000000000' AND @idGrupoCliente <= 0) OR 
	(@idGrupoCliente > 0 AND cl.id_cliente in (select id_cliente from CLIENTE_GRUPO_CLIENTE where id_grupo_cliente = @idGrupoCliente))
	)
	and (ci.id_ciudad = @idCiudad OR (@buscaSedesGrupoCliente = 'TRUE' AND @idGrupoCliente > 0)  or @idCiudad = '00000000-0000-0000-0000-000000000000')
	--and (us.id_usuario = @idUsuario or @idUsuario = '00000000-0000-0000-0000-000000000000')
	--AND (sc.estado_movimiento_almacen = @estado or @estado = -1  )
	--AND sc.estado = 1
	and ma.estado = 1
	and (ma.anulado = 0 OR  ma.anulado = @anulado)
	and (ma.facturado = 0 OR  ma.facturado = @facturado)
	and ma.tipo_movimiento = 'S'
	AND ma.tipo_documento = 'GR'
	AND (@motivoTraslado = '0' OR @motivoTraslado = ma.motivo_traslado)
	order by ma.numero_documento asc ;
END
else

--SELECT * FROM MOVIMIENTO_ALMACEN


		SELECT 
	--GUIA_REMISION
	ma.serie_documento, ma.numero_documento,ma.id_movimiento_almacen,
	ma.fecha_emision, ma.fecha_traslado,
	ma.atencion_parcial,  ma.ultima_atencion_parcial,  ma.anulado,ma.facturado,
	ma.no_entregado, ma.tipo_extorno, ma.motivo_traslado, ma.ingresado,
	--PEDIDO
	pe.id_pedido, pe.numero as numero_pedido,
	--CLIENTE
	 cl.codigo,  cl.id_cliente,cl.razon_social, cl.ruc, 
	 --USUARIO
	 us.nombre as nombre_usuario, us.id_usuario,
	 --CIUDAD
	ci.id_ciudad, ci.nombre as nombre_ciudad--, 
	--SEGUIMIENTO
--	sc.estado_movimiento_almacen as estado_seguimiento,
--	us2.nombre as usuario_seguimiento, sc.observacion as observacion_seguimiento,
--	us2.id_usuario as id_usuario_seguimiento	
	
	FROM MOVIMIENTO_ALMACEN as ma
	INNER JOIN CIUDAD AS ci ON ma.id_sede_origen = ci.id_ciudad
	INNER JOIN USUARIO AS us on ma.usuario_creacion = us.id_usuario
--	INNER JOIN SEGUIMIENTO_MOVIMIENTO_ALMACEN sc on sc.id_movimiento_almacen = ma.id_movimiento_almacen
	--INNER JOIN USUARIO AS us2 on sc.id_usuario = us2.id_usuario
	LEFT JOIN PEDIDO AS pe ON ma.id_pedido = pe.id_pedido
	LEFT JOIN CLIENTE AS cl ON cl.id_cliente = ma.id_cliente
	where ((ma.numero_documento = @numeroDocumento
	OR pe.numero = @numeroPedido )
	AND (ma.id_sede_origen = (Select id_ciudad FROM USUARIO where id_usuario = @idUsuario)
	OR motivo_traslado = 'T'
	OR ma.id_sede_origen = @idCiudad)
	)
	and ma.estado = 1
	and ma.tipo_movimiento = 'S'
	AND ma.tipo_documento = 'GR'


END;










/* Agrega parametro @buscaSedesGrupoCliente */
ALTER PROCEDURE [dbo].[ps_notasIngreso] 
@numeroDocumento bigint,
@idCiudad uniqueidentifier,
@idCliente uniqueidentifier,
@idGrupoCliente int,
@buscaSedesGrupoCliente bit,
@idUsuario uniqueidentifier,
@fechaTrasladoDesde datetime,
@fechaTrasladoHasta datetime, 
@anulado smallint, 
@numeroGuiaReferencia int, 
@numeroPedido bigint,
@motivoTraslado char(1)
AS
BEGIN

if  @numeroDocumento = 0 AND @numeroPedido = 0
BEGIN
	SELECT 
	--GUIA_REMISION
	ma.serie_documento, ma.numero_documento,ma.id_movimiento_almacen,
	ma.fecha_emision, ma.fecha_traslado,
	ma.atencion_parcial, ma.ultima_atencion_parcial,  ma.anulado, ma.facturado,
	ma.no_entregado, ma.tipo_extorno,ma.motivo_traslado,
	--PEDIDO
	pe.id_pedido, pe.numero as numero_pedido,
	--CLIENTE
	 cl.codigo,  cl.id_cliente,cl.razon_social, cl.ruc, 
	 --USUARIO
	 us.nombre as nombre_usuario, us.id_usuario,
	 --CIUDAD
	ci.id_ciudad, ci.nombre as nombre_ciudad--, 
	--SEGUIMIENTO
	--sc.estado_movimiento_almacen as estado_seguimiento,
--	us2.nombre as usuario_seguimiento, sc.observacion as observacion_seguimiento,
--	us2.id_usuario as id_usuario_seguimiento	
	
	FROM MOVIMIENTO_ALMACEN as ma
	INNER JOIN CIUDAD AS ci ON ma.id_sede_destino = ci.id_ciudad
	INNER JOIN USUARIO AS us on ma.usuario_creacion = us.id_usuario
	--LEFT JOIN SEGUIMIENTO_MOVIMIENTO_ALMACEN sc on sc.id_movimiento_almacen = ma.id_movimiento_almacen
	/*INNER JOIN USUARIO AS us2 on sc.id_usuario = us2.id_usuario*/
	LEFT JOIN PEDIDO AS pe ON ma.id_pedido = pe.id_pedido
	LEFT JOIN CLIENTE AS cl ON cl.id_cliente = ma.id_cliente
	LEFT JOIN MOVIMIENTO_ALMACEN as mag ON mag.id_movimiento_almacen = ma.id_movimiento_almacen_ingresado
	where   ma.fecha_traslado > @fechaTrasladoDesde 
	and ma.fecha_traslado <=  @fechaTrasladoHasta
	and (cl.id_cliente = @idCliente OR  
		(@idCliente = '00000000-0000-0000-0000-000000000000' AND @idGrupoCliente <= 0) OR 
		(@idGrupoCliente > 0 AND cl.id_cliente in (select id_cliente from CLIENTE_GRUPO_CLIENTE where id_grupo_cliente = @idGrupoCliente)) 
	)
	and (ci.id_ciudad = @idCiudad OR (@buscaSedesGrupoCliente = 'TRUE' AND @idGrupoCliente > 0) or @idCiudad = '00000000-0000-0000-0000-000000000000')
	--and (us.id_usuario = @idUsuario or @idUsuario = '00000000-0000-0000-0000-000000000000')
	--AND (sc.estado_movimiento_almacen = @estado or @estado = -1  )
	--AND sc.estado = 1
	and ma.estado = 1
	and (ma.anulado = 0 OR  ma.anulado = @anulado)
	and ma.tipo_movimiento = 'E'
	AND ma.tipo_documento = 'NI'
	AND (ma.numero_guia_referencia = @numeroGuiaReferencia OR @numeroGuiaReferencia  = 0
	OR mag.numero_documento = @numeroGuiaReferencia
	)
	AND (@motivoTraslado = '0' OR @motivoTraslado = ma.motivo_traslado)
	order by ma.numero_documento asc ;
END
else

--SELECT TOP 5 * FROM MOVIMIENTO_ALMACEN ORDER  BY fecha_creacion DESC

--SELECT * FROM MOVIMIENTO_ALMACEN


		SELECT 
	--GUIA_REMISION
	ma.serie_documento, ma.numero_documento,ma.id_movimiento_almacen,
	ma.fecha_emision, ma.fecha_traslado,
	 ma.atencion_parcial,  ma.ultima_atencion_parcial,  ma.anulado,ma.facturado,
	 ma.no_entregado,  ma.tipo_extorno,ma.motivo_traslado,
	--PEDIDO
	pe.id_pedido, pe.numero as numero_pedido,
	--CLIENTE
	 cl.codigo,  cl.id_cliente,cl.razon_social, cl.ruc, 
	 --USUARIO
	 us.nombre as nombre_usuario, us.id_usuario,
	 --CIUDAD
	ci.id_ciudad, ci.nombre as nombre_ciudad--, 
	--SEGUIMIENTO
--	sc.estado_movimiento_almacen as estado_seguimiento,
--	us2.nombre as usuario_seguimiento, sc.observacion as observacion_seguimiento,
--	us2.id_usuario as id_usuario_seguimiento	
	
	FROM MOVIMIENTO_ALMACEN as ma
	INNER JOIN CIUDAD AS ci ON ma.id_sede_destino = ci.id_ciudad
	INNER JOIN USUARIO AS us on ma.usuario_creacion = us.id_usuario
--	INNER JOIN SEGUIMIENTO_MOVIMIENTO_ALMACEN sc on sc.id_movimiento_almacen = ma.id_movimiento_almacen
	--INNER JOIN USUARIO AS us2 on sc.id_usuario = us2.id_usuario
	LEFT JOIN PEDIDO AS pe ON ma.id_pedido = pe.id_pedido
	LEFT JOIN CLIENTE AS cl ON cl.id_cliente = ma.id_cliente
	where (ma.numero_documento = @numeroDocumento
	OR pe.numero = @numeroPedido 
	)
	--filtro que evita que un usuario pueda obtener una cotización de otro usuario a través del codigo
--	and (us.id_usuario = @idUsuario or @idUsuario = '00000000-0000-0000-0000-000000000000')
--	AND (sc.estado_cotizacion = @estado or @estado = -1  )
--	AND sc.estado = 1
	and ma.estado = 1
	and ma.tipo_movimiento = 'E'
	AND ma.tipo_documento = 'NI'
END;








