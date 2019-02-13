/****** Object:  StoredProcedure [dbo].[pi_grupoCliente]    Script Date: 13/02/2019 6:36:29 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[pi_grupoCliente] 
@idUsuario uniqueidentifier,
@nombre  varchar(200),
@contacto  varchar(100),
@telefonoContacto varchar(50),
@emailContacto varchar(50),
@idCiudad uniqueidentifier,
/*Plazo credito*/
@plazoCreditoSolicitado int,
@plazoCreditoAprobado int,
@sobrePlazo int, 
/*Monto Crédito*/
@creditoSolicitado decimal(12,2),
@creditoAprobado decimal(12,2),
@sobreGiro decimal(12,2),

@observacionesCredito varchar(1000),
@observaciones varchar(1000),
@codigo VARCHAR(4),

@newId uniqueidentifier OUTPUT
AS
BEGIN TRAN

INSERT INTO GRUPO_CLIENTE
           (grupo
           ,[contacto]
		   ,codigo
		   ,telefono_contacto
		   ,email_contacto
		   ,[id_ciudad]
		   ,estado
		   ,[usuario_creacion]
		   ,[fecha_creacion]
		   ,[usuario_modificacion]
		   ,[fecha_modificacion],
		   /*Plazo credito*/
			plazo_credito_solicitado,
			plazo_credito_aprobado,
			sobre_plazo, 
			/*Monto Crédito*/
			credito_solicitado,
			credito_aprobado,
			sobre_giro, 

			observaciones_credito,
			observaciones
		   )
     VALUES
           (
		    @nombre,
		    @contacto,
			@codigo,
			@telefonoContacto,
            @emailContacto,
			@idCiudad,
			1,
			@idUsuario,
			GETDATE(),
			@idUsuario,
			GETDATE(),		
			@plazoCreditoSolicitado,
			@plazoCreditoAprobado,
			@sobrePlazo, 
			/*Monto Crédito*/
			@creditoSolicitado,
			@creditoAprobado,
			@sobreGiro, 		
			@observacionesCredito,
			@observaciones
			);



COMMIT



/****** Object:  Table [dbo].[GRUPO_CLIENTE]    Script Date: 13/02/2019 6:37:03 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GRUPO_CLIENTE](
	[id_grupo_cliente] [int] IDENTITY(1,1) NOT NULL,
	[codigo] [char](4) NULL,
	[grupo] [varchar](250) NULL,
	[codigo_negociacion_compra] [varchar](2) NULL,
	[id_responsable_comercial] [int] NULL,
	[canal] [varchar](20) NULL,
	[fecha_resgistro] [date] NULL,
	[observaciones] [varchar](250) NULL,
	[estado] [smallint] NULL,
	[usuario_creacion] [uniqueidentifier] NULL,
	[fecha_creacion] [datetime] NULL,
	[usuario_modificacion] [uniqueidentifier] NULL,
	[fecha_modificacion] [datetime] NULL,
	[contacto] [varchar](100) NULL,
	[plazo_credito_solicitado] [int] NULL,
	[plazo_credito_aprobado] [int] NULL,
	[id_ciudad] [uniqueidentifier] NULL,
	[telefono_contacto] [varchar](50) NULL,
	[email_contacto] [varchar](100) NULL,
	[credito_solicitado] [decimal](12, 2) NULL,
	[credito_aprobado] [decimal](12, 2) NULL,
	[sobre_giro] [decimal](12, 2) NULL,
	[sobre_plazo] [int] NULL,
	[observaciones_credito] [varchar](1000) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_grupo_cliente] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


