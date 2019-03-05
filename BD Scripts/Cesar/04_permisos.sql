CREATE TABLE PERMISO (
	id_permiso int identity(1,1) not null primary key,
	estado bit,
	usuario_creacion uniqueIdentifier, 
	fecha_creacion datetime,
	usuario_modificacion uniqueIdentifier, 
	fecha_modificacion datetime,
	codigo CHAR(4) not null,
	descripcion_corta varchar(50),
	descripcion_larga varchar(500)
)

CREATE TABLE USUARIO_PERMISO
(
	id_permiso int not null,
	id_usuario uniqueIdentifier,
	estado bit,
	usuario_creacion uniqueIdentifier, 
	fecha_creacion datetime,
	usuario_modificacion uniqueIdentifier, 
	fecha_modificacion datetime
)

INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P001','administra_permisos');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P002','aprueba_cotizaciones_lima');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P003','aprueba_cotizaciones_provincias');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P004','aprueba_pedidos_lima');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P005','aprueba_pedidos_provincias');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P006','crea_guias');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P007','administra_guias_lima');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P008','administra_guias_provincias');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P009','crea_documentos_venta');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P010','administra_documentos_venta_lima');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P011','administra_documentos_venta_provincias');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P012','crea_cotizaciones_provincias');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P013','toma_pedidos_provincias');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P014','programa_pedidos');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P015','modifica_maestro_clientes');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P016','modifica_maestro_productos');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P017','visualiza_documentos_venta');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P018','visualiza_pedidos_lima');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P019','visualiza_guias_remision');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P020','visualiza_cotizaciones');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P021','libera_pedidos');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P022','bloquea_pedidos');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P023','aprueba_anulaciones');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P024','crea_notas_credito');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P025','visualiza_pedidos_provincias');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P026','visualiza_costos');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P027','crea_notas_debito');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P028','realiza_refacturacion');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P029','visualiza_margen');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P030','confirma_stock');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P031','crea_cotizaciones_lima');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P032','toma_pedidos_lima');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P033','toma_pedidos_compra');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P034','toma_pedidos_almacen');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P035','define_plazo_credito');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P036','define_monto_credito');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P037','define_responsable_comercial');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P038','define_supervisor_comercial');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P039','define_asistente_atencion_cliente');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P040','define_responsable_portafolio');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P041','modifica_pedido_venta_fecha_entrega_hasta');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P042','bloquea_clientes');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P043','modifica_canales');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P044','realiza_carga_masiva_pedidos');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P045','modifica_pedido_fecha_entrega_extendida');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P046','crea_factura_consolidada_multiregional');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P047','crea_factura_consolidada_local');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P048','visualiza_guias_pendientes_facturacion');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P049','modifica_negociacion_multiregional');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P050','busca_sedes_grupo_cliente');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P051','modifica_producto');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P052','aprueba_pedidos_compra');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P053','aprueba_pedidos_almacen');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P054','crea_cotizaciones_grupo_cliente');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P055','crea_cotizaciones_grupales');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P056','aprueba_cotizaciones_grupales');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P057','modifica_subdistribuidor');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P058','modifica_origen');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P059','realiza_carga_masiva_productos');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P060','realiza_carga_masiva_clientes');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P061','modifica_grupo_clientes');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P062','visualiza_grupo_clientes');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P063','visualiza_productos');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P064','visualiza_subdistribuidores');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P065','visualiza_origenes');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P066','visualiza_clientes');
INSERT INTO PERMISO (estado, fecha_creacion,  fecha_modificacion,codigo, descripcion_corta)	values (1, GETDATE(), GETDATE(), 'P067','elimina_cotizaciones_aceptadas');




INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'aprueba_cotizaciones_lima') id_permiso, id_usuario,	ISNULL(aprueba_cotizaciones_lima,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'aprueba_cotizaciones_provincias') id_permiso, id_usuario,	ISNULL(aprueba_cotizaciones_provincias,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'aprueba_pedidos_lima') id_permiso, id_usuario,	ISNULL(aprueba_pedidos_lima,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'aprueba_pedidos_provincias') id_permiso, id_usuario,	ISNULL(aprueba_pedidos_provincias,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'crea_guias') id_permiso, id_usuario,	ISNULL(crea_guias,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'administra_guias_lima') id_permiso, id_usuario,	ISNULL(administra_guias_lima,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'administra_guias_provincias') id_permiso, id_usuario,	ISNULL(administra_guias_provincias,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'crea_documentos_venta') id_permiso, id_usuario,	ISNULL(crea_documentos_venta,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'administra_documentos_venta_lima') id_permiso, id_usuario,	ISNULL(administra_documentos_venta_lima,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'administra_documentos_venta_provincias') id_permiso, id_usuario,	ISNULL(administra_documentos_venta_provincias,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'crea_cotizaciones_provincias') id_permiso, id_usuario,	ISNULL(crea_cotizaciones_provincias,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'toma_pedidos_provincias') id_permiso, id_usuario,	ISNULL(toma_pedidos_provincias,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'programa_pedidos') id_permiso, id_usuario,	ISNULL(programa_pedidos,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'modifica_maestro_clientes') id_permiso, id_usuario,	ISNULL(modifica_maestro_clientes,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'modifica_maestro_productos') id_permiso, id_usuario,	ISNULL(modifica_maestro_productos,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'visualiza_documentos_venta') id_permiso, id_usuario,	ISNULL(visualiza_documentos_venta,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'visualiza_pedidos_lima') id_permiso, id_usuario,	ISNULL(visualiza_pedidos_lima,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'visualiza_guias_remision') id_permiso, id_usuario,	ISNULL(visualiza_guias_remision,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'visualiza_cotizaciones') id_permiso, id_usuario,	ISNULL(visualiza_cotizaciones,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'libera_pedidos') id_permiso, id_usuario,	ISNULL(libera_pedidos,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'bloquea_pedidos') id_permiso, id_usuario,	ISNULL(bloquea_pedidos,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'aprueba_anulaciones') id_permiso, id_usuario,	ISNULL(aprueba_anulaciones,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'crea_notas_credito') id_permiso, id_usuario,	ISNULL(crea_notas_credito,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'visualiza_pedidos_provincias') id_permiso, id_usuario,	ISNULL(visualiza_pedidos_provincias,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'visualiza_costos') id_permiso, id_usuario,	ISNULL(visualiza_costos,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'crea_notas_debito') id_permiso, id_usuario,	ISNULL(crea_notas_debito,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'realiza_refacturacion') id_permiso, id_usuario,	ISNULL(realiza_refacturacion,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'visualiza_margen') id_permiso, id_usuario,	ISNULL(visualiza_margen,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'confirma_stock') id_permiso, id_usuario,	ISNULL(confirma_stock,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'crea_cotizaciones_lima') id_permiso, id_usuario,	ISNULL(crea_cotizaciones_lima,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'toma_pedidos_lima') id_permiso, id_usuario,	ISNULL(toma_pedidos_lima,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'toma_pedidos_compra') id_permiso, id_usuario,	ISNULL(toma_pedidos_compra,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'toma_pedidos_almacen') id_permiso, id_usuario,	ISNULL(toma_pedidos_almacen,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'define_plazo_credito') id_permiso, id_usuario,	ISNULL(define_plazo_credito,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'define_monto_credito') id_permiso, id_usuario,	ISNULL(define_monto_credito,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'define_responsable_comercial') id_permiso, id_usuario,	ISNULL(define_responsable_comercial,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'define_supervisor_comercial') id_permiso, id_usuario,	ISNULL(define_supervisor_comercial,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'define_asistente_atencion_cliente') id_permiso, id_usuario,	ISNULL(define_asistente_atencion_cliente,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'define_responsable_portafolio') id_permiso, id_usuario,	ISNULL(define_responsable_portafolio,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'modifica_pedido_venta_fecha_entrega_hasta') id_permiso, id_usuario,	ISNULL(modifica_pedido_venta_fecha_entrega_hasta,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'bloquea_clientes') id_permiso, id_usuario,	ISNULL(bloquea_clientes,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'modifica_canales') id_permiso, id_usuario,	ISNULL(modifica_canales,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'realiza_carga_masiva_pedidos') id_permiso, id_usuario,	ISNULL(realiza_carga_masiva_pedidos,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'modifica_pedido_fecha_entrega_extendida') id_permiso, id_usuario,	ISNULL(modifica_pedido_fecha_entrega_extendida,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'crea_factura_consolidada_multiregional') id_permiso, id_usuario,	ISNULL(crea_factura_consolidada_multiregional,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'crea_factura_consolidada_local') id_permiso, id_usuario,	ISNULL(crea_factura_consolidada_local,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'visualiza_guias_pendientes_facturacion') id_permiso, id_usuario,	ISNULL(visualiza_guias_pendientes_facturacion,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'modifica_negociacion_multiregional') id_permiso, id_usuario,	ISNULL(modifica_negociacion_multiregional,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'busca_sedes_grupo_cliente') id_permiso, id_usuario,	ISNULL(busca_sedes_grupo_cliente,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'modifica_producto') id_permiso, id_usuario,	ISNULL(modifica_producto,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'aprueba_pedidos_compra') id_permiso, id_usuario,	ISNULL(aprueba_pedidos_compra,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'aprueba_pedidos_almacen') id_permiso, id_usuario,	ISNULL(aprueba_pedidos_almacen,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'crea_cotizaciones_grupo_cliente') id_permiso, id_usuario,	ISNULL(crea_cotizaciones_grupo_cliente,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'crea_cotizaciones_grupales') id_permiso, id_usuario,	ISNULL(crea_cotizaciones_grupales,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'aprueba_cotizaciones_grupales') id_permiso, id_usuario,	ISNULL(aprueba_cotizaciones_grupales,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'modifica_subdistribuidor') id_permiso, id_usuario,	ISNULL(modifica_subdistribuidor,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'modifica_origen') id_permiso, id_usuario,	ISNULL(modifica_origen,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'realiza_carga_masiva_productos') id_permiso, id_usuario,	ISNULL(realiza_carga_masiva_productos,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'realiza_carga_masiva_clientes') id_permiso, id_usuario,	ISNULL(realiza_carga_masiva_clientes,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'modifica_grupo_clientes') id_permiso, id_usuario,	ISNULL(modifica_grupo_clientes,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'visualiza_grupo_clientes') id_permiso, id_usuario,	ISNULL(visualiza_grupo_clientes,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'visualiza_productos') id_permiso, id_usuario,	ISNULL(visualiza_productos,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'visualiza_subdistribuidores') id_permiso, id_usuario,	ISNULL(visualiza_subdistribuidores,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'visualiza_origenes') id_permiso, id_usuario,	ISNULL(visualiza_origenes,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'visualiza_clientes') id_permiso, id_usuario,	ISNULL(visualiza_clientes,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;
INSERT INTO USUARIO_PERMISO SELECT * FROM (	SELECT (SELECT id_permiso FROM PERMISO WHERE descripcion_corta = 'elimina_cotizaciones_aceptadas') id_permiso, id_usuario,	ISNULL(elimina_cotizaciones_aceptadas,0) estado, null id_usuario_c, GETDATE() fecha_c , null id_usuario_m, GETDATE()  fecha_m FROM USUARIO WHERE estado = 1 ) pc where pc.estado = 1;


CREATE TABLE CATEGORIA_PERMISO
(
id_categoria_permiso int primary key,
descripcion varchar(30)
)

INSERT INTO CATEGORIA_PERMISO values (1,'Administración');
INSERT INTO CATEGORIA_PERMISO values (2,'Cotización');
INSERT INTO CATEGORIA_PERMISO values (3,'Pedido');
INSERT INTO CATEGORIA_PERMISO values (4,'Movimientos Almacen');
INSERT INTO CATEGORIA_PERMISO values (5,'Facturación');
INSERT INTO CATEGORIA_PERMISO values (6,'Cliente');
INSERT INTO CATEGORIA_PERMISO values (7,'Maestros');

ALTER TABLE PERMISO ADD id_categoria_permiso int
ALTER TABLE PERMISO ADD orden_permiso int

UPDATE PERMISO set id_categoria_permiso = 1, orden_permiso = 1 where codigo = 'P001'

UPDATE PERMISO set id_categoria_permiso = 2, orden_permiso = 	1	 where codigo ='P031';
UPDATE PERMISO set id_categoria_permiso = 2, orden_permiso = 	2	 where codigo ='P012';
UPDATE PERMISO set id_categoria_permiso = 2, orden_permiso = 	3	 where codigo ='P002';
UPDATE PERMISO set id_categoria_permiso = 2, orden_permiso = 	4	 where codigo ='P003';
UPDATE PERMISO set id_categoria_permiso = 2, orden_permiso = 	5	 where codigo ='P020';
UPDATE PERMISO set id_categoria_permiso = 2, orden_permiso = 	6	 where codigo ='P067';
UPDATE PERMISO set id_categoria_permiso = 2, orden_permiso = 	7	 where codigo ='P055';
UPDATE PERMISO set id_categoria_permiso = 2, orden_permiso = 	8	 where codigo ='P054';
UPDATE PERMISO set id_categoria_permiso = 2, orden_permiso = 	9	 where codigo ='P056';
UPDATE PERMISO set id_categoria_permiso = 2, orden_permiso = 	10	 where codigo ='P029';
UPDATE PERMISO set id_categoria_permiso = 2, orden_permiso = 	11	 where codigo ='P026';

UPDATE PERMISO set id_categoria_permiso = 3, orden_permiso = 	1	 where codigo = 'P032';
UPDATE PERMISO set id_categoria_permiso = 3, orden_permiso = 	2	 where codigo = 'P013';
UPDATE PERMISO set id_categoria_permiso = 3, orden_permiso = 	3	 where codigo = 'P004';
UPDATE PERMISO set id_categoria_permiso = 3, orden_permiso = 	4	 where codigo = 'P005';
UPDATE PERMISO set id_categoria_permiso = 3, orden_permiso = 	5	 where codigo = 'P018';
UPDATE PERMISO set id_categoria_permiso = 3, orden_permiso = 	6	 where codigo = 'P025';
UPDATE PERMISO set id_categoria_permiso = 3, orden_permiso = 	7	 where codigo = 'P014';
UPDATE PERMISO set id_categoria_permiso = 3, orden_permiso = 	8	 where codigo = 'P021';
UPDATE PERMISO set id_categoria_permiso = 3, orden_permiso = 	9	 where codigo = 'P022';
UPDATE PERMISO set id_categoria_permiso = 3, orden_permiso = 	10	 where codigo = 'P030';
UPDATE PERMISO set id_categoria_permiso = 3, orden_permiso = 	11	 where codigo = 'P041';
UPDATE PERMISO set id_categoria_permiso = 3, orden_permiso = 	12	 where codigo = 'P045';
UPDATE PERMISO set id_categoria_permiso = 3, orden_permiso = 	13	 where codigo = 'P044';
UPDATE PERMISO set id_categoria_permiso = 3, orden_permiso = 	14	 where codigo = 'P033';
UPDATE PERMISO set id_categoria_permiso = 3, orden_permiso = 	15	 where codigo = 'P052';
UPDATE PERMISO set id_categoria_permiso = 3, orden_permiso = 	16	 where codigo = 'P034';
UPDATE PERMISO set id_categoria_permiso = 3, orden_permiso = 	17	 where codigo = 'P053';

UPDATE PERMISO set id_categoria_permiso = 4, orden_permiso = 	1	 where codigo = 'P006';
UPDATE PERMISO set id_categoria_permiso = 4, orden_permiso = 	2	 where codigo = 'P007';
UPDATE PERMISO set id_categoria_permiso = 4, orden_permiso = 	3	 where codigo = 'P008';
UPDATE PERMISO set id_categoria_permiso = 4, orden_permiso = 	4	 where codigo = 'P019';
UPDATE PERMISO set id_categoria_permiso = 4, orden_permiso = 	5	 where codigo = 'P048';

UPDATE PERMISO set id_categoria_permiso = 5, orden_permiso = 	1	 where codigo = 'P009';
UPDATE PERMISO set id_categoria_permiso = 5, orden_permiso = 	2	 where codigo = 'P024';
UPDATE PERMISO set id_categoria_permiso = 5, orden_permiso = 	3	 where codigo = 'P027';
UPDATE PERMISO set id_categoria_permiso = 5, orden_permiso = 	4	 where codigo = 'P046';
UPDATE PERMISO set id_categoria_permiso = 5, orden_permiso = 	5	 where codigo = 'P047';
UPDATE PERMISO set id_categoria_permiso = 5, orden_permiso = 	6	 where codigo = 'P010';
UPDATE PERMISO set id_categoria_permiso = 5, orden_permiso = 	7	 where codigo = 'P011';
UPDATE PERMISO set id_categoria_permiso = 5, orden_permiso = 	8	 where codigo = 'P017';
UPDATE PERMISO set id_categoria_permiso = 5, orden_permiso = 	9	 where codigo = 'P023';
UPDATE PERMISO set id_categoria_permiso = 5, orden_permiso = 	10	 where codigo = 'P028';

UPDATE PERMISO set id_categoria_permiso = 6, orden_permiso = 	1	 where codigo = 'P015';
UPDATE PERMISO set id_categoria_permiso = 6, orden_permiso = 	2	 where codigo = 'P060';
UPDATE PERMISO set id_categoria_permiso = 6, orden_permiso = 	3	 where codigo = 'P066';
UPDATE PERMISO set id_categoria_permiso = 6, orden_permiso = 	4	 where codigo = 'P035';
UPDATE PERMISO set id_categoria_permiso = 6, orden_permiso = 	5	 where codigo = 'P036';
UPDATE PERMISO set id_categoria_permiso = 6, orden_permiso = 	6	 where codigo = 'P037';
UPDATE PERMISO set id_categoria_permiso = 6, orden_permiso = 	7	 where codigo = 'P038';
UPDATE PERMISO set id_categoria_permiso = 6, orden_permiso = 	8	 where codigo = 'P039';
UPDATE PERMISO set id_categoria_permiso = 6, orden_permiso = 	9	 where codigo = 'P040';
UPDATE PERMISO set id_categoria_permiso = 6, orden_permiso = 	10	 where codigo = 'P042';
UPDATE PERMISO set id_categoria_permiso = 6, orden_permiso = 	11	 where codigo = 'P043';
UPDATE PERMISO set id_categoria_permiso = 6, orden_permiso = 	12	 where codigo = 'P049';
UPDATE PERMISO set id_categoria_permiso = 6, orden_permiso = 	13	 where codigo = 'P050';
UPDATE PERMISO set id_categoria_permiso = 6, orden_permiso = 	14	 where codigo = 'P061';
UPDATE PERMISO set id_categoria_permiso = 6, orden_permiso = 	15	 where codigo = 'P062';

UPDATE PERMISO set id_categoria_permiso = 7, orden_permiso = 	1	 where codigo = 'P051';
UPDATE PERMISO set id_categoria_permiso = 7, orden_permiso = 	2	 where codigo = 'P016';
UPDATE PERMISO set id_categoria_permiso = 7, orden_permiso = 	3	 where codigo = 'P059';
UPDATE PERMISO set id_categoria_permiso = 7, orden_permiso = 	4	 where codigo = 'P063';
UPDATE PERMISO set id_categoria_permiso = 7, orden_permiso = 	5	 where codigo = 'P058';
UPDATE PERMISO set id_categoria_permiso = 7, orden_permiso = 	6	 where codigo = 'P065';
UPDATE PERMISO set id_categoria_permiso = 7, orden_permiso = 	7	 where codigo = 'P057';
UPDATE PERMISO set id_categoria_permiso = 7, orden_permiso = 	8	 where codigo = 'P064';






ALTER PROCEDURE [dbo].[ps_permisos]
AS
BEGIN
	SELECT 
	pe.id_permiso
    ,pe.codigo
    ,pe.descripcion_corta
    ,pe.descripcion_larga
	,pe.orden_permiso
	,pe.id_categoria_permiso
	,cp.descripcion descripcion_categoria
	FROM PERMISO pe
	INNER JOIN CATEGORIA_PERMISO cp
	ON pe.id_categoria_permiso = cp.id_categoria_permiso
	WHERE pe.estado = 1
	ORDER BY pe.id_categoria_permiso, pe.orden_permiso;
END



/****** Object:  StoredProcedure [dbo].[ps_usuario]    Script Date: 4/03/2019 8:51:04 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ps_usuario] 
@email varchar(50),
@password varchar(50)
AS
BEGIN

--DECLARE @aprueba_pedidos_lima smallint; 
--DECLARE @aprueba_pedidos smallint; 
DECLARE @id_usuario uniqueidentifier; 


SELECT @id_usuario = id_usuario FROM USUARIO 
WHERE estado = 1
AND email = @email 
AND PWDCOMPARE ( @password,password )  = 1 ;

--USUARIO
SELECT id_usuario, cargo, nombre , contacto, es_cliente, 
--Cotizaciones
crea_cotizaciones_lima as crea_cotizaciones, aprueba_cotizaciones_lima, aprueba_cotizaciones_provincias,
maximo_porcentaje_descuento_aprobacion, cotizacion_serializada,
visualiza_cotizaciones,
--Pedidos
toma_pedidos_lima as toma_pedidos,  aprueba_pedidos_lima, aprueba_pedidos_provincias, pedido_serializado,
visualiza_pedidos_lima,
visualiza_pedidos_provincias, libera_pedidos, bloquea_pedidos,
visualiza_costos,
visualiza_margen,
confirma_stock,
--Guias
crea_guias, cast(administra_guias_lima as int) as administra_guias_lima,cast (administra_guias_provincias as int) as administra_guias_provincias,
visualiza_guias_remision,
--Documentos Venta


crea_documentos_venta, 
crea_factura_consolidada_local,
crea_factura_consolidada_multiregional,
visualiza_guias_pendientes_facturacion,
administra_documentos_venta_lima, administra_documentos_venta_provincias,
visualiza_documentos_venta,
--Sede
id_ciudad,
crea_cotizaciones_provincias,
aprueba_pedidos_lima,
aprueba_pedidos_provincias,
toma_pedidos_provincias,
programa_pedidos,
modifica_maestro_clientes,
modifica_maestro_productos,
aprueba_anulaciones,
crea_notas_credito, 
crea_notas_debito, 
realiza_refacturacion,
toma_pedidos_compra,
toma_pedidos_almacen,
define_plazo_credito,
define_monto_credito,
define_responsable_comercial,
define_supervisor_comercial,
define_asistente_atencion_cliente,
define_responsable_portafolio,
modifica_pedido_venta_fecha_entrega_hasta,
realiza_carga_masiva_pedidos,
modifica_pedido_fecha_entrega_extendida,
bloquea_clientes,
modifica_negociacion_multiregional,
busca_sedes_grupo_cliente,
modifica_canales,
modifica_subdistribuidor,
modifica_origen,
modifica_producto,
crea_cotizaciones_grupales,
aprueba_cotizaciones_grupales,
modifica_grupo_clientes,
visualiza_grupo_clientes, 
visualiza_clientes, 
visualiza_productos, 
visualiza_subdistribuidores, 
visualiza_origenes,
realiza_carga_masiva_clientes, 
realiza_carga_masiva_productos,
elimina_cotizaciones_aceptadas

FROM USUARIO 
WHERE estado = 1
AND email = @email 
AND PWDCOMPARE ( @password,password )  = 1 ;

--PARAMETROS POR USUARIO
SELECT codigo, valor FROM PARAMETRO where estado = 1
UNION 
SELECT 'CPE_CABECERA_BE_ID' as codigo, CPE_CABECERA_BE_ID as valor FROM PARAMETROS_AMBIENTE_EOL
UNION 
SELECT 'CPE_CABECERA_BE_COD_GPO' as codigo, CPE_CABECERA_BE_COD_GPO as valor FROM PARAMETROS_AMBIENTE_EOL;


--USUARIOS A CARGO COTIZACION
IF (@id_usuario IS NOT NULL)
BEGIN
	SELECT id_usuario,  us.nombre, es_provincia
	FROM USUARIO us INNER JOIN 
	CIUDAD ci ON us.id_ciudad = ci.id_ciudad
	WHERE us.estado = 1 AND us.crea_cotizaciones_lima = 1
	AND id_usuario != @id_usuario ;
END

--USUARIOS A CARGO PEDIDO
IF (@id_usuario IS NOT NULL)
BEGIN
	SELECT id_usuario, us.nombre, es_provincia
	FROM USUARIO us INNER JOIN 
	CIUDAD ci ON us.id_ciudad = ci.id_ciudad
	WHERE us.estado = 1 AND us.toma_pedidos_lima = 1
	AND us.id_usuario NOT IN ('412ACDEE-FE20-4539-807C-D00CD71359D6')
	AND (us.toma_pedidos_lima = 1 OR us.toma_pedidos_provincias = 1)
	AND id_usuario != @id_usuario ;
END


--USUARIOS A CARGO GUIAS
IF (@id_usuario IS NOT NULL)
BEGIN
	SELECT id_usuario, us.nombre, es_provincia
	FROM USUARIO us INNER JOIN 
	CIUDAD ci ON us.id_ciudad = ci.id_ciudad
	WHERE us.estado = 1 AND us.crea_guias = 1
	AND id_usuario != @id_usuario ;
END

--USUARIOS A CARGO DOCUMENTOS VENTA
IF (@id_usuario IS NOT NULL)
BEGIN
	SELECT id_usuario, us.nombre, es_provincia
	FROM USUARIO us INNER JOIN 
	CIUDAD ci ON us.id_ciudad = ci.id_ciudad
	WHERE us.estado = 1 AND us.crea_documentos_venta = 1
	AND id_usuario != @id_usuario ;
END

IF (@id_usuario IS NOT NULL)
BEGIN
	SELECT cl.id_cliente, cl.razon_social, 
	cl.codigo, cl.ruc, cl.nombre_comercial 
	FROM CLIENTE cl
	INNER JOIN USUARIO_CLIENTE uc	ON cl.ruc = uc.ruc
	INNER JOIN CIUDAD ci ON  cl.id_ciudad = ci.id_ciudad
	WHERE uc.id_usuario = @id_usuario 
	AND ci.es_provincia = 0 ;

	SELECT id_vendedor, codigo, 
	descripcion,
	es_responsable_comercial, es_asistente_servicio_cliente, 
	es_responsable_portafolio, es_supervisor_comercial,
	id_usuario--, us.nombre 
	FROM VENDEDOR ve
--	LEFT JOIN USUARIO us ON ve.id_usuario = us.id_usuario
	WHERE estado = 1;
END

SELECT id_producto, sku, descripcion FROM PRODUCTO 
WHERE tipo = 5 --(5 descuento)

SELECT id_producto, sku, descripcion FROM PRODUCTO 
WHERE tipo = 4 --(4 cargos)

SELECT pe.id_permiso, pe.codigo,pe.descripcion_corta, pe.descripcion_larga FROM USUARIO_PERMISO up
INNER JOIN PERMISO pe ON up.id_permiso = pe.id_permiso
where id_usuario = @id_usuario


END
