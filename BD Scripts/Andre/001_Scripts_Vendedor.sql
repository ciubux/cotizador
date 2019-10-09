/*********** Create ps_vendedores - Lista de vendedores  *********/
create procedure [dbo].[ps_vendedores](@estado int,@cod VARCHAR(2),@descripcion varchar(50),@email varchar(50))
AS BEGIN 
IF @cod IS NULL  OR @cod ='' 
begin
SELECT Vendedor.id_vendedor,descripcion,VENDEDOR.codigo,cargo,email,vendedor.estado,VENDEDOR.usuario_creacion,VENDEDOR.fecha_creacion,VENDEDOR.usuario_modificacion,VENDEDOR.fecha_modificacion
FROM USUARIO inner join VENDEDOR 
on  VENDEDOR.id_usuario=USUARIO.id_usuario  
where vendedor.estado=@estado  and vendedor.descripcion LIKE '%'+@descripcion+'%' and usuario.email LIKE '%'+@email+'%'
end
else 
begin
SELECT Vendedor.id_vendedor,descripcion,VENDEDOR.codigo,cargo,email,vendedor.estado,VENDEDOR.usuario_creacion,VENDEDOR.fecha_creacion,VENDEDOR.usuario_modificacion,VENDEDOR.fecha_modificacion
FROM USUARIO inner join VENDEDOR 
on  VENDEDOR.id_usuario=USUARIO.id_usuario  
where codigo like '%'+@cod+'%' and vendedor.estado=@estado
END 
END

