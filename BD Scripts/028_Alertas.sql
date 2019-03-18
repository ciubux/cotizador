/* **** 1 **** */
ALTER PROCEDURE [dbo].[ps_cliente] 
@idCliente uniqueidentifier 
AS
BEGIN

SELECT cl.id_cliente, cl.codigo, cl.razon_social,
cl.nombre_comercial, cl.contacto1, cl.telefono_contacto1, cl.email_contacto1, cl.contacto2, cl.ruc,
cl.domicilio_legal, 
/*Si el cliente no tiene correo entonces se obtiene de algún pedido que tenga correo*/
CASE cl.correo_envio_factura WHEN '' THEN 
(SELECT TOP 1 correo_contacto_pedido FROM PEDIDO where id_cliente = cl.id_cliente
AND correo_contacto_pedido IS NOT NULL AND correo_contacto_pedido NOT IN ( '','.') )
ELSE cl.correo_envio_factura END AS correo_envio_factura, 

cl.razon_social_sunat, cl.nombre_comercial_sunat, 
cl.direccion_domicilio_legal_sunat, cl.estado_contribuyente_sunat, 
cl.condicion_contribuyente_sunat,
ub.codigo as codigo_ubigeo,
ub.provincia, ub.departamento, ub.distrito, cl.plazo_credito,
cl.usuario_creacion,

cl.forma_pago_factura, 
cl.sede_principal, 
cl.negociacion_multiregional, 
cl.id_ciudad,
ci.nombre as ciudad_nombre,
cl.tipo_documento,
/*PLAZO CREDITO*/
cl.plazo_credito_solicitado, --plazo credito aprobado
cl.tipo_pago_factura, --plazo credito aprobado
cl.sobre_plazo,
/*MONTO CREDITO*/
cl.credito_solicitado,
cl.credito_aprobado,
cl.sobre_giro, 
/*FLAG VENDEDORES*/
cl.vendedores_asignados,

/*Turnos entrega*/
cl.hora_inicio_primer_turno_entrega,
cl.hora_fin_primer_turno_entrega,
cl.hora_inicio_segundo_turno_entrega,
cl.hora_fin_segundo_turno_entrega,

--VENDEDORES,
verc.id_vendedor as responsable_comercial_id_vendedor,
verc.codigo as responsable_comercial_codigo,
verc.descripcion as responsable_comercial_descripcion,
verc.id_usuario as responsable_comercial_id_usuario,

vesc.id_vendedor as supervisor_comercial_id_vendedor,
vesc.codigo as supervisor_comercial_codigo,
vesc.descripcion as supervisor_comercial_descripcion,
vesc.id_usuario as supervisor_comercial_id_usuario,

veasc.id_vendedor as asistente_servicio_cliente_id_vendedor,
veasc.codigo as asistente_servicio_cliente_codigo,
veasc.descripcion as asistente_servicio_cliente_descripcion,
veasc.id_usuario as asistente_servicio_id_usuario,

cl.observaciones_credito, 
cl.observaciones, 
cl.bloqueado,

cl.pertenece_canal_multiregional,
cl.pertenece_canal_lima,
cl.pertenece_canal_provincia,
cl.pertenece_canal_pcp,
cl.pertenece_canal_ordon,
cl.es_sub_distribuidor,
cl.observacion_horario_entrega,
cl.habilitado_negociacion_grupal,

cl.id_subdistribuidor,
sub.nombre nombre_subdistribuidor, 
sub.codigo codigo_subdistribuidor, 
cl.id_origen,
ori.nombre nombre_origen, 
ori.codigo codigo_origen, 

clgr.id_grupo_cliente ,
gr.codigo as codigo_grupo_cliente,
gr.grupo as grupo_nombre

FROM CLIENTE AS cl 
INNER JOIN CIUDAD AS ci ON cl.id_ciudad = ci.id_ciudad
LEFT JOIN UBIGEO AS ub ON cl.ubigeo = ub.codigo
LEFT JOIN VENDEDOR AS verc ON cl.id_responsable_comercial = verc.id_vendedor
LEFT JOIN VENDEDOR AS vesc ON cl.id_supervisor_comercial = vesc.id_vendedor
LEFT JOIN VENDEDOR AS veasc ON cl.id_asistente_servicio_cliente = veasc.id_vendedor
LEFT JOIN CLIENTE_GRUPO_CLIENTE AS clgr ON clgr.id_cliente = cl.id_cliente
LEFT JOIN GRUPO_CLIENTE AS gr ON gr.id_grupo_cliente = clgr.id_grupo_cliente 
LEFT JOIN SUBDISTRIBUIDOR AS sub ON sub.id_subdistribuidor = cl.id_subdistribuidor 
LEFT JOIN ORIGEN AS ori ON ori.id_origen = cl.id_origen 
WHERE cl.estado > 0 AND cl.id_cliente = @idCliente 

SELECT  arch.id_archivo_adjunto,  nombre--, arch.adjunto,
FROM ARCHIVO_ADJUNTO arch
WHERE id_cliente = @idCliente
AND estado = 1
AND informacion_cliente = 'TRUE';

END







