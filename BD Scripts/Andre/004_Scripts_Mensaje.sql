/* **** Create pi_mensaje - Inserta el mensaje en las tablas mensaje y mensaje_roles  **** */

CREATE   procedure [dbo].[pi_mensaje]
(@titulo varchar(60),
@mensaje text,
@importancia varchar(20),
@id_usuario_creacion uniqueidentifier,
@roles IntegerList readonly,
@fecha_vencimiento datetime
)
as  begin 

DECLARE @idRol int 
DECLARE @rolCursor CURSOR
declare @id_mensaje UNIQUEIDENTIFIER 

 set @id_mensaje=NEWID()

insert into mensaje (id_mensaje,id_usuario_creacion,fecha_creacion,mensaje,titulo,importancia,estado,fecha_vencimiento) 
values (@id_mensaje,@id_usuario_creacion,GETDATE(),@mensaje,@titulo,@importancia,1,@fecha_vencimiento)

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

/*********** Create ps_alerta_mensaje_usuario - Obtiene los mensajes pendientes de leer para el usuario ********/

create procedure [dbo].[ps_alerta_mensaje_usuario]
(@id_usuario uniqueidentifier)
as begin 

select mensaje.id_mensaje,titulo,mensaje,importancia,mensaje.id_usuario_creacion,mensaje.fecha_creacion,USUARIO.nombre,mensaje.fecha_vencimiento  from mensaje 

inner join mensaje_roles on   mensaje_roles.id_mensaje=mensaje.id_mensaje
inner join  ROL_USUARIO on ROL_USUARIO.id_rol=mensaje_roles.id_rol
inner join USUARIO on mensaje.id_usuario_creacion=usuario.id_usuario
where Mensaje.id_mensaje not in (select id_mensaje from MENSAJE_LEIDO)  and ROL_USUARIO.id_usuario=@id_usuario and mensaje.estado=1 

end 
/*********** Create pi_mensaje_visto - Inserta el mensaje leido por un usuario especifico ********/

CREATE PROCEDURE [dbo].[pi_mensaje_visto]
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
	[fecha_vencimiento] [datetime] NULL
)



/*-----------------------CREACION DE TABLA MENSAJE_LEDIO --------------------------------*/

CREATE TABLE [dbo].[MENSAJE_LEIDO](
	[id_mensaje] [uniqueidentifier] NULL,
	[id_usuario] [uniqueidentifier] NULL,
	[fecha_leido] [datetime] NULL
)

/*-----------------------CREACION DE TABLA MENSAJE_ROLES--------------------------------*/
CREATE TABLE [dbo].[MENSAJE_ROLES](
	[id_rol] [int] NULL,
	[id_mensaje] [uniqueidentifier] NULL
)

/*----------------------- Permisos para la tabla PERMISO Y USUARIO_PERMISO--------------------------------*/


insert into PERMISO (id_permiso,estado,fecha_creacion,fecha_modificacion,codigo,descripcion_corta,id_categoria_permiso,orden_permiso)
						values (84,1,dbo.getlocaldate(),dbo.getlocaldate(),'P421','crea_mensaje',7,11)
			
insert into USUARIO_PERMISO(id_permiso,id_usuario,estado,fecha_creacion,fecha_modificacion) 
							values (84,'BB85E214-3C69-44A4-B504-4CD223EC389C',1,dbo.getlocaldate(),dbo.getlocaldate())




				