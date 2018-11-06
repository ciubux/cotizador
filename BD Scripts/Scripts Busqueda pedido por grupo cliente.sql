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
	and (ci.id_ciudad = @idCiudad 
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
