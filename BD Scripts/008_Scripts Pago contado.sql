ALTER TABLE COTIZACION 
ADD es_pago_contado bit default 'FALSE';


UPDATE COTIZACION
SET es_pago_contado = 'FALSE';




/* se agrego @esPagoContado */
ALTER PROCEDURE [dbo].[pi_cotizacion] 
@fecha datetime, 
@fechaLimiteValidezOferta datetime,
@fechaInicioVigenciaPrecios datetime,
@fechaFinVigenciaPrecios datetime,
@incluidoIgv smallint, 
@consideraCantidades smallint, 
@idCliente uniqueidentifier, 
@idGrupo uniqueidentifier, 
@idCiudad uniqueidentifier, 
@porcentajeFlete decimal(18,2), 
@igv decimal(18,2), 
@total decimal(18,2), 
@observaciones varchar(1000), 
@idUsuario uniqueidentifier,
@contacto varchar(100),
@mostrarCodigoProveedor smallint,
@mostrarValidezOfertaDias int,
@estado smallint,
@fechaEsModificada smallint,
@observacionSeguimientoCotizacion varchar(500),
@aplicaSedes bit,
@esPagoContado bit,
@newId uniqueidentifier OUTPUT, 
@codigo bigint OUTPUT 
AS
BEGIN

SET NOCOUNT ON

DECLARE @serie int;
DECLARE @aprueba_cotizaciones int;

SET @newId = NEWID();
SET @codigo = NEXT VALUE FOR dbo.SEQ_CODIGO_COTIZACION;
SET @serie = 1;


SELECT @aprueba_cotizaciones = aprueba_cotizaciones_lima FROM USUARIO WHERE id_usuario = @idUsuario;

--Si el usuario es probador la serie es 0
if @aprueba_cotizaciones = 1
BEGIN
	SET @serie = 0;
END

INSERT INTO [COTIZACION]
           ([id_cotizacion]
		   ,[codigo]
           ,[fecha]
		   ,[fecha_inicio_vigencia_precios]
		   ,[fecha_fin_vigencia_precios]
		   ,[fecha_limite_validez_oferta]
           ,[incluido_igv]
           ,[considera_cantidades]
           ,[id_cliente]
		   ,[id_grupo]
           ,[id_ciudad]
           ,[porcentaje_flete]
		   ,[igv]
		   ,[total]
		   ,[observaciones]
		   ,[contacto]
		   ,[estado]
		   ,[usuario_creacion]
		   ,[fecha_creacion]
		   ,[usuario_modificacion]
		   ,[fecha_modificacion]
		   ,[mostrar_codigo_proveedor]
		   ,[mostrar_validez_oferta_dias]
		   ,[serie]
		   ,[fecha_Es_modificada]
		   ,[aplica_sedes]
		   ,[es_pago_contado]
		   )
     VALUES
           (@newId,
		    @codigo,
		    @fecha,
			@fechaInicioVigenciaPrecios,
			@fechaFinVigenciaPrecios,
			@fechaLimiteValidezOferta,
            @incluidoIgv,
            @consideraCantidades, 
            @idCliente, 
			@idGrupo, 
            @idCiudad, 
            @porcentajeFlete,
			@igv,
			@total,
			@observaciones,
			@contacto,
			1,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE(),
			@mostrarCodigoProveedor,
			@mostrarValidezOfertaDias,
			@serie,
			@fechaEsModificada,
			@aplicaSedes,
			@esPagoContado
			);

INSERT INTO SEGUIMIENTO_COTIZACION 
(
		id_estado_seguimiento, 
		id_usuario ,
		id_cotizacion, 
		estado_cotizacion ,
		observacion ,
		estado ,
		usuario_creacion ,
		fecha_creacion ,
		usuario_modifiacion ,
		fecha_modificacion 
)
VALUES(
			NEWID(),
			@idUsuario,
			@newId,
			@estado,
			@observacionSeguimientoCotizacion,
			1,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE()
);


UPDATE CLIENTE set contacto1 = @contacto where id_cliente = @idCliente;


END






/* se agrego @esPagoContado */
ALTER PROCEDURE [dbo].[pu_cotizacion] 
@fecha datetime, 
@fechaLimiteValidezOferta datetime,
@fechaInicioVigenciaPrecios datetime,
@fechaFinVigenciaPrecios datetime,
@fechaModificacion datetime,
@incluidoIgv smallint, 
@consideraCantidades smallint, 
@idCliente uniqueidentifier, 
@idGrupo uniqueidentifier, 
@idCiudad uniqueidentifier, 
@porcentajeFlete decimal(18,2), 
@igv decimal(18,2), 
@total decimal(18,2), 
@observaciones varchar(1000), 
@idUsuario uniqueidentifier,
@contacto varchar(100),
@codigo bigint,
@mostrarCodigoProveedor smallint,
@mostrarValidezOfertaDias int,
@estado int,
@fechaEsModificada smallint,
@observacionSeguimientoCotizacion varchar(500),
@aplicaSedes bit,
@esPagoContado bit,
@fechaModificacionActual datetime OUTPUT
AS
BEGIN

