/* **** 1 **** */
CREATE TYPE IntegerList AS TABLE(
	[ID] int NULL
);

/* **** 2 **** */
CREATE PROCEDURE pi_rol 
@idUsuario uniqueidentifier,
@codigo  varchar(100),
@nombre  varchar(100),
@estado int,
@permisos IntegerList readonly,
@newId int OUTPUT 

AS
BEGIN

DECLARE @idPermiso int 
DECLARE @permisosCursor CURSOR

INSERT INTO ROL
           ([codigo]
           ,[nombre]
           ,[estado]
           ,[usuario_creacion]
           ,[fecha_creacion]
           ,[usuario_modificacion]
           ,[fecha_modificacion])
     VALUES
           (@codigo
           ,@nombre
           ,@estado
           ,@idUsuario
           ,GETDATE()
           ,@idUsuario
           ,GETDATE());

SET NOCOUNT ON
SET @newId = SCOPE_IDENTITY();

SET @permisosCursor = CURSOR FOR
SELECT ID
FROM @permisos

OPEN @permisosCursor
FETCH NEXT
FROM @permisosCursor INTO @idPermiso
WHILE @@FETCH_STATUS = 0
BEGIN
	INSERT INTO ROL_PERMISO
			   ([id_rol]
			   ,[id_permiso]
			   ,[usuario_modificacion]
			   ,[fecha_modificacion])
		 VALUES
			   (@newId
			   ,@idPermiso
			   ,@idUsuario
               ,GETDATE());



    FETCH NEXT
	FROM @permisosCursor INTO @idPermiso
END

END



/* **** 3 **** */
CREATE PROCEDURE ps_roles
@estado int
AS
BEGIN

	SELECT 
	id_rol
    ,codigo
    ,nombre
    ,estado
	FROM ROL r
	where r.estado = @estado
	ORDER BY nombre asc;

END




/* **** 4 **** */
CREATE PROCEDURE ps_rol
@idRol int
AS
BEGIN
	SELECT 
	 id_rol
      ,codigo
      ,nombre
      ,estado
	   FROM ROL r
	where r.id_rol = @idRol;


	SELECT 
	pe.id_permiso
    ,pe.codigo
    ,pe.descripcion_corta
    ,pe.descripcion_larga
	,pe.orden_permiso
	,pe.id_categoria_permiso
	,cp.descripcion descripcion_categoria
	FROM PERMISO pe
	INNER JOIN ROL_PERMISO rp 
	on rp.id_permiso = pe.id_permiso
	INNER JOIN CATEGORIA_PERMISO cp
	ON pe.id_categoria_permiso = cp.id_categoria_permiso
	WHERE pe.estado = 1
	ORDER BY pe.id_categoria_permiso, pe.orden_permiso;

END



/* **** 5 **** */
CREATE TABLE ROL_USUARIO(
	[id_rol] [int] NOT NULL,
	[id_usuario] UNIQUEIDENTIFIER NOT NULL,
	[usuario_modificacion] [uniqueidentifier] NULL,
	[fecha_modificacion] [datetime] NULL
) ON [PRIMARY]




/* **** 6 **** */
CREATE PROCEDURE ps_usuario_get
	@idUsuario uniqueidentifier
AS
BEGIN
	SELECT id_usuario, cargo, nombre , contacto, es_cliente, estado, email, id_ciudad
	FROM USUARIO 
	WHERE id_usuario = @idUsuario;
END




/* **** 7 **** */
CREATE PROCEDURE ps_rol_usuarios
	@idRol int
AS
BEGIN
	SELECT u.id_usuario, u.cargo, u.nombre , u.contacto, u.es_cliente, u.estado, u.email, u.id_ciudad
	FROM USUARIO u 
	INNER JOIN ROL_USUARIO ru ON ru.id_usuario = u.id_usuario 
	WHERE ru.id_rol = @idRol;
END




/* **** 8 **** */
CREATE PROCEDURE pi_rol_usuario
	@idRol int,
	@idUsuario uniqueidentifier,
	@idUsuarioModifica uniqueidentifier
AS
BEGIN
	INSERT INTO ROL_USUARIO
           ([id_rol]
           ,[id_usuario]
           ,[usuario_modificacion]
           ,[fecha_modificacion])
     VALUES
            (@idRol
           ,@idUsuario
           ,@idUsuarioModifica
           ,GETDATE());
END




/* **** 9 **** */
CREATE PROCEDURE pd_rol_usuario
	@idRol int,
	@idUsuario uniqueidentifier
AS
BEGIN
	DELETE FROM ROL_USUARIO
    WHERE id_rol = @idRol AND id_usuario = @idUsuario;
END





/* **** 10 **** */
CREATE PROCEDURE ps_usuarios_search
@textoBusqueda varchar(50)
AS
BEGIN

SELECT id_usuario, email, nombre
FROM USUARIO u
WHERE estado > 0
AND (REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(email, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  LIKE '%'+@textoBusqueda+'%' OR
nombre LIKE '%'+@textoBusqueda+'%')


END




