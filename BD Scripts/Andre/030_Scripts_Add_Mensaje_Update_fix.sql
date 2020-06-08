--------------------------------------------- ALTER pi_mensaje_visto - Correccion de la funcion de obtencion de la fecha ------------------------
ALTER PROCEDURE [dbo].[pi_mensaje_visto]
(@id_mensaje uniqueidentifier,
@id_usuario uniqueidentifier)
as begin 
insert MENSAJE_LEIDO(id_mensaje,id_usuario,fecha_leido) values(@id_mensaje,@id_usuario,dbo.getlocaldate())
end 

--------------------------------------------- ALTER pi_mensaje_visto_repuesta - Cambio de nombre del  cursor ------------------------
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
DECLARE @usuarioRespuestaCursor CURSOR

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

SET @usuarioRespuestaCursor = CURSOR FOR
SELECT ID
FROM @usuarios
OPEN @usuarioRespuestaCursor
FETCH NEXT
FROM @usuarioRespuestaCursor INTO @idUsuario
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
	FROM @usuarioRespuestaCursor INTO @idUsuario
	end
end

end

------------------------------------- ALTER ps_lista_mensajes - Busqueda de mensajes por bandeja -------------------------------------------------------
ALTER procedure [dbo].[ps_lista_mensajes]
(@estado smallint,
@fecha_creacion_desde datetime,
@fecha_creacion_hasta datetime,
@fecha_vencimiento_desde datetime,
@fecha_vencimiento_hasta datetime,
@id_usuario_creacion uniqueidentifier,
@fecha_entrada_desde datetime,
@fecha_entrada_hasta datetime,
@bandeja smallint)
as begin  
if @bandeja=1
begin 
 
if(@fecha_vencimiento_desde is null and @fecha_vencimiento_hasta is null and @fecha_creacion_desde is null and @fecha_creacion_hasta is null )
begin

select distinct usuario.nombre,mensaje.id_usuario_creacion,mensaje.id_hilo_mensaje ,mensaje.es_respuesta , mensaje.id_mensaje,mensaje.fecha_creacion,titulo,fecha_vencimiento,mensaje.fecha_inicio from mensaje 
INNER JOIN MENSAJE_USUARIO ON  MENSAJE_USUARIO.id_mensaje = MENSAJE.id_mensaje
left join mensaje_roles on   mensaje_roles.id_mensaje=mensaje.id_mensaje
left join  ROL_USUARIO on ROL_USUARIO.id_rol=mensaje_roles.id_rol
inner join USUARIO on mensaje.id_usuario_creacion=usuario.id_usuario
 where id_usuario_creacion=@id_usuario_creacion and (id_hilo_mensaje in(select id_mensaje from mensaje where id_usuario_creacion=@id_usuario_creacion) or id_hilo_mensaje is null) and mensaje.es_respuesta is null    or 
 mensaje.id_mensaje in (select distinct mensaje.id_mensaje from MENSAJE_USUARIO 
 left join MENSAJE on MENSAJE.id_mensaje= MENSAJE_USUARIO.id_mensaje
left join mensaje_roles on   mensaje_roles.id_mensaje=mensaje.id_mensaje
left join  ROL_USUARIO on ROL_USUARIO.id_rol=mensaje_roles.id_rol
  where id_usuario_creacion =@id_usuario_creacion and reenvio =1
  and   mensaje.id_hilo_mensaje  in (select id_mensaje from mensaje where id_usuario_creacion != @id_usuario_creacion))
order by fecha_creacion desc

end
else
begin
select distinct usuario.nombre,mensaje.id_usuario_creacion,mensaje.id_hilo_mensaje ,mensaje.es_respuesta , mensaje.id_mensaje,mensaje.fecha_creacion,titulo,fecha_vencimiento,mensaje.fecha_inicio from mensaje 
INNER JOIN MENSAJE_USUARIO ON  MENSAJE_USUARIO.id_mensaje = MENSAJE.id_mensaje
left join mensaje_roles on   mensaje_roles.id_mensaje=mensaje.id_mensaje
left join  ROL_USUARIO on ROL_USUARIO.id_rol=mensaje_roles.id_rol
inner join USUARIO on mensaje.id_usuario_creacion=usuario.id_usuario
 where id_usuario_creacion=@id_usuario_creacion and mensaje.es_respuesta is null   or  
 mensaje.id_mensaje in (select distinct mensaje.id_mensaje from MENSAJE_USUARIO 
 left join MENSAJE on MENSAJE.id_mensaje= MENSAJE_USUARIO.id_mensaje
left join mensaje_roles on   mensaje_roles.id_mensaje=mensaje.id_mensaje
left join  ROL_USUARIO on ROL_USUARIO.id_rol=mensaje_roles.id_rol
  where id_usuario_creacion =@id_usuario_creacion and reenvio =1
  and 
   mensaje.id_hilo_mensaje  in (select id_mensaje from mensaje where id_usuario_creacion != @id_usuario_creacion)) and
 ( convert(date,mensaje.fecha_creacion) >= @fecha_creacion_desde AND	convert(date,mensaje.fecha_creacion) <=  @fecha_creacion_hasta )
