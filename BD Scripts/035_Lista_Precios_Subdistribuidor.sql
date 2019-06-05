/* **** 1 **** */
ALTER PROCEDURE [dbo].[ps_usuarios_mantenedor]
@textoBusqueda varchar(100),
@estado int
AS
BEGIN
	SELECT id_usuario,
	nombre, email, estado FROM USUARIO 
	where estado = @estado
	AND (REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(email, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  LIKE '%'+@textoBusqueda+'%' OR
         REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(nombre, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ') LIKE '%'+@textoBusqueda+'%')
	ORDER BY nombre asc;
END