/* **** 2 **** */
ALTER PROCEDURE [dbo].[ps_getClientesGrupo] 
@idGrupoCliente int
AS
BEGIN

	SELECT 
	cl.id_cliente, 
	cl.codigo,
	ci.id_ciudad,
	ci.nombre as ciudad_nombre, 
	cl.habilitado_negociacion_grupal,
	
	CASE cl.tipo_documento WHEN 6 
		THEN ISNULL(cl.razon_social_sunat,cl.razon_social)
	ELSE '' END razon_social_sunat,


	CASE cl.tipo_documento WHEN 1 
		THEN cl.razon_social
	WHEN 4
		THEN cl.razon_social
	ELSE ISNULL(cl.nombre_comercial,'')  END nombre_comercial,
	

	--VENDEDORES,
	verc.id_vendedor as responsable_comercial_id_vendedor,
	verc.codigo as responsable_comercial_codigo,
	verc.descripcion as responsable_comercial_descripcion,
	verc.id_usuario as responsable_comercial_id_usuario,

	vesc.id_vendedor as supervisor_comercial_id_vendedor,
	vesc.codigo as supervisor_comercial_codigo,
	vesc.descripcion as supervisor_comercial_descripcion,
	vesc.id_usuario as supervisor_comercial_id_usuario,

	veasc.id_vendedor as asistente_servicio_cliente_id_vendedor,
	veasc.codigo as asistente_servicio_cliente_codigo,
	veasc.descripcion as asistente_servicio_cliente_descripcion,
	veasc.id_usuario as asistente_servicio_id_usuario,


	cl.tipo_documento, 
	cl.ruc
	
	FROM CLIENTE AS cl
	INNER JOIN CIUDAD AS ci ON cl.id_ciudad = ci.id_ciudad 
	INNER JOIN GRUPO_CLIENTE AS gc ON gc.id_grupo_cliente = cl.id_grupo_cliente
	LEFT JOIN VENDEDOR AS verc ON cl.id_responsable_comercial = verc.id_vendedor
	LEFT JOIN VENDEDOR AS vesc ON cl.id_supervisor_comercial = vesc.id_vendedor
	LEFT JOIN VENDEDOR AS veasc ON cl.id_asistente_servicio_cliente = veasc.id_vendedor

	WHERE 
	 cl.estado > 0
	AND @idGrupoCliente = gc.id_grupo_cliente
END





/* **** 3 **** */

CREATE TABLE [dbo].[ALERTA_VALIDACION](
	[id_alerta_validacion] [uniqueidentifier] NOT NULL,
	[nombre_tabla] [varchar](100) NULL,
	[id_registro] [varchar](50) NULL,
	[data_validacion] [text] NULL,
	[usuario_creacion] [uniqueidentifier] NULL,
	[fecha_creacion] [datetime] NULL,
	[usuario_validacion] [uniqueidentifier] NULL,
	[fecha_validacion] [datetime] NULL,
	[estado] [smallint] NULL,
	[tipo] [varchar](50) NULL,
 CONSTRAINT [PK_ALERTA_VALIDACION] PRIMARY KEY CLUSTERED 
(
	[id_alerta_validacion] ASC
)
)
GO





/* **** 4 **** */
CREATE PROCEDURE pi_alertaValidacion 
@idUsuario uniqueidentifier,
@nombreTabla  varchar(100),
@tipo varchar(50),
@idRegistro  varchar(50),
@dataValidacion text,

@newId uniqueidentifier OUTPUT
AS
BEGIN TRAN

SET NOCOUNT ON
SET @newId = NEWID();

INSERT INTO ALERTA_VALIDACION
           (id_alerta_validacion
		   ,nombre_tabla
           ,id_registro
		   ,tipo
		   ,data_validacion
		   ,usuario_creacion
		   ,fecha_creacion
		   ,estado
		   )
     VALUES
           (
		    @newId,
		    @nombreTabla,
		    @idRegistro,
			@tipo,
			@dataValidacion,
            @idUsuario,
			GETDATE(),
			1
			);



COMMIT



/* **** 5 **** */
CREATE TYPE VarcharCList AS TABLE(
[ID] varchar(100) NULL
)
GO




/* **** 6 **** */
CREATE PROCEDURE ps_alertasPendientesTipo

@tipoList AS dbo.VarcharCList READONLY

AS
BEGIN
	SELECT
	av.id_alerta_validacion,
	av.nombre_tabla,
	av.id_registro,
	av.tipo,
	av.data_validacion,
	av.fecha_creacion,

	u.id_usuario,
	u.nombre nombre_usuario

	FROM ALERTA_VALIDACION av  
	inner join USUARIO u on u.id_usuario = av.usuario_creacion
	where av.estado = 1 and av.tipo in (SELECT * FROM @tipoList);
END



/* **** 7 **** */
CREATE PROCEDURE pu_validaAlertaValidacion 
@idAlertaValidacion uniqueidentifier,
@idUsuario uniqueidentifier

AS
BEGIN

UPDATE ALERTA_VALIDACION  
	SET estado = 0 
		,usuario_validacion = @idUsuario
		,fecha_validacion = GETDATE()
	WHERE id_alerta_validacion = @idAlertaValidacion;

END


