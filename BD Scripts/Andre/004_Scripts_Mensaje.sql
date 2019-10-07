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


CREATE TABLE [dbo].[MENSAJE_USUARIO](
	[id_usuario] [int] NULL,
	[id_mensaje] [uniqueidentifier] NULL
)




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





/*********** Create ps_alerta_mensaje_usuario - Obtiene los mensajes pendientes de leer para el usuario ********/
create procedure [dbo].[ps_alerta_mensaje_usuario] 
(@id_usuario uniqueidentifier)
as begin 

select mensaje.id_mensaje,titulo,mensaje,importancia,mensaje.fecha_creacion,USUARIO.nombre,mensaje.fecha_vencimiento,mensaje.fecha_inicio from mensaje 

inner join mensaje_roles on   mensaje_roles.id_mensaje=mensaje.id_mensaje
inner join  ROL_USUARIO on ROL_USUARIO.id_rol=mensaje_roles.id_rol
inner join USUARIO on mensaje.id_usuario_creacion=usuario.id_usuario
where Mensaje.id_mensaje not in (select id_mensaje from MENSAJE_LEIDO)  and ROL_USUARIO.id_usuario=@id_usuario and mensaje.estado=1 
and fecha_inicio <= dbo.getlocaldate()  and fecha_vencimiento >= dbo.getlocaldate() and not mensaje.id_usuario = @id_usuarop 
order by  mensaje.fecha_creacion desc
end 
/*********** Create pi_mensaje_visto - Inserta el mensaje leido por un usuario especifico ********/

create PROCEDURE [dbo].[pi_mensaje_visto]
(@id_mensaje uniqueidentifier,
@id_usuario uniqueidentifier)
as begin 
insert MENSAJE_LEIDO(id_mensaje,id_usuario,fecha_leido) values(@id_mensaje,@id_usuario,dbo.getlocaldate())
end 

/*----------------------- Permisos para la tabla PERMISO Y USUARIO_PERMISO--------------------------------*/




  
insert into PERMISO (id_permiso,estado,fecha_creacion,fecha_modificacion,codigo,descripcion_corta,id_categoria_permiso,orden_permiso)
						values (93,1,dbo.getlocaldate(),dbo.getlocaldate(),'P831','modifica_mensaje',7,11)
insert into PERMISO (id_permiso,estado,fecha_creacion,fecha_modificacion,codigo,descripcion_corta,id_categoria_permiso,orden_permiso)
						values (94,1,dbo.getlocaldate(),dbo.getlocaldate(),'P830','lista_mensaje',7,11)
			





				
