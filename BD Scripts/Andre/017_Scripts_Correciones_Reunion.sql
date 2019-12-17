/**************************ALTER ps_vendedores - Se agrega busqueda por ciudad**********************************************/
ALTER procedure [dbo].[ps_vendedores]
(@estado int,
@cod VARCHAR(5),
@descripcion varchar(50),
@email varchar(50),
@ciudad uniqueidentifier)
AS BEGIN 
IF @cod IS NULL  OR @cod ='' 
begin
SELECT Vendedor.id_vendedor,usuario.nombre as descripcion,VENDEDOR.codigo,cargo,email,vendedor.estado,VENDEDOR.usuario_creacion,VENDEDOR.fecha_creacion,VENDEDOR.usuario_modificacion,VENDEDOR.fecha_modificacion
FROM VENDEDOR 
inner join USUARIO on  VENDEDOR.id_usuario=USUARIO.id_usuario  
inner join CIUDAD on CIUDAD.id_ciudad=USUARIO.id_ciudad
where vendedor.estado=1  and USUARIO.nombre LIKE '%'+@descripcion+'%' and usuario.email LIKE '%'+@email+'%' 
and USUARIO.id_ciudad IN (case when @ciudad = '00000000-0000-0000-0000-000000000000' then CIUDAD.id_ciudad else @ciudad end)
ORDER BY USUARIO.nombre ASC
end
else 
begin
SELECT Vendedor.id_vendedor,descripcion,VENDEDOR.codigo,cargo,email,vendedor.estado,VENDEDOR.usuario_creacion,VENDEDOR.fecha_creacion,VENDEDOR.usuario_modificacion,VENDEDOR.fecha_modificacion,concat('MP',CIUDAD.codigo_sede) as nombre
FROM USUARIO inner join VENDEDOR 
on  VENDEDOR.id_usuario=USUARIO.id_usuario 
inner join CIUDAD on CIUDAD.id_ciudad=USUARIO.id_ciudad  
where codigo like '%'+@cod+'%' and vendedor.estado=@estado ORDER BY USUARIO.nombre ASC
END 
END

