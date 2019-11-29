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

/***************************** ALTER PU_MENSAJE - USUARIO AÑADIDO **********************************/
ALTER   procedure [dbo].[pu_mensaje]
(@id_mensaje uniqueidentifier, 
@titulo varchar(50),
@mensaje text,
@importancia varchar(10),
@fecha_vencimiento datetime,
@fecha_inicio datetime,
@roles IntegerList readonly,
@id_usuario_modificacion uniqueidentifier,
@usuarios UniqueIdentifierList readonly)
as begin 

DECLARE @idRol int 
DECLARE @rolCursor CURSOR

DECLARE @idUsuario uniqueidentifier 
DECLARE @usuarioCursor CURSOR
 
update mensaje set  titulo=@titulo,
mensaje=@mensaje,
importancia=@importancia,
fecha_vencimiento=@fecha_vencimiento,
usuario_modificacion=@id_usuario_modificacion,
fecha_inicio=@fecha_inicio,
fecha_modificacion=getdate()
 where id_mensaje=@id_mensaje

delete from MENSAJE_ROLES where id_mensaje=@id_mensaje
 SET @rolCursor = CURSOR FOR
SELECT ID
FROM @roles

OPEN @rolCursor
FETCH NEXT
FROM @rolCursor INTO @idRol
WHILE @@FETCH_STATUS = 0
BEGIN 
 INSERT INTO mensaje_roles
			   ([id_rol]
			   ,[id_mensaje])			   
		 VALUES
			   (@idRol,
			    @id_mensaje)	
 FETCH NEXT
	FROM @rolCursor INTO @idRol
end 

delete from MENSAJE_USUARIO where id_mensaje=@id_mensaje

SET @usuarioCursor = CURSOR FOR
SELECT ID
FROM @usuarios
OPEN @usuarioCursor
FETCH NEXT
FROM @usuarioCursor INTO @idUsuario
WHILE @@FETCH_STATUS = 0
BEGIN
	INSERT INTO mensaje_usuario
			   ([id_usuario]
			   ,[id_mensaje])
			   
		 VALUES
			   (@idUsuario,
			    @id_mensaje)			   

    FETCH NEXT
	FROM @usuarioCursor INTO @idUsuario
END
end 

/********************************** ALTER ps_detalle_mensaje - USUARIO AÑADIDO  **************************************/
ALTER procedure [dbo].[ps_detalle_mensaje] 
(@id_mensaje uniqueidentifier)
as 
begin
select id_mensaje,fecha_vencimiento,titulo,importancia,mensaje,fecha_inicio from mensaje where id_mensaje=@id_mensaje
SELECT id_rol from mensaje_roles where id_mensaje=@id_mensaje 
SELECT mensaje_usuario.id_usuario,nombre,email from mensaje_usuario left join usuario on usuario.id_usuario=mensaje_usuario.id_usuario where  mensaje_usuario.id_mensaje=@id_mensaje
end 
/********************************** ALTER pi_mensaje - USUARIO AÑADIDO *****************************************/
ALTER   procedure [dbo].[pi_mensaje]
(@titulo varchar(60),
@mensaje text,
@importancia varchar(20),
@id_usuario_creacion uniqueidentifier,
@roles IntegerList readonly,
@usuarios UniqueIdentifierList readonly,
@fecha_vencimiento datetime,
@id_usuario_modificacion uniqueidentifier,
@fecha_inicio datetime
)
as  begin 

DECLARE @idRol int 
DECLARE @rolCursor CURSOR
declare @id_mensaje UNIQUEIDENTIFIER 

DECLARE @idUsuario uniqueidentifier 
DECLARE @usuarioCursor CURSOR


 set @id_mensaje=NEWID()

insert into mensaje (id_mensaje,id_usuario_creacion,fecha_creacion,mensaje,titulo,importancia,estado,fecha_vencimiento,fecha_modificacion,usuario_modificacion,fecha_inicio) 
values (@id_mensaje,@id_usuario_creacion,GETDATE(),@mensaje,@titulo,@importancia,1,@fecha_vencimiento,GETDATE(),@id_usuario_modificacion,@fecha_inicio)

SET @rolCursor = CURSOR FOR
SELECT ID
FROM @roles

OPEN @rolCursor
FETCH NEXT
FROM @rolCursor INTO @idRol
WHILE @@FETCH_STATUS = 0
BEGIN
	INSERT INTO mensaje_roles
			   ([id_rol]
			   ,[id_mensaje])
			   
		 VALUES
			   (@idRol,
			    @id_mensaje)
    FETCH NEXT
	FROM @rolCursor INTO @idRol
end
SET @usuarioCursor = CURSOR FOR
SELECT ID
FROM @usuarios
OPEN @usuarioCursor
FETCH NEXT
FROM @usuarioCursor INTO @idUsuario
WHILE @@FETCH_STATUS = 0
BEGIN
	INSERT INTO mensaje_usuario
			   ([id_usuario]
			   ,[id_mensaje])
			   
		 VALUES
			   (@idUsuario,
			    @id_mensaje)
			   

    FETCH NEXT
	FROM @usuarioCursor INTO @idUsuario
END
end
/********************************** ALTER ps_lista_mensajes - USUARIO AÑADIDO  **********************************************************/

ALTER procedure [dbo].[ps_lista_mensajes] 
(@estado smallint,
@fecha_creacion_desde datetime,
@fecha_creacion_hasta datetime,
@fecha_vencimiento_desde datetime,
@fecha_vencimiento_hasta datetime,
@id_usuario_creacion uniqueidentifier)
as begin  
if(@fecha_vencimiento_desde is null and @fecha_vencimiento_hasta is null and @fecha_creacion_desde is null and @fecha_creacion_hasta is null )
begin
select id_mensaje,mensaje.fecha_creacion,titulo,usuario.nombre,fecha_vencimiento,mensaje.fecha_inicio from mensaje inner join usuario on usuario.id_usuario=mensaje.id_usuario_creacion 
where mensaje.estado=@estado and id_usuario_creacion=@id_usuario_creacion
order by fecha_creacion desc
end
else
begin
select id_mensaje,mensaje.fecha_creacion,titulo,usuario.nombre,fecha_vencimiento,mensaje.fecha_inicio from mensaje inner join usuario on usuario.id_usuario=mensaje.id_usuario_creacion 
where mensaje.estado=@estado and id_usuario_creacion=@id_usuario_creacion 
and ( convert(date,mensaje.fecha_creacion) >= @fecha_creacion_desde AND	convert(date,mensaje.fecha_creacion) <=  @fecha_creacion_hasta )
or (mensaje.fecha_vencimiento >= @fecha_vencimiento_desde AND	mensaje.fecha_vencimiento <=  @fecha_vencimiento_hasta)
order by fecha_creacion desc
end
end 


