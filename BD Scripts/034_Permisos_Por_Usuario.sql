/* **** 1 **** */
CREATE PROCEDURE [dbo].[ps_usuarios_mantenedor]
@estado int
AS
BEGIN
	SELECT id_usuario,
	nombre, email, estado FROM USUARIO 
	where estado = @estado
	ORDER BY nombre asc;
END



/* **** 2 **** */
CREATE PROCEDURE [dbo].[ps_usuario_mantenedor]
@idUsuario uniqueidentifier
AS
BEGIN
	SELECT id_usuario,
	nombre, email, estado FROM USUARIO 
	where id_usuario = @idUsuario;
	
	
	SELECT pe.id_permiso, pe.codigo,pe.descripcion_corta, pe.descripcion_larga,   
		   CASE WHEN NOT up.id_usuario IS NULL THEN 1 ELSE 0 END as es_usuario, 
		   CASE WHEN NOT rp.id_rol IS NULL THEN 1 ELSE 0 END as es_rol,
		   pe.orden_permiso, pe.id_categoria_permiso, cp.descripcion descripcion_categoria
	FROM PERMISO PE
	LEFT JOIN ROL_USUARIO ru ON ru.id_usuario = @idUsuario
	LEFT JOIN ROL_PERMISO rp ON rp.id_rol = ru.id_rol AND pe.id_permiso = rp.id_permiso 
	LEFT JOIN USUARIO_PERMISO up ON pe.id_permiso = up.id_permiso and up.id_usuario = @idUsuario AND up.estado = 1
	INNER JOIN CATEGORIA_PERMISO cp ON pe.id_categoria_permiso = cp.id_categoria_permiso
	where pe.estado = 1 AND (NOT up.id_usuario IS NULL OR NOT rp.id_rol IS NULL)
	ORDER BY pe.id_categoria_permiso, pe.orden_permiso;

END




/* **** 3 **** */
CREATE PROCEDURE [dbo].[pu_usuario_permisos]
@idUsuario uniqueidentifier, 
@idUsuarioModificacion uniqueidentifier,
@permisos IntegerList readonly

AS
BEGIN

DECLARE @idPermiso int 
DECLARE @permisosCursor CURSOR

DELETE FROM USUARIO_PERMISO
WHERE id_usuario = @idUsuario;

SET @permisosCursor = CURSOR FOR
SELECT ID
FROM @permisos

OPEN @permisosCursor
FETCH NEXT
FROM @permisosCursor INTO @idPermiso
WHILE @@FETCH_STATUS = 0
BEGIN
	INSERT INTO USUARIO_PERMISO
			   (id_usuario
			   ,[id_permiso]
			   ,estado 
			   ,[usuario_creacion]
			   ,[fecha_creacion]
			   ,[usuario_modificacion]
			   ,[fecha_modificacion])
		 VALUES
			   (@idUsuario
			   ,@idPermiso
			   ,1
			   ,@idUsuarioModificacion
               ,GETDATE()
			   ,@idUsuarioModificacion
               ,GETDATE());



    FETCH NEXT
	FROM @permisosCursor INTO @idPermiso
END

END