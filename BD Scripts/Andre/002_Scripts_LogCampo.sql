

/* **** Create PS_CATALOGO_CAMPO - Lista de CAMPO_CATALOGO  **** */
create PROCEDURE [dbo].[ps_catalogo_tabla]
AS BEGIN
SELECT ID_CATALOGO_TABLA,NOMBRE,ESTADO FROM CATALOGO_TABLA where  estado=1
END 


/* **** Create PS_DETALLE_CATALOGO_CAMP - Obtiene el detalle de un CATALOGO_CAMPO por ID_CATALOGO_CAMPO **** */

create procedure [dbo].[ps_detalle_catalogo_campo]
(@ID_CATALOGO_TABLA INT)
AS  BEGIN
SELECT id_catalogo_campo,CATALOGO_CAMPO.estado,puede_persistir,CATALOGO_CAMPO.codigo,CATALOGO_CAMPO.nombre,CATALOGO_CAMPO.id_catalogo_tabla,CATALOGO_TABLA.nombre AS tabla_referencia,orden,campos_referencia FROM CATALOGO_CAMPO  inner join CATALOGO_TABLA on CATALOGO_TABLA.id_catalogo_tabla=CATALOGO_CAMPO.id_catalogo_tabla WHERE CATALOGO_CAMPO.id_catalogo_tabla=@ID_CATALOGO_TABLA
END 


  /****** Create PU_CATALOGO_CAMPO - Actualiza un LOGCAMPO ******/
create PROCEDURE [dbo].[pu_catalogo_campo]
(@ID_CATALOGO_CAMPO INT,
@ESTADO SMALLINT,
@PUEDE_PERSISTIR SMALLINT)
AS BEGIN
if @PUEDE_PERSISTIR = 3
BEGIN
UPDATE CATALOGO_CAMPO 
SET estado=@ESTADO
WHERE id_catalogo_campo=@ID_CATALOGO_CAMPO
END
if @ESTADO = 3
begin
UPDATE CATALOGO_CAMPO 
SET puede_persistir=@PUEDE_PERSISTIR
WHERE id_catalogo_campo=@ID_CATALOGO_CAMPO
end
END
/**********Create ps_add_catalogo_campo - Obtiene las columnas que faltan añadir a la tabla CATALOGO_CAMPO************/

create  procedure [dbo].[ps_add_catalogo_campo] 
(@id_tabla int)
as
begin 
declare @execstr nvarchar(max)
declare @nombre_tabla varchar(50)
set @nombre_tabla=(select nombre from catalogo_tabla where id_catalogo_tabla=@id_tabla)

declare @prod varchar(max) 
set @prod = concat('SELECT tab.COLUMN_NAME FROM Information_Schema.Columns tab   LEFT JOIN catalogo_campo on  
catalogo_campo.nombre=tab.COLUMN_NAME WHERE tab.TABLE_NAME =''',@nombre_tabla,''' and tab.COLUMN_NAME not in (select nombre from CATALOGO_CAMPO where id_catalogo_tabla=''',@id_tabla,''') ORDER BY COLUMN_NAME ASC');
set @execstr = CONVERT(nvarchar(max),@prod)
exec(@execstr)
end

/****** Create pi_insert_add_log_campo - Inserta las columnas que no estan registradas en CATALOGO_CAMPO ******/

create procedure [dbo].[pi_insert_add_log_campo]
(@name_log_campo varchar(50),
@id_catalogo_tabla smallint,
@ESTADO smallint,
@PUEDE_PERSISTIR smallint)
as begin
	    if  @PUEDE_PERSISTIR=3
		begin
		insert into CATALOGO_CAMPO (id_catalogo_campo,id_catalogo_tabla,nombre,estado,puede_persistir) select MAX (id_catalogo_campo)+1,@id_catalogo_tabla,@name_log_campo,@ESTADO,0 from CATALOGO_CAMPO
		end
		if @ESTADO=3
		begin
		insert into CATALOGO_CAMPO (id_catalogo_campo,id_catalogo_tabla,nombre,estado,puede_persistir) select MAX (id_catalogo_campo)+1 ,@id_catalogo_tabla,@name_log_campo,0,@PUEDE_PERSISTIR from CATALOGO_CAMPO
		end
end

/*-----------------------Permisos para la tabla PERMISO Y USUARIO_PERMISO--------------------------------*/

insert into PERMISO (id_permiso,estado,fecha_creacion,fecha_modificacion,codigo,descripcion_corta,id_categoria_permiso,orden_permiso)
						values (90,1,dbo.getlocaldate(),dbo.getlocaldate(),'P823','modifica_logcambio',7,10)

insert into PERMISO (estado,fecha_creacion,fecha_modificacion,codigo,descripcion_corta,id_categoria_permiso,orden_permiso)
						values (91,1,dbo.getlocaldate(),dbo.getlocaldate(),'P824','visualiza_logcambio',7,10)


insert into USUARIO_PERMISO(id_permiso,id_usuario,estado,fecha_creacion,fecha_modificacion) 
							values (90,'7E875C20-24FD-4B5E-9BB5-A7F23718F1FA',1,dbo.getlocaldate(),dbo.getlocaldate())

insert into USUARIO_PERMISO(id_permiso,id_usuario,estado,fecha_creacion,fecha_modificacion) 
							values (91,'7E875C20-24FD-4B5E-9BB5-A7F23718F1FA',1,dbo.getlocaldate(),dbo.getlocaldate())

							
/*-----------------------Añadir dos columnas en la tabla CATALOGO_CAMPO --------------------------------*/


alter table CATALOGO_CAMPO 
add orden int ,
    campos_referencia varchar(50) ,	
	tabla_referencia varchar(50)

