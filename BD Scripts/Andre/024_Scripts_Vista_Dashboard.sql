/*****************************	CREATE PERMISO  P880 **********************************************/
insert into  PERMISO(id_permiso,estado,codigo,descripcion_corta,id_categoria_permiso) values(102,1,'P880','modifica_vistas_dashboard',7)

/*****************************	CREATE TABLE VISTA_DASHBOARD  **********************************************/
create table  VISTA_DASHBOARD
(
id_vista_dashboard int,
id_tipo_vista_dashboard int,
codigo varchar(20),
nombre varchar(100),
descripcion varchar(500),
bloques_ancho int ,
alto_px  int ,
fecha_creacion datetime,
fecha_modificacion datetime,
usuario_creacion uniqueidentifier,
usuario_modificacion uniqueidentifier,
estado int 
)


/*****************************	CREATE TABLE TIPO_VISTA_DASHBOARD  **********************************************/
create table TIPO_VISTA_DASHBOARD
(
id_tipo_vista_dashboard int,
id_tipo_padre int,
nombre varchar(100),
orden int,
fecha_creacion datetime,
fecha_modificacion datetime,
usuario_creacion  uniqueidentifier,
usuario_modificacion uniqueidentifier)


/*****************************	CREATE TABLE ROL_VISTA_DASHBOARD  **********************************************/

create table ROL_VISTA_DASHBOARD 
(
id_rol int,
id_vista_dashboard int ,
orden int,
fecha_modificacion datetime,
usuario_modificacion uniqueidentifier)

 /*****************************	CREATE ps_vista_dashboard - Lista las vistas dashboard  **********************************************/

create procedure ps_vista_dashboard
(@codigo varchar(50),@nombre varchar(100))
as begin 
select id_vista_dashboard,id_tipo_vista_dashboard,codigo,nombre,descripcion,bloques_ancho,alto_px,estado from VISTA_DASHBOARD where (codigo like concat('%',@codigo,'%')) and (nombre like concat('%',@nombre,'%'))
end 

/*****************************	CREATE pi_vista_dashboard - Inserta una vista dashboard  **********************************************/

create procedure pi_vista_dashboard
(
@id_tipo_vista_dashboard int,
@codigo varchar(50),
@nombre varchar(50),
@descripcion varchar(50),
@bloques_ancho int,
@alto_px int,
@id_usuario uniqueidentifier)
as begin 
insert into  VISTA_DASHBOARD (id_vista_dashboard,id_tipo_vista_dashboard,codigo,nombre,descripcion,bloques_ancho,alto_px,fecha_creacion,fecha_modificacion,usuario_creacion,usuario_modificacion,estado) 
values((select MAX(VISTA_DASHBOARD.id_vista_dashboard)+1 from VISTA_DASHBOARD),@id_tipo_vista_dashboard,@codigo,@nombre,@descripcion,@bloques_ancho,@alto_px,dbo.getlocaldate(),dbo.getlocaldate(),@id_usuario,@id_usuario,1)
end 


/*****************************	CREATE PS_TIPO_VISTA_DASHBOARD - Lista los tipos de vista Dashboard  **********************************************/

create procedure PS_TIPO_VISTA_DASHBOARD
as begin 
select id_tipo_vista_dashboard,nombre,id_tipo_padre from TIPO_VISTA_DASHBOARD 
end 


/*****************************	CREATE pu_vista_dashboard - Actualiza una vista Dashboard  **********************************************/

create procedure pu_vista_dashboard
(@id_vista_dashboard int,
@id_tipo_vista_dashboard int,
@codigo varchar(50),
@nombre varchar(50),
@descripcion varchar(50),
@bloques_ancho int,
@alto_px int,
@id_usuario uniqueidentifier)
as begin 
update VISTA_DASHBOARD set id_tipo_vista_dashboard=@id_tipo_vista_dashboard,codigo=@codigo,nombre=@nombre,descripcion=@descripcion,bloques_ancho=@bloques_ancho,alto_px=@alto_px,usuario_creacion=@id_usuario,usuario_modificacion=@id_usuario,fecha_creacion=dbo.getlocaldate(),fecha_modificacion=dbo.getlocaldate(),estado=1
where id_vista_dashboard=@id_vista_dashboard
end 

/*****************************	CREATE ps_detalle_vista_dashboard - Obtiene los detalles de  una vista Dashboard  **********************************************/

create procedure ps_detalle_vista_dashboard
(@id_vista_dashboard int)
as begin 
select id_vista_dashboard,id_tipo_vista_dashboard,codigo,nombre,descripcion,bloques_ancho,alto_px from VISTA_DASHBOARD where id_vista_dashboard=@id_vista_dashboard
end 


