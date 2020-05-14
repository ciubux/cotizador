-------------------------------- ALTER  pi_mensaje_visto_repuesta - opcion de añadir usuarios a la respuesta -------------------------

ALTER PROCEDURE [dbo].[pi_mensaje_visto_repuesta]
(@id_mensaje uniqueidentifier,
@id_usuario uniqueidentifier,
@respuesta text,
@titulo varchar(50),
@importancia varchar(10),
@usuarios UniqueIdentifierList readonly,
@reenvio int
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

insert mensaje (id_mensaje,fecha_inicio,id_usuario_creacion,usuario_modificacion,fecha_creacion,fecha_modificacion,es_respuesta,id_hilo_mensaje,mensaje,estado,titulo,importancia)values 
(@newId,dbo.getlocaldate(),@id_usuario,@id_usuario,dbo.getlocaldate(),dbo.getlocaldate(),1,case when @idHiloMensaje is null then @id_mensaje else @idHiloMensaje end,@respuesta,1,@titulo,@importancia)

insert into MENSAJE_USUARIO (id_usuario,id_mensaje)values(@id_usuario_mensaje,@newId)

declare @newIdReenvio varchar(50)
set @newIdReenvio=NEWID()

if @reenvio=1
begin
insert mensaje (id_mensaje,fecha_inicio,id_usuario_creacion,usuario_modificacion,fecha_creacion,fecha_modificacion,mensaje,estado,titulo,importancia)values 
(@newIdReenvio,dbo.getlocaldate(),@id_usuario,@id_usuario,dbo.getlocaldate(),dbo.getlocaldate(),@respuesta,1,@titulo,@importancia)
end

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
			   ,[id_mensaje])
			   
		 VALUES
			   (@idUsuario,
			    @newIdReenvio)			   

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

select  mensaje.id_mensaje,id_hilo_mensaje,id_usuario_creacion,mensaje,nombre,mensaje.fecha_creacion from mensaje  
inner join usuario on usuario.id_usuario=mensaje.id_usuario_creacion 
where (mensaje.id_hilo_mensaje=@id_mensaje_hilo or mensaje.id_mensaje=@id_mensaje_hilo) 
and (mensaje.id_usuario_creacion=@id_usuario or mensaje.id_usuario_creacion= (select id_usuario_creacion from mensaje where id_mensaje=@id_mensaje_mensaje_recibido ))
and mensaje.estado=1 and not mensaje.id_mensaje=@id_mensaje_mensaje_recibido
order by mensaje.fecha_creacion desc
end

----------------------------- ALTER  ps_vendedores - se agrega el maximo porcentaje de descuento aprobado en la tabla de busqueda -----------------------------------------


ALTER procedure [dbo].[ps_vendedores]
(@estado int,
@cod VARCHAR(5),
@descripcion varchar(50),
@email varchar(50),
@ciudad uniqueidentifier)
AS BEGIN 
IF @cod IS NULL  OR @cod ='' 
begin
SELECT Vendedor.id_vendedor,usuario.nombre as descripcion,VENDEDOR.codigo,cargo,email,vendedor.estado,VENDEDOR.usuario_creacion,VENDEDOR.fecha_creacion,VENDEDOR.usuario_modificacion,VENDEDOR.fecha_modificacion,concat('MP',CIUDAD.codigo_sede) as nombre,
usuario.maximo_porcentaje_descuento_aprobacion
FROM VENDEDOR 
inner join USUARIO on  VENDEDOR.id_usuario=USUARIO.id_usuario  
inner join CIUDAD on CIUDAD.id_ciudad=USUARIO.id_ciudad
where vendedor.estado=1  and USUARIO.nombre LIKE '%'+@descripcion+'%' and usuario.email LIKE '%'+@email+'%' 
and USUARIO.id_ciudad IN (case when @ciudad = '00000000-0000-0000-0000-000000000000' then CIUDAD.id_ciudad else @ciudad end)
ORDER BY USUARIO.nombre ASC
end
else 
begin
SELECT Vendedor.id_vendedor,descripcion,VENDEDOR.codigo,cargo,email,vendedor.estado,VENDEDOR.usuario_creacion,VENDEDOR.fecha_creacion,VENDEDOR.usuario_modificacion,VENDEDOR.fecha_modificacion,concat('MP',CIUDAD.codigo_sede) as nombre,
usuario.maximo_porcentaje_descuento_aprobacion
FROM USUARIO inner join VENDEDOR 
on  VENDEDOR.id_usuario=USUARIO.id_usuario 
inner join CIUDAD on CIUDAD.id_ciudad=USUARIO.id_ciudad  
where codigo like '%'+@cod+'%' and vendedor.estado=@estado ORDER BY USUARIO.nombre ASC
END 
END

SELECT Vendedor.id_vendedor,descripcion,VENDEDOR.codigo,cargo,email,vendedor.estado,VENDEDOR.usuario_creacion,VENDEDOR.fecha_creacion,VENDEDOR.usuario_modificacion,VENDEDOR.fecha_modificacion,concat('MP',CIUDAD.codigo_sede) as nombre,
usuario.maximo_porcentaje_descuento_aprobacion
FROM USUARIO inner join VENDEDOR 
on  VENDEDOR.id_usuario=USUARIO.id_usuario 
inner join CIUDAD on CIUDAD.id_ciudad=USUARIO.id_ciudad  
where codigo like '%'+''+'%' and vendedor.estado=1 ORDER BY USUARIO.nombre ASC