create procedure [dbo].[ps_detalle_vendedores]
(@idUsuarioVenta int)
AS BEGIN 
SELECT vendedor.id_vendedor,vendedor.codigo,USUARIO.id_ciudad,
vendedor.estado,nombre,email,usuario.password,cargo,contacto,maximo_porcentaje_descuento_aprobacion,
(select vendedor.usuario_creacion from usuario  inner join vendedor on vendedor.id_usuario=usuario.id_usuario where vendedor.id_vendedor=@idUsuarioVenta)as USUARIO_CREACION,vendedor.fecha_creacion,
(select vendedor.usuario_modificacion from usuario  inner join vendedor on vendedor.id_usuario=usuario.id_usuario where vendedor.id_vendedor=@idUsuarioVenta) as USUARIO_MODIFICACION,vendedor.fecha_modificacion,
(select nombre from ciudad where id_ciudad=(select id_ciudad from USUARIO inner join vendedor on usuario.id_usuario=vendedor.id_usuario where id_vendedor=@idUsuarioVenta))AS CIUDAD FROM USUARIO  inner join vendedor on  vendedor.id_usuario=USUARIO.id_usuario WHERE vendedor.id_vendedor=@idUsuarioVenta
END