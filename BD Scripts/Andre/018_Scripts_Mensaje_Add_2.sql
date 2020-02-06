/******************* CREATE ps_mensaje_si_leido - confirma si  un mensaje esta leido por otros usuarios para que deshabilite editar *******************/
create procedure  ps_mensaje_si_leido
(@id_mensaje uniqueidentifier)
as begin 
select mensaje.id_mensaje,titulo,
 CASE WHEN EXISTS (SELECT mensaje_leido.id_mensaje FROM MENSAJE_LEIDO WHERE MENSAJE_LEIDO.id_mensaje =@id_mensaje)
       THEN '1' 
       ELSE '0'
	   END as leido from mensaje where id_mensaje=@id_mensaje
 end

/****************** CREATE ps_ver_usuario_respuestas - Busca a los usuarios que respondieron a un mensaje ***************************/

create procedure ps_ver_usuario_respuestas
(
@id_mensaje uniqueidentifier 
)
as begin
declare @id_usuario uniqueidentifier
set  @id_usuario =(select id_usuario_creacion from mensaje where id_mensaje=@id_mensaje) 

select distinct usuario.id_usuario,USUARIO.nombre from mensaje 
inner join usuario on mensaje.id_usuario_creacion = usuario.id_usuario 
where (MENSAJE.id_mensaje=@id_mensaje or id_hilo_mensaje=@id_mensaje) and mensaje.id_usuario_creacion !=@id_usuario
order by usuario.nombre asc
end  

/*********************CREATE ps_ver_respuestas_usuario - Busca las respuestas de un usuario seleccionado en un mensaje enviado***********************/

create procedure ps_ver_respuestas_usuario
(
@id_mensaje uniqueidentifier,
@id_usuario_remitente uniqueidentifier,
@id_usuario_destinatario uniqueidentifier
)
as begin
select * from mensaje inner join usuario on mensaje.id_usuario_creacion = usuario.id_usuario 
where (MENSAJE.id_mensaje=@id_mensaje or id_hilo_mensaje=@id_mensaje) and (mensaje.id_usuario_creacion=@id_usuario_remitente or mensaje.id_usuario_creacion=@id_usuario_destinatario) 
order by mensaje.fecha_creacion asc
end  
