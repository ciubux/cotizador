
/******************************* CREATE PROCEDURE - ps_reporte_contabilidad - reportes de ventas contabilidad *******************************************/
alter procedure ps_reporte_contabilidad
(@inicio_mes varchar(10),
@final_mes varchar(10),
@year varchar(4),
@mounth varchar(20))
as begin

declare @test varchar(max)
set @test ='
IF OBJECT_ID(''REPORTES_CONTABILIDAD.TMP_CPES_'+@mounth+''+@year+''',''U'') IS NOT NULL 
BEGIN
DROP TABLE REPORTES_CONTABILIDAD.TMP_CPES_'+@mounth+@year+'
END 
SELECT
CONCAT(dv.SERIE,''-'',dv.CORRELATIVO) AS CPE, 
CAST(dv.FEC_EMI AS DATE) AS FECHA_EMISION_CPE,
CASE TIP_CPE 
WHEN ''01'' THEN dv.MNT_TOT_GRV 
WHEN ''03'' THEN dv.MNT_TOT_GRV 
WHEN ''08'' THEN dv.MNT_TOT_GRV 
ELSE CAST(dv.MNT_TOT_GRV  AS decimal(12,2))*-1 END
AS SUB_TOTAL_GRABADA,
CASE TIP_CPE 
WHEN ''01'' THEN dv.MNT_TOT_INF 
WHEN ''03'' THEN dv.MNT_TOT_INF 
WHEN ''08'' THEN dv.MNT_TOT_INF 
ELSE CAST(dv.MNT_TOT_INF  AS decimal(12,2))*-1 END
AS SUB_TOTAL_INAFECTA,
CASE TIP_CPE 
WHEN ''01'' THEN dv.MNT_TOT_EXR 
WHEN ''03'' THEN dv.MNT_TOT_EXR 
WHEN ''08'' THEN dv.MNT_TOT_EXR 
ELSE CAST(dv.MNT_TOT_EXR  AS decimal(12,2))*-1 END
AS SUB_TOTAL_EXONERADA,
CASE TIP_CPE 
WHEN ''01'' THEN dv.MNT_TOT_GRT 
WHEN ''03'' THEN dv.MNT_TOT_GRT 
WHEN ''08'' THEN dv.MNT_TOT_GRT 
ELSE CAST(dv.MNT_TOT_GRT  AS decimal(12,2))*-1 END
AS SUB_TOTAL_GRATUITA,
CASE TIP_CPE 
WHEN ''01'' THEN dv.MNT_TOT_IMP 
WHEN ''03'' THEN dv.MNT_TOT_IMP 
WHEN ''08'' THEN dv.MNT_TOT_IMP 
ELSE CAST(dv.MNT_TOT_IMP  AS decimal(12,2))*-1 END
AS IGV_CPE,
CASE TIP_CPE 
WHEN ''01'' THEN dv.MNT_TOT 
WHEN ''03'' THEN dv.MNT_TOT 
WHEN ''08'' THEN dv.MNT_TOT 
ELSE CAST(dv.MNT_TOT  AS decimal(12,2))*-1 END
AS TOTAL_CPE
INTO REPORTES_CONTABILIDAD.TMP_CPES_'+@mounth+''+@year+'
FROM 
CPE_CABECERA_BE dv
WHERE estado = 1
AND ENVIADO_A_EOL = 1
AND CAST(FEC_EMI AS DATE) >= '''+@inicio_mes+'''
AND CAST(FEC_EMI AS DATE) <= '''+@final_mes+'''
AND COD_ESTD_SUNAT IN (''102'',''103'');


SELECT * FROM REPORTES_CONTABILIDAD.TMP_CPES_'+@mounth+''+@year+';

DROP VIEW if exists [REPORTES_CONTABILIDAD].[V_VENTAS_VALIDAS_TIPOS_VENTA_EXTORNO_HASTA_'+@mounth+@year+'];


DROP VIEW if exists [REPORTES_CONTABILIDAD].[V_REPORTE_VENTAS_HASTA_'+@mounth+@year+'];
'
exec sp_sqlexec @test