DECLARE @id_cotizacion uniqueidentifier;


SELECT @id_cotizacion =id_cotizacion, @fechaModificacionActual = fecha_modificacion FROM COTIZACION WHERE codigo = @codigo AND estado = 1;

/*if(convert(varchar,@fechaModificacionActual, 120)  = convert(varchar,@fechaModificacion, 120)) 
Begin*/

	UPDATE [COTIZACION_DETALLE] SET ESTADO = 0 ,
	[usuario_modificacion] = @idUsuario, [fecha_modificacion] = GETDATE()
	WHERE id_cotizacion = @id_cotizacion;


	UPDATE [COTIZACION] SET
			   [fecha] =  @fecha
			   ,[fecha_Limite_Validez_Oferta] = @fechaLimiteValidezOferta
			   ,[fecha_Inicio_Vigencia_Precios] = @fechaInicioVigenciaPrecios
			   ,[fecha_Fin_Vigencia_Precios] = @fechaFinVigenciaPrecios
			   ,[incluido_igv] = @incluidoIgv
			   ,[considera_cantidades] = @consideraCantidades
			   ,[id_cliente] = @idCliente
			   ,[id_grupo] = @idGrupo
			   ,[id_ciudad] = @idCiudad
			   ,[porcentaje_flete] = @porcentajeFlete
			   ,[igv] = @igv
			   ,[total] = @total
			   ,[observaciones] = @observaciones
			   ,[contacto] = @contacto
			   ,[usuario_modificacion] = @idUsuario
			   ,[fecha_modificacion] = GETDATE()
			--   ,[estado_aprobacion] = @estadoAprobacion
			   ,[mostrar_codigo_proveedor] = @mostrarCodigoProveedor
			   ,[mostrar_Validez_Oferta_Dias] = @mostrarValidezOfertaDias
			   ,[fecha_es_modificada]  = @fechaEsModificada
			   ,[aplica_sedes]  = @aplicaSedes
			   ,[es_pago_contado]  = @esPagoContado
			     WHERE id_cotizacion = @id_cotizacion ;
		             
	UPDATE SEGUIMIENTO_COTIZACION set estado = 0 where id_cotizacion = @id_cotizacion;
				

	INSERT INTO SEGUIMIENTO_COTIZACION 
	(
			id_estado_seguimiento, 
			id_usuario ,
			id_cotizacion, 
			estado_cotizacion ,
			observacion ,
			estado ,
			usuario_creacion ,
			fecha_creacion ,
			usuario_modifiacion ,
			fecha_modificacion 
	)
	VALUES(
				NEWID(),
				@idUsuario,
				@id_cotizacion,
				@estado,
				@observacionSeguimientoCotizacion,
				1,
				@idUsuario,
				GETDATE(),
				@idUsuario,
				GETDATE()
	);
--end;

END





/* agrega columna es_pago_contado */
ALTER PROCEDURE [dbo].[ps_cotizacion] 
@codigo bigint
AS
BEGIN

SELECT 
--COTIZACION
co.id_cotizacion, co.fecha,  co.fecha_limite_validez_oferta ,  co.fecha_inicio_vigencia_precios, co.fecha_fin_vigencia_precios, 
co.incluido_igv, co.considera_cantidades,  co.mostrar_validez_oferta_dias, co.contacto,
co.porcentaje_flete, co.igv, co.total, co.observaciones, co.mostrar_codigo_proveedor, co.fecha_modificacion,
co.fecha_es_modificada, co.aplica_sedes, co.es_pago_contado,
---CLIENTE
cl.id_cliente, cl.codigo, cl.razon_social, cl.ruc, cl.sede_principal,
cl.plazo_credito_solicitado, cl.tipo_pago_factura,
--CIUDAD
ci.id_ciudad, ci.nombre as nombre_ciudad , ci.es_provincia,
--USUARIO
us.nombre  as nombre_usuario, us.cargo, us.contacto as contacto_usuario, us.email,
--GRUPO
gr.id_grupo, gr.codigo as codigo_grupo, gr.nombre as nombre_grupo, gr.contacto as contacto_grupo,
--SEGUIMIENTO
sc.estado_cotizacion as estado_seguimiento,
us2.nombre as usuario_seguimiento, sc.observacion as observacion_seguimiento,
us2.id_usuario as id_usuario_seguimiento,
--DETALLE
(SELECT max(porcentaje_descuento) from COTIZACION_DETALLE  WHERE estado = 1 AND id_cotizacion = co.id_cotizacion) as maximo_porcentaje_descuento



