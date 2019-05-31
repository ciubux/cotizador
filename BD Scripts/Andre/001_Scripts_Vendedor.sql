

/* **** Create ps_vendedores - Lista de vendedores  **** */
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




/* **** Create ps_detalle_vendedores - Obtiene el detalle de un vendedor por id_vendedor **** */

create procedure [dbo].[ps_detalle_vendedores]
(@idUsuarioVenta int)
AS BEGIN 
SELECT vendedor.id_vendedor,vendedor.codigo,USUARIO.id_ciudad,
vendedor.estado,nombre,email,usuario.password,cargo,contacto,maximo_porcentaje_descuento_aprobacion,
(select vendedor.usuario_creacion from usuario  inner join vendedor on vendedor.id_usuario=usuario.id_usuario where vendedor.id_vendedor=@idUsuarioVenta)as USUARIO_CREACION,vendedor.fecha_creacion,
(select vendedor.usuario_modificacion from usuario  inner join vendedor on vendedor.id_usuario=usuario.id_usuario where vendedor.id_vendedor=@idUsuarioVenta) as USUARIO_MODIFICACION,vendedor.fecha_modificacion,
(select nombre from ciudad where id_ciudad=(select id_ciudad from USUARIO inner join vendedor on usuario.id_usuario=vendedor.id_usuario where id_vendedor=@idUsuarioVenta))AS CIUDAD FROM USUARIO  inner join vendedor on  vendedor.id_usuario=USUARIO.id_usuario WHERE vendedor.id_vendedor=@idUsuarioVenta
END


/* **** Create pi_vendedor - Agregar un vendedor **** */

create PROCEDURE [dbo].[pi_vendedor]
( 
  @email varchar(50),
  @codigo varchar(20),
  @pass varchar(50),
  @cargo varchar(50),
  @nombre varchar(50),
  @contacto varchar(50),
  @estado int,
  @usuario_creacion uniqueidentifier,  
  @maximo_descuento decimal(5,2),
  @id_ciudad uniqueidentifier,
  @newid uniqueidentifier
   )
as begin 

set @newid=NEWID()
insert into usuario
			(id_usuario,
			email,
			password,
			cargo,
			nombre,
			contacto,
			maximo_porcentaje_descuento_aprobacion,
			estado,
			usuario_creacion,
			fecha_creacion,
			usuario_modificacion,
			fecha_modificacion,
			id_ciudad)
			values( @newid,
					@email,
					PWDENCRYPT(@pass),
					@cargo,
					@nombre,
					@contacto,
					@maximo_descuento,
					@estado,
					@usuario_creacion,
					getdate(),
					@usuario_creacion,
					getdate(),
					@id_ciudad)
insert  into VENDEDOR(
						id_usuario,
						codigo,
						descripcion,
						estado,
						usuario_creacion,
						fecha_creacion,
						usuario_modificacion,
						fecha_modificacion) 
						values (@newid,								
								@codigo,
								@nombre,
								@estado,
								@usuario_creacion,
								getdate(),
								@usuario_creacion,
								getdate()
								)  
  END


  /* **** Create pu_vendedores - Actualiza un vendedor **** */


  create procedure [dbo].[pu_vendedores] 
						(
						@id_vendedor int , 
						@usuario_modificacion uniqueidentifier,
						@codigo varchar(10),
						@email varchar(50),
						@Newpassword varchar(50),						
						@cargo varchar(50),
						@nombre varchar(50),
						@contacto varchar(50),
						@max_por_des_apro decimal(5,2),
						@estado int,
						@id_ciudad 	uniqueidentifier													
						)
						as 
						begin 
							SET NOCOUNT ON	

							IF @Newpassword IS NULL
							begin
								update us set 
											us.email=@email,											
											us.cargo=@cargo,
											us.nombre=@nombre,
											us.contacto=@contacto,
											us.maximo_porcentaje_descuento_aprobacion=@max_por_des_apro,
											us.estado=@estado,
											us.fecha_modificacion=getdate(),
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
											ve.fecha_modificacion=getdate()	
									from vendedor as ve
								INNER JOIN usuario us ON (ve.id_usuario = us.id_usuario)
										where ve.id_vendedor=@id_vendedor
									
								end 
						     else 
								begin						
									update us set 
											us.email=@email,
											us.password=PWDENCRYPT(@Newpassword),
											us.cargo=@cargo,
											us.nombre=@nombre,
											us.contacto=@contacto,
											us.maximo_porcentaje_descuento_aprobacion=@max_por_des_apro,
											us.estado=@estado,
											us.fecha_modificacion=getdate(),
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
											ve.fecha_modificacion=getdate()	
									from vendedor as ve
								INNER JOIN usuario us ON (ve.id_usuario = us.id_usuario)
										where ve.id_vendedor=@id_vendedor

									end 
							END


  /* **** Permisos para la tabla PERMISO Y USUARIO_PERMISO **** */

insert into PERMISO (estado,fecha_creacion,fecha_modificacion,codigo,descripcion_corta,id_categoria_permiso,orden_permiso)
						values (1,GETDATE(),GETDATE(),'P073','visualiza_vendedores',7,6)

insert into PERMISO (estado,fecha_creacion,fecha_modificacion,codigo,descripcion_corta,id_categoria_permiso,orden_permiso)
						values (1,GETDATE(),GETDATE(),'P072','modifica_vendedores',7,6)


insert into USUARIO_PERMISO(id_permiso,id_usuario,estado,fecha_creacion,fecha_modificacion) 
							values (78,'7E875C20-24FD-4B5E-9BB5-A7F23718F1FA',1,getdate(),getdate())

insert into USUARIO_PERMISO(id_permiso,id_usuario,estado,fecha_creacion,fecha_modificacion) 
							values (79,'7E875C20-24FD-4B5E-9BB5-A7F23718F1FA',1,getdate(),getdate())


