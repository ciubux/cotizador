﻿using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Linq;

namespace DataLayer
{
    public class CotizacionDAL : DaoBase
    {
        public CotizacionDAL(IDalSettings settings) : base(settings)
        {
        }

        public CotizacionDAL() : this(new CotizadorSettings())
        {
        }


        

        public void InsertCotizacion(Cotizacion cotizacion)
        {
            var objCommand = GetSqlCommand("pi_cotizacion");

           // InputParameterAdd.Varchar(objCommand, "codigo", "0000000001");
            InputParameterAdd.DateTime(objCommand, "fecha", cotizacion.fecha);
            InputParameterAdd.DateTime(objCommand, "fechaLimiteValidezOferta", cotizacion.fechaLimiteValidezOferta);
            InputParameterAdd.DateTime(objCommand, "fechaInicioVigenciaPrecios", cotizacion.fechaInicioVigenciaPrecios);
            InputParameterAdd.DateTime(objCommand, "fechaFinVigenciaPrecios", cotizacion.fechaFinVigenciaPrecios);
            InputParameterAdd.SmallInt(objCommand, "incluidoIgv", short.Parse((cotizacion.incluidoIGV?1:0).ToString()));
            InputParameterAdd.Int(objCommand, "consideraCantidades", (int)cotizacion.considerarCantidades);

            //Si no se cuenta con idCliente entonces se registra el grupo
            if (cotizacion.cliente.idCliente == Guid.Empty)
            {
                InputParameterAdd.Guid(objCommand, "idCliente", null);
                InputParameterAdd.Int(objCommand, "idGrupoCliente", cotizacion.grupo.idGrupoCliente);
                InputParameterAdd.Guid(objCommand, "idCiudad", cotizacion.grupo.ciudad.idCiudad);
            }
            else
            {
                InputParameterAdd.Guid(objCommand, "idCliente", cotizacion.cliente.idCliente);
                InputParameterAdd.Int(objCommand, "idGrupoCliente", null);
                InputParameterAdd.Guid(objCommand, "idCiudad", cotizacion.ciudad.idCiudad);
            }
         
            //porcentajeFlete
            InputParameterAdd.Decimal(objCommand, "porcentajeFlete", cotizacion.flete);
            InputParameterAdd.Decimal(objCommand, "igv", cotizacion.igv);
            InputParameterAdd.Decimal(objCommand, "total", cotizacion.montoTotal);
            InputParameterAdd.Varchar(objCommand, "observaciones", cotizacion.observaciones);
            InputParameterAdd.Guid(objCommand, "idUsuario", cotizacion.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "contacto", cotizacion.contacto);
            InputParameterAdd.Bit(objCommand, "esPagoContado", cotizacion.esPagoContado);
            InputParameterAdd.Int(objCommand, "tipoCotizacion", (int)cotizacion.tipoCotizacion);

            InputParameterAdd.SmallInt(objCommand, "mostrarCodigoProveedor", short.Parse((cotizacion.mostrarCodigoProveedor ? 1 : 0).ToString()));
            InputParameterAdd.Int(objCommand, "mostrarValidezOfertaDias", cotizacion.mostrarValidezOfertaEnDias);
            InputParameterAdd.Int(objCommand, "estado", (int)cotizacion.seguimientoCotizacion.estado);

            InputParameterAdd.SmallInt(objCommand, "fechaEsModificada", (short)(cotizacion.fechaEsModificada?1:0));
            InputParameterAdd.Bit(objCommand, "aplicaSedes", cotizacion.aplicaSedes);
            InputParameterAdd.Varchar(objCommand, "observacionSeguimientoCotizacion", cotizacion.seguimientoCotizacion.observacion);


            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            OutputParameterAdd.BigInt(objCommand, "codigo");
            ExecuteNonQuery(objCommand);

            cotizacion.idCotizacion = (Guid)objCommand.Parameters["@newId"].Value;
            cotizacion.codigo = (Int64)objCommand.Parameters["@codigo"].Value;


            foreach (CotizacionDetalle cotizacionDetalle in cotizacion.cotizacionDetalleList)
            {
                cotizacionDetalle.idCotizacion = cotizacion.idCotizacion;
                this.InsertCotizacionDetalle(cotizacionDetalle, cotizacion.usuario);
            }


        }

