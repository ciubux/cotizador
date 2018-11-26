
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

		
		IF (@aplica_sedes = 'TRUE')
		BEGIN

		SELECT @ruc = ruc FROM CLIENTE WHERE id_cliente = @id_cliente AND estado = 1;


		DECLARE cursor_sedes CURSOR FAST_FORWARD
		FOR SELECT id_cliente FROM CLIENTE WHERE ruc like @ruc

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

		CLOSE cursor_sedes
		DEALLOCATE cursor_sedes
	
		--SELECT * FROM COTIZACION_DETALLE
		END
		
		ELSE 
		
		BEGIN
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
	END
END

