/* **** 1 **** */
CREATE SCHEMA CLIENTE;


/* **** 2 **** */
CREATE TABLE CLIENTE.USUARIO(
	[id_cliente_usuario] [uniqueidentifier] NOT NULL,
	[id_ciudad] [uniqueidentifier] NOT NULL,
	[id_cliente_sunat] int NOT NULL,
	[login] [varchar](50) NULL,
	[password] [varbinary](max) NOT NULL,
	[cargo] [varchar](100) NULL,
	[nombre] [varchar](200) NULL,
	[telefono] [varchar](200) NULL,
	[estado] [int] NOT NULL,
	[usuario_creacion] [uniqueidentifier] NULL,
	[fecha_creacion] [smalldatetime] NOT NULL,
	[usuario_modificacion] [uniqueidentifier] NULL,
	[fecha_modificacion] [smalldatetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


/* **** 3 **** */
CREATE TABLE CLIENTE.[PERMISO](
	[id_permiso] [int] IDENTITY(1,1) PRIMARY KEY,
	[codigo] [char](4) NOT NULL,
	[descripcion_corta] [varchar](50) NULL,
	[descripcion_larga] [varchar](2000) NULL,
	[orden_permiso] [int] NULL,
	[estado] [bit] NULL,
	[usuario_creacion] [uniqueidentifier] NULL,
	[fecha_creacion] [datetime] NULL,
	[usuario_modificacion] [uniqueidentifier] NULL,
	[fecha_modificacion] [datetime] NULL,
)


/* **** 4 **** */
CREATE TABLE CLIENTE.[USUARIO_PERMISO](
	[id_permiso] [int] NOT NULL,
	[id_usuario] [uniqueidentifier] NULL,
	[estado] [bit] NULL,
	[usuario_modificacion] [uniqueidentifier] NULL,
	[fecha_modificacion] [datetime] NULL
) ON [PRIMARY]
GO



/* **** 5 **** */

ALTER PROCEDURE [CLIENTE].[ps_usuario] 
@ruc varchar(50),
@login varchar(50),
@password varchar(50)
AS
BEGIN

DECLARE @id_usuario uniqueidentifier; 


SELECT @id_usuario = u.id_cliente_usuario FROM CLIENTE.USUARIO u
INNER JOIN CLIENTE_SUNAT cl ON cl.id_cliente_sunat = u.id_cliente_sunat
WHERE cl.ruc = @ruc
AND u.estado = 1 AND cl.estado = 1
AND u.login = @login 
AND PWDCOMPARE ( @password, u.password )  = 1;

--USUARIO
SELECT u.id_cliente_usuario, u.cargo, u.nombre , u.telefono, ci.id_ciudad, ci.nombre nombre_ciudad, ci.codigo_sede
FROM CLIENTE.USUARIO u
INNER JOIN CIUDAD ci ON ci.id_ciudad = u.id_ciudad AND ci.estado = 1
WHERE u.estado = 1
AND id_cliente_usuario = @id_usuario; 


--CLIENTES
SELECT cl.id_cliente, cl.razon_social, 
cl.codigo, cl.ruc, cl.nombre_comercial, 
ci.id_ciudad, ci.nombre, ci.codigo_sede, ci.es_provincia
FROM CLIENTE cl
INNER JOIN CIUDAD ci ON  cl.id_ciudad = ci.id_ciudad
WHERE cl.ruc = @ruc  and ci.estado = 1 and cl.estado = 1 

--PERMISOS
SELECT pe.id_permiso, pe.codigo,pe.descripcion_corta, pe.descripcion_larga 
FROM CLIENTE.PERMISO PE
LEFT JOIN CLIENTE.USUARIO_PERMISO up ON pe.id_permiso = up.id_permiso and up.id_usuario = @id_usuario and up.estado = 1
WHERE pe.estado = 1


--PARAMETRO
SELECT codigo, valor
FROM CLIENTE.PARAMETRO
WHERE estado = 1

END