        public void UpdateCotizacion(Cotizacion cotizacion)
        {
            var objCommand = GetSqlCommand("pu_cotizacion");
       

            // InputParameterAdd.Varchar(objCommand, "codigo", "0000000001");
            InputParameterAdd.DateTime(objCommand, "fecha", cotizacion.fecha);
            InputParameterAdd.DateTime(objCommand, "fechaLimiteValidezOferta", cotizacion.fechaLimiteValidezOferta);
            InputParameterAdd.DateTime(objCommand, "fechaInicioVigenciaPrecios", cotizacion.fechaInicioVigenciaPrecios);
            InputParameterAdd.DateTime(objCommand, "fechaFinVigenciaPrecios", cotizacion.fechaFinVigenciaPrecios);
            InputParameterAdd.DateTime(objCommand, "fechaModificacion", cotizacion.fechaModificacion);
            InputParameterAdd.SmallInt(objCommand, "incluidoIgv", short.Parse((cotizacion.incluidoIGV ? 1 : 0).ToString()));
            InputParameterAdd.Int(objCommand, "consideraCantidades", (int)cotizacion.considerarCantidades);

            //Si no se cuenta con idCliente entonces se registra el grupo
            if (cotizacion.cliente.idCliente == Guid.Empty)
            {
                InputParameterAdd.Guid(objCommand, "idCliente", null);
                InputParameterAdd.Int(objCommand, "idGrupoCliente", cotizacion.grupo.idGrupoCliente);
                InputParameterAdd.Guid(objCommand, "idCiudad", cotizacion.grupo.ciudad.idCiudad);
            }
            else
            {
                InputParameterAdd.Guid(objCommand, "idCliente", cotizacion.cliente.idCliente);
                InputParameterAdd.Int(objCommand, "idGrupoCliente", null);
                InputParameterAdd.Guid(objCommand, "idCiudad", cotizacion.ciudad.idCiudad);
            }
            //porcentajeFlete
            InputParameterAdd.Decimal(objCommand, "porcentajeFlete", cotizacion.flete);
            InputParameterAdd.Decimal(objCommand, "igv", cotizacion.igv);
            InputParameterAdd.Decimal(objCommand, "total", cotizacion.montoTotal);
            InputParameterAdd.Varchar(objCommand, "observaciones", cotizacion.observaciones);
            InputParameterAdd.Guid(objCommand, "idUsuario", cotizacion.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "contacto", cotizacion.contacto);
            InputParameterAdd.BigInt(objCommand, "codigo", cotizacion.codigo);
            InputParameterAdd.Int(objCommand, "mostrarCodigoProveedor", short.Parse((cotizacion.mostrarCodigoProveedor ? 1 : 0).ToString()));

            InputParameterAdd.Int(objCommand, "estado", (int)cotizacion.seguimientoCotizacion.estado);
            InputParameterAdd.Int(objCommand, "mostrarValidezOfertaDias", cotizacion.mostrarValidezOfertaEnDias);

            InputParameterAdd.Bit(objCommand, "esPagoContado", cotizacion.esPagoContado);
            InputParameterAdd.Int(objCommand, "tipoCotizacion", (int)cotizacion.tipoCotizacion);

            InputParameterAdd.SmallInt(objCommand, "fechaEsModificada", (short)(cotizacion.fechaEsModificada ? 1 : 0));
            InputParameterAdd.Varchar(objCommand, "observacionSeguimientoCotizacion", cotizacion.seguimientoCotizacion.observacion);
            InputParameterAdd.Bit(objCommand, "aplicaSedes", cotizacion.aplicaSedes);

            OutputParameterAdd.DateTime(objCommand, "fechaModificacionActual");
            ExecuteNonQuery(objCommand);

            DateTime fechaModifiacionActual = (DateTime)objCommand.Parameters["@fechaModificacionActual"].Value;

           

        //    ExecuteNonQuery(objCommand);

   
            DateTime date1 = new DateTime(fechaModifiacionActual.Year, fechaModifiacionActual.Month, fechaModifiacionActual.Day, fechaModifiacionActual.Hour, fechaModifiacionActual.Minute, fechaModifiacionActual.Second);
            DateTime date2 = new DateTime(cotizacion.fechaModificacion.Year, cotizacion.fechaModificacion.Month, cotizacion.fechaModificacion.Day, cotizacion.fechaModificacion.Hour, cotizacion.fechaModificacion.Minute, cotizacion.fechaModificacion.Second);
            /*
            int result = DateTime.Compare(date1, date2);
            if (result != 0)
            {
                //No se puede actualizar la cotización si las fechas son distintas
                //throw new Exception("CotizacionDesactualizada");
            }
            else
            { 
            */
                foreach (CotizacionDetalle cotizacionDetalle in cotizacion.cotizacionDetalleList)
                {
                    cotizacionDetalle.idCotizacion = cotizacion.idCotizacion;
                    this.InsertCotizacionDetalle(cotizacionDetalle, cotizacion.usuario);
                }
            //}

        }


