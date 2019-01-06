
/* **** TABLAS LOG CAMBIOS **** */
CREATE TABLE CATALOGO_TABLA(
	id_catalogo_Tabla int IDENTITY(1,1) NOT NULL,
	codigo char(4) NULL,
	nombre varchar(250) NULL,
	estado smallint NULL
PRIMARY KEY CLUSTERED 
(
	id_catalogo_Tabla ASC
)
) ON [PRIMARY]
GO






CREATE TABLE CATALOGO_CAMPO(
	id_catalogo_campo int IDENTITY(1,1) NOT NULL,
	id_catalogo_tabla int not null,
	codigo char(8) NULL,
	nombre varchar(250) NULL,
	estado smallint NULL
PRIMARY KEY CLUSTERED 
(
	id_catalogo_campo ASC
)
) ON [PRIMARY]
GO






CREATE TABLE CAMBIO(
	id_cambio uniqueidentifier  NOT NULL,
	id_catalogo_tabla int null,
	id_catalogo_campo int not null,
	id_registro varchar(40),
	valor varchar(1000),
	estado smallint NULL,
	fecha_inicio_vigencia date NOT NULL,
	fecha_fin_vigencia date NOT NULL,
	usuario_modificacion uniqueidentifier NOT NULL,
	fecha_modificacion datetime NOT NULL,
	 CONSTRAINT [PK_CAMBIO] PRIMARY KEY CLUSTERED 
(
	id_cambio ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



INSERT INTO CATALOGO_TABLA(codigo, nombre, estado) VALUES('0001', 'CLIENTE', 1);
INSERT INTO CATALOGO_TABLA(codigo, nombre, estado) VALUES('0002', 'PRODUCTO', 1);





INSERT INTO CATALOGO_CAMPO(id_catalogo_tabla, codigo, nombre, estado) VALUES(1, '00010001', 'id_grupo', 1);
INSERT INTO CATALOGO_CAMPO(id_catalogo_tabla, codigo, nombre, estado) VALUES(1, '00010002', 'id_responsable_comercial', 1);
INSERT INTO CATALOGO_CAMPO(id_catalogo_tabla, codigo, nombre, estado) VALUES(1, '00010003', 'id_asistente_servicio_cliente', 1);
INSERT INTO CATALOGO_CAMPO(id_catalogo_tabla, codigo, nombre, estado) VALUES(1, '00010004', 'id_supervisor_comercial', 1);
INSERT INTO CATALOGO_CAMPO(id_catalogo_tabla, codigo, nombre, estado) VALUES(1, '00010005', 'plazo_credito', 1);
INSERT INTO CATALOGO_CAMPO(id_catalogo_tabla, codigo, nombre, estado) VALUES(1, '00010006', 'credito_aprobado', 1);
INSERT INTO CATALOGO_CAMPO(id_catalogo_tabla, codigo, nombre, estado) VALUES(1, '00010007', 'negociacion_multiregional', 1);
INSERT INTO CATALOGO_CAMPO(id_catalogo_tabla, codigo, nombre, estado) VALUES(1, '00010008', 'sede_principal', 1);




 