FROM COTIZACION as co
INNER JOIN CIUDAD AS ci ON co.id_ciudad = ci.id_ciudad
INNER JOIN USUARIO AS us on co.usuario_creacion = us.id_usuario
INNER JOIN SEGUIMIENTO_COTIZACION sc ON co.id_cotizacion = sc.id_cotizacion
INNER JOIN USUARIO AS us2 on sc.id_usuario = us2.id_usuario
LEFT JOIN CLIENTE AS cl ON co.id_cliente = cl.id_cliente
LEFT JOIN GRUPO AS gr ON co.id_grupo = gr.id_grupo  
where co.codigo = @codigo and co.estado = 1
AND sc.estado = 1;



select * from (
	SELECT cd.id_cotizacion_detalle, 
	cd.cantidad, 
	cd.precio_sin_igv, 
	cd.costo_sin_igv, 
	cd.equivalencia,
	cd.unidad, 
	cd.porcentaje_descuento,
	cd.precio_neto, 
	cd.es_precio_alternativo, 
	cd.observaciones,
	cd.fecha_modificacion,
	cd.flete,
	pr.id_producto, 
	pr.sku, 
	pr.descripcion, 
	pr.sku_proveedor, 
	pr.imagen, 
	pr.proveedor, 
	pr.costo as producto_costo, 
	pr.precio as producto_precio,
	pr.precio_provincia as producto_precio_provincia,

	--pc.unidad, 
	CASE pc.es_unidad_alternativa WHEN 1 THEN pc.precio_neto * pc.equivalencia 
	ELSE pc.precio_neto END as precio_neto_vigente, 



	pc.flete as flete_vigente, 

	CASE pc.es_unidad_alternativa WHEN 1 THEN pc.precio_unitario * pc.equivalencia 
	ELSE pc.precio_unitario END as precio_unitario_vigente, 


	pc.equivalencia as equivalencia_vigente,
	pc.id_precio_cliente_producto,
	pc.fecha_inicio_vigencia,
	pc.fecha_fin_vigencia,
	pc.id_cliente,
	
	ROW_NUMBER() OVER(PARTITION BY cd.id_producto,co.id_cliente ORDER BY 
	pc.fecha_inicio_vigencia DESC, pc.codigo DESC) AS RowNumber

	FROM COTIZACION_DETALLE as cd INNER JOIN 
	COTIZACION as co ON cd.id_cotizacion = co.id_cotizacion 
	INNER JOIN PRODUCTO pr ON cd.id_producto = pr.id_producto
	LEFT JOIN 
	(SELECT pc.*, co.codigo FROM 
		PRECIO_CLIENTE_PRODUCTO pc 
		LEFT JOIN COTIZACION co ON pc.id_cotizacion = co.id_cotizacion
		WHERE /*fecha_inicio_vigencia < GETDATE()
		AND fecha_inicio_vigencia >= DATEADD(month,-6,GETDATE())  
		AND (fecha_fin_vigencia is NULL OR fecha_fin_vigencia >= GETDATE())*/
		 fecha_inicio_vigencia > DATEADD(day, cast((SELECT valor FROM PARAMETRO where codigo = 'DIAS_MAX_BUSQUEDA_PRECIOS') as int) * -1 , GETDATE()) 

		--ORDER BY fecha_inicio_vigencia DESC
	) pc ON pc.id_producto = pr.id_producto 
	AND co.id_cliente = pc.id_cliente AND cd.equivalencia = pc.equivalencia
--	AND cd.es_precio_alternativo = pc.es_unidad_alternativa
	where co.codigo = @codigo and cd.estado = 1 ) SQuery 
	where RowNumber = 1
	ORDER BY fecha_modificacion ASC;
END







ALTER TABLE PEDIDO 
ADD es_pago_contado bit default 'FALSE';


UPDATE PEDIDO
SET es_pago_contado = 'FALSE';







/* se agrego @esPagoContado */
ALTER PROCEDURE [dbo].[pi_pedido] 
@numeroGrupo bigint, 
@idCotizacion uniqueidentifier, 
@idCiudad uniqueidentifier, 
@idCliente uniqueidentifier, 
@numeroReferenciaCliente varchar(100),
@idDireccionEntrega uniqueidentifier,
@direccionEntrega varchar(200),
@contactoEntrega varchar(100),
@telefonoContactoEntrega varchar(100),
@fechaSolicitud  datetime,
@fechaEntregaDesde datetime,
@fechaEntregaHasta datetime,
@horaEntregaDesde datetime,
@horaEntregaHasta datetime,
@idSolicitante uniqueIdentifier,
@contactoPedido varchar(100),
@telefonoContactoPedido varchar(100),
@correoContactoPedido varchar(100),
@incluidoIGV smallint, 
@tasaIGV decimal(18,2), 
@igv decimal(18,2), 
@total decimal(18,2), 
@observaciones varchar(500), 
@idUsuario uniqueidentifier,
@estado smallint,
@estadoCrediticio smallint,
@esPagoContado bit,
@observacionSeguimientoPedido varchar(500),
@observacionSeguimientoCrediticioPedido varchar(500),
@ubigeoEntrega char(6),
@tipoPedido char(1),
@observacionesGuiaRemision varchar(200),
@observacionesFactura varchar(200),
@otrosCargos Decimal(12,2),
@tipo char(1),
@newId uniqueidentifier OUTPUT, 
@numero bigint OUTPUT 
AS
BEGIN