        public void InsertCotizacionDetalle(CotizacionDetalle cotizacionDetalle, Usuario usuario)
        {
            var objCommand = GetSqlCommand("pi_cotizacionDetalle");
            InputParameterAdd.Guid(objCommand, "idCotizacion", cotizacionDetalle.idCotizacion);
            InputParameterAdd.Guid(objCommand, "idProducto", cotizacionDetalle.producto.idProducto);
            InputParameterAdd.Int(objCommand, "cantidad", cotizacionDetalle.cantidad);
            //Siempre se almacena el precio sin igv de la unidad estandar
            InputParameterAdd.Decimal(objCommand, "precioSinIGV", cotizacionDetalle.producto.precioSinIgv);
            //InputParameterAdd.Decimal(objCommand, "precioProvinciaSinIGV", cotizacionDetalle.producto.precioProvinciaSinIgv);
            //Siempre se almacena el costo sin igv de la unidad estandar
            InputParameterAdd.Decimal(objCommand, "costoSinIGV", cotizacionDetalle.producto.costoSinIgv);

            if (cotizacionDetalle.esPrecioAlternativo)
            {
                InputParameterAdd.Decimal(objCommand, "equivalencia", cotizacionDetalle.ProductoPresentacion.Equivalencia);
            }
            else
            {
                InputParameterAdd.Decimal(objCommand, "equivalencia", 1);
            }         

            InputParameterAdd.Varchar(objCommand, "unidad", cotizacionDetalle.unidad);
            InputParameterAdd.Decimal(objCommand, "porcentajeDescuento", cotizacionDetalle.porcentajeDescuento);
            InputParameterAdd.Decimal(objCommand, "precioNeto", cotizacionDetalle.precioNeto);
            InputParameterAdd.Int(objCommand, "esPrecioAlternativo", cotizacionDetalle.esPrecioAlternativo?1:0);
            InputParameterAdd.Guid(objCommand, "idUsuario", usuario.idUsuario);
            InputParameterAdd.Decimal(objCommand, "flete", cotizacionDetalle.flete);
            InputParameterAdd.Varchar(objCommand, "observaciones", cotizacionDetalle.observacion);
            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");

            if (cotizacionDetalle.esPrecioAlternativo)
            {
                InputParameterAdd.Decimal(objCommand, "idProductoPresentacion", cotizacionDetalle.ProductoPresentacion.IdProductoPresentacion);
            }
            else
            {
                InputParameterAdd.Decimal(objCommand, "idProductoPresentacion", 1);
            }

            ExecuteNonQuery(objCommand);

            cotizacionDetalle.idCotizacionDetalle = (Guid)objCommand.Parameters["@newId"].Value;
        }

