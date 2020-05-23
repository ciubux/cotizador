--------------------------------------------------CONFIGURACION PLUGIN ARCHIVOS ADJUNTOS------------------------------------------------------------------------

@Html.Action("cargarArchivo", "ArchivoAdjunto", new { origen = "PRODUCTO", idRegistro = producto.idProducto })
 @Scripts.RenderFormat("<script type=\"text/javascript\" src=\"{0}?nocache=" + TempData["ScriptVersion"] + "\"></script>", "~/bundles/archivoAdjunto")

-------------------------------- CREATE  ps_archivos_adjuntos - Obtiene la lista de archivos subidos a un determinado origen ----------------------------
CREATE procedure [dbo].[ps_archivos_adjuntos] 
(@nombre VARCHAR(1000),
@origen varchar(50))
as 
begin
select ARCHIVO_ADJUNTO.nombre as nombre_archivo ,USUARIO.nombre as nombre_usuario,ARCHIVO_ADJUNTO.fecha_creacion,id_archivo_adjunto from ARCHIVO_ADJUNTO inner join usuario on usuario.id_usuario=ARCHIVO_ADJUNTO.usuario_creacion
where ARCHIVO_ADJUNTO.nombre like @nombre or
origen like @origen
end

----------------------------------- CREATE  ps_adjuntos_id_registro - Busca los archivos relacionados con el idRegistro ----------------------------
CREATE procedure [dbo].[ps_adjuntos_id_registro] 
(@id_registro uniqueidentifier)
as 
begin
if(@id_registro != '00000000-0000-0000-0000-000000000000')
begin
	SELECT  arch.id_archivo_adjunto,  nombre, arch.adjunto,arch.id_registro,id_cliente
	FROM ARCHIVO_ADJUNTO arch	
	WHERE arch.id_registro = @id_registro
	AND arch.estado = 1	
	end
end

----------------------------------- CREATE  pi_archivo_adjunto -Insertar archivo del plugin ----------------------------
CREATE PROCEDURE [dbo].[pi_archivo_adjunto]
@id_archivo_adjunto uniqueidentifier,
@id_cliente uniqueidentifier,
@nombre varchar(100),
@adjunto varbinary(MAX),
@id_usuario uniqueidentifier,
@id_registro uniqueidentifier,
@origen varchar(20),
@estado int,
@newId uniqueidentifier OUTPUT 
AS
BEGIN

SET NOCOUNT ON
SET @newId = NEWID();

IF @id_archivo_adjunto = '00000000-0000-0000-0000-000000000000'
BEGIN
	INSERT INTO [ARCHIVO_ADJUNTO]
			   (id_archivo_adjunto
			   ,id_cliente
			   ,nombre
			   ,adjunto
			   ,estado
			   ,usuario_creacion
			   ,fecha_creacion
			   ,usuario_modificacion
			   ,fecha_modificacion,
			   id_registro
			   ,origen)
		 VALUES
			   (@newId,
				@id_cliente,
				@nombre,
				@adjunto,
				1,
				@id_usuario,
				[dbo].[getlocaldate](),
				@id_usuario,
				[dbo].[getlocaldate](),
				@id_registro,
				@origen
				);

	END
ELSE 
BEGIN 
UPDATE ARCHIVO_ADJUNTO 
SET estado = @estado
,usuario_modificacion=@id_usuario
,fecha_modificacion=dbo.getlocaldate()
WHERE id_archivo_adjunto = @id_archivo_adjunto
	
END

END