/********** Create ps_lista_usuarios - Obtiene una lista de usuario para el chosen ****************/
create procedure [dbo].[ps_lista_usuarios]
(@BusquedaUsuario varchar(50))
as begin
if @BusquedaUsuario=''
begin
select id_usuario,email,nombre,* from USUARIO 
where 
NOT EXISTS(SELECT VENDEDOR.id_usuario FROM VENDEDOR WHERE VENDEDOR.id_usuario=USUARIO.id_usuario) AND 
USUARIO.usuario_pruebas != 1 
and USUARIO.estado = 1 
and USUARIO.es_cliente != 1 
and USUARIO.id_usuario!='412ACDEE-FE20-4539-807C-D00CD71359D6'
and USUARIO.id_usuario!='AFBE4EB1-B5AE-4430-9EA1-15D79D4B5E98' order by USUARIO.nombre asc
end 
else 
select 
id_usuario,
 REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(nombre, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')   as nombre,
   REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(email, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  as email ,
email,
nombre from USUARIO 
where 
NOT EXISTS(SELECT VENDEDOR.id_usuario FROM VENDEDOR WHERE VENDEDOR.id_usuario=USUARIO.id_usuario) 
AND USUARIO.usuario_pruebas != 1 
and USUARIO.estado = 1 
and USUARIO.es_cliente != 1 
AND (REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(nombre, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  LIKE '%'+@BusquedaUsuario+'%' OR
nombre like '%'+@BusquedaUsuario +'%' or email like '%'+@BusquedaUsuario +'%' or
REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(email, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  LIKE '%'+@BusquedaUsuario+'%')
and USUARIO.id_usuario != '412ACDEE-FE20-4539-807C-D00CD71359D6' 
and  USUARIO.id_usuario != 'AFBE4EB1-B5AE-4430-9EA1-15D79D4B5E98' order by USUARIO.nombre asc
end 
/********** Create ps_lista_vendedores_supervisores - Obtiene una lista de supervisores para el chosen ****************/

create procedure [dbo].[ps_lista_vendedores_supervisores] 
(@BusquedaVendedor varchar(50))
as begin 
if @BusquedaVendedor=''
begin
select usuario.nombre,usuario.email,VENDEDOR.codigo,VENDEDOR.id_vendedor from VENDEDOR inner join usuario on usuario.id_usuario=VENDEDOR.id_usuario
where VENDEDOR.es_supervisor_comercial = 1 
and VENDEDOR.estado=1
and VENDEDOR.id_usuario is not  null
end

else
select 
USUARIO.id_usuario,
VENDEDOR.codigo,VENDEDOR.id_vendedor,
 REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(usuario.nombre, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')   as nombre,
   REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(usuario.email, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  as email 

  from VENDEDOR inner join usuario on usuario.id_usuario=VENDEDOR.id_usuario
where VENDEDOR.es_supervisor_comercial = 1 
and VENDEDOR.estado=1
and VENDEDOR.id_usuario is not  null
AND (REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(USUARIO.nombre, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  LIKE '%'+@BusquedaVendedor+'%' OR
nombre like '%'+@BusquedaVendedor +'%' or email like '%'+@BusquedaVendedor +'%' or
REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(USUARIO.email, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  LIKE '%'+@BusquedaVendedor+'%')
end 

/********** Create ps_supervisor_get - Obtiene datos del supervisor seleccionado por el chosen  ****************/

CREATE procedure ps_supervisor_get 
(@id_supervisor uniqueidentifier)
as begin 
select vendedor.id_usuario,id_vendedor,usuario.nombre,usuario.email from vendedor inner join usuario on vendedor.id_usuario=USUARIO.id_usuario where VENDEDOR.id_usuario=@id_supervisor and VENDEDOR.estado=1
end 

/********** Create ps_usuario_get_vendedor - Obtiene datos del usuario seleccionado por el chosen ***********/
create PROCEDURE [dbo].[ps_usuario_get_vendedor]
	@idUsuario uniqueidentifier
AS
BEGIN
	SELECT id_usuario, cargo, nombre , contacto, es_cliente, estado, email, id_ciudad
	FROM USUARIO 
	WHERE id_usuario = @idUsuario;
END

/********************* Create ps_detalle_vendedores - Obtiene el detalle de un vendedor por id_vendedor **************/

create procedure [dbo].[ps_detalle_vendedores] 
(@idUsuarioVenta int)
AS BEGIN 
SELECT vendedor.id_vendedor,
vendedor.codigo,
USUARIO.id_ciudad,
USUARIO.id_usuario,
VENDEDOR.es_asistente_servicio_cliente,
VENDEDOR.es_supervisor_comercial,
VENDEDOR.es_responsable_comercial,
VENDEDOR.id_supervisor_comercial,
(select usuario.nombre from vendedor inner join usuario on usuario.id_usuario=VENDEDOR.id_usuario where id_vendedor=(select id_supervisor_comercial from vendedor where id_vendedor=@idUsuarioVenta)) AS nombre_supervisor,
(select usuario.id_usuario from vendedor inner join usuario on usuario.id_usuario=VENDEDOR.id_usuario where id_vendedor=(select id_supervisor_comercial from vendedor where id_vendedor=@idUsuarioVenta)) AS id_usuario_supervisor,
(select usuario.email from vendedor inner join usuario on usuario.id_usuario=VENDEDOR.id_usuario where id_vendedor=(select id_supervisor_comercial from vendedor where id_vendedor=@idUsuarioVenta)) AS email_supervisor,
vendedor.estado,
nombre,email,
usuario.password
,cargo,contacto,
maximo_porcentaje_descuento_aprobacion,(select vendedor.usuario_creacion from usuario  inner join vendedor on vendedor.id_usuario=usuario.id_usuario where vendedor.id_vendedor=@idUsuarioVenta)as USUARIO_CREACION,vendedor.fecha_creacion,
(select vendedor.usuario_modificacion from usuario  inner join vendedor on vendedor.id_usuario=usuario.id_usuario where vendedor.id_vendedor=@idUsuarioVenta) as USUARIO_MODIFICACION,vendedor.fecha_modificacion,
(select nombre from ciudad where id_ciudad=(select id_ciudad from USUARIO inner join vendedor on usuario.id_usuario=vendedor.id_usuario where id_vendedor=@idUsuarioVenta))AS CIUDAD FROM USUARIO  inner join vendedor on  vendedor.id_usuario=USUARIO.id_usuario WHERE vendedor.id_vendedor=@idUsuarioVenta
END

/************* Create pi_vendedor - Agregar un vendedor ********************/

create PROCEDURE [dbo].[pi_vendedor]
( 
 
  @codigo varchar(20), 
  @cargo varchar(50),
  @nombre varchar(50),
  @contacto varchar(50),
  @estado int,
  @usuario_creacion uniqueidentifier,  
  @maximo_descuento decimal(5,2),
  @id_ciudad uniqueidentifier,
  @id_usuario_vendedor uniqueidentifier,
  @es_supervisor_comercial int,
  @es_responsable_comercial int ,
  @es_asistente_servicio_cliente int,
  @id_supervisor_comercial int
   )
as begin 
declare @id_vendedor int
set @id_vendedor=(SELECT MAX( id_vendedor )+1  FROM VENDEDOR)
update usuario set 
					cargo=@cargo,
					nombre=@nombre,
					contacto=@contacto,
					id_ciudad=@id_ciudad,
					usuario_modificacion=@usuario_creacion,				
					fecha_modificacion=GETDATE()
					where usuario.id_usuario=@id_usuario_vendedor
					
insert  into VENDEDOR(
						id_usuario,
						id_vendedor,
						codigo,
						descripcion,
						estado,
						es_supervisor_comercial,
						es_responsable_comercial,
						es_asistente_servicio_cliente,
						usuario_creacion,
						fecha_creacion,
						usuario_modificacion,
						fecha_modificacion,
						id_supervisor_comercial) 
						values (@id_usuario_vendedor,
								@id_vendedor,								
								@codigo,
								@nombre,
								@estado,
								@es_supervisor_comercial,
								1,
								@es_asistente_servicio_cliente,
								@usuario_creacion,
								dbo.getlocaldate(),
								@usuario_creacion,
								dbo.getlocaldate(),
								(case when @id_supervisor_comercial = 0 then null else @id_supervisor_comercial end)
								)  
  END

  /***************** Create pu_vendedores - Actualiza un vendedor ***********************/

  create procedure [dbo].[pu_vendedores] 
						(
						@id_vendedor int , 
						@usuario_modificacion uniqueidentifier,
						@codigo varchar(10),	
						@cargo varchar(50),
						@nombre varchar(50),
						@contacto varchar(50),
						@max_por_des_apro decimal(5,2),
						@estado int,
						@id_ciudad 	uniqueidentifier	,
						@id_usuario_vendedor uniqueidentifier,
							  @es_supervisor_comercial int,
							  @es_responsable_comercial int ,
							  @es_asistente_servicio_cliente int,
							  @id_supervisor_comercial int												
						)
						as 
						begin 
							SET NOCOUNT ON	
							
								begin						
									update us set 											
											us.cargo=@cargo,
											us.nombre=@nombre,
											us.contacto=@contacto,
											us.maximo_porcentaje_descuento_aprobacion=@max_por_des_apro,
											us.estado=@estado,
											us.fecha_modificacion=dbo.getlocaldate(),
											us.usuario_modificacion=@usuario_modificacion,		
											us.id_ciudad=@id_ciudad
									from usuario as us
									INNER JOIN vendedor ve ON (ve.id_usuario = us.id_usuario)
										where ve.id_vendedor=@id_vendedor
							update ve set 
											ve.codigo=@codigo,
											ve.descripcion=@nombre,
											ve.estado=@estado,			
											ve.usuario_modificacion=@usuario_modificacion,
											ve.fecha_modificacion=dbo.getlocaldate(),

											ve.es_supervisor_comercial=@es_supervisor_comercial,
											ve.es_responsable_comercial =1 ,
											
											ve.es_asistente_servicio_cliente=@es_asistente_servicio_cliente,
											ve.id_supervisor_comercial=(case when @id_supervisor_comercial = 0 then null else @id_supervisor_comercial end)

									from vendedor as ve
								INNER JOIN usuario us ON (ve.id_usuario = us.id_usuario)
										where ve.id_vendedor=@id_vendedor
									end 
							END
/******************************** ALTER TABLE VENDEDORES **************************************************/

alter table vendedor 
add id_supervisor_comercial int

					
/******************************** Permisos para la tabla PERMISO Y USUARIO_PERMISO **************************************************/

insert into PERMISO (estado,fecha_creacion,fecha_modificacion,codigo,descripcion_corta,id_categoria_permiso,orden_permiso)
						values (1,dbo.getlocaldate(),dbo.getlocaldate(),'P073','visualiza_vendedores',7,6)

insert into PERMISO (estado,fecha_creacion,fecha_modificacion,codigo,descripcion_corta,id_categoria_permiso,orden_permiso)
						values (1,dbo.getlocaldate(),dbo.getlocaldate(),'P072','modifica_vendedores',7,6)


insert into USUARIO_PERMISO(id_permiso,id_usuario,estado,fecha_creacion,fecha_modificacion) 
							values (78,'7E875C20-24FD-4B5E-9BB5-A7F23718F1FA',1,dbo.getlocaldate(),dbo.getlocaldate())

insert into USUARIO_PERMISO(id_permiso,id_usuario,estado,fecha_creacion,fecha_modificacion) 
							values (79,'7E875C20-24FD-4B5E-9BB5-A7F23718F1FA',1,dbo.getlocaldate(),dbo.getlocaldate())


