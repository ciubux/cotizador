
/************** ALTER ps_lista_venta - ajuste de busqueda de ventas *****************************************/
ALTER procedure  [dbo].[ps_lista_venta] 
(
@idCiudad uniqueidentifier,
@idCliente uniqueidentifier,
@idUsuario uniqueidentifier,
@tipoDocumento int,
@numeroFactura bigint,
@numeroPedido bigint,
@numeroGuia bigint, 
@fechaTrasladoDesde datetime,
@fechaTrasladoHasta datetime,
@sku varchar(10) = NULL
)
as
begin 
	if  @numeroGuia = 0 AND @numeroPedido = 0 and @numeroFactura =0
	begin 
	select 	
	venta.id_venta,venta.id_movimiento_almacen,	
	PEDIDO.numero,
	CONCAT(CPE_CABECERA_BE.serie,'-',CPE_CABECERA_BE.correlativo) as doc_venta,
	concat('G',MOVIMIENTO_ALMACEN.serie_documento,'-',MOVIMIENTO_ALMACEN.numero_documento) as mov_almacen ,
	usuario.nombre,
	MOVIMIENTO_ALMACEN.fecha_emision,
	CLIENTE.codigo,
	cliente.razon_social,
	cliente.ruc,
	concat('MP',CIUDAD.codigo_sede) as sede,
	venta.total,
	ve.descripcion as vendedor
	
from VENTA	
	left JOIN CPE_CABECERA_BE on venta.id_documento_venta=CPE_CABECERA_BE.id_cpe_cabecera_be 
	inner join pedido on PEDIDO.id_pedido=VENTA.id_pedido 
	INNER JOIN MOVIMIENTO_ALMACEN on MOVIMIENTO_ALMACEN.id_movimiento_almacen=VENTA.id_movimiento_almacen
	inner join cliente on cliente.id_cliente=VENTA.id_cliente	
	inner join USUARIO on usuario.id_usuario=MOVIMIENTO_ALMACEN.usuario_creacion
	inner join CIUDAD on ciudad.id_ciudad=MOVIMIENTO_ALMACEN.id_sede_origen
	LEFT JOIN VENDEDOR AS ve ON cliente.id_responsable_comercial = ve.id_vendedor

where MOVIMIENTO_ALMACEN.tipo_documento = 'GR' and 
	MOVIMIENTO_ALMACEN.tipo_movimiento = 'S' AND 
	MOVIMIENTO_ALMACEN.motivo_traslado='V' AND    
	(VENTA.estado=1 or	(MOVIMIENTO_ALMACEN.estado=1 and MOVIMIENTO_ALMACEN.anulado=0)) AND
    MOVIMIENTO_ALMACEN.fecha_emision >= @fechaTrasladoDesde and
	MOVIMIENTO_ALMACEN.fecha_emision <=  @fechaTrasladoHasta  	
	AND CIUDAD.id_ciudad = (case when @idCiudad='00000000-0000-0000-0000-000000000000' then CIUDAD.id_ciudad else @idCiudad end) 
	
	AND MOVIMIENTO_ALMACEN.id_cliente = (case when @idCliente='00000000-0000-0000-0000-000000000000' then MOVIMIENTO_ALMACEN.id_cliente else @idCliente end) 

	AND (@sku IS NULL OR  @sku = '' OR	EXISTS (SELECT	mad.id_movimiento_almacen_detalle
		FROM MOVIMIENTO_ALMACEN_DETALLE mad 
		INNER JOIN PRODUCTO pr ON mad.id_producto = pr.id_producto
		WHERE mad.id_movimiento_almacen = MOVIMIENTO_ALMACEN.id_movimiento_almacen 
		AND pr.sku like @sku)	)
	order by VENTA.numero asc ;
end 

