using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Linq;

namespace DataLayer
{
    public class VentaDAL : DaoBase
    {
        public VentaDAL(IDalSettings settings) : base(settings)
        {
        }

        public VentaDAL() : this(new CotizadorSettings())
        {
        }

        public void InsertVentaNotaCredito(Venta venta)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("pi_ventaNotaCredito");
            InputParameterAdd.Guid(objCommand, "idUsuario", venta.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "serieDocumentoReferencia", venta.documentoReferencia.serie);
            InputParameterAdd.Varchar(objCommand, "numeroDocumentoReferencia", venta.documentoReferencia.numero);
            InputParameterAdd.Int(objCommand, "tipoDocumentoReferencia", (int)venta.documentoReferencia.tipoDocumento);
            InputParameterAdd.DateTime(objCommand, "fechaEmisionDocumentoReferencia", venta.documentoReferencia.fechaEmision);
            InputParameterAdd.Guid(objCommand, "idCiudad", venta.cliente.ciudad.idCiudad);
            InputParameterAdd.Guid(objCommand, "idCliente", venta.cliente.idCliente);
            InputParameterAdd.Varchar(objCommand, "observaciones", venta.observaciones);
            InputParameterAdd.Varchar(objCommand, "sustento", venta.sustento);
            InputParameterAdd.Int(objCommand, "tipoNotaCredito", (int)venta.tipoNotaCredito);


            OutputParameterAdd.UniqueIdentifier(objCommand, "idVenta");
            OutputParameterAdd.BigInt(objCommand, "numeroVenta");
            OutputParameterAdd.UniqueIdentifier(objCommand, "idDocumentoReferenciaVenta");

            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError", 500);
            ExecuteNonQuery(objCommand);

            venta.idVenta = (Guid)objCommand.Parameters["@idVenta"].Value;
            venta.numero = (Int64)objCommand.Parameters["@numeroVenta"].Value;
            venta.documentoReferencia.idDocumentoReferenciaVenta = (Guid)objCommand.Parameters["@idDocumentoReferenciaVenta"].Value;

            var tipoErrorValidacion = (GuiaRemisionValidacion.TiposErrorValidacion)(int)objCommand.Parameters["@tipoError"].Value;
            var descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;

            this.InsertVentaDetalleNotaCredito(venta);


            objCommand = GetSqlCommand("pu_venta");
            InputParameterAdd.Guid(objCommand, "idVenta", venta.idVenta);
            InputParameterAdd.Varchar(objCommand, "observaciones", "Se crea Venta");
            ExecuteNonQuery(objCommand);

            this.Commit();

        }


        public void InsertVentaDetalleNotaCredito(Venta venta)
        {
            foreach (DocumentoDetalle documentoDetalle in venta.pedido.pedidoDetalleList)
            {
                var objCommand = GetSqlCommand("pi_ventaDetalleNotaCredito");
                InputParameterAdd.Guid(objCommand, "idVenta", venta.idVenta);
                InputParameterAdd.Guid(objCommand, "idUsuario", venta.usuario.idUsuario);
                InputParameterAdd.Guid(objCommand, "idProducto", documentoDetalle.producto.idProducto);
                InputParameterAdd.Int(objCommand, "cantidad", documentoDetalle.cantidad);
                InputParameterAdd.Varchar(objCommand, "observaciones", documentoDetalle.observacion);
                InputParameterAdd.Varchar(objCommand, "unidadInternacional", documentoDetalle.unidad);
                InputParameterAdd.Decimal(objCommand, "precioUnitario", documentoDetalle.precioUnitario);
                ExecuteNonQuery(objCommand);
            }     
        }

