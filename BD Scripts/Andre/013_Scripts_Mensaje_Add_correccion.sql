/***********ALTER ps_alerta_mensaje_usuario -Correccion de alerta de mensaje para usuario***************/
ALTER procedure [dbo].[ps_alerta_mensaje_usuario] 
(@id_usuario uniqueidentifier)
as begin 
select mensaje.id_mensaje,titulo,mensaje,importancia,mensaje.fecha_creacion,USUARIO.nombre,mensaje.fecha_vencimiento,mensaje.fecha_inicio from mensaje
left join MENSAJE_USUARIO on mensaje.id_mensaje=MENSAJE_USUARIO.id_mensaje
left join mensaje_roles on   mensaje_roles.id_mensaje=mensaje.id_mensaje
left join  ROL_USUARIO on ROL_USUARIO.id_rol=mensaje_roles.id_rol
inner join USUARIO on mensaje.id_usuario_creacion=usuario.id_usuario
where Mensaje.id_mensaje not in (select id_mensaje from MENSAJE_LEIDO where id_usuario=@id_usuario)  
and (ROL_USUARIO.id_usuario=@id_usuario or MENSAJE_USUARIO.id_usuario=@id_usuario) and mensaje.estado=1 
and fecha_inicio <= convert(date,dbo.getlocaldate())  and fecha_vencimiento >= convert(date,dbo.getlocaldate())
order by  mensaje.fecha_creacion desc
end

