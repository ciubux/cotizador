/******** ALTER PS_DETALLE_CATALOGO_CAMPO - Se agrega la exclusion de es funcional *********/
ALTER procedure [dbo].[PS_DETALLE_CATALOGO_CAMPO] 
(@ID_CATALOGO_TABLA INT)
AS  BEGIN
SELECT id_catalogo_campo,CATALOGO_CAMPO.estado,puede_persistir,CATALOGO_CAMPO.codigo,CATALOGO_CAMPO.nombre,CATALOGO_CAMPO.id_catalogo_tabla,CATALOGO_TABLA.nombre AS tabla_referencia,orden,campos_referencia,es_funcional FROM CATALOGO_CAMPO  
inner join CATALOGO_TABLA on CATALOGO_TABLA.id_catalogo_tabla=CATALOGO_CAMPO.id_catalogo_tabla 
WHERE CATALOGO_CAMPO.id_catalogo_tabla=@ID_CATALOGO_TABLA and (es_funcional !=-1 or es_funcional is null)
END 