or (mensaje.fecha_vencimiento >= @fecha_vencimiento_desde AND	mensaje.fecha_vencimiento <=  @fecha_vencimiento_hasta)
order by fecha_creacion desc
end
end
if @bandeja=0
select distinct mensaje.id_mensaje,mensaje.fecha_creacion,titulo,usuario.nombre,fecha_vencimiento,mensaje.fecha_inicio  from mensaje
left join MENSAJE_USUARIO on mensaje.id_mensaje=MENSAJE_USUARIO.id_mensaje
left join mensaje_roles on   mensaje_roles.id_mensaje=mensaje.id_mensaje
left join  ROL_USUARIO on ROL_USUARIO.id_rol=mensaje_roles.id_rol
inner join USUARIO on mensaje.id_usuario_creacion=usuario.id_usuario
where 
(ROL_USUARIO.id_usuario=@id_usuario_creacion
or MENSAJE_USUARIO.id_usuario=@id_usuario_creacion) and mensaje.estado=1
and  mensaje.es_respuesta is null and fecha_vencimiento is not null or mensaje.id_mensaje in (select distinct MENSAJE_USUARIO.id_mensaje from MENSAJE_USUARIO 
  where  MENSAJE_USUARIO.id_usuario=@id_usuario_creacion and reenvio =1)
and fecha_inicio >= convert(date,@fecha_entrada_desde)  and fecha_inicio <= convert(date,@fecha_entrada_hasta)
order by  mensaje.fecha_creacion desc
end 


/************************************ ALTER ps_ver_usuario_respuestas - busqueda de usuariosque respondieron al mensaje por bandeja *****************************************************/

ALTER procedure [dbo].[ps_ver_usuario_respuestas] 
(
@id_mensaje uniqueidentifier,
@id_usuario uniqueidentifier,
@bandeja int 
)
as begin
if (@bandeja = 1)
begin 

declare @user_mensaje_original uniqueidentifier
set @user_mensaje_original = (select id_usuario_creacion from mensaje where id_mensaje=(select id_hilo_mensaje from mensaje where id_mensaje=@id_mensaje))
select distinct USUARIO.nombre,usuario.id_usuario from MENSAJE_USUARIO 
inner join  MENSAJE on MENSAJE.id_mensaje= MENSAJE_USUARIO.id_mensaje
inner join usuario on USUARIO.id_usuario= MENSAJE_USUARIO.id_usuario
where MENSAJE.id_mensaje=@id_mensaje
and USUARIO.id_usuario  not in (@id_usuario,case when @user_mensaje_original is null then '00000000-0000-0000-0000-000000000000'  else @user_mensaje_original end)
end
if (@bandeja = 0)
begin 
SELECT DISTINCT usuario.id_usuario,USUARIO.nombre, MIN(mensaje.fecha_creacion) as fecha_creacion 
FROM  mensaje 
inner join usuario on mensaje.id_usuario_creacion = usuario.id_usuario
left join MENSAJE_USUARIO on mensaje.id_mensaje=MENSAJE_USUARIO.id_mensaje
left join mensaje_roles on   mensaje_roles.id_mensaje=mensaje.id_mensaje
left join  ROL_USUARIO on ROL_USUARIO.id_rol=mensaje_roles.id_rol
where (MENSAJE.id_mensaje=@id_mensaje)
GROUP BY usuario.id_usuario,USUARIO.nombre
ORDER BY MIN(mensaje.fecha_creacion) asc, usuario.id_usuario
end
end 

/**************************ALTER ps_ver_respuestas_usuario -  asjustes para respuestas de reenvio ******************************************/
ALTER procedure [dbo].[ps_ver_respuestas_usuario] 
(
@id_mensaje uniqueidentifier,
@id_usuario_destinatario uniqueidentifier,
@id_usuario_remitente uniqueidentifier
)
as begin

select * from mensaje 
left join usuario on mensaje.id_usuario_creacion = usuario.id_usuario
  where 
(MENSAJE.id_mensaje= @id_mensaje or id_hilo_mensaje= @id_mensaje) and 
(MENSAJE.id_usuario_creacion=@id_usuario_destinatario or MENSAJE.id_usuario_creacion=@id_usuario_remitente)
or
( mensaje.id_mensaje=(select id_mensaje from MENSAJE_USUARIO where id_mensaje=@id_mensaje and id_usuario=@id_usuario_destinatario and  reenvio=1))
order by mensaje.fecha_creacion asc
 end

 