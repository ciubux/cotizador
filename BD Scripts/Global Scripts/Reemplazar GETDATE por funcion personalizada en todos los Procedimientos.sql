/* ****  1 **** */
CREATE FUNCTION dbo.getlocaldate()

RETURNS DATETIME

AS
BEGIN

     RETURN dateadd(hh,-5,GETDATE());

END

SELECT dbo.getlocaldate() ;




/* ****  2 **** */
DECLARE C CURSOR FOR
        SELECT sm.definition, so.type
        FROM   sys.objects so
        JOIN   sys.all_sql_modules sm ON sm.object_id = so.object_id
        WHERE  so.type IN ('P')
        ORDER BY so.name
DECLARE @SQL NVARCHAR(MAX), @ojtype NVARCHAR(MAX)
OPEN C
FETCH NEXT FROM C INTO @SQL, @ojtype
WHILE @@FETCH_STATUS = 0 BEGIN
    SET @SQL = REPLACE(@SQL, 'CREATE PROCEDURE', 'ALTER PROCEDURE') 
    SET @SQL = REPLACE(@SQL, 'GETDATE()', '[dbo].[getlocaldate]()') 
    --PRINT @SQL
    EXEC (@SQL)
    FETCH NEXT FROM C INTO @SQL, @ojtype
END
CLOSE C
DEALLOCATE C