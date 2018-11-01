create PROCEDURE [dbo].[ps_pedido_seguimiento] 
@idPedido uniqueIdentifier
AS
BEGIN

SELECT sp.id_seguimiento_pedido,  sp.estado_pedido, sp.id_usuario, sp.observacion, sp.fecha_creacion, u.nombre as nombre_usuario
FROM SEGUIMIENTO_PEDIDO sp
INNER JOIN USUARIO u ON sp.id_usuario = u.id_usuario
where sp.id_pedido = @idPedido 
order by sp.fecha_creacion desc

END


create PROCEDURE [dbo].[ps_cotizacion_seguimiento] 
@idCotizacion uniqueIdentifier
AS
BEGIN

SELECT  sc.id_estado_seguimiento, sc.estado_cotizacion, sc.id_usuario, sc.observacion, sc.fecha_creacion, u.nombre as nombre_usuario
FROM SEGUIMIENTO_COTIZACION sc
INNER JOIN USUARIO u ON sc.id_usuario = u.id_usuario
where sc.id_cotizacion = @idCotizacion 
order by sc.fecha_creacion desc

END