else	
	select 	
	VENTA.id_venta,venta.id_movimiento_almacen,	
	pe.numero,
	CONCAT(CPE_CABECERA_BE.serie,'-',CPE_CABECERA_BE.correlativo) as doc_venta,
	concat('G',ma.serie_documento,'-',ma.numero_documento) as mov_almacen ,
	us.nombre,
	ma.fecha_emision,
	cl.codigo,
	cl.razon_social,
	cl.ruc,
	concat('MP',CIUDAD.codigo_sede) as sede,
	venta.total,
	ve.descripcion as vendedor
	FROM VENTA
	left JOIN CPE_CABECERA_BE on venta.id_documento_venta=CPE_CABECERA_BE.id_cpe_cabecera_be 
	inner join pedido AS pe on pe.id_pedido=VENTA.id_pedido 
	INNER JOIN MOVIMIENTO_ALMACEN as ma on ma.id_movimiento_almacen=VENTA.id_movimiento_almacen
	inner join CIUDAD on ciudad.id_ciudad=ma.id_sede_origen	
	INNER JOIN USUARIO AS us on VENTA.usuario_creacion = us.id_usuario
	INNER JOIN CLIENTE AS cl ON cl.id_cliente = VENTA.id_cliente
	LEFT JOIN VENDEDOR AS ve ON cl.id_responsable_comercial = ve.id_vendedor

	where 
		ma.tipo_movimiento = 'S'
	AND ma.tipo_documento = 'GR'
	AND ma.motivo_traslado='V'
	AND ((ma.estado=1 and ma.anulado=0)	or VENTA.estado=1 )	
	AND	(CPE_CABECERA_BE.TIP_CPE in (case when @tipoDocumento=0 then 1 end,case when @tipoDocumento=0 then 3 end,
										case when @tipoDocumento=1 then 1 end,
										case when @tipoDocumento=3 then 3 end)  
	OR CPE_CABECERA_BE.TIP_CPE IS NULL)	   	
	AND (ma.id_sede_origen = @idCiudad 	OR (ma.id_sede_origen = (Select id_ciudad FROM USUARIO where id_usuario = @idUsuario)))
	AND (ma.numero_documento = @numeroGuia OR pe.numero = @numeroPedido or CONVERT(INT,CPE_CABECERA_BE.CORRELATIVO)=@numeroFactura)	
	end

/************************ alter ps_ver_usuario_respuestas  - ajuste de busqueda *****************/
alter procedure ps_ver_usuario_respuestas 
(
@id_mensaje uniqueidentifier 

)
as begin

declare @id_usuario uniqueidentifier
set  @id_usuario =(select id_usuario_creacion from mensaje where id_mensaje=@id_mensaje) 

select distinct mensaje.fecha_creacion,usuario.id_usuario,USUARIO.nombre from mensaje 
inner join usuario on mensaje.id_usuario_creacion = usuario.id_usuario 
where (MENSAJE.id_mensaje=@id_mensaje or id_hilo_mensaje=@id_mensaje) 
order by mensaje.fecha_creacion asc
end  

/********************************* ALTER  ps_alerta_mensaje_usuario - añede fecha en el modal de alerta de mensaje************************************************/

		ALTER procedure [dbo].[ps_alerta_mensaje_usuario]
(@id_usuario uniqueidentifier)
as begin 
select mensaje.id_mensaje,titulo,mensaje,importancia,mensaje.fecha_creacion,USUARIO.nombre,mensaje.fecha_vencimiento,mensaje.es_respuesta,id_hilo_mensaje,fecha_inicio,mensaje.fecha_creacion from mensaje
left join MENSAJE_USUARIO on mensaje.id_mensaje=MENSAJE_USUARIO.id_mensaje
left join mensaje_roles on   mensaje_roles.id_mensaje=mensaje.id_mensaje
left join  ROL_USUARIO on ROL_USUARIO.id_rol=mensaje_roles.id_rol
inner join USUARIO on mensaje.id_usuario_creacion=usuario.id_usuario
where Mensaje.id_mensaje not in (select id_mensaje from MENSAJE_LEIDO where id_usuario=@id_usuario)  
and (ROL_USUARIO.id_usuario=@id_usuario or MENSAJE_USUARIO.id_usuario=@id_usuario) and mensaje.estado=1
and fecha_inicio <= convert(date,dbo.getlocaldate())  and (fecha_vencimiento >= convert(date,dbo.getlocaldate()) or fecha_vencimiento is null)

order by  mensaje.fecha_creacion desc
end