declare @test2 varchar(max)
set @test2='
CREATE VIEW [REPORTES_CONTABILIDAD].[V_VENTAS_VALIDAS_TIPOS_VENTA_EXTORNO_HASTA_'+@mounth+@year+']
AS SELECT * FROM V_VENTAS_VALIDAS_TIPOS_VENTA_EXTORNO
WHERE CAST(FECHA AS DATE) >= ''2018-01-01''
AND CAST(FECHA AS DATE) <= '''+@final_mes+'''
'
exec sp_sqlexec @test2

declare @test3 varchar(max)
set @test3='
CREATE VIEW [REPORTES_CONTABILIDAD].[V_REPORTE_VENTAS_HASTA_'+@mounth+@year+'] AS
SELECT 
CASE ve.tipo WHEN ''MS'' THEN
	CASE CONCAT(''G'',ma.serie_documento,''-'',ma.numero_documento) WHEN ''G-'' THEN ''-''
	ELSE CONCAT(''G'',ma.serie_documento,''-'',ma.numero_documento) END
ELSE 
	CASE CONCAT(''NI'',ma.serie_documento,''-'',ma.numero_documento) WHEN ''NI-'' THEN ''-''
	ELSE CONCAT(''NI'',ma.serie_documento,''-'',ma.numero_documento)  END
END AS GUIA,
CAST(ma.fecha_emision AS DATE) AS FECHA_EMISION_GUIA,
CONCAT(dv.SERIE,''-'',dv.CORRELATIVO) AS CPE, 
CAST(dv.FEC_EMI AS DATE) AS FECHA_EMISION_CPE,
dv.MNT_TOT_VAL_VTA AS SUB_TOTAL_CPE,
dv.MNT_TOT_IMP AS IGV_CPE,
dv.MNT_TOT AS TOTAL_CPE,
ve.tipo AS TIPO_TRANSACCION, 
ve.motivo  AS MOTIVO_TRANSACCION,
CAST(ve.fecha AS DATE) AS FECHA_TRANSACCION,
ve.sub_total AS SUB_TOTAL_VENTA, 
cl.razon_social CLIENTE, 
cl.ruc DOCUMENTO,
cl.codigo CODIGO,
CASE CONCAT(''G'',mae.serie_documento,''-'',mae.numero_documento) WHEN ''G-'' THEN ''-''
ELSE CONCAT(''G'',mae.serie_documento,''-'',mae.numero_documento)  END AS GUIA_EXTORNADA,
CAST(mae.fecha_emision AS DATE) AS FECHA_EMISION_GUIA_EXTORNADA
 FROM 
REPORTES_CONTABILIDAD.V_VENTAS_VALIDAS_TIPOS_VENTA_EXTORNO_HASTA_'+@mounth+''+@year+'  ve
LEFT JOIN CLIENTE cl ON cl.id_cliente = ve.id_cliente
LEFT JOIN MOVIMIENTO_ALMACEN ma ON ve.id_movimiento_almacen = ma.id_movimiento_almacen
LEFT JOIN V_CPE_CABECERA_BE_ACEPTADAS dv ON ve.id_documento_venta = dv.id_cpe_cabecera_be
LEFT JOIN MOVIMIENTO_ALMACEN mae ON mae.id_movimiento_almacen = ma.id_movimiento_almacen_extornado

'
exec sp_sqlexec @test3


declare @test4 varchar(max)
set @test4=
'
IF OBJECT_ID(''REPORTES_CONTABILIDAD.TMP_VENTAS_'+@mounth+''+@year+'_FACTURADAS_EN_UN_MES_POSTERIOR'',''U'') IS NOT NULL 
BEGIN
DROP TABLE REPORTES_CONTABILIDAD.TMP_VENTAS_'+@mounth+''+@year+'_FACTURADAS_EN_UN_MES_POSTERIOR
END 

SELECT *
INTO REPORTES_CONTABILIDAD.TMP_VENTAS_'+@mounth+''+@year+'_FACTURADAS_EN_UN_MES_POSTERIOR
FROM REPORTES_CONTABILIDAD.V_REPORTE_VENTAS_HASTA_'+@mounth+''+@year+'
WHERE CAST(FECHA_TRANSACCION AS DATE) >= '''+@inicio_mes+'''
AND CAST(FECHA_TRANSACCION AS DATE) <= '''+@final_mes+'''
AND CAST(FECHA_EMISION_CPE AS DATE) > '''+@final_mes+'''
AND TIPO_TRANSACCION = ''MS''

