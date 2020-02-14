/* **** 1 **** */
CREATE TABLE [CLIENTE_HISTORIAL_REASIGNACION](
	[id_cliente_historial_reasignacion] [uniqueidentifier] NOT NULL,
	[id_cliente] [uniqueidentifier] NOT NULL,
	[fecha_inicio_vigencia] NOT [date] NULL,
	[campo] [varchar](100) NOT NULL,
	[valor] [varchar](250) NOT NULL,
	[observacion] [varchar](500) NULL,
	[estado] [smallint] NOT NULL,
	[usuario_modificacion] [uniqueidentifier] NULL,
	[fecha_modificacion] [datetime] NULL
 CONSTRAINT [PK_CLIENTE_HISTORIAL_REASIGNACIONES] PRIMARY KEY CLUSTERED 
(
	[id_cliente_historial_reasignacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



/* **** 2 **** */
CREATE PROCEDURE [dbo].[pi_cliente_historial_reasignacion] 
@idCliente uniqueidentifier,
@campo varchar(100),
@valor varchar(250),
@fechaInicioVigencia date,
@observacion varchar(500),
@idUsuario uniqueidentifier,
@newId uniqueidentifier OUTPUT 

AS
BEGIN
	SET NOCOUNT ON
	SET @newId = NEWID();
	
	INSERT INTO CLIENTE_HISTORIAL_REASIGNACION
           (id_cliente_historial_reasignacion
			,id_cliente 
			,fecha_inicio_vigencia
			,campo
			,valor
			,observacion
			,estado
			,usuario_modificacion
			,fecha_modificacion
		   )
     VALUES
           (@newId,
			@idCliente,
			@fechaInicioVigencia,
			@campo,
			@valor,
			@observacion,
			1,
			@idUsuario,
			dbo.getlocaldate()
			);
END



/* **** 3 **** */
CREATE PROCEDURE [dbo].[ps_cliente_historial_reasignacion_vendedor] 
@idCliente uniqueidentifier,
@campo varchar(100)
AS
BEGIN
	SELECT cr.valor, cr.observacion, cr.fecha_inicio_vigencia, cr.fecha_modificacion, v.codigo, v.descripcion, u.nombre nombre_usuario
	FROM CLIENTE_HISTORIAL_REASIGNACION cr
	INNER JOIN VENDEDOR v ON v.id_vendedor = cr.valor
	INNER JOIN USUARIO u ON u.id_usuario = cr.usuario_modificacion 
	WHERE cr.id_cliente = @idCliente and cr.campo = @campo 
	ORDER BY cr.fecha_inicio_vigencia desc, cr.fecha_modificacion desc 
END




/* **** 4 **** */
ALTER PROCEDURE [dbo].[ps_cliente] 
@idCliente uniqueidentifier 
AS
BEGIN

SELECT
cl.usuario_creacion,
 cl.id_cliente, cl.codigo, cl.razon_social,
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
cs.estado_liberacion_creditica,
cl.fecha_creacion,

cl.pertenece_canal_multiregional,
cl.pertenece_canal_lima,
cl.pertenece_canal_provincia,
cl.pertenece_canal_pcp,
cl.pertenece_canal_ordon,
cl.es_sub_distribuidor,
cl.observacion_horario_entrega,
cl.configuraciones,
cl.habilitado_negociacion_grupal,
ISNULL(cs.habilitado_modificar_direccion_entrega,1) habilitado_modificar_direccion_entrega, 
cl.id_subdistribuidor,
sub.nombre nombre_subdistribuidor, 
sub.codigo codigo_subdistribuidor, 
cl.id_rubro,
rub.nombre nombre_rubro, 
rub.codigo codigo_rubro, 
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
LEFT JOIN RUBRO AS rub ON rub.id_rubro = cl.id_rubro  
LEFT JOIN ORIGEN AS ori ON ori.id_origen = cl.id_origen 
LEFT JOIN CLIENTE_SUNAT AS cs ON cs.id_cliente_sunat = cl.id_cliente_sunat
WHERE cl.estado > 0 AND cl.id_cliente = @idCliente 


/*ARCHIVOS ADJUNTOS*/
SELECT  arch.id_archivo_adjunto,  nombre--, arch.adjunto,
,checksum
FROM ARCHIVO_ADJUNTO arch
WHERE id_cliente = @idCliente
AND estado = 1
AND informacion_cliente = 'TRUE';


END
GO










