ALTER TABLE CLIENTE 
ADD sede_principal BIT default 'FALSE';


ALTER TABLE COTIZACION
ADD aplica_sedes BIT default 'FALSE';

UPDATE CLIENTE
SET sede_principal = 'FALSE';

UPDATE COTIZACION
SET aplica_sedes = 'FALSE';

CREATE PROCEDURE [dbo].[ps_clienteSedes]
@ruc varchar(20) 
AS
BEGIN

SELECT cl.id_cliente, cl.codigo, cl.razon_social, cl.nombre_comercial, cl.ruc, ci.id_ciudad, ci.nombre as ciudad_nombre
FROM CLIENTE cl
INNER JOIN CIUDAD AS ci ON cl.id_ciudad = ci.id_ciudad
WHERE cl.estado = 1 AND cl.ruc like @ruc and cl.sede_principal = 0

END



/* Alter ps_getcliente - Agregar sede_principal */

ALTER PROCEDURE [dbo].[ps_getcliente] 
@idCliente uniqueidentifier 
AS
BEGIN

SELECT cl.id_cliente, cl.codigo, cl.razon_social,cl.nombre_comercial, cl.contacto1, cl.contacto2, cl.ruc,
cl.domicilio_legal, 
/*Si el cliente no tiene correo entonces se obtiene de algún pedido que tenga correo*/
CASE cl.correo_envio_factura WHEN '' THEN 
(SELECT TOP 1 correo_contacto_pedido FROM PEDIDO where id_cliente = cl.id_cliente
AND correo_contacto_pedido IS NOT NULL AND correo_contacto_pedido NOT IN ( '','.') )
ELSE cl.correo_envio_factura END AS correo_envio_factura, 

cl.razon_social_sunat, cl.nombre_comercial_sunat, 
cl.direccion_domicilio_legal_sunat, cl.estado_contribuyente_sunat, 
cl.condicion_contribuyente_sunat, cl.sede_principal,
ub.codigo as codigo_ubigeo,
ub.provincia, ub.departamento, ub.distrito, cl.plazo_credito,
cl.tipo_pago_factura,cl.forma_pago_factura, cl.id_ciudad


FROM CLIENTE AS cl LEFT JOIN UBIGEO AS ub ON cl.ubigeo = ub.codigo
WHERE estado = 1 AND id_cliente = @idCliente 

END




/* Alter ps_cliente - Agregar sede_principal */


ALTER PROCEDURE [dbo].[ps_cliente] 
@idCliente uniqueidentifier 
AS
BEGIN

SELECT cl.id_cliente, cl.codigo, cl.razon_social,
cl.nombre_comercial, cl.contacto1, cl.contacto2, cl.ruc,
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

--VENDEDORES,
verc.id_vendedor as responsable_comercial_id_vendedor,
verc.codigo as responsable_comercial_codigo,
verc.descripcion as responsable_comercial_descripcion,

vesc.id_vendedor as supervisor_comercial_id_vendedor,
vesc.codigo as supervisor_comercial_codigo,
vesc.descripcion as supervisor_comercial_descripcion,

veasc.id_vendedor as asistente_servicio_cliente_id_vendedor,
veasc.codigo as asistente_servicio_cliente_codigo,
veasc.descripcion as asistente_servicio_cliente_descripcion,

cl.observaciones_credito, 
cl.observaciones, 
cl.bloqueado,

cl.pertenece_canal_multiregional,
cl.pertenece_canal_lima,
cl.pertenece_canal_provincia,
cl.pertenece_canal_pcp,
cl.pertenece_canal_ordon,
cl.es_sub_distribuidor,

clgr.id_grupo_cliente ,
gr.grupo as grupo_nombre

FROM CLIENTE AS cl 
INNER JOIN CIUDAD AS ci ON cl.id_ciudad = ci.id_ciudad
LEFT JOIN UBIGEO AS ub ON cl.ubigeo = ub.codigo
LEFT JOIN VENDEDOR AS verc ON cl.id_responsable_comercial = verc.id_vendedor
LEFT JOIN VENDEDOR AS vesc ON cl.id_supervisor_comercial = vesc.id_vendedor
LEFT JOIN VENDEDOR AS veasc ON cl.id_asistente_servicio_cliente = veasc.id_vendedor
LEFT JOIN CLIENTE_GRUPO_CLIENTE AS clgr ON clgr.id_cliente = cl.id_cliente
LEFT JOIN GRUPO_CLIENTE AS gr ON gr.id_grupo_cliente = clgr.id_grupo_cliente 
WHERE cl.estado = 1 AND cl.id_cliente = @idCliente 

END










/* Alter pi_seguimiento_cotizacion - Agregar precios a sedes */

ALTER PROCEDURE [dbo].[pi_seguimiento_cotizacion] 
@codigo bigint,
@idUsuario uniqueidentifier,
@estado int,
@observacion varchar(2000),
@fechaModificacion datetime,
@fechaModificacionActual datetime OUTPUT
AS
BEGIN

