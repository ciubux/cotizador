/************************ ALTER TABLA MENSAJE -  agregado el hilo de mensaje ******************************/
alter table mensaje 
add id_hilo_mensaje uniqueidentifier ,
es_respuesta bit 

/************************** CREATE PROCEDURE ps_ver_hilo_mensaje -Busca el hilo del  mensaje tomando el mensaje recibido *****************************/
create  procedure ps_ver_hilo_mensaje 
(@id_mensaje_mensaje_recibido uniqueidentifier,
@id_usuario uniqueidentifier)
as begin 

declare @id_mensaje_hilo uniqueidentifier 
set @id_mensaje_hilo=(select id_hilo_mensaje from mensaje where id_mensaje=@id_mensaje_mensaje_recibido)

select  id_mensaje,id_hilo_mensaje,id_usuario_creacion,mensaje,nombre,mensaje.fecha_creacion from mensaje  
inner join usuario on usuario.id_usuario=mensaje.id_usuario_creacion 
where (mensaje.id_hilo_mensaje=@id_mensaje_hilo or mensaje.id_mensaje=@id_mensaje_hilo)
and (mensaje.id_usuario_creacion=@id_usuario or mensaje.id_usuario_creacion= (select id_usuario_creacion from mensaje where id_mensaje=@id_mensaje_mensaje_recibido ))
and mensaje.estado=1 and not id_mensaje=@id_mensaje_mensaje_recibido 
order by mensaje.fecha_creacion desc
end
/********************* CREATE PROCEDURE pi_mensaje_visto_repuesta - Inserta una respuesta y insertar el mensjaje leido en la taba MENSAJE_LEIDO *******************************/

CREATE PROCEDURE [dbo].pi_mensaje_visto_repuesta
(@id_mensaje uniqueidentifier,
@id_usuario uniqueidentifier,
@respuesta text,
@titulo varchar(50),
@importancia varchar(10))
as begin 
declare @newId varchar(50)
set @newId=NEWID()

declare @id_usuario_mensaje uniqueidentifier
set @id_usuario_mensaje=(select id_usuario_creacion  from mensaje where id_mensaje=@id_mensaje)
declare @idHiloMensaje uniqueidentifier
set @idHiloMensaje=(select id_hilo_mensaje  from mensaje where id_mensaje=@id_mensaje)

insert MENSAJE_LEIDO(id_mensaje,id_usuario,fecha_leido) values(@id_mensaje,@id_usuario,dbo.getlocaldate())

insert mensaje (id_mensaje,fecha_inicio,id_usuario_creacion,usuario_modificacion,fecha_creacion,fecha_modificacion,es_respuesta,id_hilo_mensaje,mensaje,estado,titulo,importancia)values 
(@newId,dbo.getlocaldate(),@id_usuario,@id_usuario,dbo.getlocaldate(),dbo.getlocaldate(),1,case when @idHiloMensaje is null then @id_mensaje else @idHiloMensaje end,@respuesta,1,@titulo,@importancia)

insert into MENSAJE_USUARIO (id_usuario,id_mensaje)values(@id_usuario_mensaje,@newId)

end 

/*************************** ALTER procedure ps_alerta_mensaje_usuario - se agrega la notificacion de respuesta(mensaje de respuesta) **********************************************************/

ALTER procedure [dbo].[ps_alerta_mensaje_usuario] 
(@id_usuario uniqueidentifier)
as begin 
select mensaje.id_mensaje,titulo,mensaje,importancia,mensaje.fecha_creacion,USUARIO.nombre,mensaje.fecha_vencimiento,mensaje.es_respuesta,id_hilo_mensaje,fecha_inicio from mensaje
left join MENSAJE_USUARIO on mensaje.id_mensaje=MENSAJE_USUARIO.id_mensaje
left join mensaje_roles on   mensaje_roles.id_mensaje=mensaje.id_mensaje
left join  ROL_USUARIO on ROL_USUARIO.id_rol=mensaje_roles.id_rol
inner join USUARIO on mensaje.id_usuario_creacion=usuario.id_usuario
where Mensaje.id_mensaje not in (select id_mensaje from MENSAJE_LEIDO where id_usuario=@id_usuario)  
and (ROL_USUARIO.id_usuario=@id_usuario or MENSAJE_USUARIO.id_usuario=@id_usuario) and mensaje.estado=1 
and fecha_inicio <= convert(date,dbo.getlocaldate())  and (fecha_vencimiento >= convert(date,dbo.getlocaldate()) or fecha_vencimiento is null)
order by  mensaje.fecha_creacion desc
end