SET NOCOUNT ON





SET @newId = NEWID();
SET @numero = NEXT VALUE FOR dbo.SEQ_PEDIDO_NUMERO;







/*Si el numero de Grupo es distinto de null se actualiza en el Pedido Origen*/
IF @numeroGrupo IS NOT NULL 
BEGIN
	UPDATE PEDIDO SET numero_grupo = @numeroGrupo where numero = @numeroGrupo;
END

IF @idDireccionEntrega = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)
BEGIN 
	SET @idDireccionEntrega  = NEWID();
	INSERT INTO DIRECCION_ENTREGA
	(id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono, estado, usuario_creacion,
	fecha_creacion, usuario_modificacion, fecha_modificacion )
	 VALUES(@idDireccionEntrega, @idCliente, @ubigeoEntrega, 
	@direccionEntrega,@contactoEntrega,@telefonoContactoEntrega,1,@idUsuario, 
	GETDATE(), @idUsuario, GETDATE());
END 
ELSE
BEGIN
	UPDATE DIRECCION_ENTREGA SET 
	descripcion = @direccionEntrega, 
	ubigeo = @ubigeoEntrega, 
	contacto = @contactoEntrega,
	telefono = @telefonoContactoEntrega,
	usuario_modificacion = @idUsuario,
	fecha_modificacion = GETDATE() 
	where id_direccion_entrega = @idDireccionEntrega;
END 


IF @idSolicitante = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)
BEGIN 
	SET @idSolicitante  = NEWID();
	INSERT INTO SOLICITANTE 
	(id_solicitante, id_cliente, nombre, telefono, correo, estado, 
	usuario_creacion, fecha_creacion, usuario_modificacion, fecha_modificacion)
	VALUES(@idSolicitante, @idCliente, @contactoPedido, @telefonoContactoPedido, @correoContactoPedido,1,
	@idUsuario,GETDATE(), @idUsuario, GETDATE());
END 
ELSE
BEGIN
	UPDATE SOLICITANTE SET 
	nombre = @contactoPedido, 
	telefono = @telefonoContactoPedido, 
	correo = @correoContactoPedido,
	usuario_modificacion = @idUsuario,
	fecha_modificacion = GETDATE() 
	where id_solicitante = @idSolicitante;
END 



INSERT INTO [PEDIDO]
           ([id_pedido]
		   ,id_Cotizacion
		   ,[numero]
		   ,numero_grupo
			,[id_ciudad] 
			,[id_cliente] 
			,numero_referencia_cliente
			,id_direccion_entrega
			,direccion_entrega
			,contacto_entrega
			,telefono_contacto_entrega
			,[fecha_solicitud]
			,[fecha_entrega_desde]
			,[fecha_entrega_hasta]
			,[hora_entrega_desde]
			,[hora_entrega_hasta]
			,contacto_pedido
			,telefono_contacto_pedido
			,correo_contacto_pedido
			,incluido_igv 
			,tasa_igv
			,igv
			,total
			,observaciones
		   ,[estado]
		   ,[usuario_creacion]
		   ,[fecha_creacion]
		   ,[usuario_modificacion]
		   ,[fecha_modificacion]
		   ,ubigeo_entrega
		   ,tipo_pedido
		   ,observaciones_guia_remision
		   ,observaciones_factura
		   ,otros_cargos
		   ,id_solicitante
		   ,tipo
		   ,es_pago_contado
		   )
     VALUES
           (@newId
		   ,@idCotizacion
		    ,@numero
		    ,@numeroGrupo
			,@idCiudad
			,@idCliente 
			,@numeroReferenciaCliente
			,@idDireccionEntrega
			,@direccionEntrega
			,@contactoEntrega
			,@telefonoContactoEntrega
			,@fechaSolicitud
			,@fechaEntregaDesde
			,@fechaEntregaHasta
			,@horaEntregaDesde
			,@horaEntregaHasta
			,@contactoPedido
			,@telefonoContactoPedido
			,@correoContactoPedido
			,@incluidoIGV
			,@tasaIGV
			,@igv
			,@total
			,@observaciones
			,1
			,@idUsuario
			,GETDATE()
			,@idUsuario
			,GETDATE()
			,@ubigeoEntrega	
			,@tipoPedido
		   ,@observacionesGuiaRemision
		   ,@observacionesFactura
		   ,@otrosCargos
		   ,@idSolicitante

		   ,@tipo --[V|C|A]
		   ,@esPagoContado
			);

INSERT INTO SEGUIMIENTO_PEDIDO 
(
		id_seguimiento_pedido, 
		id_usuario ,
		id_pedido, 
		estado_pedido,
		observacion ,
		estado ,
		usuario_creacion ,
		fecha_creacion ,
		usuario_modifiacion ,
		fecha_modificacion 
)
VALUES(
			NEWID(),
			@idUsuario,
			@newId,
			@estado,
			@observacionSeguimientoPedido,
			1,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE()
);

