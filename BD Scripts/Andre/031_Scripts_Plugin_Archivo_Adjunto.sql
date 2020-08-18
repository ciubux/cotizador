---------------------- Lineas para agregar en Vista Editar ------------------
@Html.Action("cargarArchivo", "ArchivoAdjunto", new { origen = "PRODUCTO", idRegistro = producto.idProducto })
---------------------- Lineas para agregar en funciones de create() ------------------
			HttpSessionStateBase sessionParams = this.Session;
            var arcBL = new ArchivoAdjuntoBL();
            arcBL.asociarAchivoRegistro(sessionParams, producto.idProducto, ArchivoAdjunto.ArchivoAdjuntoOrigen.PRODUCTO);

---------------------- Lineas para agregar en la funcion de cancelar()  ------------------
			HttpSessionStateBase sessionParams = this.Session;
            var arcBL = new ArchivoAdjuntoBL();
            arcBL.asociarAchivoRegistro(sessionParams, producto.idProducto, ArchivoAdjunto.ArchivoAdjuntoOrigen.PRODUCTO);
			

--------------------------------------------- ALTER ps_adjuntos_id_registro - se omite la busqueda de la columna adjunto  ------------------------

ALTER procedure [dbo].[ps_adjuntos_id_registro] 
(@id_registro uniqueidentifier)
as 
begin
if(@id_registro != '00000000-0000-0000-0000-000000000000')
begin
	SELECT  arch.id_archivo_adjunto,  nombre,arch.id_registro,id_cliente
	FROM ARCHIVO_ADJUNTO arch	
	WHERE arch.id_registro = @id_registro
	AND arch.estado = 1	
	end
end

--------------------------------------------- CREATE pu_asociar_archivo_adjunto - Inserta el id_registro a los archivos adjuntos ------------------------

CREATE  procedure [dbo].[pu_asociar_archivo_adjunto]
(@id_registro uniqueidentifier,
@archivos UniqueIdentifierList readonly
)
as  begin 

DECLARE @idArchivoAdjunto uniqueidentifier 
DECLARE @archivoAdjuntoCursor CURSOR

SET @archivoAdjuntoCursor = CURSOR FOR
SELECT ID
FROM @archivos

OPEN @archivoAdjuntoCursor
FETCH NEXT
FROM @archivoAdjuntoCursor INTO @idArchivoAdjunto
WHILE @@FETCH_STATUS = 0
BEGIN

	update ARCHIVO_ADJUNTO 
		set id_registro=@id_registro 
		where id_archivo_adjunto=@idArchivoAdjunto	
    FETCH NEXT
	FROM @archivoAdjuntoCursor INTO @idArchivoAdjunto
end
end