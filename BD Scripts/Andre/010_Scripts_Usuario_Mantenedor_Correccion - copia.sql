/********************** ALTER pi_usuario_mantenedor - Inserta un usuario  *********************************/
alter procedure pi_usuario_mantenedor
(
@id_usuario_modificacion uniqueidentifier,
@cargo varchar(50),
@contacto varchar(50),
@nombre varchar(50),
@estado int ,
@es_cliente smallint,
@email varchar(50),
@pass varchar(50),
@id_ciudad 	uniqueidentifier,
@max_por_des_apro float
)
as begin 
declare @newid uniqueidentifier
set @newid=NEWID()
insert into usuario
			(id_usuario,
			email,
			password,
			cargo,
			nombre,
			contacto,			
			estado,
			usuario_creacion,
			fecha_creacion,
			usuario_modificacion,
			fecha_modificacion,
			id_ciudad,
			es_cliente,
			maximo_porcentaje_descuento_aprobacion)
			values( @newid,
					@email,
					PWDENCRYPT(@pass),
					@cargo,
					@nombre,
					@contacto,					
					@estado,
					@id_usuario_modificacion,
					getdate(),
					@id_usuario_modificacion,
					getdate(),
					@id_ciudad,
					@es_cliente,
					@max_por_des_apro)
end 

/********************** ALTER pu_usuario_mantenedor - Actualiza un usuario  *********************************/

alter procedure pu_usuario_mantenedor
(
						@id_usuario uniqueidentifier , 
						@id_usuario_modificacion uniqueidentifier,						
						@email varchar(50),
						@pass varchar(50),						
						@cargo varchar(50),
						@nombre varchar(50),
						@contacto varchar(50),						
						@estado int,
						@id_ciudad 	uniqueidentifier,
						@es_cliente smallint,	
						@max_por_des_apro float												
						)
						as 
						begin 
							SET NOCOUNT ON	

							IF @pass ='' or @pass is null 
							begin
								update us set 
											us.email=@email,											
											us.cargo=@cargo,
											us.nombre=@nombre,
											us.contacto=@contacto,											
											us.estado=@estado,
											us.fecha_modificacion=getdate(),
											us.usuario_modificacion=@id_usuario_modificacion,		
											us.id_ciudad=@id_ciudad,
											us.es_cliente=@es_cliente,
											us.maximo_porcentaje_descuento_aprobacion=@max_por_des_apro
											
									from usuario as us
									where us.id_usuario=@id_usuario
							end
									else 
														
									update us set 
											us.email=@email,
											us.password=PWDENCRYPT(@pass),
											us.cargo=@cargo,
											us.nombre=@nombre,
											us.contacto=@contacto,											
											us.estado=@estado,
											us.fecha_modificacion=getdate(),
											us.usuario_modificacion=@id_usuario_modificacion,		
											us.id_ciudad=@id_ciudad,
											us.maximo_porcentaje_descuento_aprobacion=@max_por_des_apro
									from usuario as us									
										where us.id_usuario=@id_usuario

end

