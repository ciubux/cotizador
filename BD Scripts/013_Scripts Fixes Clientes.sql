
ALTER PROCEDURE ps_clientes 
@codigo varchar(4),
@idCiudad uniqueIdentifier,
@textoBusqueda varchar(50),
@idResponsableComercial int,
@idSupervisorComercial int,
@idAsistenteServicioCliente int,
@sinPlazoCreditoAprobado int, 
@bloqueado int,
@idGrupoCliente int
AS
BEGIN

IF @codigo IS NULL OR @codigo = ''
BEGIN
	SELECT 
	cl.id_cliente, 
	cl.codigo,
	cl.sede_principal, 
	cl.negociacion_multiregional, 
	cl.pertenece_canal_multiregional,
	cl.pertenece_canal_lima,
	cl.pertenece_canal_provincia,
	cl.pertenece_canal_pcp,
	cl.pertenece_canal_ordon,
	cl.es_sub_distribuidor,
	ci.id_ciudad,
	ci.nombre as ciudad_nombre, 
	
	CASE cl.tipo_documento WHEN 6 
		THEN ISNULL(cl.razon_social_sunat,cl.razon_social)
	ELSE '' END razon_social_sunat,

	ISNULL(cl.nombre_comercial,'') nombre_comercial,
	cl.tipo_documento, 
	cl.ruc,
		--VENDEDORES,
	verc.codigo as responsable_comercial_codigo,
	ISNULL(verc.descripcion,'') as responsable_comercial_descripcion,

	vesc.codigo as supervisor_comercial_codigo,
	ISNULL(vesc.descripcion,'') as supervisor_comercial_descripcion,

	veasc.codigo as asistente_servicio_cliente_codigo,
	ISNULL(veasc.descripcion,'') as asistente_servicio_cliente_descripcion,

	cl.tipo_pago_factura, --plazo credito aprobado
	cl.credito_aprobado,
	cl.bloqueado,
	cgc.id_grupo_cliente,
	ISNULL(gc.grupo,'-') grupo
	FROM CLIENTE AS cl
	INNER JOIN CIUDAD AS ci ON cl.id_ciudad = ci.id_ciudad 
	--LEFT JOIN UBIGEO AS ub ON cl.ubigeo = ub.codigo
	LEFT JOIN VENDEDOR AS verc ON cl.id_responsable_comercial = verc.id_vendedor
	LEFT JOIN VENDEDOR AS vesc ON cl.id_supervisor_comercial = vesc.id_vendedor
	LEFT JOIN VENDEDOR AS veasc ON cl.id_asistente_servicio_cliente = veasc.id_vendedor
	LEFT JOIN CLIENTE_GRUPO_CLIENTE  AS cgc ON cl.id_cliente = cgc.id_cliente
	LEFT JOIN GRUPO_CLIENTE AS gc ON gc.id_grupo_cliente = cgc.id_grupo_cliente
	WHERE 
	(
	(REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(cl.nombre_comercial, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  LIKE '%'+@textoBusqueda+'%' OR
	cl.ruc LIKE '%'+@textoBusqueda+'%' OR
	REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(cl.razon_social, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  LIKE '%'+@textoBusqueda+'%'
	OR REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(cl.razon_social_sunat, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  LIKE '%'+@textoBusqueda+'%'
	)
	OR @textoBusqueda IS NULL
	OR @textoBusqueda = ''
	)
	AND (cl.id_responsable_comercial = @idResponsableComercial OR @idResponsableComercial = 0)
	AND (cl.id_supervisor_comercial = @idSupervisorComercial OR @idSupervisorComercial = 0)
	AND (cl.id_asistente_servicio_cliente = @idAsistenteServicioCliente OR @idAsistenteServicioCliente = 0)
	AND (cl.bloqueado = 1 OR @bloqueado = 0 )
	AND cl.estado = 1
	AND (cl.id_ciudad = @idCiudad OR @idCiudad = '00000000-0000-0000-0000-000000000000')
	AND (@idGrupoCliente = 0 OR @idGrupoCliente = cgc.id_grupo_cliente)
	--FALTA sinPlazoCreditoAprobado

END
ELSE
BEGIN 

	SELECT 
	cl.id_cliente, 
	cl.codigo,
	ci.id_ciudad,
	cl.sede_principal, 
	cl.negociacion_multiregional, 
	cl.pertenece_canal_multiregional,
	cl.pertenece_canal_lima,
	cl.pertenece_canal_provincia,
	cl.pertenece_canal_pcp,
	cl.pertenece_canal_ordon,
	cl.es_sub_distribuidor,
	ci.nombre as ciudad_nombre, 
	
	CASE cl.tipo_documento WHEN 6 
		THEN cl.razon_social_sunat
		ELSE '' END razon_social_sunat,

	ISNULL(cl.nombre_comercial,'') nombre_comercial,
	cl.tipo_documento, 
	cl.ruc,
		--VENDEDORES,
	verc.codigo as responsable_comercial_codigo,
	ISNULL(verc.descripcion,'') as responsable_comercial_descripcion,

	vesc.codigo as supervisor_comercial_codigo,
	ISNULL(vesc.descripcion,'') as supervisor_comercial_descripcion,

	veasc.codigo as asistente_servicio_cliente_codigo,
	ISNULL(veasc.descripcion,'') as asistente_servicio_cliente_descripcion,

	cl.tipo_pago_factura, --plazo credito aprobado
	cl.credito_aprobado,
	cl.bloqueado,
	cgc.id_grupo_cliente,
	ISNULL(gc.grupo,'-') grupo
	FROM CLIENTE AS cl
	INNER JOIN CIUDAD AS ci ON cl.id_ciudad = ci.id_ciudad 
	--LEFT JOIN UBIGEO AS ub ON cl.ubigeo = ub.codigo
	LEFT JOIN VENDEDOR AS verc ON cl.id_responsable_comercial = verc.id_vendedor
	LEFT JOIN VENDEDOR AS vesc ON cl.id_supervisor_comercial = vesc.id_vendedor
	LEFT JOIN VENDEDOR AS veasc ON cl.id_asistente_servicio_cliente = veasc.id_vendedor
	LEFT JOIN CLIENTE_GRUPO_CLIENTE  AS cgc ON cl.id_cliente = cgc.id_cliente
	LEFT JOIN GRUPO_CLIENTE AS gc ON gc.id_grupo_cliente = cgc.id_grupo_cliente
	WHERE cl.codigo = @codigo AND cl.estado = 1;
END


END

