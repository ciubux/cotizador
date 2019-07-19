/* **** Create ps_validacionNotificacionDocumentoVenta - Lista de Documentos Venta que faltan Notificar  **** */

ALTER  procedure [dbo].[ps_validacionNotificacionDocumentoVenta] 
as begin 
select NotificacionAnulacionDocumentoVenta.id_cpe_cabecera_be,NotificacionAnulacionDocumentoVenta.estado_notificacion,CPE_CABECERA_BE.SERIE,CPE_CABECERA_BE.CORRELATIVO,CPE_CABECERA_BE.CORREO_ENVIO,CPE_CABECERA_BE.fecha_creacion,CPE_CABECERA_BE.MNT_TOT_NAC,CLIENTE.razon_social,CLIENTE.ruc,USUARIO.nombre,CLIENTE.contacto1
from NotificacionAnulacionDocumentoVenta 
inner join CPE_CABECERA_BE on NotificacionAnulacionDocumentoVenta.id_cpe_cabecera_be=CPE_CABECERA_BE.id_cpe_cabecera_be
inner join venta on venta.id_venta=CPE_CABECERA_BE.id_venta
inner join cliente on cliente.id_cliente=venta.id_cliente 
inner join usuario on VENTA.usuario_creacion=usuario.id_usuario	 where venta.estado=1 order by 	CPE_CABECERA_BE.fecha_creacion asc
end 

/*********** Create pu_proceso_cambiar - Actualiza un LOGCAMPO ********/

ALTER procedure [dbo].[pu_proceso_cambiar]
(@estado int,
 @id_documento_venta uniqueidentifier)
as begin 
update NotificacionAnulacionDocumentoVenta set estado_notificacion=@estado WHERE id_cpe_cabecera_be=@id_documento_venta
end 

/*-----------------------CREACION DE TABLA NotificacionAnulacionDocumentoVenta--------------------------------*/

create table NotificacionAnulacionDocumentoVenta
(
	   id_cpe_cabecera_be uniqueidentifier ,
	   estado int ,
	   estado_notificacion int ,
	   usuario_solicitud uniqueidentifier,
	   usuario_aprobacion uniqueidentifier,
	   fecha_notificacion smalldatetime,
	   fecha_creacion smalldatetime,
	   fecha_modificacion smalldatetime,
	   usuario_creacion uniqueidentifier ,
	   usuario_modificacion uniqueidentifier 
)

/*-----------------------Permisos para la tabla PERMISO Y USUARIO_PERMISO--------------------------------*/


insert into PERMISO (id_permiso,estado,fecha_creacion,fecha_modificacion,codigo,descripcion_corta,id_categoria_permiso,orden_permiso)
						values (83,1,dbo.getlocaldate(),dbo.getlocaldate(),'P420','visualiza_documentoventanotificacion',5,4)
			

insert into USUARIO_PERMISO(id_permiso,id_usuario,estado,fecha_creacion,fecha_modificacion) 
							values (83,'BB85E214-3C69-44A4-B504-4CD223EC389C',1,dbo.getlocaldate(),dbo.getlocaldate())