DECLARE @id_cotizacion uniqueidentifier;
DECLARE @id_cliente uniqueidentifier;
DECLARE @id_cliente_sede uniqueidentifier;
DECLARE @aplica_sedes bit;
DECLARE @ruc varchar(20);


SELECT @id_cotizacion =id_cotizacion, @id_cliente = id_cliente, @aplica_sedes = aplica_sedes,  @fechaModificacionActual = fecha_modificacion FROM COTIZACION WHERE codigo = @codigo AND estado = 1;
/*
if(convert(varchar,@fechaModificacionActual, 120)  = convert(varchar,@fechaModificacion, 120)) 
Begin*/
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
				@observacion,
				1,
				@idUsuario,
				GETDATE(),
				@idUsuario,
				GETDATE()
	);
	--Si estado es Aceptar
	IF (@estado = 3)
	BEGIN

	--SELECT * FROM COTIZACION_DETALLE

	INSERT INTO PRECIO_CLIENTE_PRODUCTO
		(id_precio_cliente_producto, id_cliente, id_producto,
		fecha_inicio_vigencia, fecha_fin_vigencia, unidad, moneda, 
		precio_neto, flete, precio_unitario, id_cotizacion,
		estado, usuario_creacion, fecha_creacion, usuario_modifiacion, fecha_modificacion,
		equivalencia, es_unidad_alternativa )


		SELECT NEWID(), c.id_cliente, p.id_producto, 
		CASE WHEN co.fecha_inicio_vigencia_precios IS NULL 
		THEN GETDATE() ELSE co.fecha_inicio_vigencia_precios END,
		co.fecha_fin_vigencia_precios, dc.unidad, 
		'S', dc.precio_neto, dc.flete, dc.precio_neto + dc.flete, @id_cotizacion,1, @idUsuario, GETDATE(), @idUsuario, GETDATE(),
		 dc.equivalencia, dc.es_precio_alternativo
		 FROM  COTIZACION  co
		 INNER JOIN COTIZACION_DETALLE dc ON co.id_cotizacion = dc.id_cotizacion
		 INNER JOIN PRODUCTO p ON dc.id_producto = p.id_producto
		 INNER JOIN CLIENTE c ON co.id_cliente = c.id_cliente
		 WHERE co.id_cotizacion = @id_cotizacion                                              
		 AND dc.estado = 1 AND co.estado = 1
		 AND c.estado = 1;
	END 

	IF (@aplica_sedes = 'TRUE')
	BEGIN

	SELECT @ruc = ruc FROM CLIENTE WHERE id_cliente = @id_cliente AND estado = 1;


	DECLARE cursor_sedes CURSOR FAST_FORWARD
	FOR SELECT id_cliente FROM CLIENTE WHERE sede_principal = 'FALSE' AND ruc like @ruc

	OPEN cursor_sedes
	FETCH NEXT FROM cursor_sedes INTO @id_cliente_sede

		WHILE @@FETCH_STATUS = 0
		BEGIN

			INSERT INTO PRECIO_CLIENTE_PRODUCTO
				(id_precio_cliente_producto, id_cliente, id_producto,
				fecha_inicio_vigencia, fecha_fin_vigencia, unidad, moneda, 
				precio_neto, flete, precio_unitario, id_cotizacion,
				estado, usuario_creacion, fecha_creacion, usuario_modifiacion, fecha_modificacion,
				equivalencia, es_unidad_alternativa )


				SELECT NEWID(), @id_cliente_sede, p.id_producto, 
				CASE WHEN co.fecha_inicio_vigencia_precios IS NULL 
				THEN GETDATE() ELSE co.fecha_inicio_vigencia_precios END,
				co.fecha_fin_vigencia_precios, dc.unidad, 
				'S', dc.precio_neto, dc.flete, dc.precio_neto + dc.flete, @id_cotizacion,1, @idUsuario, GETDATE(), @idUsuario, GETDATE(),
				 dc.equivalencia, dc.es_precio_alternativo
				 FROM  COTIZACION  co
				 INNER JOIN COTIZACION_DETALLE dc ON co.id_cotizacion = dc.id_cotizacion
				 INNER JOIN PRODUCTO p ON dc.id_producto = p.id_producto
				 INNER JOIN CLIENTE c ON co.id_cliente = c.id_cliente
				 WHERE co.id_cotizacion = @id_cotizacion                                              
				 AND dc.estado = 1 AND co.estado = 1
				 AND c.estado = 1;

			FETCH NEXT FROM cursor_sedes INTO @id_cliente_sede
		END

	CLOSE the_cursor
	DEALLOCATE the_cursor
	
	--SELECT * FROM COTIZACION_DETALLE

	
	END 

END






/* Alter pi_cotizacion - Registrar  aplica_sedes */


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
			@aplicaSedes
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








/* Alter pu_cotizacion - Agregar aplica_sedes */


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





