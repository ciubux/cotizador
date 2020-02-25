
/******************* ALTER Table ps_lista_parametros -  Se agrega busqueda por codigo y descripcion ******************/

ALTER procedure [dbo].[ps_lista_parametros]
(@codigo varchar(50))
as begin 
if(@codigo is null)
begin 
select * from PARAMETRO where (editable=1 or editable is null) order by codigo asc
end
else 
select * from PARAMETRO where (editable=1 or editable is null) and (codigo like '%'+@codigo+'%' or descripcion like '%'+@codigo+'%')  order by codigo asc
end 

/*********************** INSERT - Se agrega el permiso de ENVIA_MENSAJE*******************************************************/

insert into permiso (id_permiso,estado,fecha_creacion,fecha_modificacion,codigo,descripcion_corta) values (100,1,dbo.getlocaldate(),dbo.getlocaldate(),'P832','ENVIA_MENSAJE')

/********************** ALTER ps_lista_mensajes - Se agrega la busqueda en la bandeja de mensaje recibidos****************************************/

ALTER procedure [dbo].[ps_lista_mensajes]
(@estado smallint,
@fecha_creacion_desde datetime,
@fecha_creacion_hasta datetime,
@fecha_vencimiento_desde datetime,
@fecha_vencimiento_hasta datetime,
@id_usuario_creacion uniqueidentifier,
@fecha_entrada_desde datetime,
@fecha_entrada_hasta datetime,
@bandeja smallint)
as begin  
if @bandeja=1
begin 
 
if(@fecha_vencimiento_desde is null and @fecha_vencimiento_hasta is null and @fecha_creacion_desde is null and @fecha_creacion_hasta is null )
begin
select id_mensaje,mensaje.fecha_creacion,titulo,usuario.nombre,fecha_vencimiento,mensaje.fecha_inicio,mensaje.mensaje  from mensaje inner join usuario on usuario.id_usuario=mensaje.id_usuario_creacion 
where mensaje.estado=@estado and (mensaje.es_respuesta !=1 or mensaje.es_respuesta is null) and id_usuario_creacion=@id_usuario_creacion
order by fecha_creacion desc
end
else
begin
select id_mensaje,mensaje.fecha_creacion,titulo,usuario.nombre,fecha_vencimiento,mensaje.fecha_inicio,mensaje.mensaje from mensaje inner join usuario on usuario.id_usuario=mensaje.id_usuario_creacion 
where mensaje.estado=@estado and (mensaje.es_respuesta !=1 or mensaje.es_respuesta is null) and id_usuario_creacion=@id_usuario_creacion 
and ( convert(date,mensaje.fecha_creacion) >= @fecha_creacion_desde AND	convert(date,mensaje.fecha_creacion) <=  @fecha_creacion_hasta )
or (mensaje.fecha_vencimiento >= @fecha_vencimiento_desde AND	mensaje.fecha_vencimiento <=  @fecha_vencimiento_hasta)
order by fecha_creacion desc
end
end
else 

select mensaje.id_mensaje,mensaje.fecha_creacion,titulo,usuario.nombre,fecha_vencimiento,mensaje.fecha_inicio,mensaje.mensaje  from mensaje
left join MENSAJE_USUARIO on mensaje.id_mensaje=MENSAJE_USUARIO.id_mensaje
left join mensaje_roles on   mensaje_roles.id_mensaje=mensaje.id_mensaje
left join  ROL_USUARIO on ROL_USUARIO.id_rol=mensaje_roles.id_rol
inner join USUARIO on mensaje.id_usuario_creacion=usuario.id_usuario

where (ROL_USUARIO.id_usuario=@id_usuario_creacion or MENSAJE_USUARIO.id_usuario=@id_usuario_creacion) and mensaje.estado=1
and (es_respuesta != 1 or es_respuesta is null)
and fecha_inicio >= convert(date,@fecha_entrada_desde)  and fecha_inicio <= convert(date,@fecha_entrada_hasta)

order by  mensaje.fecha_creacion desc

end 

