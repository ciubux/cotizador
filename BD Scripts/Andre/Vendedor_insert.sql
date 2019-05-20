ALTER  PROCEDURE [dbo].[PI_VENDEDOR]
( 
  @email varchar(50),
  @codigo varchar(20),
  @pass varchar(50),
  @cargo varchar(50),
  @nombre varchar(50),
  @contacto varchar(50),
  @estado int,
  @usuario_creacion uniqueidentifier,  
  @maximo_descuento decimal,
  @id_ciudad uniqueidentifier,
  @newid  uniqueidentifier OUTPUT  )
as begin 
set @newid=newid()
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
			fecha_modificacion,id_ciudad)
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
					getdate(),@id_ciudad)
insert  into VENDEDOR(
						id_usuario,
						codigo,
						descripcion,
						estado,
						usuario_creacion,
						fecha_creacion,
						usuario_modificacion,
						fecha_modificacion) 
						values (
								@newid,								
								@codigo,
								@nombre,
								@estado,
								@usuario_creacion,
								getdate(),
								@usuario_creacion,
								getdate()
								)	
	  
  END