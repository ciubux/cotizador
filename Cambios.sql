--------
--------VENTA GRATUITA

ALTER TABLE VENTA_DETALLE add costo_unitario Decimal(12,4)
ALTER TABLE VENTA_DETALLE add igv_costo_unitario Decimal(12,6)

--Se utilizarán para calcular directament el precio de la linea, igv_linea
ALTER TABLE VENTA_DETALLE add precio Decimal(12,4)
ALTER TABLE VENTA_DETALLE add igv_precio Decimal(12,2)
ALTER TABLE VENTA_DETALLE add costo Decimal(12,4)



--SE DEBE REVISAR QUE PROCEDIMIENTOS_ALMACENADOS USAN ESTE CAMPO
---ALTER TABLE VENTA_DETALLE drop column igv_costo

 
--UPDATE VENTA SET costo_unitario = 0
--UPDATE VENTA SET igv_costo_unitario = 0
 



UPDATE PROCEDURE pu_venta --Se agrego Calculo Gratuitas 
UPDATE PROCEDURE pi_movimientoAlmacenDetalleSalida --Se agrego costo
UPDATE PROCEDURE pu_ventaDetalle 
-----------------NOTAS DE CREDITO Y ACTUALIZACION DE VENTAS
UPDATE PROCEDURE pi_ventaDetalle -----PENDIENTE DE ACTUALIZACION COSTO

UPDATE VENTA_DETALLE SET costo_unitario = (CASE es_precio_alternativo WHEN 1 THEN costo_sin_igv/equivalencia 
ELSE costo_sin_igv END)

UPDATE VENTA_DETALLE SET igv_costo_unitario = costo_unitario * 0.18

UPDATE VENTA_DETALLE SET costo = CAST((costo_unitario * cantidad) AS decimal(12,4))

UPDATE VENTA_DETALLE SET igv_costo = costo * 0.18

UPDATE PROCEDURE pi_documentoVenta --Se agrego Costo











/*MODIFICAIONES PEDIDO -- VISUALIZAR MARGEN*/


ALTER TABLE USUARIO ADD visualiza_margen smallint;
ALTER TABLE USUARIO ADD confirma_stock smallint;

ALTER TABLE PEDIDO ADD stock_confirmado smallint;

ps_usuario
pu_pedidoStockConfirmado
ps_pedidos
pu_actualizarPedido













equipos con fuente
switcxh administrable










































-------NOTAS DE INGRESO

--Se agrega columna a tabla PEDIDO (indicará si es venta, compra, almacen)

ALTER TABLE PEDIDO ADD tipo char(1);
UPDATE PEDIDO SET tipo = 'V';


--Se agregó columna tipo

UPDATE PROCEDURE pi_pedido
CREATE PROCEDURE pi_pedidoCompra

--Se agregó columna tipo en el filtro
UPDATE PROCEDURE ps_pedidos
CREATE PROCEDURE ps_pedidosCompra

CREATE PROCEDURE pi_movimientoAlmacenEntrada

---DUDAS ¿El número de pedido será el mismo para todos los pedidos?, se tiene el número de la orden de compra tambien
---¿Se creará la tabla Proveedor?


ALTER TABLE SERIE_DOCUMENTO_ELECTRONICO ADD siguiente_numero_guia_remision int
ALTER TABLE SERIE_DOCUMENTO_ELECTRONICO ADD siguiente_numero_nota_ingreso int

--SE agregó columna tipo
UPDATE PROCEDURE ps_pedido



DROP PROCEDURE ps_getciudades

CREATE PROCEDURE [dbo].[ps_ciudades]

CREATE PROCEDURE ps_seriesDocumentoPorSede


DROP pi_movimientoAlmacenDetalleSalida

CREATE PROCEDURE pi_movimientoAlmacenDetalle

ALTER TABLE [dbo].[MOVIMIENTO_ALMACEN]  DROP CONSTRAINT [constraint_tipo_movimiento] 

ALTER TABLE [dbo].[MOVIMIENTO_ALMACEN]  WITH CHECK ADD  CONSTRAINT [constraint_tipo_movimiento] CHECK  (([tipo_movimiento]='S' OR [tipo_movimiento]='E'))