SELECT * FROM REPORTES_CONTABILIDAD.TMP_VENTAS_'+@mounth+''+@year+'_FACTURADAS_EN_UN_MES_POSTERIOR 

-----------------------------------------
IF OBJECT_ID(''REPORTES_CONTABILIDAD.TMP_VENTAS_'+@mounth+''+@year+'_AUN_NO_FACTURADAS'',''U'') IS NOT NULL 
BEGIN
DROP TABLE REPORTES_CONTABILIDAD.TMP_VENTAS_'+@mounth+''+@year+'_AUN_NO_FACTURADAS
END 


SELECT * 
INTO REPORTES_CONTABILIDAD.TMP_VENTAS_'+@mounth+''+@year+'_AUN_NO_FACTURADAS
FROM REPORTES_CONTABILIDAD.V_REPORTE_VENTAS_HASTA_'+@mounth+''+@year+'
WHERE CAST(FECHA_TRANSACCION AS DATE) >= '''+@inicio_mes+'''
AND CAST(FECHA_TRANSACCION AS DATE) <= '''+@final_mes+'''
AND FECHA_EMISION_CPE IS NULL
AND TIPO_TRANSACCION = ''MS''

SELECT * FROM REPORTES_CONTABILIDAD.TMP_VENTAS_'+@mounth+''+@year+'_AUN_NO_FACTURADAS

---------------------------------

IF OBJECT_ID(''REPORTES_CONTABILIDAD.TMP_VENTAS_ANTERIORES_A_'+@mounth+''+@year+'_FACTURADAS_EN_'+@mounth+''+@year+''',''U'') IS NOT NULL 
BEGIN
DROP TABLE REPORTES_CONTABILIDAD.TMP_VENTAS_ANTERIORES_A_'+@mounth+''+@year+'_FACTURADAS_EN_'+@mounth+''+@year+'
END 

SELECT *
INTO REPORTES_CONTABILIDAD.TMP_VENTAS_ANTERIORES_A_'+@mounth+''+@year+'_FACTURADAS_EN_'+@mounth+''+@year+'
FROM REPORTES_CONTABILIDAD.V_REPORTE_VENTAS_HASTA_'+@mounth+''+@year+'
WHERE CAST(FECHA_TRANSACCION AS DATE) < '''+@inicio_mes+'''
AND CAST(FECHA_EMISION_CPE AS DATE) >= '''+@inicio_mes+'''
AND CAST(FECHA_EMISION_CPE AS DATE) <= '''+@final_mes+'''
AND TIPO_TRANSACCION = ''MS''

SELECT * FROM REPORTES_CONTABILIDAD.TMP_VENTAS_ANTERIORES_A_'+@mounth+''+@year+'_FACTURADAS_EN_'+@mounth+''+@year+'



IF OBJECT_ID(''REPORTES_CONTABILIDAD.TMP_VENTAS_EXTORNADAS_'+@mounth+''+@year+''',''U'') IS NOT NULL 
BEGIN
DROP TABLE REPORTES_CONTABILIDAD.TMP_VENTAS_EXTORNADAS_'+@mounth+''+@year+'
END 


SELECT *
INTO REPORTES_CONTABILIDAD.TMP_VENTAS_EXTORNADAS_'+@mounth+''+@year+'
FROM REPORTES_CONTABILIDAD.V_REPORTE_VENTAS_HASTA_'+@mounth+''+@year+'
WHERE CAST(FECHA_TRANSACCION AS DATE) >= '''+@inicio_mes+'''
AND CAST(FECHA_TRANSACCION AS DATE) <= '''+@final_mes+'''
AND FECHA_EMISION_CPE IS NULL
AND TIPO_TRANSACCION = ''NI''

SELECT * FROM REPORTES_CONTABILIDAD.TMP_VENTAS_EXTORNADAS_'+@mounth+''+@year+''

exec sp_sqlexec @test4

end