alter procedure pu_vendedores 
						(
						@id_vendedor int , 
						@usuario_modificacion uniqueidentifier,
						@codigo varchar(10),
						@email varchar(50),
						@password varchar(50),
						@cargo varchar(50),
						@nombre varchar(50),
						@contacto varchar(50),
						@max_por_des_apro varchar(50),
						@estado int,
						@id_ciudad 	uniqueidentifier													
						)
						as 
						begin 
								
						update us set 
											us.email=@email,
											us.password=PWDENCRYPT(@password),
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

							end 

							begin
																			 
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