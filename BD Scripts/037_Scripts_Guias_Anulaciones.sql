/* **** 1 **** */
ALTER TABLE MOVIMIENTO_ALMACEN 
add anulacion_solicitada int null;


/* **** 2 **** */
ALTER TABLE MOVIMIENTO_ALMACEN 
add usuario_solicita_anulacion uniqueidentifier null;


/* **** 3 **** */
ALTER TABLE MOVIMIENTO_ALMACEN 
add usuario_anulacion uniqueidentifier null;


/* **** 4 **** */
ALTER TABLE MOVIMIENTO_ALMACEN 
add anulacion_aprobada int null;


/* **** 5 **** */
ALTER TABLE MOVIMIENTO_ALMACEN 
add usuario_aprueba_anulacion uniqueidentifier null;



/* **** 6 **** */
ALTER TABLE MOVIMIENTO_ALMACEN 
add comentario_solicitud_anulacion varchar(500) null;


/* **** 7 **** */
ALTER PROCEDURE [dbo].[pu_jobDiario] 
AS
BEGIN

UPDATE CPE_CABECERA_BE 
SET permite_anulacion = 0
where CAST(FEC_EMI AS DATE) <DATEADD(day, -5, [dbo].[getlocaldate]()) and permite_anulacion = 1;

UPDATE MOVIMIENTO_ALMACEN 
SET permite_anulacion = 0
where CAST(fecha_emision AS DATE) <DATEADD(day, -4, [dbo].[getlocaldate]()) and permite_anulacion = 1;

END





/* **** 2 **** */












/* **** 3 **** */






















