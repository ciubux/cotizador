ALTER TABLE COTIZACION ADD tipo_cotizacion smallint

UPDATE COTIZACION set tipo_cotizacion = es_transitoria

ALTER TABLE COTIZACION DROP COLUMN es_transitoria


/****** Object:  StoredProcedure [dbo].[ps_cotizacion]    Script Date: 20/02/2019 3:50:34 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

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
co.es_transitoria, co.tipo_cotizacion,
---CLIENTE
cl.id_cliente, cl.codigo, cl.razon_social, cl.ruc, cl.sede_principal,
cl.plazo_credito_solicitado, cl.tipo_pago_factura,
--CIUDAD
ci.id_ciudad, ci.nombre as nombre_ciudad , ci.es_provincia,
--USUARIO
us.nombre  as nombre_usuario, us.cargo, us.contacto as contacto_usuario, us.email,
--GRUPO
gr.id_grupo_cliente, gr.codigo as codigo_grupo, gr.grupo as nombre_grupo, gr.contacto as contacto_grupo,
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
LEFT JOIN GRUPO_CLIENTE AS gr ON co.id_grupo_cliente = gr.id_grupo_cliente
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
	
	pc.costo_sin_igv_anterior,
	pc.precio_neto_anterior ,
	pc.precio_sin_igv_anterior ,
	pc.es_unidad_alternativa,

	/*
		
	CASE ci.es_provincia WHEN 0 THEN
		(p.PRECIO * pc.precio_neto) /cd.precio_sin_igv
	ELSE 
		(p.precio_provincia * pc.precio_neto) /cd.precio_sin_igv
	END
		AS precio_neto_recalculado*/
	
	ROW_NUMBER() OVER (PARTITION BY cd.id_producto,co.id_cliente ORDER BY 
	pc.fecha_inicio_vigencia DESC, pc.codigo DESC) AS RowNumber

	FROM COTIZACION_DETALLE as cd INNER JOIN 
	COTIZACION as co ON cd.id_cotizacion = co.id_cotizacion 
	INNER JOIN PRODUCTO pr ON cd.id_producto = pr.id_producto
	LEFT JOIN 
	(SELECT pc.*, co.codigo, cd.precio_neto as precio_neto_anterior, 
	cd.precio_sin_igv as precio_sin_igv_anterior, 
	cd.costo_sin_igv as costo_sin_igv_anterior
		FROM 
		PRECIO_CLIENTE_PRODUCTO pc 
		LEFT JOIN COTIZACION co ON pc.id_cotizacion = co.id_cotizacion
		LEFT JOIN COTIZACION_DETALLE cd ON co.id_Cotizacion = cd.id_cotizacion AND pc.id_producto = cd.id_producto
		WHERE 
		cd.estado = 1 AND
		/*fecha_inicio_vigencia < GETDATE()
		AND fecha_inicio_vigencia >= DATEADD(month,-6,GETDATE())  
		AND (fecha_fin_vigencia is NULL OR fecha_fin_vigencia >= GETDATE())*/
		 fecha_inicio_vigencia > DATEADD(day, cast((SELECT valor FROM PARAMETRO where codigo = 'DIAS_MAX_BUSQUEDA_PRECIOS') as int) * -1 , GETDATE()) 

		--ORDER BY fecha_inicio_vigencia DESC
	) pc ON pc.id_producto = pr.id_producto 
	AND (co.id_cliente = pc.id_cliente
	OR pc.id_grupo_cliente = (SELECT id_grupo_cliente FROM CLIENTE_GRUPO_CLIENTE where id_cliente = co.id_cliente)
	) AND cd.equivalencia = pc.equivalencia
--	AND cd.es_precio_alternativo = pc.es_unidad_alternativa
	where co.codigo = @codigo and cd.estado = 1 ) SQuery 
	where RowNumber = 1
	ORDER BY fecha_modificacion ASC;



END


USE [cotizadormp]
GO
/****** Object:  StoredProcedure [dbo].[pi_cotizacion]    Script Date: 20/02/2019 6:24:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[pi_cotizacion] 
@fecha datetime, 
@fechaLimiteValidezOferta datetime,
@fechaInicioVigenciaPrecios datetime,
@fechaFinVigenciaPrecios datetime,
@incluidoIgv smallint, 
@consideraCantidades smallint, 
@idCliente uniqueidentifier, 
@idGrupoCliente int, 
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
@tipoCotizacion smallint,
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
		   ,[id_grupo_cliente]
		   ,[tipo_cotizacion]
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
			@esPagoContado,
			@idGrupoCliente,
			@tipoCotizacion
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
/*
IF @idCliente IS NOT NULL
BEGIN 
	UPDATE CLIENTE set contacto1 = @contacto where id_cliente = @idCliente;
END
IF @idCliente IS NOT NULL
BEGIN 
	UPDATE CLIENTE set contacto1 = @contacto where id_cliente = @idCliente;
END
ELSE IF @idGrupoCliente IS NOT NULL
BEGIN 
	UPDATE GRUPO_CLIENTE set contacto = @contacto where id_grupo_cliente = @idGrupoCliente;
END
*/


END






USE [cotizadormp]
GO
/****** Object:  StoredProcedure [dbo].[pu_cotizacion]    Script Date: 20/02/2019 3:57:09 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[pu_cotizacion] 
@fecha datetime, 
@fechaLimiteValidezOferta datetime,
@fechaInicioVigenciaPrecios datetime,
@fechaFinVigenciaPrecios datetime,
@fechaModificacion datetime,
@incluidoIgv smallint, 
@consideraCantidades smallint, 
@idCliente uniqueidentifier, 
@idGrupoCliente int, 
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
@tipo_cotizacion smallint, 
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
			   ,[id_grupo_cliente] = @idGrupoCliente
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
			   ,[tipo_cotizacion]  = @tipo_cotizacion
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