INSERT INTO SEGUIMIENTO_CREDITICIO_PEDIDO 
(
		id_seguimiento_crediticio_pedido, 
		id_usuario ,
		id_pedido, 
		estado_pedido,
		observacion ,
		estado ,
		usuario_creacion ,
		fecha_creacion ,
		usuario_modifiacion ,
		fecha_modificacion 
)
VALUES(
			NEWID(),
			@idUsuario,
			@newId,
			@estadoCrediticio,
			@observacionSeguimientoCrediticioPedido,
			1,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE()
);


END







/* se agrego @esPagoContado */
ALTER PROCEDURE [dbo].[pu_pedido] 
@idPedido uniqueidentifier, 
@numeroGrupo bigint, 
@idCotizacion uniqueidentifier, 
@idCiudad uniqueidentifier, 
@idCliente uniqueidentifier, 
@numeroReferenciaCliente varchar(100),
@idDireccionEntrega uniqueidentifier,
@direccionEntrega varchar(200),
@contactoEntrega varchar(100),
@telefonoContactoEntrega varchar(100),
@fechaSolicitud  datetime,
@fechaEntregaDesde datetime,
@fechaEntregaHasta datetime,
@horaEntregaDesde datetime,
@horaEntregaHasta datetime,
@idSolicitante uniqueIdentifier,
@contactoPedido varchar(100),
@telefonoContactoPedido varchar(100),
@correoContactoPedido varchar(100),
@incluidoIGV smallint, 
@tasaIGV decimal(18,2), 
@igv decimal(18,2), 
@total decimal(18,2), 
@observaciones varchar(500), 
@idUsuario uniqueidentifier,
@estado smallint,
@estadoCrediticio smallint,
@esPagoContado bit,
@observacionSeguimientoPedido varchar(500),
@observacionSeguimientoCrediticioPedido varchar(500),
@tipoPedido char(1),
@observacionesGuiaRemision varchar(200),
@observacionesFactura varchar(200),
@ubigeoEntrega char(6),
@otrosCargos decimal(12,2)
AS
BEGIN

SET NOCOUNT ON

IF @numeroGrupo IS NOT NULL 
BEGIN
	UPDATE PEDIDO SET numero_grupo = @numeroGrupo where numero = @numeroGrupo;
END


IF @idDireccionEntrega = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)
BEGIN 
	SET @idDireccionEntrega  = NEWID();
	INSERT INTO DIRECCION_ENTREGA
	(id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono, estado, usuario_creacion,
	fecha_creacion, usuario_modificacion, fecha_modificacion )
	 VALUES(@idDireccionEntrega, @idCliente, @ubigeoEntrega, 
	@direccionEntrega,@contactoEntrega,@telefonoContactoEntrega,1,@idUsuario, 
	GETDATE(), @idUsuario, GETDATE());
END 
ELSE
BEGIN
	UPDATE DIRECCION_ENTREGA SET 
	descripcion = @direccionEntrega, 
	ubigeo = @ubigeoEntrega, 
	contacto = @contactoEntrega,
	telefono = @telefonoContactoEntrega,
	usuario_modificacion = @idUsuario,
	fecha_modificacion = GETDATE() 
	where id_direccion_entrega = @idDireccionEntrega;
END 


IF @idSolicitante = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)
BEGIN 
	SET @idSolicitante  = NEWID();
	INSERT INTO SOLICITANTE 
	(id_solicitante, id_cliente, nombre, telefono, correo, estado, 
	usuario_creacion, fecha_creacion, usuario_modificacion, fecha_modificacion)
	VALUES(@idSolicitante, @idCliente, @contactoPedido, @telefonoContactoPedido, @correoContactoPedido,1,
	@idUsuario,GETDATE(), @idUsuario, GETDATE());
END 
ELSE
BEGIN
	UPDATE SOLICITANTE SET 
	nombre = @contactoPedido, 
	telefono = @telefonoContactoPedido, 
	correo = @correoContactoPedido,
	usuario_modificacion = @idUsuario,
	fecha_modificacion = GETDATE() 
	where id_solicitante = @idSolicitante;
END 

UPDATE PEDIDO_DETALLE SET ESTADO = 0 ,
[usuario_modificacion] = @idUsuario, [fecha_modificacion] = GETDATE()
WHERE  id_pedido = @idPedido;


UPDATE ARCHIVO_ADJUNTO SET estado = 0 where id_archivo_adjunto IN (
SELECT id_archivo_adjunto FROM PEDIDO_ARCHIVO WHERE  id_pedido = @idPedido);

UPDATE PEDIDO_ARCHIVO SET estado = 0 WHERE  id_pedido = @idPedido

