/* **** 1 **** */
CREATE PROCEDURE [dbo].[ps_getParametro] 
@codigo varchar(50) 
AS
BEGIN

SELECT valor 
FROM PARAMETRO 
WHERE estado = 1 AND codigo = @codigo; 

END
