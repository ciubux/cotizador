DROP TABLE DOMICILIO_LEGAL
DROP TABLE CLIENTE_SUNAT
DROP SEQUENCE SEQ_ID_CLIENTE_SUNAT
DROP SEQUENCE SEQ_ID_DOMICILIO_LEGAL

ALTER TABLE CLIENTE ADD id_cliente_sunat int
ALTER TABLE DIRECCION_ENTREGA ADD id_domicilio_legal int 


CREATE TABLE DOMICILIO_LEGAL 
(
	id_domicilio_legal int primary key,
	estado bit, 
	usuario_creacion uniqueIdentifier, 
	fecha_creacion datetime, 
	usuario_modificacion uniqueIdentifier, 
	fecha_modificacion datetime, 
	id_cliente_sunat int not null,
	codigo CHAR(4),
	tipo_establecimiento VARCHAR(100),
	direccion VARCHAR(120) NOT NULL,
	ubigeo CHAR(6),
	es_establecimiento_anexo bit
)



CREATE TABLE CLIENTE_SUNAT
(
	id_cliente_sunat int primary key,
	estado bit,
	usuario_creacion uniqueIdentifier, 
	fecha_creacion datetime, 
	usuario_modificacion uniqueIdentifier, 
	fecha_modificacion datetime, 
	ruc varchar(11),
	razon_social varchar(300),
	nombre_comercial varchar(300),
	estado_contribuyente varchar(50),
	condicion_contribuyente varchar(50),
	correo_envio_factura varchar(100),
	id_domicilio_legal int	
)

CREATE SEQUENCE SEQ_ID_CLIENTE_SUNAT AS INT START WITH 1 INCREMENT BY 1
CREATE SEQUENCE SEQ_ID_DOMICILIO_LEGAL AS INT START WITH 1 INCREMENT BY 1

CREATE UNIQUE INDEX IDX_CLIENTE_SUNAT_RUC ON CLIENTE_SUNAT (RUC)

-- CLIENTE SUNAT 

INSERT INTO CLIENTE_SUNAT (id_cliente_sunat, ruc,estado, fecha_creacion, fecha_modificacion,
	razon_social,
	nombre_comercial,
	estado_contribuyente,
	condicion_contribuyente)
	SELECT (NEXT VALUE FOR SEQ_ID_CLIENTE_SUNAT), ruc, 1, fecha_creacion, fecha_modificacion, 
	  razon_social_sunat, nombre_comercial_sunat, -- direccion_domicilio_legal_sunat,
estado_contribuyente_sunat, condicion_contribuyente_sunat  FROM (
SELECT ruc,  fecha_creacion, fecha_modificacion, razon_social_sunat, nombre_comercial_sunat, -- direccion_domicilio_legal_sunat,
estado_contribuyente_sunat, condicion_contribuyente_sunat, 
  row_number() over (partition by ruc order by fecha_modificacion desc)
    as OrderNumberForThisCustomer
FROM CLIENTE WHERE RUC IN  
(
	SELECT ruc FROM CLIENTE where estado > 0 AND tipo_documento = 6 AND ruc IS NOT NULL
	GROUP BY ruc  
	HAVING COUNT(*) > 1
) AND razon_social_sunat IS NOT NULL AND razon_social_sunat != ''
) aa WHERE aa.OrderNumberForThisCustomer  = 1


INSERT INTO CLIENTE_SUNAT (id_cliente_sunat, ruc,estado, fecha_creacion, fecha_modificacion,
	razon_social,
	nombre_comercial,
	estado_contribuyente,
	condicion_contribuyente)
	SELECT (NEXT VALUE FOR SEQ_ID_CLIENTE_SUNAT), ruc, 1, fecha_creacion, fecha_modificacion, 
	  razon_social_sunat, nombre_comercial_sunat, -- direccion_domicilio_legal_sunat,
estado_contribuyente_sunat, condicion_contribuyente_sunat  FROM (
SELECT ruc,fecha_creacion, fecha_modificacion,  razon_social_sunat, nombre_comercial_sunat, -- direccion_domicilio_legal_sunat,
estado_contribuyente_sunat, condicion_contribuyente_sunat, 
  row_number() over (partition by ruc order by fecha_modificacion desc)
    as OrderNumberForThisCustomer
FROM CLIENTE WHERE RUC IN  
(
	SELECT ruc FROM CLIENTE where estado > 0 AND tipo_documento = 6 AND ruc IS NOT NULL
	GROUP BY ruc  
	HAVING COUNT(*) = 1
) AND razon_social_sunat IS NOT NULL AND razon_social_sunat != ''
) aa WHERE aa.OrderNumberForThisCustomer  = 1


UPDATE CLIENTE_SUNAT SET estado_contribuyente = NULL where ruc IN ('10026051628',
'10158650393','10181382941','10267094123','10431787984')

-- DOMICILIO LEGAL


INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, direccion, ubigeo, 
	es_establecimiento_anexo,
	estado, fecha_creacion, fecha_modificacion
	)
	SELECT (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL), (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE ruc = aa.ruc), 
	'0000', direccion_domicilio_legal_sunat,	ubigeo, 0,	
	1, fecha_creacion, fecha_modificacion
 FROM (
SELECT ruc, 
direccion_domicilio_legal_sunat, ubigeo, 
 fecha_creacion, fecha_modificacion,
  row_number() over (partition by ruc order by fecha_modificacion desc)
    as OrderNumberForThisCustomer
FROM CLIENTE WHERE RUC IN  
(
	SELECT ruc FROM CLIENTE where estado > 0 AND tipo_documento = 6 AND ruc IS NOT NULL
	GROUP BY ruc  
	HAVING COUNT(*) > 1
) AND razon_social_sunat IS NOT NULL AND razon_social_sunat != ''
) aa WHERE aa.OrderNumberForThisCustomer  = 1



INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, direccion, ubigeo, 
	es_establecimiento_anexo,
	estado, fecha_creacion, fecha_modificacion
	)
	SELECT (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL), (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE ruc = aa.ruc), 
	'0000', direccion_domicilio_legal_sunat,	ubigeo, 0,	
	1, fecha_creacion, fecha_modificacion
 FROM (
SELECT ruc, 
direccion_domicilio_legal_sunat, ubigeo, 
 fecha_creacion, fecha_modificacion,
  row_number() over (partition by ruc order by fecha_modificacion desc)
    as OrderNumberForThisCustomer
FROM CLIENTE WHERE RUC IN  
(
	SELECT ruc FROM CLIENTE where estado > 0 AND tipo_documento = 6 AND ruc IS NOT NULL
	GROUP BY ruc  
	HAVING COUNT(*) = 1
) AND razon_social_sunat IS NOT NULL AND razon_social_sunat != ''
) aa WHERE aa.OrderNumberForThisCustomer  = 1


select CONCAT('UPDATE CLIENTE_SUNAT SET id_domicilio_legal = ',id_domicilio_legal, ' WHERE id_cliente_sunat = ', id_cliente_sunat)   FROM DOMICILIO_LEGAL dl 