        public Cotizacion aprobarCotizacion(Cotizacion cotizacion)
        {
            var objCommand = GetSqlCommand("pu_aprobarCotizacion");

            InputParameterAdd.BigInt(objCommand, "codigo", cotizacion.codigo);
            InputParameterAdd.Guid(objCommand, "idUsuario", cotizacion.usuario.idUsuario);
            InputParameterAdd.Int(objCommand, "estado", (int)cotizacion.seguimientoCotizacion.estado);
            InputParameterAdd.Varchar(objCommand, "observacion", cotizacion.seguimientoCotizacion.observacion);

            ExecuteNonQuery(objCommand);
            return cotizacion;

        }


       




        public Cotizacion SelectCotizacion(Cotizacion cotizacion, Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_cotizacion");
            InputParameterAdd.BigInt(objCommand, "codigo", cotizacion.codigo);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable cotizacionDataTable = dataSet.Tables[0];
            DataTable cotizacionDetalleDataTable = dataSet.Tables[1];

            //CABECERA DE COTIZACIÓN
            foreach (DataRow row in cotizacionDataTable.Rows)
            {
                cotizacion.idCotizacion = Converter.GetGuid(row,"id_cotizacion");
                cotizacion.fecha = Converter.GetDateTime(row, "fecha");
                cotizacion.fechaLimiteValidezOferta = Converter.GetDateTime(row, "fecha_limite_validez_oferta");
                cotizacion.fechaInicioVigenciaPrecios = Converter.GetDateTimeNullable(row, "fecha_inicio_vigencia_precios");
                cotizacion.fechaFinVigenciaPrecios = Converter.GetDateTimeNullable(row, "fecha_fin_vigencia_precios");
                cotizacion.incluidoIGV = Converter.GetBool(row, "incluido_igv");
                cotizacion.considerarCantidades =   (Cotizacion.OpcionesConsiderarCantidades)Converter.GetInt(row, "considera_cantidades");
                cotizacion.mostrarValidezOfertaEnDias = Converter.GetInt(row, "mostrar_validez_oferta_dias");
                cotizacion.flete = Converter.GetDecimal(row, "porcentaje_flete");
                cotizacion.igv = Converter.GetDecimal(row, "igv");
                cotizacion.montoTotal = Converter.GetDecimal(row, "total");
                cotizacion.observaciones = Converter.GetString(row, "observaciones");
                cotizacion.mostrarCodigoProveedor = Converter.GetBool(row, "mostrar_codigo_proveedor");
                cotizacion.contacto = Converter.GetString(row, "contacto");
                cotizacion.montoSubTotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacion.montoTotal / (1+cotizacion.igv)));
                cotizacion.montoIGV = cotizacion.montoTotal - cotizacion.montoSubTotal;
                cotizacion.fechaModificacion = Converter.GetDateTime(row, "fecha_modificacion");
                cotizacion.fechaEsModificada = Converter.GetBool(row, "fecha_Es_Modificada");
                cotizacion.aplicaSedes = Converter.GetBool(row, "aplica_sedes");
                cotizacion.maximoPorcentajeDescuentoPermitido = Converter.GetDecimal(row, "maximo_porcentaje_descuento");
                cotizacion.esPagoContado = Converter.GetBool(row, "es_pago_contado");
                cotizacion.tipoCotizacion = (Cotizacion.TiposCotizacion)Converter.GetInt(row, "tipo_cotizacion");
                //CLIENTE
                cotizacion.cliente = new Cliente();
                cotizacion.cliente.codigo = Converter.GetString(row, "codigo");
                cotizacion.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                cotizacion.cliente.habilitadoModificarDireccionEntrega = Converter.GetBool(row, "habilitado_modificar_direccion_entrega");
                cotizacion.cliente.razonSocial = Converter.GetString(row, "razon_social");
                cotizacion.cliente.ruc = Converter.GetString(row, "ruc");
                cotizacion.cliente.sedePrincipal = Converter.GetBool(row, "sede_principal");
                cotizacion.cliente.plazoCreditoSolicitado = (DocumentoVenta.TipoPago)Converter.GetInt(row, "plazo_credito_solicitado");
                cotizacion.cliente.tipoPagoFactura =  (DocumentoVenta.TipoPago)Converter.GetInt(row, "tipo_pago_factura");
                if (cotizacion.cliente.sedePrincipal) {
                    ClienteDAL dalCliente = new ClienteDAL();
                    cotizacion.cliente.sedeList = dalCliente.getSedes(cotizacion.cliente.ruc);
                }
                //GRUPO CLIENTE
                cotizacion.grupo = new GrupoCliente();
                cotizacion.grupo.idGrupoCliente = Converter.GetInt(row, "id_grupo_cliente");
                cotizacion.grupo.codigo = Converter.GetString(row, "codigo_grupo");
                cotizacion.grupo.nombre = Converter.GetString(row, "nombre_grupo");
                cotizacion.grupo.contacto = Converter.GetString(row, "contacto_grupo");
                cotizacion.grupo.plazoCreditoSolicitado = (DocumentoVenta.TipoPago)Converter.GetInt(row, "plazo_credito_solicitado");
                cotizacion.grupo.plazoCreditoAprobado = (DocumentoVenta.TipoPago)Converter.GetInt(row, "plazo_credito_aprobado");
                //CIUDAD
                cotizacion.ciudad = new Ciudad();
                cotizacion.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                cotizacion.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");
                cotizacion.ciudad.esProvincia = Converter.GetBool(row, "es_provincia");
                cotizacion.grupo.ciudad = cotizacion.ciudad;
                //USUARIO
                cotizacion.usuario = new Usuario();
                cotizacion.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                cotizacion.usuario.cargo = Converter.GetString(row, "cargo");
                cotizacion.usuario.contacto = Converter.GetString(row, "contacto_usuario");
                cotizacion.usuario.email = Converter.GetString(row, "email");           
                //ESTADO
                cotizacion.seguimientoCotizacion = new SeguimientoCotizacion();
                cotizacion.seguimientoCotizacion.estado = (SeguimientoCotizacion.estadosSeguimientoCotizacion)Converter.GetInt(row, "estado_seguimiento");
                cotizacion.seguimientoCotizacion.observacion = Converter.GetString(row, "observacion_seguimiento");
                cotizacion.seguimientoCotizacion.usuario = new Usuario();
                cotizacion.seguimientoCotizacion.usuario.idUsuario = Converter.GetGuid(row, "id_usuario_seguimiento");
                cotizacion.seguimientoCotizacion.usuario.nombre = Converter.GetString(row, "usuario_seguimiento");

            }


