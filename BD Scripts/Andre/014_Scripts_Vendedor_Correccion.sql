/**********************************************/
ALTER PROCEDURE [dbo].[ps_usuario_get_vendedor]
	@idUsuario uniqueidentifier
AS
BEGIN
	SELECT id_usuario, cargo, nombre , contacto, es_cliente, estado, email, id_ciudad,maximo_porcentaje_descuento_aprobacion
	FROM USUARIO 
	WHERE id_usuario = @idUsuario;
END
/***********************************************************************/
ALTER procedure [dbo].[ps_lista_usuarios] 
(@BusquedaUsuario varchar(50))
as begin
select 
id_usuario,
 REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(nombre, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')   as nombre,
   REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(email, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  as email 
from USUARIO 
where 
USUARIO.estado = 1 and
NOT EXISTS(SELECT VENDEDOR.id_usuario FROM VENDEDOR WHERE VENDEDOR.id_usuario=USUARIO.id_usuario) 
and not USUARIO.id_usuario = '412ACDEE-FE20-4539-807C-D00CD71359D6' 
and not USUARIO.id_usuario = 'AFBE4EB1-B5AE-4430-9EA1-15D79D4B5E98' 
and (USUARIO.usuario_pruebas =0 or USUARIO.usuario_pruebas is null)
and (USUARIO.es_cliente = 0 or USUARIO.es_cliente is null)
AND (REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(nombre, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  LIKE '%'+@BusquedaUsuario+'%' OR
nombre like '%'+@BusquedaUsuario +'%' or email like '%'+@BusquedaUsuario +'%' or
REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(email, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  LIKE '%'+@BusquedaUsuario+'%') 
order by USUARIO.nombre asc
end 
  /*******************************************************************/
alter PROCEDURE [dbo].[pi_vendedor]
( 
 
  @codigo varchar(20), 
  @cargo varchar(50),
  @nombre varchar(50),
  @contacto varchar(50),
  @estado int,
  @usuario_creacion uniqueidentifier,  
  @maximo_descuento float,
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
					fecha_modificacion=GETDATE(),
					maximo_porcentaje_descuento_aprobacion=@maximo_descuento
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

  /**********************************************************************/

    ALTER procedure [dbo].[pu_vendedores] 
						(
						@id_vendedor int , 
						@usuario_modificacion uniqueidentifier,
						@codigo varchar(10),	
						@cargo varchar(50),
						@nombre varchar(50),
						@contacto varchar(50),
						@max_por_des_apro float,
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

