alter table usuario 
add password_modificado datetime

/***************************************************************************/

create procedure ps_comparar_comtraseña 
(@id_usuario uniqueidentifier,
@password_actual varchar(50))
as begin 
SELECT CASE WHEN EXISTS (
    SELECT id_usuario FROM USUARIO WHERE estado = 1 and id_usuario=@id_usuario AND PWDCOMPARE ( @password_actual,password )  = 1
)
THEN CAST(1 AS BIT)
ELSE CAST(0 AS BIT)
END
end 

/***********************************************************************/

create procedure pu_cambiar_password
(@id_usuario uniqueidentifier,
@pass_nuevo varchar(50))
as begin 
update usuario set  password=pwdencrypt(@pass_nuevo) ,password_modificado=dbo.getlocaldate()  where id_usuario=@id_usuario
end  

/************************************************************************/