            cotizacion.cotizacionDetalleList = new List<CotizacionDetalle>();
            //DETALLE DE COTIZACIÓN
            foreach (DataRow row in cotizacionDetalleDataTable.Rows)
            {
                CotizacionDetalle cotizacionDetalle = new CotizacionDetalle(usuario.visualizaCostos, usuario.visualizaMargen);
                cotizacionDetalle.producto = new Producto();
                cotizacionDetalle.idCotizacionDetalle = Converter.GetGuid(row, "id_cotizacion_detalle");
                cotizacionDetalle.cantidad = Converter.GetInt(row, "cantidad");
                cotizacionDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                cotizacionDetalle.flete = Converter.GetDecimal(row, "flete");
                cotizacionDetalle.producto.proveedor = Converter.GetString(row, "proveedor");
                ///En caso corresponda a una presentación que no es la estandar se debe instanciar el objeto ProductoPresentacion
                if (cotizacionDetalle.esPrecioAlternativo)
                {
                    cotizacionDetalle.ProductoPresentacion = new ProductoPresentacion();
                    cotizacionDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                    cotizacionDetalle.ProductoPresentacion.IdProductoPresentacion = Converter.GetInt(row, "id_producto_presentacion");
                }

                cotizacionDetalle.unidad = Converter.GetString(row, "unidad");
                cotizacionDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                cotizacionDetalle.producto.sku = Converter.GetString(row, "sku");
                cotizacionDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                cotizacionDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                cotizacionDetalle.producto.proveedor = Converter.GetString(row, "proveedor");
                cotizacionDetalle.producto.image = Converter.GetBytes(row, "imagen");
                cotizacionDetalle.porcentajeDescuento = Converter.GetDecimal(row, "porcentaje_descuento");
                cotizacionDetalle.observacion = Converter.GetString(row, "observaciones");


                /*Precio Cliente Producto*/
                PrecioClienteProducto precioClienteProducto = new PrecioClienteProducto();
                precioClienteProducto.precioNeto = Converter.GetDecimal(row, "precio_neto_vigente");
                precioClienteProducto.flete = Converter.GetDecimal(row, "flete_vigente");
                precioClienteProducto.precioUnitario = Converter.GetDecimal(row, "precio_unitario_vigente");
                precioClienteProducto.equivalencia = Converter.GetDecimal(row, "equivalencia_vigente");
                precioClienteProducto.esUnidadAlternativa = Converter.GetBool(row, "es_unidad_alternativa");
                precioClienteProducto.idPrecioClienteProducto = Converter.GetGuid(row, "id_precio_cliente_producto");
                precioClienteProducto.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");
                precioClienteProducto.fechaFinVigencia = Converter.GetDateTimeNullable(row, "fecha_fin_vigencia");
                precioClienteProducto.cliente = new Cliente();
                precioClienteProducto.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                cotizacionDetalle.producto.precioClienteProducto = precioClienteProducto;



                
                //SI ES UNA RECOTIZACIÓN
                if (cotizacion.esRecotizacion)
                {
                    //Si es recotizacion entonces el precio Lista, precio lista provincia y el costo  Lista son los tomados desde el producto
                    cotizacionDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "producto_precio");
                    cotizacionDetalle.producto.precioProvinciaSinIgv = Converter.GetDecimal(row, "producto_precio_provincia");
                    cotizacionDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "producto_costo");