/*************************ALTER PU-VENDEDORES - la actualizacion de un vendedor no afecta a la tabala de usuario*****************************************/
  ALTER procedure [dbo].[pu_vendedores] 
						(
						@id_vendedor int , 
						@usuario_modificacion uniqueidentifier,
						@codigo varchar(10),						
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
											us.maximo_porcentaje_descuento_aprobacion=@max_por_des_apro,											
											us.fecha_modificacion=dbo.getlocaldate(),
											us.usuario_modificacion=@usuario_modificacion,		
											us.id_ciudad=@id_ciudad
									from usuario as us
									INNER JOIN vendedor ve ON (ve.id_usuario = us.id_usuario)
										where ve.id_vendedor=@id_vendedor
							update ve set 
											ve.codigo=@codigo,											
											ve.estado=@estado,			
											ve.usuario_modificacion=@usuario_modificacion,
											ve.fecha_modificacion=dbo.getlocaldate(),
											ve.es_supervisor_comercial=@es_supervisor_comercial,
											ve.es_responsable_comercial =@es_responsable_comercial ,
											
											ve.es_asistente_servicio_cliente=@es_asistente_servicio_cliente,
											ve.id_supervisor_comercial=(case when @id_supervisor_comercial = 0 then null else @id_supervisor_comercial end)

									from vendedor as ve
								INNER JOIN usuario us ON (ve.id_usuario = us.id_usuario)
										where ve.id_vendedor=@id_vendedor

									end 
							END

/************************ALTER pi_vendedor - los datos ingresados no afectan a la tabla de  usuario******************************************/

ALTER PROCEDURE [dbo].[pi_vendedor]
( 
 
  @codigo varchar(20), 
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
					id_ciudad=@id_ciudad,
					usuario_modificacion=@usuario_creacion,				
					fecha_modificacion=GETDATE(),
					maximo_porcentaje_descuento_aprobacion=@maximo_descuento
					where usuario.id_usuario=@id_usuario_vendedor
					
insert  into VENDEDOR(
						id_usuario,
						id_vendedor,
						codigo,												
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
								@estado,
								@es_supervisor_comercial,
								@es_responsable_comercial,
								@es_asistente_servicio_cliente,
								@usuario_creacion,
								dbo.getlocaldate(),
								@usuario_creacion,
								dbo.getlocaldate(),
								(case when @id_supervisor_comercial = 0 then null else @id_supervisor_comercial end)
								)  
  END
/************************  Cambio de  nombre de la columna de importancia a prioridad *****************************************/
EXEC sp_RENAME 'MENSAJE.importancia' , 'prioridad', 'COLUMN'
/********************* ALTER  pi_mensaje -Cambio de nombre de columna importancia a prioridad  ********************************/
ALTER   procedure [dbo].[pi_mensaje]
(@titulo varchar(60),
@mensaje text,
@prioridad varchar(20),
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

insert into mensaje (id_mensaje,id_usuario_creacion,fecha_creacion,mensaje,titulo,prioridad,estado,fecha_vencimiento,fecha_modificacion,usuario_modificacion,fecha_inicio) 
values (@id_mensaje,@id_usuario_creacion,GETDATE(),@mensaje,@titulo,@prioridad,1,@fecha_vencimiento,GETDATE(),@id_usuario_modificacion,@fecha_inicio)

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

 /**************************   ALTER  pu_mensaje - Cambio de nombre de columna importancia a prioridad  ****************************************/

ALTER   procedure [dbo].[pu_mensaje]
(@id_mensaje uniqueidentifier, 
@titulo varchar(50),
@mensaje text,
@prioridad varchar(10),
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
prioridad=@prioridad,
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

/*************************** ALTER ps_alerta_mensaje_usuario - cambio de importancia a prioridad ****************************************/

ALTER procedure [dbo].[ps_alerta_mensaje_usuario] 
(@id_usuario uniqueidentifier)
as begin 
select mensaje.id_mensaje,titulo,mensaje,prioridad,mensaje.fecha_creacion,USUARIO.nombre,mensaje.fecha_vencimiento,mensaje.es_respuesta,id_hilo_mensaje,fecha_inicio from mensaje
left join MENSAJE_USUARIO on mensaje.id_mensaje=MENSAJE_USUARIO.id_mensaje
left join mensaje_roles on   mensaje_roles.id_mensaje=mensaje.id_mensaje
left join  ROL_USUARIO on ROL_USUARIO.id_rol=mensaje_roles.id_rol
inner join USUARIO on mensaje.id_usuario_creacion=usuario.id_usuario
where Mensaje.id_mensaje not in (select id_mensaje from MENSAJE_LEIDO where id_usuario=@id_usuario)  
and (ROL_USUARIO.id_usuario=@id_usuario or MENSAJE_USUARIO.id_usuario=@id_usuario) and mensaje.estado=1 
and fecha_inicio <= convert(date,dbo.getlocaldate())  and (fecha_vencimiento >= convert(date,dbo.getlocaldate()) or fecha_vencimiento is null)
order by  mensaje.fecha_creacion desc
end

/************************* ALTER ps_detalle_mensaje - cambio de importancia a prioridad ******************************/
ALTER procedure [dbo].[ps_detalle_mensaje] 
(@id_mensaje uniqueidentifier)
as 
begin

select id_mensaje,fecha_vencimiento,titulo,prioridad,mensaje,fecha_inicio from mensaje where id_mensaje=@id_mensaje

SELECT id_rol from mensaje_roles where id_mensaje=@id_mensaje 

SELECT mensaje_usuario.id_usuario,nombre,email from mensaje_usuario left join usuario on usuario.id_usuario=mensaje_usuario.id_usuario where  mensaje_usuario.id_mensaje=@id_mensaje

end 
/**************************** ALTER pi_mensaje_visto_repuesta - cambio de importancia a prioridad ****************************************/

ALTER PROCEDURE [dbo].[pi_mensaje_visto_repuesta]
(@id_mensaje uniqueidentifier,
@id_usuario uniqueidentifier,
@respuesta text,
@titulo varchar(50),
@prioridad varchar(10))
as begin 

declare @newId varchar(50)
set @newId=NEWID()

declare @id_usuario_mensaje uniqueidentifier
set @id_usuario_mensaje=(select id_usuario_creacion  from mensaje where id_mensaje=@id_mensaje)
declare @idHiloMensaje uniqueidentifier
set @idHiloMensaje=(select id_hilo_mensaje  from mensaje where id_mensaje=@id_mensaje)

insert MENSAJE_LEIDO(id_mensaje,id_usuario,fecha_leido) values(@id_mensaje,@id_usuario,dbo.getlocaldate())

insert mensaje (id_mensaje,fecha_inicio,id_usuario_creacion,usuario_modificacion,fecha_creacion,fecha_modificacion,es_respuesta,id_hilo_mensaje,mensaje,estado,titulo,prioridad)values 
(@newId,dbo.getlocaldate(),@id_usuario,@id_usuario,dbo.getlocaldate(),dbo.getlocaldate(),1,case when @idHiloMensaje is null then @id_mensaje else @idHiloMensaje end,@respuesta,1,@titulo,@prioridad)

insert into MENSAJE_USUARIO (id_usuario,id_mensaje)values(@id_usuario_mensaje,@newId)

end