UPDATE [PEDIDO] SET
id_cotizacion = @idCotizacion,
[id_ciudad]  = @idCiudad
,[id_cliente] = @idCliente 
,numero_referencia_cliente = @numeroReferenciaCliente
,ubigeo_entrega = @ubigeoEntrega
,id_direccion_entrega = @idDireccionEntrega
,direccion_entrega = @direccionEntrega
,contacto_entrega = @contactoEntrega
,telefono_contacto_entrega = @telefonoContactoEntrega
,[fecha_solicitud] = @fechaSolicitud
,[fecha_entrega_desde] = @fechaEntregaDesde
,[fecha_entrega_hasta] = @fechaEntregaHasta
,[hora_entrega_desde] = @horaEntregaDesde
,[hora_entrega_hasta] = @horaEntregaHasta
,contacto_pedido = @contactoPedido
,telefono_contacto_pedido = @telefonoContactoPedido
,correo_contacto_pedido = @correoContactoPedido
,incluido_igv = @incluidoIGV
,tasa_igv = @tasaIGV
,igv = @igv
,total = @total
,observaciones = @observaciones
,tipo_pedido = @tipoPedido
,observaciones_guia_remision = @observacionesGuiaRemision
,observaciones_factura = @observacionesFactura
,[estado] = 1
,[usuario_modificacion] = @idUsuario
,[fecha_modificacion] = GETDATE()
,[otros_cargos] = @otrosCargos
,id_solicitante = @idSolicitante
,es_pago_contado = @esPagoContado
 WHERE id_pedido = @idPedido;

UPDATE SEGUIMIENTO_PEDIDO set estado = 0 where id_pedido = @idPedido;
UPDATE SEGUIMIENTO_CREDITICIO_PEDIDO set estado = 0 where id_pedido = @idPedido;

INSERT INTO SEGUIMIENTO_PEDIDO 
(
		id_seguimiento_pedido, 
		id_usuario ,
		id_pedido, 
		estado_pedido,
		observacion ,
		estado ,
		usuario_creacion ,
		fecha_creacion ,
		usuario_modifiacion ,
		fecha_modificacion 
)
VALUES(
			NEWID(),
			@idUsuario,
			@idPedido,
			@estado,
			@observacionSeguimientoPedido,
			1,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE()
);

INSERT INTO SEGUIMIENTO_CREDITICIO_PEDIDO 
(
		id_seguimiento_crediticio_pedido, 
		id_usuario ,
		id_pedido, 
		estado_pedido,
		observacion ,
		estado ,
		usuario_creacion ,
		fecha_creacion ,
		usuario_modifiacion ,
		fecha_modificacion 
)
VALUES(
			NEWID(),
			@idUsuario,
			@idPedido,
			@estadoCrediticio,
			@observacionSeguimientoCrediticioPedido,
			1,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE()
);



END







/* agrega columna es_pago_contado, plazo_credito_solicitado */
ALTER PROCEDURE [dbo].[ps_pedido] 
@idPedido uniqueIdentifier
AS
BEGIN

SELECT 
--PEDIDO
pe.numero, pe.numero_grupo, pe.fecha_solicitud,  
pe.fecha_entrega_desde, pe.fecha_entrega_hasta, pe.fecha_entrega_extendida,
pe.hora_entrega_desde, pe.hora_entrega_hasta,
pe.incluido_igv,  pe.igv, pe.total, pe.observaciones,  pe.fecha_modificacion,
pe.numero_referencia_cliente, pe.id_direccion_entrega, pe.direccion_entrega, pe.contacto_entrega,
pe.telefono_contacto_entrega, 
pe.fecha_programacion,
pe.tipo_pedido, pe.observaciones_factura, pe.observaciones_guia_remision,
pe.contacto_pedido,pe.telefono_contacto_pedido, pe.correo_contacto_pedido,
pe.otros_cargos,
pe.numero_referencia_adicional,
pe.fecha_creacion as fecha_registro,
cpe.serie AS serie_factura,
cpe.CORRELATIVO AS numero_factura,
pe.id_solicitante,
pe.tipo,
pe.es_pago_contado,
--UBIGEO
pe.ubigeo_entrega, ub.departamento, ub.provincia, ub.distrito,
---CLIENTE
cl.id_cliente, cl.codigo, cl.razon_social, cl.ruc, cic.id_ciudad as id_ciudad_cliente, cic.nombre as nombre_ciudad_cliente,
cl.razon_social_sunat, cl.direccion_domicilio_legal_sunat, cl.correo_envio_factura, cl.plazo_credito, cl.plazo_credito_solicitado,
cl.tipo_pago_factura,
cl.forma_pago_factura,


---VENTA
ve.igv as igv_venta,
ve.sub_total as sub_total_venta,
ve.total as total_venta,
ve.id_venta,

--CIUDAD
ci.id_ciudad, ci.nombre as nombre_ciudad , ci.es_provincia,
--USUARIO
us.nombre  as nombre_usuario, us.cargo, us.contacto as contacto_usuario, us.email,
--SEGUIMIENTO
sp.estado_pedido as estado_seguimiento,
us2.nombre as usuario_seguimiento, sp.observacion as observacion_seguimiento,
us2.id_usuario as id_usuario_seguimiento,
--SEGUIMIENTO CREDITICIO
spc.estado_pedido as estado_seguimiento_crediticio,
us3.nombre as usuario_seguimiento_crediticio, spc.observacion as observacion_seguimiento_crediticio,
us3.id_usuario as id_usuario_seguimiento_crediticio,

