

/* **** Create PS_CATALOGO_CAMPO - Lista de CAMPO_CATALOGO  **** */
CREATE PROCEDURE [dbo].[PS_CATALOGO_CAMPO]
AS BEGIN
SELECT ID_CATALOGO_CAMPO,NOMBRE,ESTADO,puede_persistir FROM CATALOGO_CAMPO
END 





/* **** Create PS_DETALLE_CATALOGO_CAMP - Obtiene el detalle de un CATALOGO_CAMPO por ID_CATALOGO_CAMPO **** */

CREATE procedure [dbo].[PS_DETALLE_CATALOGO_CAMPO]
(@ID_CATALOGO_CAMPO INT)
AS  BEGIN
SELECT id_catalogo_campo,CATALOGO_CAMPO.estado,puede_persistir,CATALOGO_CAMPO.codigo,CATALOGO_CAMPO.nombre,CATALOGO_CAMPO.id_catalogo_tabla,CATALOGO_TABLA.nombre AS tabla_referencia,orden,campos_referencia FROM CATALOGO_CAMPO  inner join CATALOGO_TABLA on CATALOGO_TABLA.id_catalogo_tabla=CATALOGO_CAMPO.id_catalogo_tabla WHERE id_catalogo_campo=@ID_CATALOGO_CAMPO
END 



  /* **** Create PU_CATALOGO_CAMPO - Actualiza un LOGCAMPO **** */
  CREATE PROCEDURE [dbo].[PU_CATALOGO_CAMPO]
(@ID_CATALOGO_CAMPO INT,
@ESTADO SMALLINT,
@PUEDE_PERSISTIR SMALLINT)
AS BEGIN
UPDATE CATALOGO_CAMPO 
SET estado=@ESTADO,puede_persistir=@PUEDE_PERSISTIR
WHERE id_catalogo_campo=@ID_CATALOGO_CAMPO
END

/*-----------------------Permisos para la tabla PERMISO Y USUARIO_PERMISO--------------------------------*/

insert into PERMISO (estado,fecha_creacion,fecha_modificacion,codigo,descripcion_corta,id_categoria_permiso,orden_permiso)
						values (1,dbo.getlocaldate(),dbo.getlocaldate(),'P823','modifica_logcambio',7,10)

insert into PERMISO (estado,fecha_creacion,fecha_modificacion,codigo,descripcion_corta,id_categoria_permiso,orden_permiso)
						values (1,dbo.getlocaldate(),dbo.getlocaldate(),'P824','visualiza_logcambio',7,10)


insert into USUARIO_PERMISO(id_permiso,id_usuario,estado,fecha_creacion,fecha_modificacion) 
							values (80,'7E875C20-24FD-4B5E-9BB5-A7F23718F1FA',1,dbo.getlocaldate(),dbo.getlocaldate())

insert into USUARIO_PERMISO(id_permiso,id_usuario,estado,fecha_creacion,fecha_modificacion) 
							values (81,'7E875C20-24FD-4B5E-9BB5-A7F23718F1FA',1,dbo.getlocaldate(),dbo.getlocaldate())


/*-----------------------Añadir dos columnas en la tabla CATALOGO_CAMPO --------------------------------*/

alter table CATALOGO_CAMPO 
add orden int ,
    campos_referencia varchar(50) ,	
	tabla_referencia varchar(50)				