select CONCAT('UPDATE CLIENTE SET id_cliente_sunat = ',id_cliente_sunat, ' WHERE ruc = ''', ruc,'''')   FROM CLIENTE_SUNAT  


----UPDATE DOMICILIO_LEGAL PRINCIPAL EN CLIENTE_SUNAT
/*


SELECT * FROM CLIENTE WHERE id_cliente_sunat*/

--SELECT * FROM CLIENTE where tipo_documento = 6 AND (id_cliente_sunat = 0 OR id_cliente_sunat IS NULL)

/*
SELECT * FROM CLIENTE where ruc = '20505670443'
SELECT * FROM CLIENTE_SUNAT where ruc = '20505670443'
SELECT * FROM DOMICILIO_LEGAL where id_domicilio_legal = '301'*/
--ID_CLIENTE SUNAT 187 PARA CASA ANDINA

/*
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),
1, 1,GETDATE(), GETDATE())*/


INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0001','LO. L. COMERCIAL','CAL.PORTAL ESPINAR NRO. 142 CUSCO - CUSCO - CUSCO',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'CUSCO' AND PROVINCIA = 'CUSCO' AND DISTRITO = 'CUSCO'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0008','LO. L. COMERCIAL','CAL.SAN AGUSTIN KM. 371 CUSCO - CUSCO - CUSCO',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'CUSCO' AND PROVINCIA = 'CUSCO' AND DISTRITO = 'CUSCO'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0011','LO. L. COMERCIAL','NRO. S/N ISLA SUASI (EN EL LAGO TITIKAKA) PUNO - MOHO - CONIMA',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'PUNO' AND PROVINCIA = 'MOHO' AND DISTRITO = 'CONIMA'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0002','LO. L. COMERCIAL','CAL.JERUSALEN NRO. 603 AREQUIPA - AREQUIPA - AREQUIPA',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'AREQUIPA' AND PROVINCIA = 'AREQUIPA' AND DISTRITO = 'AREQUIPA'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0010','LO. L. COMERCIAL','CAL.SANTA CATALINA ANGOSTA NRO. 149 CUSCO - CUSCO - CUSCO',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'CUSCO' AND PROVINCIA = 'CUSCO' AND DISTRITO = 'CUSCO'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0009','LO. L. COMERCIAL','----5TO PARADERO NRO. SN YANAHUARA (VALLE SAGRADO) CUSCO - URUBAMBA - URUBAMBA',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'CUSCO' AND PROVINCIA = 'URUBAMBA' AND DISTRITO = 'URUBAMBA'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0029','LO. L. COMERCIAL','MZA. B LOTE. 10 URB. LAS FLORES DEL GOLF III LA LIBERTAD - TRUJILLO - VICTOR LARCO HERRERA',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'LA LIBERTAD' AND PROVINCIA = 'TRUJILLO' AND DISTRITO = 'VICTOR LARCO HERRERA'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0024','LO. L. COMERCIAL','CAL.SCHELL NRO. 452 URB. MIRAFLORES LIMA - LIMA - MIRAFLORES',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'LIMA' AND PROVINCIA = 'LIMA' AND DISTRITO = 'MIRAFLORES'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0003','LO. L. COMERCIAL','AV. 28 DE JULIO NRO. 1088 URB. SAN ANTONIO LIMA - LIMA - MIRAFLORES',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'LIMA' AND PROVINCIA = 'LIMA' AND DISTRITO = 'MIRAFLORES'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0007','LO. L. COMERCIAL','JR. INDEPENDENCIA NRO. 143 INT. A PUNO - PUNO - PUNO',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'PUNO' AND PROVINCIA = 'PUNO' AND DISTRITO = 'PUNO'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0017','LO. L. COMERCIAL','AV. LA PAZ NRO. 451 LIMA - LIMA - MIRAFLORES',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'LIMA' AND PROVINCIA = 'LIMA' AND DISTRITO = 'MIRAFLORES'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0020','LO. L. COMERCIAL','CAR.PANAMERICANA SUR NRO. SN (ESQ. CARR. PANAM. Y CARRETERA A SUMAMPE) ICA - CHINCHA - SUNAMPE',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'ICA' AND PROVINCIA = 'CHINCHA' AND DISTRITO = 'SUNAMPE'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0018','DE. DEPOSITO','AV. GARCILAZO NRO. 316 CUSCO - CUSCO - WANCHAQ',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'CUSCO' AND PROVINCIA = 'CUSCO' AND DISTRITO = 'WANCHAQ'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0027','LO. L. COMERCIAL','CAR.PANAMERICA NORTE KM. 1232 LOTE. D1- (LOTE D1 Y C10) TUMBES - CONTRALMIRANTE VILLAR - ZORRITOS',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'TUMBES' AND PROVINCIA = 'CONTRALMIRANTE VILLAR' AND DISTRITO = 'ZORRITOS'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0014','LO. L. COMERCIAL','AV. SESQUICENTENARIO NRO. 1970 HUAJJE PUNO - PUNO - PUNO',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'PUNO' AND PROVINCIA = 'PUNO' AND DISTRITO = 'PUNO'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0023','LO. L. COMERCIAL','----CAS. ACHAMAQUI NRO. S N CRUCE ACHAMAQUI (KM.39 DE LA CARRETERA PEDRO RUIZ) AMAZONAS - CHACHAPOYAS - CHACHAPOYAS',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'AMAZONAS' AND PROVINCIA = 'CHACHAPOYAS' AND DISTRITO = 'CHACHAPOYAS'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0016','LO. L. COMERCIAL','CAL.UGARTE NRO. 403 AREQUIPA - AREQUIPA - AREQUIPA',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'AREQUIPA' AND PROVINCIA = 'AREQUIPA' AND DISTRITO = 'AREQUIPA'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0028','LO. L. COMERCIAL','AV. IMPERIO DE LOS INCAS NRO. E-34 CUSCO - URUBAMBA - MACHUPICCHU',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'CUSCO' AND PROVINCIA = 'URUBAMBA' AND DISTRITO = 'MACHUPICCHU'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0030','LO. L. COMERCIAL','CAL.BILLINGHURST NRO. 170 TACNA - TACNA - TACNA',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'TACNA' AND PROVINCIA = 'TACNA' AND DISTRITO = 'TACNA'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0004','LO. L. COMERCIAL','CAL.HUAYNA CAPAC MZA. E2 LOTE. 01 VALLE DEL COLCA AREQUIPA - CAYLLOMA - CHIVAY',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'AREQUIPA' AND PROVINCIA = 'CAYLLOMA' AND DISTRITO = 'CHIVAY'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0022','LO. L. COMERCIAL','CAL.FEDERICO VILLAREAL NRO. 115 P.J. JOSE OLAYA LAMBAYEQUE - CHICLAYO - CHICLAYO',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'LAMBAYEQUE' AND PROVINCIA = 'CHICLAYO' AND DISTRITO = 'CHICLAYO'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0019','OF. OF.ADMINIST.','PZA.PLAZOLETA LIMACPAMPA CHIC NRO. 485 CUSCO - CUSCO - CUSCO',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'CUSCO' AND PROVINCIA = 'CUSCO' AND DISTRITO = 'CUSCO'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0025','LO. L. COMERCIAL','AV. VELASCO ASTETE NRO. S N AEROPUERTO VELASCO ASTETE CUSCO - CUSCO - WANCHAQ',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'CUSCO' AND PROVINCIA = 'CUSCO' AND DISTRITO = 'WANCHAQ'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0015','LO. L. COMERCIAL','AV. PETIT THOUARS NRO. 5444 LIMA - LIMA - MIRAFLORES',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'LIMA' AND PROVINCIA = 'LIMA' AND DISTRITO = 'MIRAFLORES'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0013','LO. L. COMERCIAL','----LIMACPAMPA CHICO NRO. 473 URB. CENTRO HISTORICO CUSCO - CUSCO - CUSCO',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'CUSCO' AND PROVINCIA = 'CUSCO' AND DISTRITO = 'CUSCO'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0021','LO. L. COMERCIAL','CAL.UGARTE NRO. 400 (CERCADO DE AREQUIPA) AREQUIPA - AREQUIPA - AREQUIPA',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'AREQUIPA' AND PROVINCIA = 'AREQUIPA' AND DISTRITO = 'AREQUIPA'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0026','DE. DEPOSITO','----PASEO ZARZUELA LOTE. K-3 URB. HUANCARO CUSCO - CUSCO - SANTIAGO',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'CUSCO' AND PROVINCIA = 'CUSCO' AND DISTRITO = 'SANTIAGO'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0012','LO. L. COMERCIAL','CAL.CHIHUAMPATA NRO. 278 URB. SAN BLAS CUSCO - CUSCO - CUSCO',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'CUSCO' AND PROVINCIA = 'CUSCO' AND DISTRITO = 'CUSCO'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0031','LO. L. COMERCIAL','----P. DE FLORES NRO. 104 (NRO : 104 - 116) AREQUIPA - AREQUIPA - AREQUIPA',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'AREQUIPA' AND PROVINCIA = 'AREQUIPA' AND DISTRITO = 'AREQUIPA'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0033','LO. L. COMERCIAL','AV. BOLOGNESI 143 NRO. 145 PIURA - TALARA - PARIÑAS',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'PIURA' AND PROVINCIA = 'TALARA' AND DISTRITO = 'PARIÑAS'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0034','LO. L. COMERCIAL','CAL.BILLINGHURST NRO. 170 TACNA - TACNA - TACNA',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'TACNA' AND PROVINCIA = 'TACNA' AND DISTRITO = 'TACNA'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0035','LO. L. COMERCIAL','NRO. 1963 APV. YARAVICO (LOTE 16A) MOQUEGUA - MARISCAL NIETO - MOQUEGUA',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'MOQUEGUA' AND PROVINCIA = 'MARISCAL NIETO' AND DISTRITO = 'MOQUEGUA'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0032','LO. L. COMERCIAL','AV. RAMON MUJICA NRO. 152 URB. EL CHIPE PIURA - PIURA - PIURA',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'PIURA' AND PROVINCIA = 'PIURA' AND DISTRITO = 'PIURA'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0036','LO. L. COMERCIAL','CAL.DIEGO DE ALMAGRO NRO. 586 TRUJILLO LA LIBERTAD - TRUJILLO - TRUJILLO',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'LA LIBERTAD' AND PROVINCIA = 'TRUJILLO' AND DISTRITO = 'TRUJILLO'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0037','LO. L. COMERCIAL','JR. AYACUCHO NRO. 372 TRUJILLO LA LIBERTAD - TRUJILLO - TRUJILLO',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'LA LIBERTAD' AND PROVINCIA = 'TRUJILLO' AND DISTRITO = 'TRUJILLO'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0038','LO. L. COMERCIAL','JR. INDEPENDENCIA NRO. 543 TRUJILLO (N 543-547) LA LIBERTAD - TRUJILLO - TRUJILLO',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'LA LIBERTAD' AND PROVINCIA = 'TRUJILLO' AND DISTRITO = 'TRUJILLO'),1, 1,GETDATE(), GETDATE());
INSERT INTO DOMICILIO_LEGAL (id_domicilio_legal, id_cliente_sunat, codigo, tipo_establecimiento, direccion, ubigeo, es_establecimiento_anexo, estado, fecha_creacion, fecha_modificacion) VALUES ( (NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL),(SELECT id_cliente_sunat FROM cliente_sunat WHERE ruc = '20505670443'),'0006','LO. L. COMERCIAL','CAL.BOLOGNESI NRO. 367 ICA - NASCA - NASCA',(SELECT codigo  FROM UBIGEO WHERE DEPARTAMENTO = 'ICA' AND PROVINCIA = 'NASCA' AND DISTRITO = 'NASCA'),1, 1,GETDATE(), GETDATE());

UPDATE DIRECCION_ENTREGA SET estado = 0 
WHERE id_cliente IN (SELECT id_cliente FROM CLIENTE where ruc = '20505670443')


--SELECT * FROM DOMICILIO_LEGAL  where id_cliente_sunat = 187
UPDATE DIRECCION_ENTREGA SET estado = 0 where id_direccion_entrega IN 
(SELECT id_direccion_entrega FROM DIRECCION_ENTREGA de INNER JOIN 
CLIENTE cl  ON de.id_cliente = cl.id_cliente
 where id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')
 AND cl.estado = 1 AND de.estado = 1 )

 SELECT * FROM DIRECCION_ENTREGA de INNER JOIN 
CLIENTE cl  ON de.id_cliente = cl.id_cliente
 where id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')
 AND cl.estado = 1 AND de.estado = 1 

 --SELECT TOp 5 * FROM DIRECCION_ENTREGA 
 ALTER TABLE DIRECCION_ENTREGA ADD correo VARCHAR(100)


INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'A') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0002' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Calle Jerusalén 603','Gerente: Rodrigo Vargas Supervisor: Sup: Edith Cardenas','984365455','Casa Andina Standard Arequipa',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0002' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'rvargas@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'A') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0004' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Huayna Capac s/n Chivay','Gerente: Javier Tejeda ','989295416','Casa Andina Standard Colca',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0004' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'jtejeda@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'L') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0020' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Panamericana Sur 197.5','Gerente: Pier Spigno Supervisor: Robert Medina','989339170','Casa Andina Standard Chincha ',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0020' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'pspigno@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'Q') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0010' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Sta. Catalina Angosta 149  ','Gerente: LuchoEspejo ','984108024','Casa Andina Standard Catedral',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0010' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'lespejo@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'Q') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0001' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Portal Espinar 142','Gerente: Gian Franco  ','984108027','Casa Andina Standard Cusco Plaza',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0001' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'jfranco@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'Q') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0008' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Calle San Agustín 371','Gerente: Adriana Valer (Gte. Provisonal) ','984108024','Casa Andina Standard Koricancha',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0008' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'avaler@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'Q') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0012' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Calle. Chihuampata 278-San Blas','Gerente: Rocio Garcia ','984108026','Casa Andina Standard San Blas',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0012' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'rgarcia@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'Q') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0028' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Prolongación imperio de los incas, mz 5,lote 32 – 34, Aguas Calientes','Gerente: Ian Cabrera Nin  Supervisor: Camilo Fernandez','986290487','Casa Andina Standard Mapi',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0028' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'icabrera@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'L') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0003' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Av. 28 de Julio 1088 Miraflores','Gerente: Marcela Solis ','984365455','Casa Andina Standard San Antonio',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0003' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'msolis@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'L') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0015' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Av. Petit Thouars 5444','Gerente: Oliver Barrio ','984260098','Casa Andina Standard Centro',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0015' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'obarrios@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'L') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0006' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Jr. Bolognesi 367','Gerente: Lizbeth Huallpa ','949078425','Casa Andina Standard Nasca',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0006' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'lhuallpa@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'P') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0032' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Av. Ramón Mujica S/N Urb. San Eduardo, El Chipe','Gerente: Javier macedo  Supervisor: Claribel Samaniego / Jessica Huertas','989184246','Casa Andina Standard Piura',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0032' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'jmacedo@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'P') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0033' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Av. Bolognesi Nro. 143 – 145','Gerente: Alex Garcia Supervisor: Maria Fiestas','952362855','Casa Andina Standard TALARA',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0033' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'agarcia@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'A') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0007' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Jr. Independencia 143','Gerente: Fabricio Carrion ','984108023','Casa Andina Standard Tikarani ',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0007' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'fcarrion@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'T') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0036' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Jr. Diego de Almagro 586','Gerente: Freddy Vicuña Supervisor: Gaspar Liñan','952362005','Casa Andina Standard Trujillo ',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0036' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'fvicuña@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'A') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0030' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Calle Billingurst Nro. 170','Gerente: Henri Belligan Supervisor: Jose Agurto','945472328','Casa  Andina Select Tacna',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0030' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'hbelligand@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'A') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0035' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Av. Manuel de la Torre s/n , Mariscal Nieto','Gerente: Diana Benavides Supervisor: Rocio Vilchez ','982059203','Casa  Andina Select Moquegua',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0035' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'dbenavides@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'A') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0031' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Portal DE  Flores 116, distrito de Arequipa-  Cercado','Gerente: Alvaro Cohello Supervisor: Maria Alejandra','989043458','Casa  Andina Select Arequipa',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0031' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'acoello@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'L') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0024' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Av. Shell 452 Miraflores','Gerente: Fernando Miranda/ Jefa Alojamiento: Katia Rivera/ Supervisoras: Karina Ruesta y Emilia Rios','958477288','Casa  Andina Select Miraflores',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0024' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'fmiranda@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'C') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0022' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Av Federico Villarreal 115 Chiclayo','Gerente: Jorge Marroquin Supervisor: Esmeralda Tejada  y  Edgar Cumpa','987522724','Casa  Andina Select Chiclayo           ',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0022' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'jmarroquin@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'P') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0027' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Carretera Panamericana Norte Km 1232 Bocapan- Zorritos ','Gerente: Alejandro Lazo Supervisor: Grace Alamo','913007079','Casa  Andina Select TUMBES',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0027' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'alazo@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'A') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0016' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Calle Ugarte 403, Cercado','Gerente: Cesar Diaz Supervisor: Graciela Mamani','986845370','Casa  Andina Premium Arequipa',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0016' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'cdiaz@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'Q') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0009' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'5to Paradero Yanahuara  - Urubamba ','Gerente: Silvana Villavicencio Supervisor: Lucy Jimenez','984770279','Casa  Andina Premium Valle',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0009' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'svillavicencio@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'Q') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0013' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Plazoleta Limacpampa Chico 473 ','Gerente: Jimmy Robles Supervisor: Adriana valer','965302081','Casa  Andina Premium Cusco',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0013' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'jrobles@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'A') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0014' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Av. Sesquicentenario 1970','Gerente: Wagner Arrieta Supervisor: Marleni','940230513','Casa  Andina Premium Puno',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0014' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'warrieta@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'T') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0029' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Av. El Golf 591, Urb. La Flores - Trujillo','Gerente: Miguel Trujillo/ Jefe de Housekeeping: Martha Encomenderos','962387481','Casa  Andina Premium Trujillo',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0029' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'mtrujillo@casa-andina.com');
INSERT INTO DIRECCION_ENTREGA (id_direccion_entrega, id_cliente, ubigeo, descripcion, contacto, telefono,nombre,  fecha_creacion,  fecha_modificacion, estado, id_domicilio_legal, correo) VALUES (NEWID(),(SELECT id_cliente FROM CLIENTE WHERE id_ciudad = (SELECT id_ciudad FROM CIUDAD WHERE codigo_sede = 'L') AND ruc = '20505670443' ),(SELECT ubigeo FROM DOMICILIO_LEGAL WHERE codigo = '0000' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'Av. La Paz 463 Miraflores','Gerente: Bruno del Castillo/ Jefe Housekeeping: Lourdes Franco','989120363','Casa  Andina Premium Miraflores',GETDATE(),GETDATE(),1,(SELECT id_domicilio_legal FROM DOMICILIO_LEGAL WHERE codigo = '0000' AND id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')),'bdelcastillo@casa-andina.com');




SELECT de.nombre as [Sede CLiente], ci.nombre [sede MP], cl.codigo [CLIENTE], 
CONCAT(de.descripcion,'-',ubg.departamento,'-', ubg.provincia,'-', ubg.distrito) [direccion GUIA],
dl.direccion [direccion FACTURA],
dl.codigo [CODIGO ANEXO CLIENTE]
  FROM DIRECCION_ENTREGA de
INNER JOIN CLIENTE cl ON de.id_cliente = cl.id_cliente
INNER JOIN CIUDAD ci ON ci.id_ciudad = cl.id_ciudad
INNER JOIN CLIENTE_SUNAT cs ON cs.id_cliente_sunat = cl.id_cliente_sunat
INNER JOIN DOMICILIO_LEGAL dl ON dl.id_domicilio_legal = de.id_domicilio_legal
INNER JOIN UBIGEO ubg ON de.ubigeo = ubg.codigo
INNER JOIN UBIGEO ubf ON dl.ubigeo = ubf.codigo
WHERE cs.id_cliente_sunat = (SELECT id_cliente_sunat FROM CLIENTE_SUNAT WHERE RUC = '20505670443')
AND de.estado = 1






-----STORED PROCEDURES 

USE [cotizadormp]
GO
/****** Object:  StoredProcedure [dbo].[ps_venta]    Script Date: 11/03/2019 6:10:53 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ps_venta] 
@idMovimientoAlmacen uniqueIdentifier
AS
BEGIN
	SELECT 
	pe.id_pedido,
	pe.numero, pe.numero_grupo, pe.fecha_solicitud,  
	pe.fecha_entrega_desde, pe.fecha_entrega_hasta,
	pe.hora_entrega_desde, pe.hora_entrega_hasta,
	pe.incluido_igv,  pe.igv, pe.total, pe.observaciones,  pe.fecha_modificacion,
	pe.numero_referencia_cliente, pe.id_direccion_entrega, pe.direccion_entrega, pe.contacto_entrega,
	pe.telefono_contacto_entrega, 
	pe.fecha_programacion,
	pe.tipo_pedido, pe.observaciones_factura, pe.observaciones_guia_remision,
	pe.contacto_pedido,pe.telefono_contacto_pedido, pe.correo_contacto_pedido,
	pe.otros_cargos,
	pe.numero_referencia_adicional,
	pe.numero_requerimiento,
	--UBIGEO
	pe.ubigeo_entrega, ub.departamento, ub.provincia, ub.distrito,
	---CLIENTE
	cl.id_cliente, cl.codigo, cl.razon_social, cl.ruc, cic.id_ciudad as id_ciudad_cliente, cic.nombre as nombre_ciudad_cliente,
	cl.razon_social_sunat, 
	--11/03/2018
	ISNULL(dl.direccion, ISNULL(dlp.direccion, UPPER(cl.direccion_domicilio_legal_sunat)) ) direccion_domicilio_legal_sunat	
	, cl.correo_envio_factura, cl.plazo_credito,
	CASE pe.es_pago_contado WHEN 'TRUE' 
	THEN 1 
	ELSE cl.tipo_pago_factura END 
	AS tipo_pago_factura,
	cl.forma_pago_factura,
	cl.tipo_documento,
	---VENTA
	ve.igv as igv_venta,
	ve.sub_total as sub_total_venta,
	ve.total as total_venta,
	ve.id_venta,
	--CIUDAD
	ci.id_ciudad, ci.nombre as nombre_ciudad ,
	--USUARIO
	us.nombre  as nombre_usuario, us.cargo, us.contacto as contacto_usuario, us.email,
	--SEGUIMIENTO
	ma.fecha_emision,
	--DIRECCION_ENTREGA
	de.nombre as direccion_entrega_nombre,
	de.codigo_mp as direccion_entrega_codigo_mp,
	de.codigo_cliente as direccion_entrega_codigo_cliente
	FROM VENTA as ve
	INNER JOIN MOVIMIENTO_ALMACEN as ma ON ma.id_movimiento_almacen = ve.id_movimiento_almacen
	INNER JOIN PEDIDO pe ON ve.id_pedido = pe.id_pedido
	INNER JOIN CIUDAD AS ci ON pe.id_ciudad = ci.id_ciudad
	INNER JOIN USUARIO AS us on pe.usuario_creacion = us.id_usuario
	INNER JOIN CLIENTE AS cl ON pe.id_cliente = cl.id_cliente
	INNER JOIN CIUDAD AS cic ON cl.id_ciudad = cic.id_ciudad
	LEFT JOIN UBIGEO ub ON CAST(pe.ubigeo_entrega AS CHAR(6)) = ub.codigo
	--LEFT JOIN DIRECCION_ENTREGA de ON de.id_direccion_entrega = pe.id_direccion_entrega
	 --11/03/2018
	LEFT JOIN CLIENTE_SUNAT cs ON cl.id_cliente_sunat = cs.id_cliente_sunat
	LEFT JOIN DIRECCION_ENTREGA de ON pe.id_direccion_entrega = de.id_direccion_entrega
	LEFT JOIN DOMICILIO_LEGAL dl ON dl.id_cliente_sunat = cs.id_cliente_sunat AND dl.id_domicilio_legal = de.id_domicilio_legal
	--domicilio legal principal
	LEFT JOIN DOMICILIO_LEGAL dlp ON  dlp.id_domicilio_legal = cs.id_domicilio_legal
	where ve.id_movimiento_almacen = @idMovimientoAlmacen 
	AND ve.id_documento_venta IS NULL 
	AND	pe.estado = 1
	AND ve.estado = 1

	--DETALLE PEDIDO
	SELECT vd.id_venta_detalle, 
	vd.cantidad,
	vd.precio_sin_igv, 
	vd.costo_sin_igv, 
	vd.equivalencia as equivalencia,
	vd.unidad, 
	vd.porcentaje_descuento, 
	vd.precio_neto, 
	vd.es_precio_alternativo, 
	vd.flete,
	pr.id_producto, pr.sku, pr.descripcion, pr.sku_proveedor, pr.imagen, pr.proveedor, 
	pr.costo as costo_producto, pr.precio as precio_producto,
	pd.observaciones,
	pd.fecha_modificacion,
	CAST(pd.precio_neto + pd.flete AS decimal(12,2)) as precio_unitario_original,
	vd.precio_unitario as precio_unitario_venta,
	vd.igv_precio_unitario as igv_precio_unitario_venta

	FROM 
	VENTA_DETALLE as vd 
	INNER JOIN VENTA ve ON vd.id_venta = ve.id_venta
	INNER JOIN PEDIDO as pe ON ve.id_pedido = pe.id_pedido 
	INNER JOIN PRODUCTO pr ON vd.id_producto = pr.id_producto
	LEFT JOIN PEDIDO_DETALLE as pd ON vd.id_pedido_detalle = pd.id_pedido_detalle
	WHERE ve.id_movimiento_almacen = @idMovimientoAlmacen
	AND ve.id_documento_venta IS NULL
	AND ve.estado = 1
	ORDER BY fecha_modificacion ASC;

	SELECT arch.id_archivo_adjunto, null adjunto, nombre
	FROM ARCHIVO_ADJUNTO arch
	INNER JOIN PEDIDO_ARCHIVO parch ON arch.id_archivo_adjunto = parch.id_archivo_adjunto
	WHERE parch.id_pedido = (SELECT id_pedido FROM MOVIMIENTO_ALMACEN
	WHERE id_movimiento_almacen = @idMovimientoAlmacen   ) AND 
	arch.estado = 1 AND parch.estado = 1;
END






USE [cotizadormp]
GO
/****** Object:  StoredProcedure [dbo].[pi_documentoVenta]    Script Date: 11/03/2019 5:54:33 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[pi_documentoVenta] 
@idVenta uniqueIdentifier,
@idMovimientoAlmacen uniqueIdentifier,
@tipoDocumento int,-- 01 (Factura), 03 (Boleta de venta), 07 (Nota de crédito), 08 (Nota de débito)
@fechaEmision DateTime, --Formato YYYY-MM-DD
@fechaVencimiento DateTime, --Formato YYYY-MM-DD
@tipoPago CHAR(3), --Tabla 1: Tipo de Pago, se debe agregar combo para selección y revisar si tenemos todos los tipos de pago
@formaPago CHAR(3), --Tabla 2: Forma de Pago, Se debe agregar combo para selección		
@idDocumentoVentaReferencia uniqueIdentifier, --Utilizado cuando es una nota de crédito
@idUsuario uniqueIdentifier,
@serie char(3),
@numeroReferenciaCliente varchar(20),
@observaciones varchar(500),
@codigoCliente varchar(50),

@idDocumentoVenta uniqueIdentifier OUTPUT,
@idVentaSalida  uniqueIdentifier OUTPUT,
@tipoError int OUTPUT,
@descripcionError varchar(500) OUTPUT
AS
BEGIN
SET NOCOUNT ON


DECLARE @tipo_documento CHAR(2);
DECLARE @fecha_emision CHAR(10); --Formato YYYY-MM-DD
DECLARE @hora_emision CHAR(8); --Formato hh:mm:ss
DECLARE @fecha_vencimiento CHAR(10); --Formato YYYY-MM-DD
DECLARE @tipo_pago CHAR(3); --Tabla 1: Tipo de Pago, se debe agregar combo para selección y revisar si tenemos todos los tipos de pago
DECLARE @forma_Pago CHAR(3); --Tabla 2: Forma de Pago, Se debe agregar combo para selección
DECLARE @motivo_traslado CHAR(1); --V Venta, G Gratuito
DECLARE @numero_referencia_adicional VARCHAR(30);


---------------CABECERA---------------

DECLARE @COD_OPE_LEY VARCHAR(2); -- REVISAR
DECLARE @PREFIJO_SERIE CHAR(1); --validar si se reutilizaran las series de facturación manual (OK - Se pueden reutilizar)
DECLARE @MONEDA CHAR(3); --Por Mejorar OK (De momento solo maneja soles)
DECLARE @COD_TIP_OPE CHAR(4); --Por Revisar
DECLARE @TIP_DOC_RCT CHAR(1); 
DECLARE @MNT_DCTO_GLB VARCHAR(15); --Monto Descuento Global en moneda de la transacción
DECLARE @FAC_DCTO_GLB VARCHAR(15); --Monto Descuento Global en moneda de la transacción
DECLARE @MNT_CARG_GLB VARCHAR(15); --Monto del cargo global en la moneda de la transacción (Revisar)
DECLARE @FAC_CARG_GLOB VARCHAR(15); --Factor del Cargo Global
DECLARE @MNT_TOT_PER VARCHAR(15); --Monto Total Percepción en la Moneda de la Transacción
DECLARE @FAC_PER VARCHAR(8); --Factor de la Percepción

DECLARE	@MNT_TOT_GRV	VARCHAR(20);
DECLARE	@MNT_TOT_INF	VARCHAR(20);
DECLARE	@MNT_TOT_EXR	VARCHAR(20);
DECLARE	@MNT_TOT_GRT	VARCHAR(20);
DECLARE	@MNT_TOT_EXP	VARCHAR(20);
DECLARE	@MNT_TOT_TRB_IGV	VARCHAR(20);
DECLARE	@MNT_TOT_TRB_ISC	VARCHAR(20);
DECLARE	@MNT_TOT_TRB_OTR	VARCHAR(20);
DECLARE	@MNT_TOT_VAL_VTA	VARCHAR(20);
DECLARE	@MNT_TOT_PRC_VTA	VARCHAR(20);
DECLARE	@MNT_TOT_DCTO	VARCHAR(20);
DECLARE	@MNT_TOT_OTR_CGO	VARCHAR(20);
--DECLARE	@MNT_TOT	VARCHAR(20);
DECLARE	@MNT_TOT_ANTCP	VARCHAR(20);
DECLARE	@TIP_CMB	VARCHAR(20);
DECLARE	@MNT_DCTO_GLB_NAC	VARCHAR(20);
DECLARE	@MNT_CARG_GLB_NAC	VARCHAR(20);
DECLARE	@MNT_TOT_PER_NAC	VARCHAR(20);
--DECLARE	@MNT_TOT_IMP_NAC	VARCHAR(20);
DECLARE	@MNT_TOT_GRV_NAC	VARCHAR(20);
DECLARE	@MNT_TOT_INF_NAC	VARCHAR(20);
DECLARE	@MNT_TOT_EXR_NAC	VARCHAR(20);
DECLARE	@MNT_TOT_GRT_NAC	VARCHAR(20);
DECLARE	@MNT_TOT_EXP_NAC	VARCHAR(20);
DECLARE	@MNT_TOT_TRB_IGV_NAC	VARCHAR(20);
DECLARE	@MNT_TOT_TRB_ISC_NAC	VARCHAR(20);
DECLARE	@MNT_TOT_TRB_OTR_NAC	VARCHAR(20);
DECLARE	@MNT_TOT_VAL_VTA_NAC	VARCHAR(20);
DECLARE	@MNT_TOT_PRC_VTA_NAC	VARCHAR(20);
DECLARE	@MNT_TOT_DCTO_NAC	VARCHAR(20);
DECLARE	@MNT_TOT_OTR_CGO_NAC	VARCHAR(20);
DECLARE	@MNT_TOT_NAC	VARCHAR(20);
DECLARE	@MNT_TOT_ANTCP_NAC	VARCHAR(20);
DECLARE @COD_TIP_DETRACCION VARCHAR(3);
DECLARE @MNT_TOT_DETRACCION	VARCHAR(15);
DECLARE @FAC_DETRACCION VARCHAR(6);
DECLARE @COD_SOF_FACT	VARCHAR(20); --¿QUe ingresar en este campo?
DECLARE @MNT_TOT_ISC	VARCHAR(15);

----------------------DETALLE----------------------
DECLARE @LIN_ITM VARCHAR(3);
DECLARE @COD_UND_ITM VARCHAR(3);
DECLARE @CANT_UND_ITM VARCHAR(15);
DECLARE @VAL_VTA_ITM VARCHAR(15);
DECLARE @tasa_igv DECIMAL(5,2);


DECLARE @PRC_VTA_UND_ITM VARCHAR(15);
DECLARE @VAL_UNIT_ITM VARCHAR(15);
DECLARE @MNT_IGV_ITM VARCHAR(15);
DECLARE @POR_IGV_ITM VARCHAR(15);
DECLARE @PRC_VTA_ITEM VARCHAR(15);
DECLARE @VAL_VTA_BRT_ITEM VARCHAR(15);
DECLARE @COD_TIP_AFECT_IGV_ITM VARCHAR(2);
DECLARE @TXT_DES_ITM VARCHAR(250);
DECLARE @COD_ITM VARCHAR(30);

DECLARE @COD_FACTURA CHAR(2);
DECLARE @COD_BOLETA CHAR(2);
DECLARE @COD_NOTA_CREDITO CHAR(2);
DECLARE @COD_NOTA_DEBITO CHAR(2);
DECLARE @PREFIJO_SERIE_FACTURA CHAR(1);
DECLARE @PREFIJO_SERIE_BOLETA CHAR(1);


DECLARE @idCiudad UniqueIdentifier;
DECLARE @siguienteNumeroFactura CHAR(8);
DECLARE @anteriorNumeroFactura CHAR(8);
DECLARE @siguienteNumeroBoleta CHAR(8);
DECLARE @anteriorNumeroBoleta CHAR(8);


DECLARE @cantidadDocumentosConFechaPosterior int;
DECLARE @cantidadGuiasRemisionFactura int;




/*CONSTANTES*/
SET @COD_FACTURA = '01';
SET @COD_BOLETA = '03';
SET @COD_NOTA_CREDITO = '07';
SET @COD_NOTA_DEBITO = '08';
SET @PREFIJO_SERIE_FACTURA = 'F';
SET @PREFIJO_SERIE_BOLETA = 'B';

SET @MONEDA = 'PEN';	--PEN – Soles
SET @COD_TIP_OPE = '0101'; -- 0101 - Venta Interna
SET @TIP_DOC_RCT =  '6';  --“6” que corresponde al RUC
SET @MNT_DCTO_GLB = '0.00';
SET @FAC_DCTO_GLB = '0.00';
SET @MNT_CARG_GLB = '0.00';
SET @FAC_CARG_GLOB = '0.00';
SET @MNT_TOT_PER = '0.00';
SET @FAC_PER = '0.00';
SET @MNT_TOT_INF = '0.00'; --47
SET @MNT_TOT_EXR = '0.00'; --48
SET @MNT_TOT_GRT = '0.00'; --49
SET @MNT_TOT_EXP = '0.00'; --50
SET @MNT_TOT_ISC = '0.00'; --51
SET @MNT_TOT_TRB_ISC = '0.00'; --53
SET @MNT_TOT_TRB_OTR = '0.00'; --54
SET @MNT_TOT_VAL_VTA = '0.00'; --55
SET @MNT_TOT_PRC_VTA = '0.00'; --56
SET @MNT_TOT_DCTO = '0.00'; --57
SET @MNT_TOT_OTR_CGO = '0.00'; --58
SET @MNT_TOT_ANTCP = '0.00'; --60
SET @TIP_CMB = '';		 --Enviar vacío en el caso que la moneda de la transacción sea soles (PEN)
SET @MNT_DCTO_GLB_NAC = '0.00'; --62
SET @MNT_CARG_GLB_NAC = '0.00'; --63
SET @MNT_TOT_PER_NAC = '0.00'; --64
SET @MNT_TOT_INF_NAC = '0.00'; --67
SET @MNT_TOT_EXR_NAC = '0.00'; --68
SET @MNT_TOT_GRT_NAC = '0.00'; --69
SET @MNT_TOT_EXP_NAC = '0.00'; --70
SET @MNT_TOT_TRB_ISC_NAC = '0.00';  --72
SET @MNT_TOT_TRB_OTR_NAC = '0.00';	--73
SET @MNT_TOT_VAL_VTA_NAC = '0.00';	--74
SET @MNT_TOT_PRC_VTA_NAC = '0.00';	--75
SET @MNT_TOT_DCTO_NAC = '0.00';		--76
SET @MNT_TOT_OTR_CGO_NAC = '0.00';	--77
SET @MNT_TOT_ANTCP_NAC = '0.00';	--79

/*FIN CONSTANTES*/


-------------------------------------
------PARAMETROS DE ENTRADA
----------------------------------

SET @tipo_documento = CONCAT('0',@tipoDocumento);
SET @fecha_emision = LEFT(CONVERT(VARCHAR, @fechaEmision, 120), 10);
SET @hora_emision = LEFT(CONVERT(VARCHAR, @fechaEmision, 8), 8); --hh:mm:ss
SET @fecha_vencimiento =  LEFT(CONVERT(VARCHAR, @fechaVencimiento, 120), 10);
SET @tipo_pago =  REPLACE(STR(@tipoPago, 3), SPACE(1), '0') 
SET @forma_Pago = REPLACE(STR(@formaPago, 3), SPACE(1), '0') 

/*SE INICIALIZAN LAS VARIABLES DE VALIDACION*/
SET @tipoError = 0;
SET @descripcionError = '-';

---SE RECUPERA EL ID DE LA VENTA A PARTIR DEL ID DEL MOVIMIENTO
IF @idVenta = cast(cast(0 as binary) as uniqueidentifier)
BEGIN
	SELECT @idVenta = id_venta, @idCiudad = id_ciudad  
	FROM VENTA where id_movimiento_almacen = @idMovimientoAlmacen AND ESTADO = 1;
END

--SE RECUPERA LA TASA IGV
SELECT @tasa_igv = CAST(VALOR AS DECIMAL(4,2)) FROM PARAMETRO WHERE CODIGO = 'IGV';

--SE IDENTIFICA EL MOTIVO_TRASLADO
SELECT @motivo_traslado = motivo_traslado  FROM MOVIMIENTO_ALMACEN where id_movimiento_almacen = @idMovimientoAlmacen AND ESTADO = 1;





--CONSULTAR: ¿EN GRATUITAS DONDE VA EL MONTO PRC_VTA_UND_ITM y PRC_VTA_ITEM?

DECLARE detalle_venta_cursor CURSOR FOR    
	SELECT 
	/*1*/ROW_NUMBER() OVER(ORDER BY vd.fecha_creacion ASC) AS LIN_ITM,										
	/*2*/vd.unidad_internacional AS COD_UND_ITM,			
	/*3*/ CONVERT(decimal(12,2), vd.cantidad) AS CANT_UND_ITM,	


	

	/*MEJORA: Se debe calcular en la venta*/
	/*4*/CONVERT(decimal(12,2),(vd.precio_unitario) * vd.cantidad)  AS VAL_VTA_ITM,


	/*MEJORA: Se debe calcular en la venta*/
	/*5*/(CASE pr.exonerado_igv WHEN 1 THEN vd.precio_unitario ELSE CONVERT(decimal(12,6),vd.precio_unitario + vd.igv_precio_unitario)  END)  AS PRC_VTA_UND_ITM,  











	/*6*/vd.precio_unitario AS VAL_UNIT_ITM,	

	/*Se cambió para considerar Gratuitos*/
	CASE ve.motivo WHEN 'GG' THEN
	/*7*/CONVERT(decimal(12,2),vd.igv_costo)
	ELSE /*MEJORA: Se debe calcular en la venta*/
	/*7*/(CASE pr.exonerado_igv WHEN 1 THEN 0 ELSE CONVERT(decimal(12,2),vd.igv_precio_unitario * vd.cantidad)  END)  
	END AS MNT_IGV_ITM, 
	
	
	/*8*/(CASE pr.exonerado_igv WHEN 1 THEN 0 ELSE @tasa_igv * 100 END) AS POR_IGV_ITM,	

	/*9*//*MEJORA: Se debe calcular en la venta*/
	(CASE pr.exonerado_igv WHEN 1 THEN (CONVERT(decimal(12,2),(vd.precio_unitario*vd.cantidad) )) 
	ELSE
	(CONVERT(decimal(12,2),( CONVERT(decimal(12,6),vd.precio_unitario+vd.igv_precio_unitario) *vd.cantidad) )) END )


	  AS PRC_VTA_ITEM,  




	/*MEJORA: Se debe calcular en la venta*/
	/*10*/CONVERT(decimal(12,2),(vd.precio_unitario) * vd.cantidad)  AS VAL_VTA_BRT_ITEM, 



	CASE ve.motivo WHEN 'GG' THEN
	/*11*/'13' ---- POR VALIDAR ----KC lo envía como 13
	ELSE
	/*11*/(CASE pr.exonerado_igv WHEN 1 THEN '20' ELSE '10' END) 

	END	AS COD_TIP_AFECT_IGV_ITM, 

	/*12-23*/
	/*24*/CONCAT(ISNULL(vd.descripcion_adicional,''),  pr.descripcion,' - ', vd.unidad) AS TXT_DES_ITM, 
	/*26 Opcional*/ pr.sku AS COD_ITM--,
	FROM VENTA_DETALLE vd
	INNER JOIN PRODUCTO pr ON vd.id_producto = pr.id_producto
	INNER JOIN VENTA ve ON ve.id_venta = vd.id_venta
	WHERE ve.id_Venta = @idVenta;


/*SE CREA EL ID DEL DOCUMENTO DE VENTA*/
SET @idDocumentoVenta = NEWID();


	--	SELECT * FROM USUARIO WHERE email like 'admin%'
	--AREQUIPA C879593F-C9AF-4E0E-9EE5-187EBC99E86E
		--HUACHIN 'CB9FA89D-D54F-4221-8BBE-75B8368A6F67'
		--PIURA --605F5311-3D19-4762-BD1D-EA6EE3D2EDC1
		--CESAR --7E875C20-24FD-4B5E-9BB5-A7F23718F1FA	
		--CARMEN  AA721477-14C2-42CC-8A3F-9CF54C71A55D

		--ALEJANDRO
			
		/*INICIO VALIDACIONES*/ 




		SELECT * FROM USUARIO
/*IF @idUsuario <> 'AA721477-14C2-42CC-8A3F-9CF54C71A55D'
AND @idUsuario <> 'B7642A37-E22C-4D29-830E-840A1A98C40F'--'7E875C20-24FD-4B5E-9BB5-A7F23718F1FA'*/
IF @idUsuario NOT IN (SELECT id_usuario FROM USUARIO where crea_documentos_venta = 1)
BEGIN 
		SET @tipoError = 3;
		SET @descripcionError = 'Usuario No autorizado para facturar TEMPORALMENTE.';
	END


	IF @tipo_documento = @COD_FACTURA
	BEGIN 
		--------FACTURA		
		SET @PREFIJO_SERIE = @PREFIJO_SERIE_FACTURA;	
		SELECT @anteriorNumeroFactura = REPLACE(STR((siguiente_numero_factura -1), 8), SPACE(1), '0'),
		@siguienteNumeroFactura = REPLACE(STR((siguiente_numero_factura), 8), SPACE(1), '0')
		FROM  SERIE_DOCUMENTO_ELECTRONICO sd WHERE sd.serie = @serie

		--	SELECT * FROM USUARIO WHERE EMAIL LIKE '%c.huach%'
		
		SELECT @cantidadDocumentosConFechaPosterior = COUNT(*) FROM CPE_CABECERA_BE
		where SERIE = Concat(@PREFIJO_SERIE,@serie  )
		AND CORRELATIVO = @anteriorNumeroFactura
		AND TIP_CPE = @tipo_documento
		AND estado = 1
		AND ENVIADO_A_EOL = 1
		AND SOLICITUD_ANULACION = 0
		AND  CONVERT(datetime, CONCAT(FEC_EMI,' ', HOR_EMI), 101) > @fechaEmision;

		--SET @cantidadGuiasRemisionFactura = 0;


		SELECT @cantidadGuiasRemisionFactura = COUNT(*)  FROM VENTA ve 
		INNER JOIN MOVIMIENTO_ALMACEN ma 
		ON ve.id_movimiento_almacen = ma.id_movimiento_almacen
		WHERE ve.id_venta = @idVenta
		AND (ma.facturado = 1
		OR ve.id_documento_venta IS NOT NULL);			


		IF(@cantidadDocumentosConFechaPosterior > 0)
		BEGIN 
			SET @tipoError = 1;
			SELECT TOP 1 @descripcionError =
			 CONCAT('La factura ',SERIE,'-', CORRELATIVO, ' se emitió: ', CAST(CONVERT(datetime, CONCAT(FEC_EMI,' ', HOR_EMI), 101)  AS VARCHAR),'.')  
			FROM CPE_CABECERA_BE
			where SERIE = Concat(@PREFIJO_SERIE,@serie  )
			AND CORRELATIVO = @anteriorNumeroFactura
			AND TIP_CPE = @tipo_documento
			AND estado = 1
			AND ENVIADO_A_EOL = 1
			AND SOLICITUD_ANULACION = 0
			AND  CONVERT(datetime, CONCAT(FEC_EMI,' ', HOR_EMI), 101) > @fechaEmision
			ORDER BY CONVERT(datetime, CONCAT(FEC_EMI,' ', HOR_EMI), 101) DESC

		END




		

	END
	ELSE IF @tipo_documento = @COD_BOLETA 
	BEGIN 
		--BOLETA DE VENTA

		SET @PREFIJO_SERIE = @PREFIJO_SERIE_BOLETA;
		SELECT @anteriorNumeroBoleta = REPLACE(STR((siguiente_numero_boleta -1), 8), SPACE(1), '0'),
		@siguienteNumeroBoleta = REPLACE(STR((siguiente_numero_boleta), 8), SPACE(1), '0')
		FROM  SERIE_DOCUMENTO_ELECTRONICO sd WHERE sd.serie = @serie

		SELECT @cantidadDocumentosConFechaPosterior = COUNT(*) FROM CPE_CABECERA_BE
		where SERIE = Concat(@PREFIJO_SERIE,@serie  )
		AND CORRELATIVO = @anteriorNumeroBoleta
		AND TIP_CPE = @tipo_documento
		AND estado = 1
		AND ENVIADO_A_EOL = 1
		AND SOLICITUD_ANULACION = 0
		AND  CONVERT(datetime, CONCAT(FEC_EMI,' ', HOR_EMI), 101) > @fechaEmision;


		SELECT @cantidadGuiasRemisionFactura = COUNT(*)  FROM VENTA ve 
		INNER JOIN MOVIMIENTO_ALMACEN ma 
		ON ve.id_movimiento_almacen = ma.id_movimiento_almacen
		WHERE ve.id_venta = @idVenta
		AND (ma.facturado = 1
		OR ve.id_documento_venta IS NOT NULL);			


		IF(@cantidadDocumentosConFechaPosterior > 0)
		BEGIN 
			SET @tipoError = 1;
			SELECT TOP 1 @descripcionError =
			 CONCAT('La Boleta ',SERIE,'-', CORRELATIVO, ' se emitió: ', CAST(CONVERT(datetime, CONCAT(FEC_EMI,' ', HOR_EMI), 101)  AS VARCHAR),'.')  
			FROM CPE_CABECERA_BE
			where SERIE = Concat(@PREFIJO_SERIE,@serie  )
			AND CORRELATIVO = @anteriorNumeroBoleta
			AND TIP_CPE = @tipo_documento
			AND estado = 1
			AND ENVIADO_A_EOL = 1
			AND SOLICITUD_ANULACION = 0
			AND  CONVERT(datetime, CONCAT(FEC_EMI,' ', HOR_EMI), 101) > @fechaEmision
			ORDER BY CONVERT(datetime, CONCAT(FEC_EMI,' ', HOR_EMI), 101) DESC

		END

	END
	
		


	IF(@cantidadGuiasRemisionFactura > 0)
	BEGIN 
		SET @tipoError = 2;
		SELECT TOP 1 @descripcionError =
			CONCAT('La Guía ',ma.SERIE_documento,'-', ma.numero_documento, ' ya se encuentra facturada.')  

			FROM VENTA ve 
		INNER JOIN MOVIMIENTO_ALMACEN ma 
		ON ve.id_movimiento_almacen = ma.id_movimiento_almacen
		WHERE ve.id_venta = @idVenta
		AND ( ma.facturado = 1

		OR ve.id_documento_venta IS NOT NULL
		)
		ORDER BY ma.fecha_Creacion DESC;

	END
			
		/*FIN DE VALIDACIONES*/	


		--¿UBI_RCT Es obligatorio?
		--Consultar por los campos numero 22 hasta numero 28
		--FRM_PAG 
		--¿NRO_ORD_COM es obligatorio?
		--Tenemos más tipos de pago
		--Consultar por los campos numero 32,33,
		--34 va siempre vacía, queda como NULL
		--36 se envía vacío
		--41 COnsultar, se envía vacío.
		--
	
		IF @tipoError = 0
		BEGIN

		---VENTA
			INSERT INTO CPE_CABECERA_BE
			(id_cpe_cabecera_be, 
			/*1*/ID,					/*2*/COD_GPO,				/*3*/ TIP_CPE,				/*4*/COD_OPE_LEY, 
			/*5*/FEC_EMI,				/*6*/HOR_EMI,				/*7*/ SERIE,				/*8*/CORRELATIVO,
			/*9*/FEC_VCTO,				/*10*/MONEDA,				/*11*/COD_TIP_OPE,			/*12*/TIP_DOC_EMI,
			/*13*/NRO_DOC_EMI,			/*14*/NOM_EMI,				/*15*/NOM_COM_EMI,			/*16*/COD_LOC_EMI, ---REVISAR QUE CORRESPONDE EN LIMA
			/*17*/TIP_DOC_RCT,			/*18*/NRO_DOC_RCT,		
				/*19*/NOM_RCT,				/*20*/DIR_DES_RCT,
		/*21*/UBI_RCT, 
			/*29*/TIP_PAG,				/*30*/FRM_PAG,				/*31*/NRO_ORD_COM,			/*32*/COD_TIP_GRE,
			/*33*/NRO_GRE,											/*35*/FMT_IMPR, 
			/*37*/MNT_DCTO_GLB,			/*38*/FAC_DCTO_GLB,			/*39*/MNT_CARG_GLB,			/*40*/FAC_CARG_GLOB,
			/*41*/TIP_CARG_GLOB,		/*42*/MNT_TOT_PER,										/*44*/FAC_PER,
			/*45*/MNT_TOT_IMP,			/*46*/MNT_TOT_GRV,			/*47*/MNT_TOT_INF,			/*48*/MNT_TOT_EXR,
			/*49*/MNT_TOT_GRT,			/*50*/MNT_TOT_EXP,			/*51*/MNT_TOT_ISC,			/*52*/MNT_TOT_TRB_IGV,
			/*53*/MNT_TOT_TRB_ISC,		/*54*/MNT_TOT_TRB_OTR,		/*55*/MNT_TOT_VAL_VTA,		/*56*/MNT_TOT_PRC_VTA,
			/*57*/MNT_TOT_DCTO,			/*58*/MNT_TOT_OTR_CGO,		/*59*/MNT_TOT,				/*60*/MNT_TOT_ANTCP,
			/*61*/TIP_CMB,				/*62*/MNT_DCTO_GLB_NAC,		/*63*/MNT_CARG_GLB_NAC,		/*64*/MNT_TOT_PER_NAC,
			/*65*/MNT_TOT_IMP_NAC,		/*66*/MNT_TOT_GRV_NAC,		/*67*/MNT_TOT_INF_NAC,		/*68*/MNT_TOT_EXR_NAC,
			/*69*/MNT_TOT_GRT_NAC,		/*70*/MNT_TOT_EXP_NAC,		/*71*/MNT_TOT_TRB_IGV_NAC,	/*72*/MNT_TOT_TRB_ISC_NAC,
			/*73*/MNT_TOT_TRB_OTR_NAC,	/*74*/MNT_TOT_VAL_VTA_NAC,	/*75*/MNT_TOT_PRC_VTA_NAC,	/*76*/MNT_TOT_DCTO_NAC,
			/*77*/MNT_TOT_OTR_CGO_NAC,	/*78*/MNT_TOT_NAC,			/*79*/MNT_TOT_ANTCP_NAC	,	/*80*/COD_TIP_DETRACCION,
			/*81*/MNT_TOT_DETRACCION,	/*82*/FAC_DETRACCION,		/*83*/COD_SOF_FACT,			
																	/*87*/CORREO_ENVIO,			/*88*/CORREO_COPIA,
			/*89*/CORREO_OCULTO,									/*91*/FLG_TIP_CAMBIO,		/*92*/COD_PROCEDENCIA,
			/*93*/ID_EXT_RZN,
			estado, usuario_Creacion, fecha_creacion, usuario_modificacion, fecha_modificacion,
			SOLICITUD_ANULACION, ENVIADO_A_EOL, id_venta, permite_anulacion
			)


			SELECT @idDocumentoVenta,
			/*1*/vpar.CPE_CABECERA_BE_ID,	/*2*/vpar.CPE_CABECERA_BE_COD_GPO,	/*3*/@tipo_documento,					/*4*/@COD_OPE_LEY,
			/*5*/@fecha_emision,			/*6*/@hora_emision,					/*7*/CONCAT(@PREFIJO_SERIE,@serie),			/*8*/CASE @tipo_documento WHEN @COD_BOLETA THEN @siguienteNumeroBoleta ELSE  @siguienteNumeroFactura END,
			/*9*/@fecha_vencimiento,		/*10*/@MONEDA,						/*11*/@COD_TIP_OPE,								/*12*/vpar.TIP_DOC_EMI, 
			/*13*/vpar.NRO_DOC_EMI,			/*14*/vpar.NOM_EMI,					/*15*/vpar.NOM_COM_EMI,							/*16*/ci.codigo_domicilio_fiscal ,
--			/*17*/@TIP_DOC_RCT,				/*18*/cl.ruc,						/*19*/UPPER(cl.razon_social_sunat),				/*20*/UPPER(cl.direccion_domicilio_legal_sunat),
			/*17*/cl.tipo_documento,		/*18*/cl.ruc,						
			
			
			--11/03/2019
			/*19*/CASE @tipo_documento WHEN @COD_BOLETA THEN cl.razon_social ELSE ISNULL(UPPER(cs.razon_social),UPPER(cl.razon_social_sunat)) END ,				
			/*20*/CASE @tipo_documento WHEN @COD_BOLETA THEN '-' ELSE ISNULL(dl.direccion, ISNULL(dlp.direccion, UPPER(cl.direccion_domicilio_legal_sunat)) )END,
			--Si es DNI entonces no es necesario enviar el ubigeo
			/*21*/CASE @tipo_documento WHEN @COD_BOLETA THEN '000000' ELSE ISNULL(dl.ubigeo, ISNULL(dlp.ubigeo,cl.ubigeo)) END,		


			/*29*/@tipo_pago,				/*30*/@forma_pago,					/*31*/pe.numero_referencia_cliente,				/*32*/'09',--revisar
		
		--004-0007001
/*33*/CONCAT('G',ma.serie_documento,'-',ma.numero_documento),		/*35*/vpar.FMT_IMPR,

	--	/*33*/'G004-7001',		/*35*/vpar.FMT_IMPR,----
			----119061 DE ENTEL

			/*37*/@MNT_DCTO_GLB,			/*38*/@FAC_DCTO_GLB,				/*39*/vpar.otros_cargos,							/*40*/CASE vpar.otros_cargos WHEN 0  THEN 0 ELSE CONVERT(decimal(8,5),((vpar.otros_cargos * 100)/vpar.total)*0.01) END,
			/*41*/CASE vpar.otros_cargos WHEN 0 THEN NULL
				ELSE '50' END,
			


											/*42*/@MNT_TOT_PER,																	/*44*/@FAC_PER,
			/*45*/vpar.igv,					/*46*/vpar.sub_total_grabada,		/*47*/@MNT_TOT_INF,								/*48*/vpar.sub_total_exonerada,
			/*49*/vpar.sub_total_gratuita,	/*50*/@MNT_TOT_EXP,					/*51*/@MNT_TOT_ISC,								/*52*/vpar.igv, 
			/*53*/@MNT_TOT_TRB_ISC,			/*54*/@MNT_TOT_TRB_OTR,				/*55*/vpar.sub_total,							/*56*/vpar.total,
			/*57*/@MNT_TOT_DCTO,			/*58*/@MNT_TOT_OTR_CGO,				/*59*/vpar.total,								/*60*/@MNT_TOT_ANTCP,
			/*61*/@TIP_CMB,					/*62*/@MNT_DCTO_GLB_NAC,			/*63*/@MNT_CARG_GLB_NAC,						/*64*/@MNT_TOT_PER_NAC,
			/*65*/vpar.igv,					/*66*/vpar.sub_total_grabada,		/*67*/@MNT_TOT_EXR_NAC,							/*68*/vpar.sub_total_exonerada,
			/*69*/vpar.sub_total_gratuita,	/*70*/@MNT_TOT_EXP_NAC,				/*71*/vpar.igv,									/*72*/@MNT_TOT_TRB_ISC_NAC,
			/*73*/@MNT_TOT_TRB_OTR_NAC,		/*74*/vpar.sub_total,				/*75*/vpar.total,								/*76*/@MNT_TOT_DCTO_NAC,
			/*77*/@MNT_TOT_OTR_CGO_NAC,		/*78*/vpar.total,					/*79*/@MNT_TOT_ANTCP_NAC,						/*80*/@COD_TIP_DETRACCION,
			/*81*/@MNT_TOT_DETRACCION,		/*82*/@FAC_DETRACCION,				/*83*/@COD_SOF_FACT,			
																				/*87*/cl.correo_envio_factura,					/*88*/vpar.CORREO_COPIA,
			/*89*/vpar.CORREO_OCULTO,											/*91*/vpar.FLG_TIP_CAMBIO,						/*92*/vpar.COD_PROCEDENCIA,
			/*93*/vpar.ID_EXT_RZN,		
			1,@idUsuario,GETDATE(), @idUsuario, GETDATE(),
			0,0, @idVenta, 1

			FROM 
			(SELECT v.*, par.CPE_CABECERA_BE_ID, par.CPE_CABECERA_BE_COD_GPO, par.TIP_DOC_EMI, par.NRO_DOC_EMI, par.FLG_TIP_CAMBIO,
			 par.NOM_COM_EMI, par.NOM_EMI, par.FMT_IMPR, par.ID_EXT_RZN, par.COD_PROCEDENCIA,
			 par.CORREO_COPIA, par.CORREO_OCULTO
		 
			 FROM VENTA v, PARAMETROS_AMBIENTE_EOL par	 where v.id_Venta = @idVenta --'B1DA3A64-449A-413A-9AA3-91CC299CE2EE'
			--  AND v.estado = 1 
			  AND par.estado = 1) AS vpar  
			 INNER JOIN CIUDAD ci ON ci.id_ciudad = vpar.id_ciudad --JOIN CON CIUDAD PARA OBTENER codigo_domicilio_fiscal
			 INNER JOIN CLIENTE cl ON vpar.id_cliente = cl.id_cliente --JOIN CON CLIENTE PARA OBTENER RUC, Razón Social del Receptor, Domicilio Fiscal,Ubigeo, Número de registro MTC
			 INNER JOIN PEDIDO pe ON vpar.id_pedido = pe.id_pedido -- Se recupera el numero de orden de compra
			 INNER JOIN MOVIMIENTO_ALMACEN AS ma ON vpar.id_movimiento_almacen = ma.id_movimiento_almacen 
			 --11/03/2018
			 LEFT JOIN CLIENTE_SUNAT cs ON cl.id_cliente_sunat = cs.id_cliente_sunat
			 LEFT JOIN DIRECCION_ENTREGA de ON pe.id_direccion_entrega = de.id_direccion_entrega
			 LEFT JOIN DOMICILIO_LEGAL dl ON dl.id_cliente_sunat = cs.id_cliente_sunat AND dl.id_domicilio_legal = de.id_domicilio_legal
			 --domicilio legal principal
			 LEFT JOIN DOMICILIO_LEGAL dlp ON  dlp.id_domicilio_legal = cs.id_domicilio_legal
			 ;


		
		/*SE INSERTA LAS OBSERVACIONES*/
		INSERT INTO CPE_DAT_ADIC_BE (id_cpe_dat_adic_be, id_cpe_cabecera_be,
		NUM_LIN_ADIC_SUNAT, COD_TIP_ADIC_SUNAT, TXT_DESC_ADIC_SUNAT
		) VALUES (NEWID(), @idDocumentoVenta, '159','159',@observaciones);


		/*SE INSERTA EL CODIGO DEL CLIENTE*/
		INSERT INTO CPE_DAT_ADIC_BE (id_cpe_dat_adic_be, id_cpe_cabecera_be,
		NUM_LIN_ADIC_SUNAT, COD_TIP_ADIC_SUNAT, TXT_DESC_ADIC_SUNAT
		) VALUES (NEWID(), @idDocumentoVenta, '21','21',@codigoCliente);



		SELECT @numero_referencia_adicional = COALESCE(pe.numero_referencia_adicional,'')
		FROM PEDIDO pe
		INNER JOIN VENTA ve ON pe.id_pedido = ve.id_pedido
		WHERE ve.id_venta = @idVenta;

		
		IF @numero_referencia_adicional  <> ''
		BEGIN
			SELECT * FROM CPE_DOC_REF_BE
			/*SE INSERTA EL CO */
			INSERT INTO CPE_DOC_REF_BE (id_cpe_doc_ref_be, id_cpe_cabecera_be, NUM_LIN_REF,
			COD_TIP_DOC_REF, NUM_SERIE_CPE_REF, NUM_CORRE_CPE_REF, FEC_DOC_REF, COD_TIP_OTR_DOC_REF,NUM_OTR_DOC_REF)
			VALUES (NEWID(), @idDocumentoVenta, '1','','','','','99',@numero_referencia_adicional);
		END;

		
			OPEN detalle_venta_cursor  

			FETCH NEXT FROM detalle_venta_cursor   
			INTO @LIN_ITM, @COD_UND_ITM, @CANT_UND_ITM, @VAL_VTA_ITM,
				/*5*/@PRC_VTA_UND_ITM,		/*6*/@VAL_UNIT_ITM,			/*7*/@MNT_IGV_ITM,	/*8*/@POR_IGV_ITM,	
				/*9*/@PRC_VTA_ITEM,			/*10*/@VAL_VTA_BRT_ITEM,	/*11*/@COD_TIP_AFECT_IGV_ITM, /*12-23*/
				/*24*/@TXT_DES_ITM,			/*26 Opcional*/@COD_ITM;
		
			IF @@FETCH_STATUS <> 0   
			PRINT '         <<None>>'     

			WHILE @@FETCH_STATUS = 0  
			BEGIN  
	
				--INSERT INTO CPE_CABECERA_BE
				INSERT INTO CPE_DETALLE_BE(
						id_cpe_detalle_be,		
						id_cpe_cabecera_be,
						/*1*/LIN_ITM,			
						/*2*/COD_UND_ITM,			
						/*3*/CANT_UND_ITM,		
						/*4*/VAL_VTA_ITM,
						/*5*/PRC_VTA_UND_ITM, 
						/*6*/ VAL_UNIT_ITM,	
						/*7*/MNT_IGV_ITM, 
						/*8*/POR_IGV_ITM,	
						/*9*/PRC_VTA_ITEM,  
						/*10*/VAL_VTA_BRT_ITEM, 
						/*11*/COD_TIP_AFECT_IGV_ITM, 
						/*12-23*/
						/*24*/TXT_DES_ITM, 
						/*26 Opcional*/COD_ITM
				)
				VALUES(
						NEWID(),
						@idDocumentoVenta,
						/*1*/@LIN_ITM,			
						/*2*/@COD_UND_ITM,			
						/*3*/@CANT_UND_ITM,		
						/*4*/@VAL_VTA_ITM,
						/*5*/@PRC_VTA_UND_ITM,  
						/*6*/@VAL_UNIT_ITM,	
						/*7*/@MNT_IGV_ITM, 
						/*8*/@POR_IGV_ITM,	
						/*9*/@PRC_VTA_ITEM,  
						/*10*/@VAL_VTA_BRT_ITEM, 
						/*11*/@COD_TIP_AFECT_IGV_ITM, 
						/*12-23*/
						/*24*/@TXT_DES_ITM, 
						/*26 Opcional*/@COD_ITM
				)

				FETCH NEXT FROM detalle_venta_cursor   
				INTO @LIN_ITM, @COD_UND_ITM,	@CANT_UND_ITM, @VAL_VTA_ITM,
				/*5*/@PRC_VTA_UND_ITM,		/*6*/@VAL_UNIT_ITM,			/*7*/@MNT_IGV_ITM,				/*8*/@POR_IGV_ITM,	
				/*9*/@PRC_VTA_ITEM,			/*10*/@VAL_VTA_BRT_ITEM,	/*11*/@COD_TIP_AFECT_IGV_ITM, /*12-23*/
				/*24*/@TXT_DES_ITM,			/*26 Opcional*/@COD_ITM;

			END

		CLOSE detalle_venta_cursor;  
		DEALLOCATE detalle_venta_cursor;  
		
		END




	/*SE RETORNA EL ID DE VENTA EN LA VARIABLE ID_VENTA_SALIDA*/
	SET @idVentaSalida = @idVenta;
END





USE [cotizadormp]
GO
/****** Object:  StoredProcedure [dbo].[pi_clienteSunat]    Script Date: 11/03/2019 6:16:11 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[pi_clienteSunat] 
@idUsuario uniqueidentifier,
@razonSocial  varchar(200),
@nombreComercial  varchar(200),
@ruc  varchar(20),
@contacto1  varchar(100),
@idCiudad uniqueidentifier,
@correoEnvioFactura varchar(1000),
@razonSocialSunat varchar(500),
@nombreComercialSunat varchar(500),
@direccionDomicilioLegalSunat varchar(1000),
@estadoContribuyente varchar(50),
@condicionContribuyente varchar(50),
@ubigeo varchar(6),
@formaPagoFactura int,
/*Plazo credito*/
@plazoCreditoSolicitado int,
@tipoPagoFactura int,
@sobrePlazo int, 
/*Monto Crédito*/
@creditoSolicitado decimal(12,2),
@creditoAprobado decimal(12,2),
@sobreGiro decimal(12,2),
/*Vendedores*/
@idResponsableComercial int,
@idAsistenteServicioCliente int,
@idSupervisorComercial int,

@tipoDocumento char(1),
@observacionesCredito varchar(1000),
@observaciones varchar(1000),
@vendedoresAsignados smallint,
@bloqueado smallint,
@perteneceCanalMultiregional smallint,
@perteneceCanalLima smallint,
@perteneceCanalProvincias smallint,
@perteneceCanalPCP smallint,
@esSubDistribuidor smallint,

@idOrigen int,
@idSubDistribuidor int,

@idGrupoCliente int,
@horaInicioPrimerTurnoEntrega datetime,
@horaFinPrimerTurnoEntrega datetime,
@horaInicioSegundoTurnoEntrega datetime,
@horaFinSegundoTurnoEntrega datetime,


/* Campos agregados  */
@habilitadoNegociacionGrupal bit,
@sedePrincipal bit,
@negociacionMultiregional bit,
@telefonoContacto1 varchar(50),
@emailContacto1 varchar(50),

@observacionHorarioEntrega varchar(1000), 
@fechaInicioVigencia date,

@newId uniqueidentifier OUTPUT, 
@codigoAlterno int OUTPUT,
@codigo VARCHAR(4) OUTPUT

AS
BEGIN TRAN

DECLARE @id_cliente_sunat int;
DECLARE @id_domicilio_legal int;
DECLARE @existe_cliente bit;



Select @codigo = siguiente_codigo_cliente FROM CIUDAD where id_ciudad = @idCiudad;


SET NOCOUNT ON
SET @newId = NEWID();
SET @codigoAlterno = NEXT VALUE FOR SEQ_CODIGO_ALTERNO_CLIENTE;

IF @tipoDocumento <> 6
BEGIN 
	SET @razonSocial = @nombreComercial;
END
ELSE
BEGIN
	SET @razonSocial = @razonSocialSunat;
	SELECT @existe_cliente = COUNT(*) FROM CLIENTE_SUNAT WHERE ruc = @ruc

	IF @existe_cliente = 0
	BEGIN --Crear ClienteSunat
		SET @id_cliente_sunat = NEXT VALUE FOR SEQ_ID_CLIENTE_SUNAT;
		SET @id_domicilio_legal = NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL;

		INSERT INTO CLIENTE_SUNAT 
		(id_cliente_sunat, ruc,estado, fecha_creacion, fecha_modificacion,
		razon_social,	nombre_comercial,	estado_contribuyente,	condicion_contribuyente)
		VALUES
		( 
		@id_cliente_sunat, @ruc, 1, GETDATE(), GETDATE(), 
		@razonSocialSunat, @nombreComercialSunat, @estadoContribuyente, @condicionContribuyente
		)
		
		INSERT INTO DOMICILIO_LEGAL 
		(id_domicilio_legal, id_cliente_sunat, codigo, direccion, ubigeo, 
		es_establecimiento_anexo,
		estado, fecha_creacion, fecha_modificacion, usuario_creacion, usuario_modificacion
		)
		VALUES
		(@id_domicilio_legal, @id_cliente_sunat, '0000', @direccionDomicilioLegalSunat,	@ubigeo, 0,	
		1, GETDATE(), GETDATE(), @idUsuario, @idUsuario	)

		UPDATE CLIENTE_SUNAT set id_domicilio_legal = @id_domicilio_legal where id_cliente_sunat = @id_cliente_sunat;

	END 
	ELSE --Actualizar ClienteSunat
	BEGIN 
		SELECT @id_cliente_sunat = id_cliente_sunat, @id_domicilio_legal = id_domicilio_legal FROM CLIENTE_SUNAT WHERE ruc = @ruc;

		UPDATE CLIENTE_SUNAT set 
		razon_social = @razonSocialSunat,
		nombre_comercial = @nombreComercialSunat, 
		estado_contribuyente = @estadoContribuyente,
		condicion_contribuyente = @condicionContribuyente,
		usuario_modificacion = @idUsuario,
		fecha_modificacion = GETDATE()
		WHERE id_cliente_sunat = @id_cliente_sunat

		UPDATE DOMICILIO_LEGAL set
		direccion = @direccionDomicilioLegalSunat,
		ubigeo = @ubigeo,
		usuario_modificacion = @idUsuario,
		fecha_modificacion = GETDATE()
		WHERE id_domicilio_legal = @id_domicilio_legal

	END

END



INSERT INTO CLIENTE
           ([id_cliente]
		   ,[codigo_alterno]
           ,[razon_Social]
		   ,[nombre_Comercial]
           ,[ruc]
           ,[contacto1]
		   ,[id_ciudad]
		   ,estado
		   ,[usuario_creacion]
		   ,[fecha_creacion]
		   ,[usuario_modificacion]
		   ,[fecha_modificacion]
		   ,correo_Envio_Factura
		   ,razon_Social_Sunat
		   ,nombre_Comercial_Sunat
		   ,direccion_Domicilio_Legal_Sunat
		   ,estado_Contribuyente_sunat
		   ,condicion_Contribuyente_sunat
		   ,ubigeo
		   ,direccion_despacho
		   ,forma_pago_factura
		   ,tipo_documento
		   ,codigo,
		   /*Plazo credito*/
			plazo_credito_solicitado,
			tipo_pago_factura,
			sobre_plazo, 
			/*Monto Crédito*/
			credito_solicitado,
			credito_aprobado,
			sobre_giro, 
			/*Vendedores*/
			id_responsable_comercial,
			id_asistente_servicio_cliente,
			id_supervisor_comercial,
			observaciones_credito,
			observaciones,
			vendedores_asignados,
			bloqueado,
			pertenece_canal_multiregional,
			pertenece_canal_lima,
			pertenece_canal_provincia,
			pertenece_canal_pcp,
			es_sub_distribuidor,
			hora_inicio_primer_turno_entrega,
			hora_fin_primer_turno_entrega,
			hora_inicio_segundo_turno_entrega,
			hora_fin_segundo_turno_entrega,
			es_proveedor,
			sede_principal,
			negociacion_multiregional,
			habilitado_negociacion_grupal,
			telefono_contacto1,
			email_contacto1,
			id_origen,
			id_subdistribuidor,
			fecha_inicio_vigencia,
			observacion_horario_entrega,
			id_grupo_cliente, 
			id_cliente_sunat
		   )
     VALUES
           (@newId,
		    @codigoAlterno,
		    @razonSocial,
			@nombreComercial,
            @ruc,
            @contacto1, 
			@idCiudad,
			1,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE(),
			@correoEnvioFactura,
			@razonSocialSunat,
			@nombreComercialSunat,
			@direccionDomicilioLegalSunat,
			@estadoContribuyente,
			@condicionContribuyente,
			@ubigeo,
			@direccionDomicilioLegalSunat,
		--	@tipoPagoFactura,
			@formaPagoFactura,
			@tipoDocumento,
			@codigo,--codigo
			@plazoCreditoSolicitado,
			@tipoPagoFactura,
			@sobrePlazo, 
			/*Monto Crédito*/
			@creditoSolicitado,
			@creditoAprobado,
			@sobreGiro, 
			/*Vendedores*/
			@idResponsableComercial,
			@idAsistenteServicioCliente ,
			@idSupervisorComercial,
			@observacionesCredito,
			@observaciones,
			@vendedoresAsignados,
			@bloqueado ,
			@perteneceCanalMultiregional ,
			@perteneceCanalLima ,
			@perteneceCanalProvincias ,
			@perteneceCanalPCP ,
			@esSubDistribuidor, 
			@horaInicioPrimerTurnoEntrega,
			@horaFinPrimerTurnoEntrega,
			@horaInicioSegundoTurnoEntrega,
			@horaFinSegundoTurnoEntrega,
			1, --ES PROVEEDOR
			@sedePrincipal,
			@negociacionMultiregional,
			@habilitadoNegociacionGrupal,
			@telefonoContacto1,
			@emailContacto1,
			@idOrigen,
			@idSubDistribuidor,
			@fechaInicioVigencia,
			@observacionHorarioEntrega,
			@idGrupoCliente,
			@id_cliente_sunat
			);

INSERT INTO SOLICITANTE 
(id_solicitante, id_cliente, nombre, telefono, correo, estado, 
usuario_creacion, fecha_creacion, usuario_modificacion, fecha_modificacion)
VALUES
(newid(), @newId, @contacto1, @telefonoContacto1, @emailContacto1,1,
@idUsuario,GETDATE(), @idUsuario, GETDATE())


IF @codigo = 'LY99'
BEGIN
	UPDATE CIUDAD set siguiente_codigo_cliente = 'LV01'	where id_ciudad = @idCiudad;
END 
ELSE 
BEGIN
	UPDATE CIUDAD set siguiente_codigo_cliente = 
	CONCAT( LEFT(siguiente_codigo_cliente,2),
	FORMAT (CAST(SUBSTRing (siguiente_codigo_cliente,3,2) AS INT) + 1 , '00' ))
	where id_ciudad = @idCiudad
END


	
IF @negociacionMultiregional = 'FALSE'
BEGIN
	UPDATE CLIENTE 
	SET sede_principal = 'FALSE',
	fecha_inicio_vigencia = @fechaInicioVigencia
	WHERE ruc like @ruc;
END

UPDATE CLIENTE 
SET   negociacion_multiregional = @negociacionMultiregional, pertenece_canal_multiregional = @perteneceCanalMultiregional
        ,razon_Social_Sunat = @razonSocialSunat
		,nombre_Comercial_Sunat = nombre_Comercial_Sunat
		,direccion_Domicilio_Legal_Sunat = @direccionDomicilioLegalSunat
		,estado_Contribuyente_sunat = @estadoContribuyente
		,condicion_Contribuyente_sunat = @condicionContribuyente
		,es_sub_distribuidor = @esSubDistribuidor
		,id_subdistribuidor = @idSubDistribuidor
		,ubigeo = @ubigeo
		,fecha_inicio_vigencia = @fechaInicioVigencia
WHERE ruc like @ruc;

IF @idGrupoCliente > 0 
BEGIN
	INSERT INTO CLIENTE_GRUPO_CLIENTE 
	VALUES (@newId, @idGrupoCliente, GETDATE(), 1, @idUsuario, GETDATE(), @idUsuario, GETDATE())
END







COMMIT




USE [cotizadormp]
GO
/****** Object:  StoredProcedure [dbo].[pu_clienteSunat]    Script Date: 12/03/2019 10:15:44 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[pu_clienteSunat] 
@idCliente uniqueidentifier,
@idUsuario uniqueidentifier,
@razonSocial  varchar(200),
@nombreComercial  varchar(200),
@contacto1  varchar(100),
@idCiudad uniqueidentifier,
@correoEnvioFactura varchar(1000),
@razonSocialSunat varchar(500),
@nombreComercialSunat varchar(500),
@direccionDomicilioLegalSunat varchar(1000),
@estadoContribuyente varchar(50),
@condicionContribuyente varchar(50),
@ubigeo varchar(6),
@formaPagoFactura int,

/*Plazo credito*/
@plazoCreditoSolicitado int,
@tipoPagoFactura int,
@sobrePlazo int, 
/*Monto Crédito*/
@creditoSolicitado decimal(12,2),
@creditoAprobado decimal(12,2),
@sobreGiro decimal(12,2),
/*Vendedores*/
@idResponsableComercial int,
@idAsistenteServicioCliente int,
@idSupervisorComercial int,

@idOrigen int,
@idSubDistribuidor int,

@observacionesCredito varchar(1000),
@observaciones varchar(1000),
@vendedoresAsignados smallint,
@bloqueado smallint,
@perteneceCanalMultiregional smallint,
@perteneceCanalLima smallint,
@perteneceCanalProvincias smallint,
@perteneceCanalPCP smallint,
@esSubDistribuidor smallint,
@idGrupoCliente int,
@horaInicioPrimerTurnoEntrega datetime,
@horaFinPrimerTurnoEntrega datetime,
@horaInicioSegundoTurnoEntrega datetime,
@horaFinSegundoTurnoEntrega datetime,
/* Campos agregados  */
@habilitadoNegociacionGrupal bit,
@sedePrincipal bit,
@negociacionMultiregional bit,
@telefonoContacto1 varchar(50),
@emailContacto1 varchar(50),
@observacionHorarioEntrega varchar(1000), 
@fechaInicioVigencia date,

@existenCambiosCreditos smallint OUTPUT,
@usuarioSolicitanteCredito uniqueIdentifier OUTPUT,
@correoUsuarioSolicitanteCredito VARCHAR(50) OUTPUT

AS
BEGIN TRAN

DECLARE @id_cliente_sunat int;
DECLARE @id_domicilio_legal int;
DECLARE @existe_cliente bit;

DECLARE @ruc varchar(20); 
DECLARE @nrAnterior bit; 
DECLARE @idResponsableComercialAnterior int;  /* Agregado */
DECLARE @idAsistenteServicioClienteAnterior int;  /* Agregado */
DECLARE @idSupervisorComercialAnterior int;  /* Agregado */
DECLARE @negociacionMultiregionalAnterior bit;  /* Agregado */
DECLARE @sedePrincipalAnterior bit;  /* Agregado */
DECLARE @plazoCreditoSolicitadoAnterior int;
DECLARE @tipoPagoFacturaAnterior int;
DECLARE @formaPagoFacturaAnterior int;
DECLARE @creditoSolicitadoAnterior decimal(12,2);
DECLARE @creditoAprobadoAnterior decimal(12,2);
DECLARE @usuarioSolicitanteCreditoAnterior uniqueIdentifier;
DECLARE @enviarCorreoCreditos int;
DECLARE @enviarCorreoUsuarioNoCreditos int;
DECLARE @defineMontoCredito smallint;
DECLARE @definePlazoCredito smallint;

DECLARE @countSolicitante int;
DECLARE @idSolicitante uniqueIdentifier;


SET NOCOUNT ON
SET @existenCambiosCreditos = 0;
SET @usuarioSolicitanteCredito = '00000000-0000-0000-0000-000000000000';
SET @correoUsuarioSolicitanteCredito = '';

IF (SELECT tipo_documento FROM CLIENTE where id_cliente = @idCliente) <> 6
BEGIN 
	SET @razonSocial = @nombreComercial;
END
ELSE
BEGIN 
SET @razonSocial = @razonSocialSunat;
	SELECT @existe_cliente = COUNT(*) FROM CLIENTE_SUNAT WHERE ruc = @ruc

	IF @existe_cliente = 0
	BEGIN --Crear ClienteSunat
		SET @id_cliente_sunat = NEXT VALUE FOR SEQ_ID_CLIENTE_SUNAT;
		SET @id_domicilio_legal = NEXT VALUE FOR SEQ_ID_DOMICILIO_LEGAL;

		INSERT INTO CLIENTE_SUNAT 
		(id_cliente_sunat, ruc,estado, fecha_creacion, fecha_modificacion,
		razon_social,	nombre_comercial,	estado_contribuyente,	condicion_contribuyente)
		VALUES
		( 
		@id_cliente_sunat, @ruc, 1, GETDATE(), GETDATE(), 
		@razonSocialSunat, @nombreComercialSunat, @estadoContribuyente, @condicionContribuyente
		)
		
		INSERT INTO DOMICILIO_LEGAL 
		(id_domicilio_legal, id_cliente_sunat, codigo, direccion, ubigeo, 
		es_establecimiento_anexo,
		estado, fecha_creacion, fecha_modificacion, usuario_creacion, usuario_modificacion
		)
		VALUES
		(@id_domicilio_legal, @id_cliente_sunat, '0000', @direccionDomicilioLegalSunat,	@ubigeo, 0,	
		1, GETDATE(), GETDATE(), @idUsuario, @idUsuario	)

		UPDATE CLIENTE_SUNAT set id_domicilio_legal = @id_domicilio_legal where id_cliente_sunat = @id_cliente_sunat;

	END 
	ELSE --Actualizar ClienteSunat
	BEGIN 
		SELECT @id_cliente_sunat = id_cliente_sunat, @id_domicilio_legal = id_domicilio_legal FROM CLIENTE_SUNAT WHERE ruc = @ruc;

		UPDATE CLIENTE_SUNAT set 
		razon_social = @razonSocialSunat,
		nombre_comercial = @nombreComercialSunat, 
		estado_contribuyente = @estadoContribuyente,
		condicion_contribuyente = @condicionContribuyente,
		usuario_modificacion = @idUsuario,
		fecha_modificacion = GETDATE()
		WHERE id_cliente_sunat = @id_cliente_sunat

		UPDATE DOMICILIO_LEGAL set
		direccion = @direccionDomicilioLegalSunat,
		ubigeo = @ubigeo,
		usuario_modificacion = @idUsuario,
		fecha_modificacion = GETDATE()
		WHERE id_domicilio_legal = @id_domicilio_legal

	END

END



SELECT @ruc = ruc, @nrAnterior = negociacion_multiregional ,
@plazoCreditoSolicitadoAnterior = plazo_credito_solicitado,
@tipoPagoFacturaAnterior = tipo_pago_factura,
@formaPagoFacturaAnterior = forma_pago_factura,
@idResponsableComercialAnterior = id_responsable_comercial,
@idAsistenteServicioClienteAnterior = id_asistente_Servicio_cliente,
@idSupervisorComercialAnterior = id_supervisor_comercial,
@negociacionMultiregionalAnterior = negociacion_multiregional,
@sedePrincipalAnterior = sede_principal,
@creditoSolicitadoAnterior = credito_solicitado,
@creditoAprobadoAnterior =credito_aprobado,
@usuarioSolicitanteCreditoAnterior = usuario_solicitante_credito
FROM CLIENTE WHERE id_cliente = @idCliente; /* Agregado */

IF @plazoCreditoSolicitadoAnterior <>  @plazoCreditoSolicitado
	OR @tipoPagoFacturaAnterior <> @tipoPagoFactura
	OR @formaPagoFacturaAnterior <> @formaPagoFactura
	OR @creditoSolicitadoAnterior <> @creditoSolicitado
	OR @creditoAprobadoAnterior <> @creditoAprobado
BEGIN 
 
	SET @existenCambiosCreditos = 1;

	/*Si no se indica el usuario solicitante entonces se recupera el ultimo solicitanteCredito en caso exista*/
	/*IF @usuarioSolicitanteCredito = '00000000-0000-0000-0000-000000000000'
	BEGIN 
		SET @usuarioSolicitanteCredito = @usuarioSolicitanteCreditoAnterior
	END */
	
	
	SELECT @defineMontoCredito = ISNULL(define_monto_credito,0),
	@definePlazoCredito = ISNULL(define_plazo_credito,0)
	FROM USUARIO where id_usuario = @idUsuario

	IF  @defineMontoCredito = 1 OR @definePlazoCredito = 1
	BEGIN 
		/*Si el usuario es aprobador de creditos se recupera el usuario solicitante anterior*/
		SET @usuarioSolicitanteCredito = @usuarioSolicitanteCreditoAnterior;
		SELECT @correoUsuarioSolicitanteCredito = ISNULL(email,'') FROM USUARIO 
		WHERE id_usuario = @usuarioSolicitanteCreditoAnterior;

	END 
	ELSE
	BEGIN
		/*Si el usuario NO es aprobador de creditos se actualiza con el usuario actual*/
		SELECT @usuarioSolicitanteCredito = @idUsuario,
		@correoUsuarioSolicitanteCredito = email
		FROM USUARIO 
		WHERE id_usuario = @idUsuario;
	END
END 

UPDATE CLIENTE SET [razon_Social] = @razonSocial
		   ,[nombre_Comercial] = @nombreComercial   
		   ,[id_ciudad] = @idCiudad
		   ,[usuario_modificacion] = @idUsuario
		   ,[fecha_modificacion] = GETDATE()
		   ,correo_Envio_Factura = @correoEnvioFactura
		   ,razon_Social_Sunat = @razonSocialSunat
		   ,nombre_Comercial_Sunat = nombre_Comercial_Sunat
		   ,direccion_Domicilio_Legal_Sunat = @direccionDomicilioLegalSunat
		   ,estado_Contribuyente_sunat = @estadoContribuyente
		   ,condicion_Contribuyente_sunat = @condicionContribuyente
		   ,ubigeo = @ubigeo
		   ,forma_pago_factura = @formaPagoFactura
		   ,contacto1 =@contacto1
		   /*Plazo credito*/
			,plazo_credito_solicitado = @plazoCreditoSolicitado
			,tipo_pago_factura = @tipoPagoFactura
			,sobre_plazo = @sobrePlazo
			/*Monto Crédito*/
			,credito_solicitado = @creditoSolicitado
			,credito_aprobado = @creditoAprobado
			,sobre_giro = @sobreGiro
			/*Vendedores*/
			,id_responsable_comercial = @idResponsableComercial
			,id_asistente_Servicio_cliente = @idAsistenteServicioCliente
			,id_supervisor_comercial = @idSupervisorComercial
			,observaciones_credito = @observacionesCredito
			,observaciones = @observaciones
			,vendedores_asignados = @vendedoresAsignados
			,bloqueado = @bloqueado
			,pertenece_canal_multiregional = @perteneceCanalMultiregional
			,pertenece_canal_lima = @perteneceCanalLima
			,pertenece_canal_provincia = @perteneceCanalProvincias
			,pertenece_canal_pcp =@perteneceCanalPCP
			,es_sub_distribuidor = @esSubDistribuidor
			,hora_inicio_primer_turno_entrega = @horaInicioPrimerTurnoEntrega
			,hora_fin_primer_turno_entrega = @horaFinPrimerTurnoEntrega
			,hora_inicio_segundo_turno_entrega = @horaInicioSegundoTurnoEntrega
			,hora_fin_segundo_turno_entrega = @horaFinSegundoTurnoEntrega
			,sede_principal = @sedePrincipal
			,negociacion_multiregional = @negociacionMultiregional
			,habilitado_negociacion_grupal = @habilitadoNegociacionGrupal
			,telefono_contacto1 = @telefonoContacto1
			,email_contacto1 = @emailContacto1
			,usuario_solicitante_credito = @usuarioSolicitanteCredito
			,observacion_horario_entrega = @observacionHorarioEntrega
			,id_origen = @idOrigen
			,id_subdistribuidor = @idSubDistribuidor
			,fecha_inicio_vigencia = @fechaInicioVigencia
			,id_grupo_cliente = @idGrupoCliente
			,id_cliente_sunat = @id_cliente_sunat
     WHERE 
          id_cliente = @idCliente;

		  
/* IF Agregado */

	
	IF @negociacionMultiregional = 'FALSE'
	BEGIN
		UPDATE CLIENTE 
		SET sede_principal = 'FALSE',
		fecha_inicio_vigencia = @fechaInicioVigencia	
		WHERE ruc like @ruc;
	END

	UPDATE CLIENTE 
	SET negociacion_multiregional = @negociacionMultiregional, pertenece_canal_multiregional = @perteneceCanalMultiregional
	       ,razon_Social_Sunat = @razonSocialSunat
		   ,nombre_Comercial_Sunat = nombre_Comercial_Sunat
		   ,direccion_Domicilio_Legal_Sunat = @direccionDomicilioLegalSunat
		   ,estado_Contribuyente_sunat = @estadoContribuyente
		   ,condicion_Contribuyente_sunat = @condicionContribuyente
		   ,ubigeo = @ubigeo
		   ,es_sub_distribuidor = @esSubDistribuidor
		   ,id_subdistribuidor = @idSubDistribuidor
		   ,fecha_inicio_vigencia = @fechaInicioVigencia
	WHERE ruc like @ruc;


DELETE CLIENTE_GRUPO_CLIENTE 
where id_cliente = @idCliente;


IF @idGrupoCliente > 0 
BEGIN
	INSERT INTO CLIENTE_GRUPO_CLIENTE 
	VALUES (@idCliente, @idGrupoCliente, GETDATE(), 1, @idUsuario, GETDATE(), @idUsuario, GETDATE())
END



SELECT @countSolicitante = COUNT(*) FROM SOLICITANTE
WHERE id_cliente = @idCliente 
AND estado = 1 
AND REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(@contacto1), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')   = 
REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(nombre), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ') 
	
IF @countSolicitante = 1
BEGIN 
	SELECT @idSolicitante = id_solicitante FROM SOLICITANTE
	WHERE id_cliente = @idCliente 
	AND estado = 1 
	AND REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(@contacto1), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')   = 
	REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(UPPER(nombre), 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ') 
END 


IF @idSolicitante = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)
BEGIN 
	SET @idSolicitante  = NEWID();
	INSERT INTO SOLICITANTE 
	(id_solicitante, id_cliente, nombre, telefono, correo, estado, 
	usuario_creacion, fecha_creacion, usuario_modificacion, fecha_modificacion)
	VALUES(@idSolicitante, @idCliente, @contacto1, @telefonoContacto1, @emailContacto1,1,
	@idUsuario,GETDATE(), @idUsuario, GETDATE());
END 
ELSE
BEGIN
	UPDATE SOLICITANTE SET 
	nombre = @contacto1, 
	telefono = @telefonoContacto1, 
	correo = @emailContacto1,
	usuario_modificacion = @idUsuario,
	fecha_modificacion = GETDATE() 
	where id_solicitante = @idSolicitante;
END 


UPDATE ARCHIVO_ADJUNTO SET estado = 0 where id_cliente = @idCliente;



COMMIT





USE [cotizadormp]
GO
/****** Object:  StoredProcedure [dbo].[ps_clientes]    Script Date: 12/03/2019 12:10:38 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[ps_clientes] 
@codigo varchar(4),
@idCiudad uniqueIdentifier,
@textoBusqueda varchar(50),
@idResponsableComercial int,
@idSupervisorComercial int,
@idAsistenteServicioCliente int,
@sinPlazoCreditoAprobado int, 
@sinAsesorValidado int, 
@bloqueado int,
@idGrupoCliente int, 
@perteneceCanalLima int,
@perteneceCanalProvincias int,
@perteneceCanalMultiregional int,
@perteneceCanalPCP int
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


	CASE cl.tipo_documento WHEN 1 
		THEN cl.razon_social
	WHEN 4
		THEN cl.razon_social
	ELSE ISNULL(cl.nombre_comercial,'')  END nombre_comercial,
	

	cl.tipo_documento, 
	cl.ruc,
		--VENDEDORES,
	verc.codigo as responsable_comercial_codigo,
	ISNULL(verc.descripcion,'') as responsable_comercial_descripcion,

	vesc.codigo as supervisor_comercial_codigo,
	ISNULL(vesc.descripcion,'') as supervisor_comercial_descripcion,

	veasc.codigo as asistente_servicio_cliente_codigo,
	ISNULL(veasc.descripcion,'') as asistente_servicio_cliente_descripcion,

	cl.habilitado_negociacion_grupal,

	cl.id_subdistribuidor,
	sub.nombre nombre_subdistribuidor, 
	sub.codigo codigo_subdistribuidor, 
	cl.id_origen,
	ori.nombre nombre_origen, 
	ori.codigo codigo_origen, 


	cl.tipo_pago_factura, --plazo credito aprobado
	cl.credito_aprobado,
	cl.bloqueado,
	cgc.id_grupo_cliente,
	gc.codigo as codigo_grupo,
	ISNULL(gc.grupo,'-') grupo
	FROM CLIENTE AS cl
	INNER JOIN CIUDAD AS ci ON cl.id_ciudad = ci.id_ciudad 
	--LEFT JOIN UBIGEO AS ub ON cl.ubigeo = ub.codigo
	LEFT JOIN VENDEDOR AS verc ON cl.id_responsable_comercial = verc.id_vendedor
	LEFT JOIN VENDEDOR AS vesc ON cl.id_supervisor_comercial = vesc.id_vendedor
	LEFT JOIN VENDEDOR AS veasc ON cl.id_asistente_servicio_cliente = veasc.id_vendedor
	LEFT JOIN CLIENTE_GRUPO_CLIENTE  AS cgc ON cl.id_cliente = cgc.id_cliente
	LEFT JOIN GRUPO_CLIENTE AS gc ON gc.id_grupo_cliente = cgc.id_grupo_cliente
	LEFT JOIN SUBDISTRIBUIDOR AS sub ON sub.id_subdistribuidor = cl.id_subdistribuidor 
	LEFT JOIN ORIGEN AS ori ON ori.id_origen = cl.id_origen 


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
	AND (@sinAsesorValidado = 0 OR cl.vendedores_asignados = 0) 
	AND cl.estado > 0

	AND (cl.id_ciudad = @idCiudad OR @idCiudad = '00000000-0000-0000-0000-000000000000')
	AND (@idGrupoCliente = 0 OR @idGrupoCliente = cgc.id_grupo_cliente)
	AND (@sinPlazoCreditoAprobado = 0 OR 
	(@sinPlazoCreditoAprobado = 1 AND cl.tipo_pago_factura IS NULL	) OR 
	(@sinPlazoCreditoAprobado = 1 AND cl.tipo_pago_factura = 0	))
	AND (
	(pertenece_canal_lima = @perteneceCanalLima)
	OR (pertenece_canal_provincia = @perteneceCanalProvincias )
	OR (pertenece_canal_multiregional = @perteneceCanalMultiregional)
	OR (pertenece_canal_pcp = @perteneceCanalPCP  )
	OR (@perteneceCanalLima = 1 AND @perteneceCanalProvincias = 1 AND @perteneceCanalMultiregional = 1  AND @perteneceCanalPCP =1)
	)

	/*
AND (cl.id_cliente IN (SELECT id_cliente FROM VENTA where estado = 1) 
	or CAST(cl.fecha_creacion AS DATE) >= '2018-09-01' OR cl.id_cliente 
	IN ( '9C9E1FFD-A7D0-4628-B4B6-3292C8C42246', '8B7894B2-202E-4958-9967-0CA4B82FC85E')

	)*/

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

	CASE cl.tipo_documento WHEN 1 
		THEN cl.razon_social
	WHEN 4
		THEN cl.razon_social
	ELSE ISNULL(cl.nombre_comercial,'')  END nombre_comercial,
	
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
	ISNULL(gc.grupo,'-') grupo,
	gc.codigo as codigo_grupo
	FROM CLIENTE AS cl
	INNER JOIN CIUDAD AS ci ON cl.id_ciudad = ci.id_ciudad 
	--LEFT JOIN UBIGEO AS ub ON cl.ubigeo = ub.codigo
	LEFT JOIN VENDEDOR AS verc ON cl.id_responsable_comercial = verc.id_vendedor
	LEFT JOIN VENDEDOR AS vesc ON cl.id_supervisor_comercial = vesc.id_vendedor
	LEFT JOIN VENDEDOR AS veasc ON cl.id_asistente_servicio_cliente = veasc.id_vendedor
	LEFT JOIN CLIENTE_GRUPO_CLIENTE  AS cgc ON cl.id_cliente = cgc.id_cliente
	LEFT JOIN GRUPO_CLIENTE AS gc ON gc.id_grupo_cliente = cgc.id_grupo_cliente
	WHERE cl.codigo = @codigo AND cl.estado > 0
	/*AND (cl.id_cliente IN (SELECT id_cliente FROM VENTA where estado = 1) 
	or CAST(cl.fecha_creacion AS DATE) >= '2018-09-01'  OR cl.id_cliente = '9C9E1FFD-A7D0-4628-B4B6-3292C8C42246'
	)*/
END


END