--COTIZACION
co.id_cotizacion,
co.codigo as cotizacion_codigo,
--VENDEDORES,
verc.codigo as responsable_comercial_codigo,
verc.descripcion as responsable_comercial_descripcion,
ISNULL(uverc.email,'') as responsable_comercial_email,

vesc.codigo as supervisor_comercial_codigo,
vesc.descripcion as supervisor_comercial_descripcion,
ISNULL(uvesc.email,'') as supervisor_comercial_email,

veasc.codigo as asistente_servicio_cliente_codigo,
veasc.descripcion as asistente_servicio_cliente_descripcion,
ISNULL(uveasc.email,'') as asistente_servicio_cliente_email

FROM PEDIDO as pe
INNER JOIN CIUDAD AS ci ON pe.id_ciudad = ci.id_ciudad
INNER JOIN USUARIO AS us on pe.usuario_creacion = us.id_usuario
INNER JOIN SEGUIMIENTO_PEDIDO sp ON pe.id_pedido = sp.id_pedido
INNER JOIN USUARIO AS us2 on sp.id_usuario = us2.id_usuario
INNER JOIN SEGUIMIENTO_CREDITICIO_PEDIDO spc ON pe.id_pedido = spc.id_pedido
INNER JOIN USUARIO AS us3 on spc.id_usuario = us3.id_usuario
INNER JOIN CLIENTE AS cl ON pe.id_cliente = cl.id_cliente
INNER JOIN CIUDAD AS cic ON cl.id_ciudad = cic.id_ciudad
LEFT JOIN UBIGEO ub ON pe.ubigeo_entrega = ub.codigo
LEFT JOIN COTIZACION co ON co.id_cotizacion = pe.id_cotizacion
LEFT JOIN VENTA ve ON ve.id_pedido = pe.id_pedido
LEFT JOIN CPE_CABECERA_BE cpe ON cpe.id_cpe_cabecera_be = ve.id_documento_venta
LEFT JOIN SOLICITANTE so ON so.id_solicitante = pe.id_solicitante
LEFT JOIN VENDEDOR AS verc ON cl.id_responsable_comercial = verc.id_vendedor
LEFT JOIN VENDEDOR AS vesc ON cl.id_supervisor_comercial = vesc.id_vendedor
LEFT JOIN VENDEDOR AS veasc ON cl.id_asistente_servicio_cliente = veasc.id_vendedor
LEFT JOIN USUARIO AS uverc ON uverc.id_usuario = verc.id_usuario
LEFT JOIN USUARIO AS uvesc ON uvesc.id_usuario = vesc.id_usuario
LEFT JOIN USUARIO AS uveasc ON uveasc.id_usuario = veasc.id_usuario
--LEFT JOIN GRUPO AS gr ON pe.id_grupo = gr.id_grupo  
where pe.id_pedido = @idPedido and 
pe.estado = 1
AND sp.estado = 1
AND spc.estado = 1;



--RECUPERA EL DETALLE DEL PEDIDO