                    //El precio neto ahora será el precio neto anterior                    
                    /*
                    cotizacionDetalle.precioNetoAnterior = Converter.GetDecimal(row, "precio_neto");
                    cotizacionDetalle.producto.precioListaAnterior = Converter.GetDecimal(row, "precio_sin_igv_anterior");
                    cotizacionDetalle.producto.costoListaAnterior = Converter.GetDecimal(row, "costo_sin_igv");
                    */
                    cotizacionDetalle.precioNetoAnterior = Converter.GetDecimal(row, "precio_neto");
                    cotizacionDetalle.producto.costoListaAnterior = Converter.GetDecimal(row, "costo_sin_igv");
                    cotizacionDetalle.producto.precioListaAnterior = Converter.GetDecimal(row, "precio_sin_igv");
                    
                }
                else
                {
                    //Si NO es recotizacion se considera el precio lista y el costo lista registrado en la cotización
                    cotizacionDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                    cotizacionDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");
                    

                    cotizacionDetalle.precioNetoAnterior = Converter.GetDecimal(row, "precio_neto_anterior");
                    cotizacionDetalle.producto.costoListaAnterior = Converter.GetDecimal(row, "costo_sin_igv_anterior");
                    cotizacionDetalle.producto.precioListaAnterior = Converter.GetDecimal(row, "precio_sin_igv_anterior");


                    //Si la unidad es alternativa se múltiplica por la equivalencia, dado que la capa de negocio se encarga de hacer los calculos y espera siempre el precio estándar
                    //En el caso de recotización el precioNeto es obtenido de la cotización origen
                    if (cotizacionDetalle.esPrecioAlternativo)
                    {
                        cotizacionDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * cotizacionDetalle.ProductoPresentacion.Equivalencia;
                    }
                    else
                    {
                        cotizacionDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto");
                    }
                    
                }

                cotizacion.cotizacionDetalleList.Add(cotizacionDetalle);
            }
            return cotizacion;
        }

        public List<Cotizacion> SelectCotizaciones(Cotizacion cotizacion)
        {
            var objCommand = GetSqlCommand("ps_cotizaciones");
            InputParameterAdd.BigInt(objCommand, "codigo", cotizacion.codigo);
            InputParameterAdd.Guid(objCommand, "id_cliente", cotizacion.cliente.idCliente);
            InputParameterAdd.Guid(objCommand, "id_ciudad", cotizacion.ciudad.idCiudad);

            InputParameterAdd.Bit(objCommand, "buscaSedesGrupoCliente", cotizacion.buscarSedesGrupoCliente);
            InputParameterAdd.Int(objCommand, "idGrupoCliente", cotizacion.grupo.idGrupoCliente);
            //Si se busca por codigo y el usuario es aprobador de cotizaciones no se considera el usuario
        /*    if (cotizacion.codigo > 0)
            {
                InputParameterAdd.Guid(objCommand, "id_usuario", Guid.Empty);
            }
            else
            {
                InputParameterAdd.Guid(objCommand, "id_usuario", cotizacion.usuarioBusqueda.idUsuario);
            }*/

            InputParameterAdd.Guid(objCommand, "id_usuario", cotizacion.usuarioBusqueda.idUsuario);
            InputParameterAdd.DateTime(objCommand, "fechaDesde", new DateTime(cotizacion.fechaDesde.Year, cotizacion.fechaDesde.Month, cotizacion.fechaDesde.Day, 0, 0, 0));
            InputParameterAdd.DateTime(objCommand, "fechaHasta", new DateTime(cotizacion.fechaHasta.Year, cotizacion.fechaHasta.Month, cotizacion.fechaHasta.Day, 23, 59, 59));
            InputParameterAdd.Int(objCommand, "estado", (int)cotizacion.seguimientoCotizacion.estado);
            InputParameterAdd.Bit(objCommand, "buscarSoloCotizacionesGrupales", cotizacion.buscarSoloCotizacionesGrupales);
            InputParameterAdd.Varchar(objCommand, "sku", cotizacion.sku);
            DataTable dataTable = Execute(objCommand);

            List<Cotizacion> cotizacionList = new List<Cotizacion>();

            foreach (DataRow row in dataTable.Rows)
            {
                cotizacion = new Cotizacion();
                cotizacion.codigo = Converter.GetLong(row, "cod_cotizacion");
                cotizacion.idCotizacion = Converter.GetGuid(row, "id_cotizacion");
                cotizacion.fecha = Converter.GetDateTime(row, "fecha");
                cotizacion.incluidoIGV = Converter.GetBool(row, "incluido_igv");
                cotizacion.considerarCantidades = (Cotizacion.OpcionesConsiderarCantidades)Converter.GetInt(row, "considera_cantidades");
                cotizacion.flete = Converter.GetDecimal(row, "porcentaje_flete");
                cotizacion.igv = Converter.GetDecimal(row, "igv");
                cotizacion.montoTotal = Converter.GetDecimal(row, "total");
                
                
                cotizacion.maximoPorcentajeDescuentoPermitido = Converter.GetDecimal(row, "maximo_porcentaje_descuento");
                cotizacion.minimoMargen = Converter.GetDecimal(row, "minimo_margen");

                

                ///Mover "{0:0.00}" a clase de constantes
                cotizacion.montoSubTotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacion.montoTotal / (1 + cotizacion.igv)));
                cotizacion.montoIGV = cotizacion.montoTotal - cotizacion.montoSubTotal;
                

                cotizacion.observaciones = Converter.GetString(row, "observaciones");


                cotizacion.contacto = Converter.GetString(row, "contacto");
                cotizacion.cliente = new Cliente();
                cotizacion.cliente.codigo = Converter.GetString(row, "codigo");
                cotizacion.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                cotizacion.cliente.razonSocial = Converter.GetString(row, "razon_social");
                cotizacion.cliente.ruc = Converter.GetString(row, "ruc");

                cotizacion.grupo = new GrupoCliente();
                cotizacion.grupo.idGrupoCliente = Converter.GetInt(row, "id_grupo_cliente");
                cotizacion.grupo.codigo = Converter.GetString(row, "codigo_grupo");
                cotizacion.grupo.nombre = Converter.GetString(row, "nombre_grupo");
                cotizacion.grupo.contacto = Converter.GetString(row, "contacto_grupo");


                cotizacion.usuario = new Usuario();
                cotizacion.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                cotizacion.usuario.idUsuario = Converter.GetGuid(row, "id_usuario");

                //  cotizacion.usuario_aprobador = new Usuario();
                //  cotizacion.usuario_aprobador.nombre = Converter.GetString(row, "nombre_usuario_aprobador");

                cotizacion.ciudad = new Ciudad();
                cotizacion.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                cotizacion.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

                cotizacion.seguimientoCotizacion = new SeguimientoCotizacion();
                cotizacion.seguimientoCotizacion.estado = (SeguimientoCotizacion.estadosSeguimientoCotizacion)Converter.GetInt(row, "estado_seguimiento");
                cotizacion.seguimientoCotizacion.observacion = Converter.GetString(row, "observacion_seguimiento");
                cotizacion.seguimientoCotizacion.usuario = new Usuario();
                cotizacion.seguimientoCotizacion.usuario.idUsuario = Converter.GetGuid(row, "id_usuario_seguimiento");
                cotizacion.seguimientoCotizacion.usuario.nombre = Converter.GetString(row, "usuario_seguimiento");

                cotizacionList.Add(cotizacion);
            }
            return cotizacionList;
        }


        public void insertSeguimientoCotizacion(Cotizacion cotizacion)
        {
            var objCommand = GetSqlCommand("pi_seguimiento_cotizacion");

            InputParameterAdd.BigInt(objCommand, "codigo", cotizacion.codigo);
            InputParameterAdd.Guid(objCommand, "idUsuario", cotizacion.usuario.idUsuario);
            InputParameterAdd.Int(objCommand, "estado", (int)cotizacion.seguimientoCotizacion.estado);
            InputParameterAdd.Varchar(objCommand, "observacion", cotizacion.seguimientoCotizacion.observacion);
            InputParameterAdd.DateTime(objCommand, "fechaModificacion", cotizacion.fechaModificacion);
            OutputParameterAdd.DateTime(objCommand, "fechaModificacionActual");
            ExecuteNonQuery(objCommand);

            DateTime fechaModifiacionActual = (DateTime)objCommand.Parameters["@fechaModificacionActual"].Value;

/*
            DateTime date1 = new DateTime(fechaModifiacionActual.Year, fechaModifiacionActual.Month, fechaModifiacionActual.Day, fechaModifiacionActual.Hour, fechaModifiacionActual.Minute, fechaModifiacionActual.Second);
            DateTime date2 = new DateTime(cotizacion.fechaModificacion.Year, cotizacion.fechaModificacion.Month, cotizacion.fechaModificacion.Day, cotizacion.fechaModificacion.Hour, cotizacion.fechaModificacion.Minute, cotizacion.fechaModificacion.Second);

            int result = DateTime.Compare(date1, date2);
            if (result != 0)
            {
                //No se puede actualizar la cotización si las fechas son distintas
                throw new Exception("CotizacionDesactualizada");
            }

    */
        }


        public void RechazarCotizaciones()
        {
            var objCommand = GetSqlCommand("pu_rechazarCotizaciones");
            ExecuteNonQuery(objCommand);
        }


        public List<SeguimientoCotizacion> GetHistorialSeguimiento(Guid idCotizacion)
        {
            var objCommand = GetSqlCommand("ps_cotizacion_seguimiento");
            InputParameterAdd.Guid(objCommand, "idCotizacion", idCotizacion);
            DataTable dataTable = Execute(objCommand);

            List<SeguimientoCotizacion> seguimientoCotizacion = new List<SeguimientoCotizacion>();

            foreach (DataRow row in dataTable.Rows)
            {
                SeguimientoCotizacion seg = new SeguimientoCotizacion();
                seg.idSeguimientoCotizacion = Converter.GetGuid(row, "id_estado_seguimiento");
                seg.observacion = Converter.GetString(row, "observacion");
                seg.FechaRegistro = Converter.GetDateTime(row, "fecha_creacion");
                seg.estado = (SeguimientoCotizacion.estadosSeguimientoCotizacion) Converter.GetInt(row, "estado_cotizacion");

                seg.usuario = new Usuario();
                seg.usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                seg.usuario.nombre = Converter.GetString(row, "nombre_usuario");

                seguimientoCotizacion.Add(seg);
            }

            return seguimientoCotizacion;
        }
    }
}
