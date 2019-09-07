/*********** Create ps_lista_mensajes - Lista los mensajes creados por el usuario ********/
create procedure [dbo].[ps_lista_mensajes]
(@estado smallint,
@fecha_creacion varchar(20),
@fecha_vencimiento varchar(20),
@id_usuario_creacion uniqueidentifier)
as begin 
 if @fecha_vencimiento is null and @fecha_creacion is null
 begin 
 select id_mensaje,mensaje.fecha_creacion,titulo,usuario.nombre,fecha_vencimiento,mensaje.fecha_inicio from mensaje inner join usuario on usuario.id_usuario=mensaje.id_usuario_creacion 
where mensaje.estado=@estado and id_usuario_creacion=@id_usuario_creacion order by fecha_creacion desc
end
 if @fecha_vencimiento is null 
 begin 
 select id_mensaje,mensaje.fecha_creacion,titulo,usuario.nombre,fecha_vencimiento,mensaje.fecha_inicio from mensaje inner join usuario on usuario.id_usuario=mensaje.id_usuario_creacion 
where mensaje.estado=@estado and id_usuario_creacion=@id_usuario_creacion and  CONVERT(date,mensaje.fecha_creacion)=@fecha_creacion
 end 
 if @fecha_creacion is null 
begin
select id_mensaje,mensaje.fecha_creacion,titulo,usuario.nombre,fecha_vencimiento,mensaje.fecha_inicio from mensaje inner join usuario on usuario.id_usuario=mensaje.id_usuario_creacion 
where mensaje.estado=@estado and id_usuario_creacion=@id_usuario_creacion and mensaje.fecha_vencimiento=@fecha_vencimiento
 end
end
/* **** Create pi_mensaje - Inserta el mensaje creado por el usuario en las tablas mensaje y mensaje_roles  **** */
create  procedure [dbo].[pi_mensaje]
(@titulo varchar(60),
@mensaje text,
@importancia varchar(20),
@id_usuario_creacion uniqueidentifier,
@roles IntegerList readonly,
@fecha_vencimiento datetime,
@id_usuario_modificacion uniqueidentifier,
@fecha_inicio datetime
)
as  begin 

DECLARE @idRol int 
DECLARE @rolCursor CURSOR
declare @id_mensaje UNIQUEIDENTIFIER 

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
END
end 
/*********** Create pu_mensaje - Actualiza el mensaje anteriormente insertado por el usuario ********/
create procedure [dbo].[pu_mensaje]
(@id_mensaje uniqueidentifier, 
@titulo varchar(50),
@mensaje text,
@importancia varchar(10),
@fecha_vencimiento datetime,
@fecha_inicio datetime,
@roles IntegerList readonly,
@id_usuario_modificacion uniqueidentifier)
as begin 

DECLARE @idRol int 
DECLARE @rolCursor CURSOR
 
update mensaje set  titulo=@titulo,
mensaje=@mensaje,
importancia=@importancia,
fecha_vencimiento=@fecha_vencimiento,
usuario_modificacion=@id_usuario_modificacion,
fecha_inicio=@fecha_inicio
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
end 

/*********** Create ps_detalle_mensaje - Obtiene el detalle de un mensaje creado por el usuario para su modificacion ********/

create procedure [dbo].[ps_detalle_mensaje] 
(@id_mensaje uniqueidentifier)
as 
begin

select id_mensaje,fecha_vencimiento,titulo,importancia,mensaje,fecha_inicio from mensaje where id_mensaje=@id_mensaje

SELECT id_rol from mensaje_roles where id_mensaje=@id_mensaje 

end 

/*********** Create ps_alerta_mensaje_usuario - Obtiene los mensajes pendientes de leer para el usuario ********/
create procedure [dbo].[ps_alerta_mensaje_usuario] 
(@id_usuario uniqueidentifier)
as begin 

select mensaje.id_mensaje,titulo,mensaje,importancia,mensaje.fecha_creacion,USUARIO.nombre,mensaje.fecha_vencimiento,mensaje.fecha_inicio from mensaje 

inner join mensaje_roles on   mensaje_roles.id_mensaje=mensaje.id_mensaje
inner join  ROL_USUARIO on ROL_USUARIO.id_rol=mensaje_roles.id_rol
inner join USUARIO on mensaje.id_usuario_creacion=usuario.id_usuario
where Mensaje.id_mensaje not in (select id_mensaje from MENSAJE_LEIDO)  and ROL_USUARIO.id_usuario=@id_usuario and mensaje.estado=1 
and fecha_inicio <= Format(GetDate(), N'yyyydd-MM-dd') and fecha_vencimiento>= Format(GetDate(), N'yyyydd-MM-dd')
order by  mensaje.fecha_creacion desc
end 
/*********** Create pi_mensaje_visto - Inserta el mensaje leido por un usuario especifico ********/

create PROCEDURE [dbo].[pi_mensaje_visto]
(@id_mensaje uniqueidentifier,
@id_usuario uniqueidentifier)
as begin 
insert MENSAJE_LEIDO(id_mensaje,id_usuario,fecha_leido) values(@id_mensaje,@id_usuario,GETDATE())
end 

/*-----------------------CREACION DE TABLA MENSAJE --------------------------------*/
CREATE TABLE [dbo].[MENSAJE](
	[id_mensaje] [uniqueidentifier] NULL,
	[fecha_creacion] [datetime] NULL,
	[titulo] [varchar](50) NULL,
	[mensaje] [text] NULL,
	[importancia] [varchar](10) NULL,
	[estado] [smallint] NULL,
	[id_usuario_creacion] [uniqueidentifier] NULL,
	[fecha_vencimiento] [date] NULL,
	[fecha_modificacion] [datetime] NULL,
	[usuario_modificacion] [uniqueidentifier] NULL,
	[fecha_inicio] [date] NULL
)


/*-----------------------CREACION DE TABLA MENSAJE_LEDIO --------------------------------*/

CREATE TABLE [dbo].[MENSAJE_LEIDO](
	[id_mensaje] [uniqueidentifier] NULL,
	[id_usuario] [uniqueidentifier] NULL,
	[fecha_leido] [datetime] NULL
)
select * from mensaje 
/*-----------------------CREACION DE TABLA MENSAJE_ROLES--------------------------------*/
CREATE TABLE [dbo].[MENSAJE_ROLES](
	[id_rol] [int] NULL,
	[id_mensaje] [uniqueidentifier] NULL
)

/*----------------------- Permisos para la tabla PERMISO Y USUARIO_PERMISO--------------------------------*/

insert into PERMISO (id_permiso,estado,fecha_creacion,fecha_modificacion,codigo,descripcion_corta,id_categoria_permiso,orden_permiso)
						values (84,1,dbo.getlocaldate(),dbo.getlocaldate(),'P421','modifica_mensaje',7,11)
insert into PERMISO (id_permiso,estado,fecha_creacion,fecha_modificacion,codigo,descripcion_corta,id_categoria_permiso,orden_permiso)
						values (85,1,dbo.getlocaldate(),dbo.getlocaldate(),'P422','lista_mensaje',7,11)
			
insert into USUARIO_PERMISO(id_permiso,id_usuario,estado,fecha_creacion,fecha_modificacion) 
							values (84,'BB85E214-3C69-44A4-B504-4CD223EC389C',1,dbo.getlocaldate(),dbo.getlocaldate())
insert into USUARIO_PERMISO(id_permiso,id_usuario,estado,fecha_creacion,fecha_modificacion) 
							values (85,'BB85E214-3C69-44A4-B504-4CD223EC389C',1,dbo.getlocaldate(),dbo.getlocaldate())




				
