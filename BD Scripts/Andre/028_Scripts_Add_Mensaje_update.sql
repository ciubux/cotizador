-------------------------- ALTER TABLE MENSAJE_USUARIO -----------------------------------
alter table mensaje_usuario 
add reenvio int 
-------------------------------- ALTER  pi_mensaje_visto_repuesta - opcion de añadir usuarios a la respuesta -------------------------

ALTER PROCEDURE [dbo].[pi_mensaje_visto_repuesta]
(@id_mensaje uniqueidentifier,
@id_usuario uniqueidentifier,
@respuesta text,
@titulo varchar(50),
@importancia varchar(10),
@usuarios UniqueIdentifierList readonly
)
as begin 

DECLARE @idUsuario uniqueidentifier 
DECLARE @usuarioCursor CURSOR

declare @newId varchar(50)
set @newId=NEWID()

declare @id_usuario_mensaje uniqueidentifier
set @id_usuario_mensaje=(select id_usuario_creacion  from mensaje where id_mensaje=@id_mensaje)
declare @idHiloMensaje uniqueidentifier
set @idHiloMensaje=(select id_hilo_mensaje  from mensaje where id_mensaje=@id_mensaje)

insert MENSAJE_LEIDO(id_mensaje,id_usuario,fecha_leido) values(@id_mensaje,@id_usuario,dbo.getlocaldate())

declare @reenvio int
set @reenvio=(select reenvio from mensaje_usuario where id_usuario=@id_usuario and id_mensaje=@id_mensaje)
if(@reenvio = 0 or @reenvio is null)
begin 
insert mensaje (id_mensaje,fecha_inicio,id_usuario_creacion,usuario_modificacion,fecha_creacion,fecha_modificacion,es_respuesta,id_hilo_mensaje,mensaje,estado,titulo,importancia)values 
(@newId,dbo.getlocaldate(),@id_usuario,@id_usuario,dbo.getlocaldate(),dbo.getlocaldate(),1,case when @idHiloMensaje is null then @id_mensaje else @idHiloMensaje end,@respuesta,1,@titulo,@importancia)
end 
else 
begin 
declare @fecha_vencimiento date
set  @fecha_vencimiento=(select fecha_vencimiento  from mensaje where id_mensaje=@id_mensaje)
insert into mensaje (id_mensaje,id_usuario_creacion,fecha_creacion,mensaje,titulo,importancia,estado,fecha_modificacion,usuario_modificacion,fecha_inicio,id_hilo_mensaje) 
values (@newId,@id_usuario,dbo.getlocaldate(),@respuesta,@titulo,@importancia,1,dbo.getlocaldate(),@id_usuario,dbo.getlocaldate(),@id_mensaje)
end 

insert into MENSAJE_USUARIO (id_usuario,id_mensaje)values(@id_usuario_mensaje,@newId)

SET @usuarioCursor = CURSOR FOR
SELECT ID
FROM @usuarios
OPEN @usuarioCursor
FETCH NEXT
FROM @usuarioCursor INTO @idUsuario
WHILE @@FETCH_STATUS = 0
BEGIN
	if(@id_usuario_mensaje != @idUsuario)
	begin 

	INSERT INTO mensaje_usuario
			   ([id_usuario]
			   ,[id_mensaje]
			   ,[reenvio])
			   
		 VALUES
			   (@idUsuario,
			    @newId,
				1)			   

    FETCH NEXT
	FROM @usuarioCursor INTO @idUsuario
	end
end

end

-------------------------------- ALTER  ps_ver_hilo_mensaje - usuarios  no pueden ver el historial si son agregados de una respuesta -----------------------------------------

ALTER  procedure [dbo].[ps_ver_hilo_mensaje]
(@id_mensaje_mensaje_recibido uniqueidentifier,
@id_usuario uniqueidentifier)
as begin 

declare @id_mensaje_hilo uniqueidentifier 
set @id_mensaje_hilo=(select id_hilo_mensaje from mensaje where id_mensaje=@id_mensaje_mensaje_recibido)

declare @reenvio int 
set  @reenvio=(select reenvio from MENSAJE_USUARIO where id_mensaje=@id_mensaje_mensaje_recibido and id_usuario=@id_usuario)

if(@reenvio is null or @reenvio = 0)
begin 
select  mensaje.id_mensaje,id_hilo_mensaje,id_usuario_creacion,mensaje,nombre,mensaje.fecha_creacion from mensaje  
inner join usuario on usuario.id_usuario=mensaje.id_usuario_creacion 
where (mensaje.id_hilo_mensaje=@id_mensaje_hilo or mensaje.id_mensaje=@id_mensaje_hilo) 
and (mensaje.id_usuario_creacion=@id_usuario or mensaje.id_usuario_creacion= (select id_usuario_creacion from mensaje where id_mensaje=@id_mensaje_mensaje_recibido ))
and mensaje.estado=1 and not mensaje.id_mensaje=@id_mensaje_mensaje_recibido
order by mensaje.fecha_creacion desc
end
end

------------------------------ALTER ps_ver_respuestas_usuario - Correccion de mensajes entre los dos usuarios------------------------

ALTER procedure [dbo].[ps_ver_respuestas_usuario] 
(
@id_mensaje uniqueidentifier,
@id_usuario_destinatario uniqueidentifier,
@id_usuario_remitente uniqueidentifier
)
as begin

select * from mensaje left join usuario on mensaje.id_usuario_creacion = usuario.id_usuario
left join MENSAJE_USUARIO on mensaje_usuario.id_mensaje= mensaje.id_mensaje
where 
(MENSAJE.id_mensaje=@id_mensaje or id_hilo_mensaje=@id_mensaje) and 
(MENSAJE_USUARIO.id_usuario=@id_usuario_destinatario or MENSAJE_USUARIO.id_usuario=@id_usuario_remitente)
order by mensaje.fecha_creacion asc
 end