select * from (
	SELECT pd.id_pedido_detalle, pd.cantidad, pd.precio_sin_igv, pd.costo_sin_igv, 
	pd.equivalencia as equivalencia,
	pd.unidad, pd.porcentaje_descuento, pd.precio_neto, pd.es_precio_alternativo, pd.flete,
	pr.id_producto, pr.sku, pr.descripcion, pr.sku_proveedor, pr.imagen, pr.proveedor, 
	pr.costo as costo_producto, pr.precio as precio_producto,
	pd.observaciones,
	pd.fecha_modificacion,

	--Si es precio alternativo y el precio registrado es estandar
--	 CASE pd.es_precio_alternativo WHEN 
	CASE pc.es_unidad_alternativa WHEN 1 THEN pc.precio_neto * pc.equivalencia 
	ELSE pc.precio_neto END as precio_neto_vigente, 

	pc.flete as flete_vigente, 

	CASE pc.es_unidad_alternativa WHEN 1 THEN pc.precio_unitario * pc.equivalencia 
	ELSE pc.precio_unitario END as precio_unitario_vigente, 

	pc.equivalencia as equivalencia_vigente,
	pc.id_precio_cliente_producto,
	pc.fecha_inicio_vigencia,
	pc.fecha_fin_vigencia,
	pc.id_cliente,


	vd.precio_unitario as precio_unitario_venta,
	vd.igv_precio_unitario as igv_precio_unitario_venta,	
	vd.id_venta_detalle,
	(pd.cantidad - COALESCE(mad.cantidadAtendida,0)) AS cantidadPendienteAtencion,

	ROW_NUMBER() OVER(PARTITION BY pd.id_producto,pe.id_cliente 
	ORDER BY pc.fecha_inicio_vigencia DESC, pc.fecha_creacion DESC) AS RowNumber


	FROM PEDIDO_DETALLE as pd 
	INNER JOIN PEDIDO as pe ON pd.id_pedido = pe.id_pedido 
	INNER JOIN PRODUCTO pr ON pd.id_producto = pr.id_producto
	LEFT JOIN 
	(
	SELECT mad.id_pedido_detalle, SUM(cantidad) as cantidadAtendida  FROM
	MOVIMIENTO_ALMACEN_DETALLE AS mad 
	INNER JOIN MOVIMIENTO_ALMACEN ma ON mad.id_movimiento_almacen = ma.id_movimiento_almacen
	WHERE ma.id_pedido = @idPedido
	AND mad.estado = 1 AND  ma.estado = 1 AND ma.anulado = 0
	GROUP BY mad.id_pedido_detalle
	) AS  mad ON mad.id_pedido_detalle  = pd.id_pedido_detalle
	
	LEFT JOIN VENTA_DETALLE AS vd ON vd.id_pedido_detalle = mad.id_pedido_detalle


	LEFT JOIN
	(SELECT pc.* FROM 
		PRECIO_CLIENTE_PRODUCTO pc
		WHERE --fecha_inicio_vigencia < GETDATE()
		--AND (fecha_fin_vigencia is NULL OR fecha_fin_vigencia >= GETDATE())
		--AND
		 fecha_inicio_vigencia > DATEADD(day, cast((SELECT valor FROM PARAMETRO where codigo = 'DIAS_MAX_BUSQUEDA_PRECIOS') as int) * -1 , GETDATE()) 
		--ORDER BY fecha_inicio_vigencia DESC
	) pc ON pc.id_producto = pr.id_producto 
	AND pe.id_cliente = pc.id_cliente AND pd.equivalencia = pc.equivalencia
	--AND pd.es_precio_alternativo = pc.es_unidad_alternativa


	where pd.id_pedido = @idPedido and pd.estado = 1 
	
	) SQuery 
		where RowNumber = 1
	ORDER BY fecha_modificacion ASC;


--RECUPERA LAS DIRECCIONES DE ENTREGA

SELECT TOP 0 de.id_direccion_entrega, de.descripcion, de.contacto, de.telefono, ubigeo
FROM DIRECCION_ENTREGA de
INNER JOIN PEDIDO pe ON de.id_cliente = pe.id_cliente
where pe.id_pedido = @idPedido
AND de.estado = 1;


--RECUPERA LAS GUÍAS Y FACTURAS

SELECT 
ma.id_movimiento_almacen, ma.serie_documento, ma.numero_documento, 
ma.fecha_emision,
ma.fecha_traslado,
dv.id_cpe_cabecera_be as id_documento_venta, dv.SERIE, dv.CORRELATIVO, 
 CONVERT( DATETIME,dv.FEC_EMI ,20) AS fecha_emision_factura,
 	CASE  dv.COD_ESTD_SUNAT 
	WHEN '101' THEN 'EN PROCESO'
	WHEN '102' THEN 'ACEPTADO'
	WHEN '103' THEN 'ACEPTADO CON OBS'
	WHEN '104' THEN 'RECHAZADO'
	WHEN '105' THEN 'ANULADO'
	ELSE '---' END as estado ,
mad.id_movimiento_almacen_detalle, mad.cantidad, 
mad.unidad, pr.id_producto, pr.sku, pr.descripcion
FROM MOVIMIENTO_ALMACEN_DETALLE as mad 
INNER JOIN MOVIMIENTO_ALMACEN as ma ON mad.id_movimiento_almacen = ma.id_movimiento_almacen
INNER JOIN PRODUCTO pr ON mad.id_producto = pr.id_producto
INNER JOIN VENTA ve ON ve.id_movimiento_almacen = ma.id_movimiento_almacen
LEFT JOIN CPE_CABECERA_BE dv ON ve.id_documento_venta = dv.id_cpe_cabecera_be
AND mad.estado = 1
WHERE ma.id_pedido = @idPedido
AND ma.estado = 1
AND ma.anulado = 0
ORDER BY ma.numero_documento, mad.fecha_creacion asc;



--RECUPERA LOS ARCHIVOS ADJUNTOS

SELECT arch.id_archivo_adjunto as id_pedido_adjunto,  nombre--, arch.adjunto,
 FROM ARCHIVO_ADJUNTO arch
 INNER JOIN PEDIDO_ARCHIVO parch ON arch.id_archivo_adjunto = parch.id_archivo_adjunto
   WHERE parch.id_pedido = @idPedido
   AND arch.estado = 1
   AND parch.estado = 1;


SELECT TOP 0 so.id_solicitante, so.nombre, so.telefono, so.correo
FROM SOLICITANTE so
INNER JOIN PEDIDO pe ON so.id_cliente = pe.id_cliente
where pe.id_pedido = @idPedido
AND so.estado = 1;

END










