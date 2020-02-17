
/****************** Alter Table - Se agrega la columna editable para la tabla parametro ******************/
alter table parametro 
add editable int 

/******************* CREATE pu_parametros - Modifica el valor y descripcion de un parametro ****************************/
create procedure pu_parametros
(@id_parametro uniqueidentifier , 
@valor varchar(200),
@descripcion varchar(200),
@usuario_modificacion uniqueidentifier)
as begin 
update PARAMETRO 
set 
descripcion=@descripcion,
valor=@valor,
fecha_modificacion=dbo.getlocaldate(),
usuario_modifiacion=@usuario_modificacion
where  id_parametro=@id_parametro
end  

/********************** CREATE ps_lista_parametros - Lista la tabla parametro ********************************/

alter procedure ps_lista_parametros
(@codigo varchar(50))
as begin 
if(@codigo is null)
begin 
select * from PARAMETRO where (editable=1 or editable is null) order by codigo asc
end
else 
select * from PARAMETRO where codigo like '%'+@codigo+'%' and (editable=1 or editable is null) order by codigo asc
end  

/******************************************************/
