/**************************** CREATE ps_grupos_mienbros_exportar - Obtiene la busqueda de grupos de clientes con mienbros para exportar a excel **********************/
create procedure [dbo].[ps_grupos_mienbros_exportar]
(@codigo varchar(4),
@idCiudad uniqueIdentifier,
@grupo varchar(50),
@estado int)
as begin
	
	select gc.codigo, gc.grupo, c.codigo as codigo_cliente, c.razon_social 
	from grupo_cliente gc
	left join cliente c on gc.id_grupo_cliente = c.id_grupo_cliente	
	LEFT JOIN CIUDAD ci on ci.id_ciudad = gc.id_ciudad
	where gc.estado = 1
	and (c.estado is null or c.estado=1) and ((razon_social is null or razon_social is not null) and (c.codigo is null or c.codigo is not null))
	and
	gc.codigo LIKE @codigo 
	OR
	(@codigo = '' AND
	(
	REPLACE( REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(gc.grupo, 'á', 'a'), 'é','e'), 'í', 'i'), 'ó', 'o'), 'ú','u'),'"',' ')  LIKE '%'+@grupo+'%' 
	OR @grupo IS NULL
	OR @grupo = ''
	)
	AND (ci.id_ciudad = @idCiudad OR @idCiudad = '00000000-0000-0000-0000-000000000000')
	AND gc.estado = @estado
	)	
end