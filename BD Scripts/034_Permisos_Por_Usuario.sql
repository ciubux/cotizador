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