        public void UpdateVenta(Venta venta)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);

            foreach (PedidoDetalle ventaDetalle in venta.pedido.pedidoDetalleList)
            {
                this.UpdateVentaDetalle(ventaDetalle);
            }

            //Actualiza los totales de la venta
            var objCommand = GetSqlCommand("pu_venta");
            InputParameterAdd.Guid(objCommand, "idVenta", venta.idVenta);
            InputParameterAdd.Varchar(objCommand, "observaciones", venta.observaciones);
            ExecuteNonQuery(objCommand);

            this.Commit();
        }


        public void UpdateVentaDetalle(PedidoDetalle ventaDetalle)
        {
            var objCommand = GetSqlCommand("pu_ventaDetalle");
            InputParameterAdd.Guid(objCommand, "idVentaDetalle", ventaDetalle.idVentaDetalle);
            InputParameterAdd.Decimal(objCommand, "precioUnitario", ventaDetalle.precioUnitario);
            InputParameterAdd.Decimal(objCommand, "precioNeto", ventaDetalle.precioNeto);
            InputParameterAdd.Decimal(objCommand, "porcentajeDescuento", ventaDetalle.porcentajeDescuento);
            InputParameterAdd.Decimal(objCommand, "flete", ventaDetalle.flete);
            ExecuteNonQuery(objCommand);

        }

        


        public Venta SelectVentaPlantillaNotaCredito(Venta venta)
        {
            var objCommand = GetSqlCommand("ps_plantillaNotaCredito");
            InputParameterAdd.Guid(objCommand, "idDocumentoVenta", venta.documentoVenta.idDocumentoVenta);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable ventaDataTable = dataSet.Tables[0];
            DataTable ventaDetalleDataTable = dataSet.Tables[1];          

            //   DataTable dataTable = Execute(objCommand);
            //Datos de la cotizacion
            venta.pedido = new Pedido();
            Pedido pedido = venta.pedido;

            foreach (DataRow row in ventaDataTable.Rows)
            {

                venta.cliente = new Cliente();
                venta.cliente.codigo = Converter.GetString(row, "codigo");
                venta.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                venta.cliente.razonSocial = Converter.GetString(row, "razon_social");
                venta.cliente.ruc = Converter.GetString(row, "ruc");
             /*   venta.cliente.ciudad = new Ciudad();
                venta.cliente.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad_cliente");
                venta.cliente.ciudad.nombre = Converter.GetString(row, "nombre_ciudad_cliente");
            */  venta.cliente.razonSocialSunat = Converter.GetString(row, "razon_social_sunat");
                venta.cliente.direccionDomicilioLegalSunat = Converter.GetString(row, "direccion_domicilio_legal_sunat");
                venta.cliente.correoEnvioFactura = Converter.GetString(row, "correo_envio_factura");
                venta.cliente.plazoCredito = Converter.GetString(row, "plazo_credito");
                venta.cliente.tipoPagoFactura = (DocumentoVenta.TipoPago)Converter.GetInt(row, "tipo_pago_factura");
                venta.cliente.formaPagoFactura = (DocumentoVenta.FormaPago)Converter.GetInt(row, "forma_pago_factura");

                venta.cliente.ciudad = new Ciudad();
                venta.cliente.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");

                //Se obtiene la serie y número de la nota de crédito
                venta.documentoVenta.serie = Converter.GetString(row, "serie");
                venta.documentoVenta.numero = Converter.GetInt(row, "correlativo").ToString().PadLeft(8,'0');
                
            }


            pedido.pedidoDetalleList = new List<PedidoDetalle>();
            //Detalle de la cotizacion
            foreach (DataRow row in ventaDetalleDataTable.Rows)
            {
                PedidoDetalle pedidoDetalle = new PedidoDetalle();
                pedidoDetalle.producto = new Producto();

                pedidoDetalle.idPedidoDetalle = Converter.GetGuid(row, "id_pedido_detalle");
                pedidoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                pedidoDetalle.cantidadPendienteAtencion = Converter.GetInt(row, "cantidadPendienteAtencion");
                pedidoDetalle.cantidadPorAtender = Converter.GetInt(row, "cantidadPendienteAtencion");
                pedidoDetalle.producto.equivalencia = Convert.ToInt32(Converter.GetDecimal(row, "equivalencia"));
                pedidoDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                pedidoDetalle.flete = Converter.GetDecimal(row, "flete");

                pedidoDetalle.precioUnitarioOriginal = Converter.GetDecimal(row, "precio_unitario_original");

                //Si NO es recotizacion se consideran los precios y el costo de lo guardado
                pedidoDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                pedidoDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");

                //Si la unidad es alternativa se múltiplica por la equivalencia, dado que la capa de negocio se encarga de hacer los calculos y espera siempre el precio estándar

                pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto");
                /*            //          if (pedidoDetalle.esPrecioAlternativo)
                          //         {
                          pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * pedidoDetalle.producto.equivalencia;
                        }
                          else
                          {
                              pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto");
                          }*/

                pedidoDetalle.unidad = Converter.GetString(row, "unidad");

                pedidoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                pedidoDetalle.producto.sku = Converter.GetString(row, "sku");
                pedidoDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                pedidoDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                pedidoDetalle.producto.proveedor = Converter.GetString(row, "proveedor");

                pedidoDetalle.producto.image = Converter.GetBytes(row, "imagen");

                pedidoDetalle.porcentajeDescuento = Converter.GetDecimal(row, "porcentaje_descuento");

                pedidoDetalle.observacion = Converter.GetString(row, "observaciones");


                PrecioClienteProducto precioClienteProducto = new PrecioClienteProducto();

                precioClienteProducto.precioNeto = Converter.GetDecimal(row, "precio_neto_vigente");
                precioClienteProducto.flete = Converter.GetDecimal(row, "flete_vigente");
                precioClienteProducto.precioUnitario = Converter.GetDecimal(row, "precio_unitario_vigente");
                precioClienteProducto.equivalencia = Converter.GetInt(row, "equivalencia_vigente");

                precioClienteProducto.idPrecioClienteProducto = Converter.GetGuid(row, "id_precio_cliente_producto");
                precioClienteProducto.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");
                precioClienteProducto.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");
                precioClienteProducto.cliente = new Cliente();
                precioClienteProducto.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                pedidoDetalle.producto.precioClienteProducto = precioClienteProducto;


                pedidoDetalle.precioUnitarioVenta = Converter.GetDecimal(row, "precio_unitario_venta");
                pedidoDetalle.idVentaDetalle = Converter.GetGuid(row, "id_venta_detalle");

                pedido.pedidoDetalleList.Add(pedidoDetalle);
            }            
            pedido.pedidoAdjuntoList = new List<PedidoAdjunto>();

            return venta;
        }

        public Venta SelectVenta(Venta venta)
        {
            var objCommand = GetSqlCommand("ps_venta");
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", venta.guiaRemision.idMovimientoAlmacen);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable ventaDataTable = dataSet.Tables[0];
            DataTable ventaDetalleDataTable = dataSet.Tables[1];
            DataTable pedidoAdjuntoDataTable = dataSet.Tables[2];


            //   DataTable dataTable = Execute(objCommand);
            //Datos de la cotizacion
            venta.pedido = new Pedido();
            Pedido pedido = venta.pedido;

            foreach (DataRow row in ventaDataTable.Rows)
            {
                pedido.idPedido = Converter.GetGuid(row, "id_pedido");
                pedido.numeroPedido = Converter.GetLong(row,"numero");
                pedido.numeroGrupoPedido = Converter.GetLong(row, "numero_grupo");
                pedido.fechaSolicitud = Converter.GetDateTime(row, "fecha_solicitud");
                pedido.fechaEntregaDesde = Converter.GetDateTime(row, "fecha_entrega_desde");
                pedido.fechaEntregaHasta = Converter.GetDateTime(row, "fecha_entrega_hasta");
                pedido.horaEntregaDesde = Converter.GetString(row, "hora_entrega_desde");
                pedido.horaEntregaHasta = Converter.GetString(row, "hora_entrega_hasta");
                pedido.incluidoIGV = Converter.GetBool(row, "incluido_igv");
                pedido.montoIGV = Converter.GetDecimal(row, "igv");
                pedido.montoTotal = Converter.GetDecimal(row, "total");
                pedido.observaciones = Converter.GetString(row, "observaciones");
                pedido.montoSubTotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, pedido.montoTotal - pedido.montoIGV));
                pedido.fechaModificacion = Converter.GetDateTime(row, "fecha_modificacion");
                pedido.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");
                pedido.numeroReferenciaAdicional = Converter.GetString(row, "numero_referencia_adicional");
                pedido.direccionEntrega = new DireccionEntrega();
                pedido.direccionEntrega.idDireccionEntrega = Converter.GetGuid(row, "id_direccion_entrega");
                pedido.direccionEntrega.descripcion = Converter.GetString(row, "direccion_entrega");
                pedido.direccionEntrega.contacto = Converter.GetString(row, "contacto_entrega");
                pedido.direccionEntrega.telefono = Converter.GetString(row, "telefono_contacto_entrega");
                pedido.contactoPedido = Converter.GetString(row, "contacto_pedido");
                pedido.telefonoContactoPedido = Converter.GetString(row, "telefono_contacto_pedido");
                pedido.correoContactoPedido = Converter.GetString(row, "correo_contacto_pedido");
                pedido.fechaProgramacion = Converter.GetDateTime(row, "fecha_programacion");
                pedido.observacionesFactura = Converter.GetString(row, "observaciones_factura");
                pedido.observacionesGuiaRemision = Converter.GetString(row, "observaciones_guia_remision");
                pedido.tipoPedido = (Pedido.tiposPedido)Char.Parse( Converter.GetString(row, "tipo_pedido"));
                pedido.otrosCargos = Converter.GetDecimal(row, "otros_cargos");

               
                venta.igv = Converter.GetDecimal(row, "igv_venta");
                venta.subTotal = Converter.GetDecimal(row, "sub_total_venta");
                venta.total = Converter.GetDecimal(row, "total_venta");
                venta.idVenta = Converter.GetGuid(row, "id_Venta");


                pedido.ubigeoEntrega = new Ubigeo();
                pedido.ubigeoEntrega.Id = Converter.GetString(row, "ubigeo_entrega");
                pedido.ubigeoEntrega.Departamento = Converter.GetString(row, "departamento");
                pedido.ubigeoEntrega.Provincia = Converter.GetString(row, "provincia");
                pedido.ubigeoEntrega.Distrito = Converter.GetString(row, "distrito");

                pedido.documentoVenta = new DocumentoVenta();
                pedido.documentoVenta.serie = Converter.GetString(row, "serie_factura");
                pedido.documentoVenta.numero = Converter.GetString(row, "numero_factura");

                pedido.cotizacion = new Cotizacion();
                pedido.cotizacion.codigo = Converter.GetLong(row, "cotizacion_codigo");  

                pedido.cliente = new Cliente();
                pedido.cliente.codigo = Converter.GetString(row, "codigo");
                pedido.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                pedido.cliente.razonSocial = Converter.GetString(row, "razon_social");
                pedido.cliente.ruc = Converter.GetString(row, "ruc");
                pedido.cliente.ciudad = new Ciudad();
                pedido.cliente.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad_cliente");
                pedido.cliente.ciudad.nombre = Converter.GetString(row, "nombre_ciudad_cliente");
                pedido.cliente.razonSocialSunat = Converter.GetString(row, "razon_social_sunat");
                pedido.cliente.direccionDomicilioLegalSunat = Converter.GetString(row, "direccion_domicilio_legal_sunat");
                pedido.cliente.correoEnvioFactura = Converter.GetString(row, "correo_envio_factura");
                pedido.cliente.plazoCredito = Converter.GetString(row, "plazo_credito");
                pedido.cliente.tipoPagoFactura = (DocumentoVenta.TipoPago)Converter.GetInt(row, "tipo_pago_factura");
                pedido.cliente.formaPagoFactura = (DocumentoVenta.FormaPago)Converter.GetInt(row, "forma_pago_factura");



                pedido.ciudad = new Ciudad();
                pedido.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                pedido.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

                pedido.usuario = new Usuario();
                pedido.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                pedido.usuario.cargo = Converter.GetString(row, "cargo");
                pedido.usuario.contacto = Converter.GetString(row, "contacto_usuario");
                pedido.usuario.email = Converter.GetString(row, "email");

                pedido.seguimientoPedido = new SeguimientoPedido();
                pedido.seguimientoPedido.estado = (SeguimientoPedido.estadosSeguimientoPedido)Converter.GetInt(row, "estado_seguimiento");
                pedido.seguimientoPedido.observacion = Converter.GetString(row, "observacion_seguimiento");
                pedido.seguimientoPedido.usuario = new Usuario();
                pedido.seguimientoPedido.usuario.idUsuario = Converter.GetGuid(row, "id_usuario_seguimiento");
                pedido.seguimientoPedido.usuario.nombre = Converter.GetString(row, "usuario_seguimiento");

                pedido.seguimientoCrediticioPedido = new SeguimientoCrediticioPedido();
                pedido.seguimientoCrediticioPedido.estado = (SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido)Converter.GetInt(row, "estado_seguimiento_crediticio");
                pedido.seguimientoCrediticioPedido.observacion = Converter.GetString(row, "observacion_seguimiento_crediticio");
                pedido.seguimientoCrediticioPedido.usuario = new Usuario();
                pedido.seguimientoCrediticioPedido.usuario.idUsuario = Converter.GetGuid(row, "id_usuario_seguimiento_crediticio");
                pedido.seguimientoCrediticioPedido.usuario.nombre = Converter.GetString(row, "usuario_seguimiento_crediticio");

                venta.guiaRemision.fechaEmision = Converter.GetDateTime(row, "fecha_emision");

            }


            pedido.pedidoDetalleList = new List<PedidoDetalle>();
            //Detalle de la cotizacion
            foreach (DataRow row in ventaDetalleDataTable.Rows)
            {
                PedidoDetalle pedidoDetalle = new PedidoDetalle();
                pedidoDetalle.producto = new Producto();

                pedidoDetalle.idPedidoDetalle = Converter.GetGuid(row, "id_pedido_detalle");
                pedidoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                pedidoDetalle.cantidadPendienteAtencion = Converter.GetInt(row, "cantidadPendienteAtencion");
                pedidoDetalle.cantidadPorAtender = Converter.GetInt(row, "cantidadPendienteAtencion");
                pedidoDetalle.producto.equivalencia = Convert.ToInt32(Converter.GetDecimal(row, "equivalencia"));
                pedidoDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                pedidoDetalle.flete = Converter.GetDecimal(row, "flete");

                pedidoDetalle.precioUnitarioOriginal = Converter.GetDecimal(row, "precio_unitario_original");

                //Si NO es recotizacion se consideran los precios y el costo de lo guardado
                pedidoDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                pedidoDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");

                //Si la unidad es alternativa se múltiplica por la equivalencia, dado que la capa de negocio se encarga de hacer los calculos y espera siempre el precio estándar

                if (pedidoDetalle.esPrecioAlternativo)
                {
                    pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * pedidoDetalle.producto.equivalencia;
                }
                else
                {
                    pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto");
                }

                pedidoDetalle.unidad = Converter.GetString(row, "unidad");

                pedidoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                pedidoDetalle.producto.sku = Converter.GetString(row, "sku");
                pedidoDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                pedidoDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                pedidoDetalle.producto.proveedor = Converter.GetString(row, "proveedor");

                pedidoDetalle.producto.image = Converter.GetBytes(row, "imagen");

                pedidoDetalle.porcentajeDescuento = Converter.GetDecimal(row, "porcentaje_descuento");

                pedidoDetalle.observacion = Converter.GetString(row, "observaciones");


                PrecioClienteProducto precioClienteProducto = new PrecioClienteProducto();

                precioClienteProducto.precioNeto = Converter.GetDecimal(row, "precio_neto_vigente");
                precioClienteProducto.flete = Converter.GetDecimal(row, "flete_vigente");
                precioClienteProducto.precioUnitario = Converter.GetDecimal(row, "precio_unitario_vigente");
                precioClienteProducto.equivalencia = Converter.GetInt(row, "equivalencia_vigente");

                precioClienteProducto.idPrecioClienteProducto = Converter.GetGuid(row, "id_precio_cliente_producto");
                precioClienteProducto.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");
                precioClienteProducto.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");
                precioClienteProducto.cliente = new Cliente();
                precioClienteProducto.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                pedidoDetalle.producto.precioClienteProducto = precioClienteProducto;


                pedidoDetalle.precioUnitarioVenta = Converter.GetDecimal(row, "precio_unitario_venta");
                pedidoDetalle.idVentaDetalle = Converter.GetGuid(row, "id_venta_detalle");

                pedido.pedidoDetalleList.Add(pedidoDetalle);
            }

            
            pedido.pedidoAdjuntoList = new List<PedidoAdjunto>();
            //Detalle de la cotizacion
            foreach (DataRow row in pedidoAdjuntoDataTable.Rows)
            {
                PedidoAdjunto pedidoAdjunto = new PedidoAdjunto();
                pedidoAdjunto.idPedidoAdjunto = Converter.GetGuid(row, "id_pedido_adjunto");
                pedidoAdjunto.adjunto = Converter.GetBytes(row, "adjunto");
                pedidoAdjunto.nombre = Converter.GetString(row, "nombre");
                pedido.pedidoAdjuntoList.Add(pedidoAdjunto);
            }

            return venta;
        }


        public List<Venta> SelectVentas(Venta venta)
        {/*
            var objCommand = GetSqlCommand("ps_pedidos");
            InputParameterAdd.BigInt(objCommand, "numero", pedido.numeroPedido);
            InputParameterAdd.BigInt(objCommand, "numeroGrupo", pedido.numeroGrupoPedido);
            InputParameterAdd.Guid(objCommand, "idCliente", pedido.cliente.idCliente);
            InputParameterAdd.Guid(objCommand, "idCiudad", pedido.ciudad.idCiudad);
            InputParameterAdd.Guid(objCommand, "idUsuario", pedido.usuarioBusqueda.idUsuario);
           
            InputParameterAdd.DateTime(objCommand, "fechaSolicitudDesde", pedido.fechaSolicitudDesde);
            InputParameterAdd.DateTime(objCommand, "fechaSolicitudHasta", pedido.fechaSolicitudHasta);
 
            InputParameterAdd.DateTime(objCommand, "fechaEntregaDesde", pedido.fechaEntregaDesde);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaHasta", pedido.fechaEntregaHasta);


            InputParameterAdd.Int(objCommand, "estado", (int)pedido.seguimientoPedido.estado);
            DataTable dataTable = Execute(objCommand);

            List<Pedido> pedidoList = new List<Pedido>();

            foreach (DataRow row in dataTable.Rows)
            {
                pedido = new Pedido();
                pedido.numeroPedido = Converter.GetLong(row, "numero_pedido");
                pedido.numeroGrupoPedido = Converter.GetLong(row, "numero_grupo_pedido");
                pedido.idPedido = Converter.GetGuid(row, "id_pedido");
                pedido.fechaSolicitud = Converter.GetDateTime(row, "fecha_solicitud");
                pedido.fechaEntregaDesde = Converter.GetDateTime(row, "fecha_entrega_desde");
                pedido.fechaEntregaHasta = Converter.GetDateTime(row, "fecha_entrega_hasta");
                pedido.horaEntregaDesde = Converter.GetString(row, "hora_entrega_desde");
                pedido.horaEntregaHasta = Converter.GetString(row, "hora_entrega_hasta");
                pedido.incluidoIGV = Converter.GetBool(row, "incluido_igv");

                pedido.montoIGV = Converter.GetDecimal(row, "igv");
                pedido.montoTotal = Converter.GetDecimal(row, "total");
                pedido.montoSubTotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, pedido.montoTotal - pedido.montoIGV));

                pedido.observaciones = Converter.GetString(row, "observaciones");

                pedido.cliente = new Cliente();
                pedido.cliente.codigo = Converter.GetString(row, "codigo");
                pedido.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                pedido.cliente.razonSocial = Converter.GetString(row, "razon_social");
                pedido.cliente.ruc = Converter.GetString(row, "ruc");

                pedido.usuario = new Usuario();
                pedido.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                pedido.usuario.idUsuario = Converter.GetGuid(row, "id_usuario");

                //  cotizacion.usuario_aprobador = new Usuario();
                //  cotizacion.usuario_aprobador.nombre = Converter.GetString(row, "nombre_usuario_aprobador");

                pedido.ciudad = new Ciudad();
                pedido.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                pedido.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

                pedido.seguimientoPedido = new SeguimientoPedido();
                pedido.seguimientoPedido.estado = (SeguimientoPedido.estadosSeguimientoPedido)Converter.GetInt(row, "estado_seguimiento");
                pedido.seguimientoPedido.observacion = Converter.GetString(row, "observacion_seguimiento");
                pedido.seguimientoPedido.usuario = new Usuario();
                pedido.seguimientoPedido.usuario.idUsuario = Converter.GetGuid(row, "id_usuario_seguimiento");
                pedido.seguimientoPedido.usuario.nombre = Converter.GetString(row, "usuario_seguimiento");

                pedidoList.Add(pedido);
            }
            return pedidoList;*/
            return null;
        }


        public void insertSeguimientoVenta(Pedido pedido)
        {
            
        }
        
    }
}
